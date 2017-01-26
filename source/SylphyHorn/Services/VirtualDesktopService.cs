using System;
using System.Linq;
using System.Media;
using SylphyHorn.Serialization;
using WindowsDesktop;
using WindowsDesktop.Interop;

namespace SylphyHorn.Services
{
	internal static class VirtualDesktopService
	{
		#region Get

		public static VirtualDesktop GetLeft()
		{
			var current = VirtualDesktop.CurrentOrDesktopToSwitchTo;
		    return GetLeftOf(current);
		}

        public static VirtualDesktop GetLeftOf(VirtualDesktop current)
        {
            return current.GetLeft(Settings.General.LoopDesktop);
        }

        public static VirtualDesktop GetRight()
		{
			var current = VirtualDesktop.CurrentOrDesktopToSwitchTo;
		    return GetRightOf(current);
		}

        public static VirtualDesktop GetRightOf(VirtualDesktop current)
        {
            return current.GetRight(Settings.General.LoopDesktop);
        }

        #endregion

        #region Move

        public static VirtualDesktop MoveToLeft(this IntPtr hWnd)
		{
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (current != null)
			{
				var left = current.GetLeft(Settings.General.LoopDesktop);
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, left, AdjacentDesktop.LeftDirection, Settings.General.LoopDesktop);
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
				var right = current.GetRight(Settings.General.LoopDesktop);
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, right, AdjacentDesktop.RightDirection, Settings.General.LoopDesktop);
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
				VirtualDesktopHelper.MoveToDesktop(hWnd, newone, AdjacentDesktop.Jump, false);
			    return newone;
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		#endregion

		#region Close

		public static void CloseAndSwitchLeft(SmoothSwitchData switchData)
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();
			
			if (desktops.Length > 1)
			{
				GetLeft()?.Switch(switchData, AdjacentDesktop.LeftDirection, Settings.General.LoopDesktop).Execute(null);
				current.Remove();
			}
		}

		public static void CloseAndSwitchRight(SmoothSwitchData switchData)
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			if (desktops.Length > 1)
			{
				GetRight()?.Switch(switchData, AdjacentDesktop.RightDirection, Settings.General.LoopDesktop).Execute(null);
				current.Remove();
			}
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
			VirtualDesktop.PinApplication(appId);
			RaisePinnedEvent(hWnd, PinOperations.PinApp);
		}

		public static void UnpinApp(this IntPtr hWnd)
		{
			var appId = ApplicationHelper.GetAppId(hWnd);
			VirtualDesktop.UnpinApplication(appId);
			RaisePinnedEvent(hWnd, PinOperations.UnpinApp);
		}

		public static void TogglePinApp(this IntPtr hWnd)
		{
			var appId = ApplicationHelper.GetAppId(hWnd);

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

		private static void RaisePinnedEvent(IntPtr target, PinOperations operation)
		{
			WindowPinned?.Invoke(typeof(VirtualDesktopService), new WindowPinnedEventArgs(target, operation));
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
