using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Livet;
using SylphyHorn.Interop;
using VDMHelperCLR.Common;
using WindowsDesktop;
using System.Text;

namespace SylphyHorn.Models
{
	public class HookService : IDisposable
	{
		private static string ConsoleWindowClass = "ConsoleWindowClass";

		private readonly GlobalKeyHook keyHook = new GlobalKeyHook();
		private readonly IVdmHelper helper;

		public HookService()
		{
			this.keyHook.Pressed += this.KeyHookOnPressed;
			this.keyHook.Start();

			var is64Bit = Environment.Is64BitProcess;
			var current = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var asmPath = Path.Combine(current ?? Environment.CurrentDirectory, is64Bit ? @"VDMHelperCLR64.dll" : @"VDMHelperCLR32.dll");
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
			if (IsActivatedConsoleWindow())
			{
				System.Media.SystemSounds.Asterisk.Play();
				return null;
			}
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
					if (IsCurrentProcess(hWnd))
					{
						VirtualDesktopHelper.MoveToDesktop(hWnd, left);
					}
					else
					{
						this.helper.MoveWindowToDesktop(hWnd, left.Id);
					}
					return left;
				}
			}

			return null;
		}

		private VirtualDesktop MoveToRight()
		{
			var hWnd = GetActiveWindow();
			var current = VirtualDesktop.FromHwnd(hWnd);
			if (IsActivatedConsoleWindow())
			{
				System.Media.SystemSounds.Asterisk.Play();
				return null;
			}
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
					if (IsCurrentProcess(hWnd))
					{
						VirtualDesktopHelper.MoveToDesktop(hWnd, right);
					}
					else
					{
						this.helper.MoveWindowToDesktop(hWnd, right.Id);
					}
					return right;
				}
			}

			return null;
		}

		private static IntPtr GetActiveWindow()
		{
			var hWnd = NativeMethods.GetForegroundWindow();

			return hWnd;
		}

		private static string GetActiveWindowClassName()
		{
			var hWnd = GetActiveWindow();
			var className = new StringBuilder(256);
			NativeMethods.GetClassName(hWnd, className, className.Capacity);
			return className.ToString();
		}

		private static bool IsActivatedConsoleWindow()
		{
			return GetActiveWindowClassName() == ConsoleWindowClass;
		}

		private static bool IsCurrentProcess(IntPtr hWnd)
		{
			return System.Windows.Application.Current.Windows
				.OfType<Window>()
				.Select(x => PresentationSource.FromVisual(x) as HwndSource)
				.Where(x => x != null)
				.Select(x => x.Handle)
				.Any(x => x == hWnd);
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
