using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using MetroRadiance.Interop.Win32;
using MetroTrilithon.Lifetime;
using SylphyHorn.Serialization;
using VDMHelperCLR.Common;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	public class HookService : IDisposable
	{
		private readonly ShortcutKeyDetector _detector = new ShortcutKeyDetector();
		private readonly IVdmHelper _helper;
		private int _suspendRequestCount;

		public event EventHandler<IntPtr> PinRequested;
		public event EventHandler<IntPtr> UnpinRequested;

		public HookService(IVdmHelper helper)
		{
			this._detector.Pressed += this.KeyHookOnPressed;
			this._detector.Start();
			this._helper = helper;
		}

		public IDisposable Suspend()
		{
			this._suspendRequestCount++;
			this._detector.Stop();

			return Disposable.Create(() =>
			{
				this._suspendRequestCount--;
				if (this._suspendRequestCount == 0)
				{
					this._detector.Start();
				}
			});
		}

		private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
		{
			if (args.ShortcutKey == ShortcutKey.None) return;

			if (Settings.ShortcutKey.MoveLeft.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.MoveLeftAndSwitch.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft()?.Switch());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.MoveRight.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.MoveRightAndSwitch.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight()?.Switch());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.MoveNew.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToNew());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.MoveNewAndSwitch.ToShortcutKey() == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToNew()?.Switch());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.SwitchToLeft.ToShortcutKey() == args.ShortcutKey)
			{
				if (Settings.General.OverrideWindowsDefaultKeyCombination)
				{
					VisualHelper.InvokeOnUIDispatcher(() => PrepareSwitchToLeft()?.Switch());
					args.Handled = true;
					return;
				}
			}

			if (Settings.ShortcutKey.SwitchToRight.ToShortcutKey() == args.ShortcutKey)
			{
				if (Settings.General.OverrideWindowsDefaultKeyCombination)
				{
					VisualHelper.InvokeOnUIDispatcher(() => PrepareSwitchToRight()?.Switch());
					args.Handled = true;
					return;
				}
			}

			if (Settings.ShortcutKey.Pin.ToShortcutKey() == args.ShortcutKey)
			{
				this.PinRequested?.Invoke(this, InteropHelper.GetForegroundWindowEx());
				args.Handled = true;
				return;
			}

			if (Settings.ShortcutKey.Unpin.ToShortcutKey() == args.ShortcutKey)
			{
				this.UnpinRequested?.Invoke(this, InteropHelper.GetForegroundWindowEx());
				args.Handled = true;
				return;
			}
		}

		private static VirtualDesktop PrepareSwitchToLeft()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.First().Id
				? Settings.General.LoopDesktop ? desktops.Last() : null
				: current.GetLeft();
		}

		private static VirtualDesktop PrepareSwitchToRight()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.Last().Id
				? Settings.General.LoopDesktop ? desktops.First() : null
				: current.GetRight();
		}

		private VirtualDesktop MoveToLeft()
		{
			var hWnd = InteropHelper.GetForegroundWindowEx();
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
					if (VirtualDesktopHelper.MoveToDesktop(hWnd, left)
						|| this._helper.MoveWindowToDesktop(hWnd, left.Id))
					{
						return left;
					}
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		private VirtualDesktop MoveToRight()
		{
			var hWnd = InteropHelper.GetForegroundWindowEx();
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
					if (VirtualDesktopHelper.MoveToDesktop(hWnd, right)
						|| this._helper.MoveWindowToDesktop(hWnd, right.Id))
					{
						return right;
					}
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		private VirtualDesktop MoveToNew()
		{
			var hWnd = User32.GetForegroundWindow();
			var newone = VirtualDesktop.Create();
			if (newone != null)
			{
				if (VirtualDesktopHelper.MoveToDesktop(hWnd, newone)
					|| this._helper.MoveWindowToDesktop(hWnd, newone.Id))
				{
					return newone;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public void Dispose()
		{
			this._detector.Stop();
			this._helper?.Dispose();
		}
	}
}
