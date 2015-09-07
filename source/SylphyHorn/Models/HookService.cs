using System;
using System.Linq;
using System.Windows.Threading;
using WindowsDesktop;
using Livet;
using SylphyHorn.Interop;

namespace SylphyHorn.Models
{
	public class HookService : IDisposable
	{
		private readonly GlobalKeyHook keyHook = new GlobalKeyHook();

		public HookService()
		{
			this.keyHook.Pressed += KeyHookOnPressed;
			this.keyHook.Start();
		}

		private static void KeyHookOnPressed(object sender, ShortcutKey shortcutKey)
		{
			if (ShortcutSettings.OpenDesktopSelector.Value != null) { }

			if (ShortcutSettings.MoveLeft.Value != null
				&& ShortcutSettings.MoveLeft.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => MoveToLeft());
			}

			if (ShortcutSettings.MoveLeftAndSwitch.Value != null
				&& ShortcutSettings.MoveLeftAndSwitch.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => MoveToLeft());
			}

			if (ShortcutSettings.MoveRight.Value != null
				&& ShortcutSettings.MoveRight.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => MoveToRight());
			}

			if (ShortcutSettings.MoveRightAndSwitch.Value != null
				&& ShortcutSettings.MoveRightAndSwitch.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => MoveToRight()?.Switch());
			}
		}

		private static VirtualDesktop MoveToLeft()
		{
			var hWnd = GetActiveWindow();
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, left);
					return left;
				}
			}

			return null;
		}

		private static VirtualDesktop MoveToRight()
		{
			var hWnd = GetActiveWindow();
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
					VirtualDesktopHelper.MoveToDesktop(hWnd, right);
					return right;
				}
			}

			return null;
		}

		private static IntPtr GetActiveWindow()
		{
			var hwnd = NativeMethods.GetForegroundWindow();

			return hwnd;
		}

		private static void InvokeOnUIDispatcher(Action action)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(action, DispatcherPriority.Normal);
		}

		public void Dispose()
		{
			this.keyHook.Stop();
		}
	}
}