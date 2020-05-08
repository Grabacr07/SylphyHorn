using System;
using System.Linq;
using System.Media;
using SylphyHorn.Serialization;
using SylphyHorn.UI;
using SylphyHorn.UI.Bindings;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	internal static class VirtualDesktopService
	{
		public static int CachedNumber { get; set; } = 0;
		public static int CachedCount { get; set; } = 0;
		public static int CachedPreviousNumber { get; set; } = 0;

		#region Count

		public static int Count()
		{
			return VirtualDesktop.GetDesktops().Length;
		}
		#endregion

		#region Get

		public static VirtualDesktop Get(int number)
		{
			var desktops = VirtualDesktop.GetDesktops();

			if (number < 1 || number > desktops.Length)
				return null;

			return desktops[number-1];
		}

		public static VirtualDesktop GetLeft()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.First().Id
				? Settings.General.LoopDesktop ? desktops.Last() : null
				: current.GetLeft();
		}

		public static VirtualDesktop GetRight()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.Last().Id
				? Settings.General.LoopDesktop ? desktops.First() : null
				: current.GetRight();
		}

		#endregion

		#region Move

		public static VirtualDesktop MoveTo(this IntPtr hWnd, VirtualDesktop target)
		{
			if (target == null)
			{
				SystemSounds.Asterisk.Play();
				return null;
			}

			VirtualDesktopHelper.MoveToDesktop(hWnd, target);
			return target;
		}

		public static VirtualDesktop MoveToLeft(this IntPtr hWnd)
		{
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (current != null)
			{
				var left = current.GetLeft();
				if (left == null)
				{
					if (Settings.General.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) left = desktops.Last();
					}
				}
				if (left != null)
				{
					VirtualDesktopHelper.MoveToDesktop(hWnd, left);
					return left;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToRight(this IntPtr hWnd)
		{
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (current != null)
			{
				var right = current.GetRight();
				if (right == null)
				{
					if (Settings.General.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) right = desktops.First();
					}
				}
				if (right != null)
				{
					VirtualDesktopHelper.MoveToDesktop(hWnd, right);
					return right;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public static VirtualDesktop MoveToNew(this IntPtr hWnd)
		{
			var newone = VirtualDesktop.Create();
			if (newone != null)
			{
				VirtualDesktopHelper.MoveToDesktop(hWnd, newone);
				return newone;
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		#endregion

		#region Close

		public static void CloseAndSwitchLeft()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();
			
			if (desktops.Length > 1)
			{
				GetLeft()?.Switch();
				current.Remove();
			}
		}

		public static void CloseAndSwitchRight()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			if (desktops.Length > 1)
			{
				GetRight()?.Switch();
				current.Remove();
			}
		}

		#endregion

		#region SwitchToPrevious

		public static void SwitchToPrevious()
		{
			Get(CachedPreviousNumber)?.Switch();
		}

		#endregion

		#region Pin / Unpin

		public static event EventHandler<WindowPinnedEventArgs> WindowPinned;


		public static void Pin(this IntPtr hWnd)
		{
			VirtualDesktop.PinWindow(hWnd);
			RaisePinnedEvent(hWnd, PinOperations.PinWindow);
		}

		public static void Unpin(this IntPtr hWnd)
		{
			VirtualDesktop.UnpinWindow(hWnd);
			RaisePinnedEvent(hWnd, PinOperations.UnpinWindow);
		}

		public static void TogglePin(this IntPtr hWnd)
		{
			if (VirtualDesktop.IsPinnedWindow(hWnd))
			{
				VirtualDesktop.UnpinWindow(hWnd);
				RaisePinnedEvent(hWnd, PinOperations.UnpinWindow);
			}
			else
			{
				VirtualDesktop.PinWindow(hWnd);
				RaisePinnedEvent(hWnd, PinOperations.PinWindow);
			}
		}

		public static void PinApp(this IntPtr hWnd)
		{
			var appId = ApplicationHelper.GetAppId(hWnd);
			if (appId == null) return;

			VirtualDesktop.PinApplication(appId);
			RaisePinnedEvent(hWnd, PinOperations.PinApp);
		}

		public static void UnpinApp(this IntPtr hWnd)
		{
			var appId = ApplicationHelper.GetAppId(hWnd);
			if (appId == null) return;

			VirtualDesktop.UnpinApplication(appId);
			RaisePinnedEvent(hWnd, PinOperations.UnpinApp);
		}

		public static void TogglePinApp(this IntPtr hWnd)
		{
			var appId = ApplicationHelper.GetAppId(hWnd);
			if (appId == null) return;

			if (VirtualDesktop.IsPinnedApplication(appId))
			{
				VirtualDesktop.UnpinApplication(appId);
				RaisePinnedEvent(hWnd, PinOperations.UnpinApp);
			}
			else
			{
				VirtualDesktop.PinApplication(appId);
				RaisePinnedEvent(hWnd, PinOperations.PinApp);
			}
		}

		#endregion

		#region VirtualDesktop even handlers

		public static void DesktopCreatedHandler(object sender, VirtualDesktop args)
		{
			CachedCount = Count();
		}

		public static void DesktopDestroyedHandler(object sender, VirtualDesktopDestroyEventArgs args)
		{
			CachedCount = Count();
		}

		public static void DesktopSwitchedHandler(object sender, VirtualDesktopChangedEventArgs args)
		{
			var tDesktops = VirtualDesktop.GetDesktops();
			CachedPreviousNumber = Array.IndexOf(tDesktops, args.OldDesktop) + 1;
			CachedNumber = Array.IndexOf(tDesktops, args.NewDesktop) + 1;
		}

		public static void VirtualDesktopInitializedHandler()
		{
			CachedCount = Count();
			var desktops = VirtualDesktop.GetDesktops();
			CachedNumber = Array.IndexOf(desktops, VirtualDesktop.Current) + 1;
		}

		private static void RaisePinnedEvent(IntPtr target, PinOperations operation)
		{
			WindowPinned?.Invoke(typeof(VirtualDesktopService), new WindowPinnedEventArgs(target, operation));
		}

		#endregion

		#region Rename

		public static event EventHandler<RenameEventArgs> RenameCurrent;

		public static void RaiseRenameEvent()
		{
			RenameCurrent?.Invoke(typeof(VirtualDesktopService), new RenameEventArgs());
		}

		#endregion
	}

	internal class WindowPinnedEventArgs : EventArgs
	{
		public IntPtr Target { get; }
		public PinOperations PinOperation { get; }

		public WindowPinnedEventArgs(IntPtr target, PinOperations operation)
		{
			this.Target = target;
			this.PinOperation = operation;
		}
	}

	internal class RenameEventArgs : EventArgs
	{
		public RenameEventArgs()
		{
		}
	}

	[Flags]
	internal enum PinOperations
	{
		Pin = 0x01,
		Unpin = 0x02,

		Window = 0x04,
		App = 0x08,

		PinWindow = Pin | Window,
		UnpinWindow = Unpin | Window,
		PinApp = Pin | App,
		UnpinApp = Unpin | App,
	}
}
