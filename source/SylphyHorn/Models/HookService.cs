using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Livet;
using SylphyHorn.Interop;
using VDMHelperCLR.Common;
using WindowsDesktop;

namespace SylphyHorn.Models
{
	public class HookService : IDisposable
	{
		private readonly GlobalKeyHook keyHook = new GlobalKeyHook();
		private readonly IVdmHelper helper;

		public HookService()
		{
			this.keyHook.Pressed += this.KeyHookOnPressed;
			this.keyHook.Start();

			var is64Bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
			var asmPath = Path.GetFullPath(is64Bit ? @"VDMHelperCLR64.dll" : @"VDMHelperCLR32.dll");
			var asm = Assembly.LoadFile(asmPath);
			var type = asm.GetType("VDMHelperCLR.VdmHelper");
			this.helper = (IVdmHelper)Activator.CreateInstance(type);
			this.helper.Init();
		}

		private void KeyHookOnPressed(object sender, ShortcutKey shortcutKey)
		{
			if (ShortcutSettings.OpenDesktopSelector.Value != null) { }

			if (ShortcutSettings.MoveLeft.Value != null &&
				ShortcutSettings.MoveLeft.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => this.MoveToLeft());
			}

			if (ShortcutSettings.MoveLeftAndSwitch.Value != null &&
				ShortcutSettings.MoveLeftAndSwitch.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => this.MoveToLeft()?.Switch());
			}

			if (ShortcutSettings.MoveRight.Value != null &&
				ShortcutSettings.MoveRight.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => this.MoveToRight());
			}

			if (ShortcutSettings.MoveRightAndSwitch.Value != null &&
				ShortcutSettings.MoveRightAndSwitch.Value == shortcutKey)
			{
				InvokeOnUIDispatcher(() => this.MoveToRight()?.Switch());
			}
		}

		private VirtualDesktop MoveToLeft()
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
					this.helper.MoveWindowToDesktop(hWnd, left.Id);
					return left;
				}
			}

			return null;
		}

		private VirtualDesktop MoveToRight()
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
					this.helper.MoveWindowToDesktop(hWnd, right.Id);
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
			this.helper?.Dispose();
		}
	}
}
