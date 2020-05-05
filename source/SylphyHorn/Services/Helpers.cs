using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Livet;
using MetroRadiance.Interop.Win32;
using SylphyHorn.Interop;
using SylphyHorn.Serialization;
using WindowsDesktop;

namespace SylphyHorn.Services
{
	internal static class InteropHelper
	{
		private const string _consoleWindowClass = "ConsoleWindowClass";

		public static bool IsCurrentProcess(IntPtr hWnd)
		{
			return System.Windows.Application.Current.Windows
				.OfType<Window>()
				.Select(x => PresentationSource.FromVisual(x) as HwndSource)
				.Where(x => x != null)
				.Select(x => x.Handle)
				.Any(x => x == hWnd);
		}

		public static bool IsConsoleWindow(IntPtr hWnd)
		{
			var className = new StringBuilder(256);
			NativeMethods.GetClassName(hWnd, className, className.Capacity);
			return className.ToString() == _consoleWindowClass;
		}

		public static IntPtr GetForegroundWindowEx()
		{
			var hwnd = User32.GetForegroundWindow();
			var howner = NativeMethods.GetWindow(hwnd, 4 /* GW_OWNER */);
			return howner == IntPtr.Zero ? hwnd : howner;
		}
	}

	internal static class VisualHelper
	{
		public static void InvokeOnUIDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(action, priority);
		}
	}

	internal static class DesktopHelper
	{
		public static int GetIndex(VirtualDesktop desktop)
		{
			int i = 1;
			foreach (var iDesktop in VirtualDesktop.GetDesktops())
			{
				if (desktop.Id == iDesktop.Id)
				{
					return i;
				}
				++i;
			}
			return 0;
		}
	}

	internal static class SettingsHelper
	{
		public static string GetDesktopName(int index)
		{
			switch(index)
			{
				case 1: return Settings.General.DesktopName1.Value;
				case 2: return Settings.General.DesktopName2.Value;
				case 3: return Settings.General.DesktopName3.Value;
				case 4: return Settings.General.DesktopName4.Value;
				case 5: return Settings.General.DesktopName5.Value;
				case 6: return Settings.General.DesktopName6.Value;
				case 7: return Settings.General.DesktopName7.Value;
				case 8: return Settings.General.DesktopName8.Value;
				case 9: return Settings.General.DesktopName9.Value;
				case 10: return Settings.General.DesktopName10.Value;
			}
			return null;
		}

		public static void SetDesktopName(int index, string name)
		{
			switch (index)
			{
				case 1: Settings.General.DesktopName1.Value = name; break;
				case 2: Settings.General.DesktopName2.Value = name; break;
				case 3: Settings.General.DesktopName3.Value = name; break;
				case 4: Settings.General.DesktopName4.Value = name; break;
				case 5: Settings.General.DesktopName5.Value = name; break;
				case 6: Settings.General.DesktopName6.Value = name; break;
				case 7: Settings.General.DesktopName7.Value = name; break;
				case 8: Settings.General.DesktopName8.Value = name; break;
				case 9: Settings.General.DesktopName9.Value = name; break;
				case 10: Settings.General.DesktopName10.Value = name; break;
			}
		}
	}
}
