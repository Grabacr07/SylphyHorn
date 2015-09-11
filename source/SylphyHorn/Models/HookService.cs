using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

			var is64Bit = Environment.Is64BitProcess;
			var current = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var asmPath = Path.Combine(current ?? Environment.CurrentDirectory, is64Bit ? @"VDMHelperCLR64.dll" : @"VDMHelperCLR32.dll");
			var asm = Assembly.LoadFile(asmPath);
			var type = asm.GetType("VDMHelperCLR.VdmHelper");
			this.helper = (IVdmHelper)Activator.CreateInstance(type);
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

		private void KeyHookOnPressed(object sender, ShortcutKey shortcutKey)
		{
			if (ShortcutSettings.OpenDesktopSelector.Value != null) { }

			if (ShortcutSettings.MoveLeft.Value != null &&
				ShortcutSettings.MoveLeft.Value == shortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft());
			}

			if (ShortcutSettings.MoveLeftAndSwitch.Value != null &&
				ShortcutSettings.MoveLeftAndSwitch.Value == shortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToLeft()?.Switch());
			}

			if (ShortcutSettings.MoveRight.Value != null &&
				ShortcutSettings.MoveRight.Value == shortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight());
			}

			if (ShortcutSettings.MoveRightAndSwitch.Value != null &&
				ShortcutSettings.MoveRightAndSwitch.Value == shortcutKey)
			{
				VisualHelper.InvokeOnUIDispatcher(() => this.MoveToRight()?.Switch());
			}
		}

		private VirtualDesktop MoveToLeft()
		{
			var hWnd = NativeMethods.GetForegroundWindow();
			if (InteropHelper.IsConsoleWindow(hWnd))
			{
				System.Media.SystemSounds.Asterisk.Play();
				return null;
			}

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
			var hWnd = NativeMethods.GetForegroundWindow();
			if (InteropHelper.IsConsoleWindow(hWnd))
			{
				System.Media.SystemSounds.Asterisk.Play();
				return null;
			}

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

		public void Dispose()
		{
			this.detector.Stop();
			this.helper?.Dispose();
		}
	}
}
