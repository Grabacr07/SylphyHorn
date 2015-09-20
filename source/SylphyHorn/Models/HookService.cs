using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using MetroTrilithon.Lifetime;
using SylphyHorn.Interop;
using VDMHelperCLR.Common;
using WindowsDesktop;

namespace SylphyHorn.Models
{
	public class HookService : IDisposable
	{
		private readonly ShortcutKeyDetector detector = new ShortcutKeyDetector();
		private readonly IVdmHelper helper;
		private int suspendRequestCount;

		public HookService()
		{
			this.detector.Pressed += this.KeyHookOnPressed;
			this.detector.Start();

			this.helper = VdmHelperFactory.CreateInstance();
			this.helper.Init();
		}

		public IDisposable Suspend()
		{
			this.suspendRequestCount++;
			this.detector.Stop();

			return Disposable.Create(() =>
			{
				this.suspendRequestCount--;
				if (this.suspendRequestCount == 0)
				{
					this.detector.Start();
				}
			});
		}

		private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
		{
			if (ShortcutSettings.OpenDesktopSelector.Value != null) { }

			if (ShortcutSettings.MoveLeft.Value != null &&
				ShortcutSettings.MoveLeft.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft());
				args.Handled = true;
			}

			if (ShortcutSettings.MoveLeftAndSwitch.Value != null &&
				ShortcutSettings.MoveLeftAndSwitch.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft()?.Switch());
				args.Handled = true;
			}

			if (ShortcutSettings.MoveRight.Value != null &&
				ShortcutSettings.MoveRight.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight());
				args.Handled = true;
			}

			if (ShortcutSettings.MoveRightAndSwitch.Value != null &&
				ShortcutSettings.MoveRightAndSwitch.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight()?.Switch());
				args.Handled = true;
			}

			if (ShortcutSettings.MoveNew.Value != null &&
				ShortcutSettings.MoveNew.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToNew());
				args.Handled = true;
			}

			if (ShortcutSettings.MoveNewAndSwitch.Value != null &&
				ShortcutSettings.MoveNewAndSwitch.Value == args.ShortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToNew()?.Switch());
				args.Handled = true;
			}

			if (ShortcutSettings.SwitchToLeft.Value != null &&
				ShortcutSettings.SwitchToLeft.Value == args.ShortcutKey)
			{
				if (GeneralSettings.OverrideOSDefaultKeyCombination)
				{
					VisualHelper.InvokeOnUIDispatcher(() => PrepareSwitchToLeft()?.Switch());
					args.Handled = true;
				}
			}

			if (ShortcutSettings.SwitchToRight.Value != null &&
				ShortcutSettings.SwitchToRight.Value == args.ShortcutKey)
			{
				if (GeneralSettings.OverrideOSDefaultKeyCombination)
				{
					VisualHelper.InvokeOnUIDispatcher(() => PrepareSwitchToRight()?.Switch());
					args.Handled = true;
				}
			}
		}

		private static VirtualDesktop PrepareSwitchToLeft()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.First().Id
				? GeneralSettings.LoopDesktop ? desktops.Last() : null
				: current.GetLeft();
		}

		private static VirtualDesktop PrepareSwitchToRight()
		{
			var current = VirtualDesktop.Current;
			var desktops = VirtualDesktop.GetDesktops();

			return desktops.Length >= 2 && current.Id == desktops.Last().Id
				? GeneralSettings.LoopDesktop ? desktops.First() : null
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
					if (GeneralSettings.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) left = desktops.Last();
					}
				}
				if (left != null)
				{
					if (InteropHelper.IsCurrentProcess(hWnd))
					{
						VirtualDesktopHelper.MoveToDesktop(hWnd, left);
						return left;
					}

					if (this.helper.MoveWindowToDesktop(hWnd, left.Id))
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
					if (GeneralSettings.LoopDesktop)
					{
						var desktops = VirtualDesktop.GetDesktops();
						if (desktops.Length >= 2) right = desktops.First();
					}
				}
				if (right != null)
				{
					if (InteropHelper.IsCurrentProcess(hWnd))
					{
						VirtualDesktopHelper.MoveToDesktop(hWnd, right);
						return right;
					}

					if (this.helper.MoveWindowToDesktop(hWnd, right.Id))
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
			var hWnd = NativeMethods.GetForegroundWindow();
			var newone = VirtualDesktop.Create();
			if (newone != null)
			{
				if (InteropHelper.IsCurrentProcess(hWnd))
				{
					VirtualDesktopHelper.MoveToDesktop(hWnd, newone);
					return newone;
				}

				if (this.helper.MoveWindowToDesktop(hWnd, newone.Id))
				{
					return newone;
				}
			}

			SystemSounds.Asterisk.Play();
			return null;
		}

		public void Dispose()
		{
			this.detector.Stop();
			this.helper?.Dispose();
		}
	}
}
