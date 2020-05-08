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
using System.Collections.ObjectModel;

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
		public static int GetNumber(VirtualDesktop desktop)
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
		public static void ResizeSettingsProperties()
		{
			var desktopCount = VirtualDesktopService.Count();
			var desktopNames = Settings.General.DesktopNames.Value;
			if (desktopNames == null)
			{
				var temp = new StringHolder[desktopCount];
				for(int i = 0; i < desktopCount; ++i)
				{
					temp[i] = new StringHolder();
				}
				desktopNames = new ObservableCollection<StringHolder>(temp);
				Settings.General.DesktopNames.Value = desktopNames;
			}
			else if (desktopNames.Count != desktopCount)
			{
				var temp = new StringHolder[desktopNames.Count];
				desktopNames.CopyTo(temp, 0);
				Array.Resize<StringHolder>(ref temp, desktopCount);
				for (int i = desktopNames.Count; i < temp.Length; i++)
				{
					temp[i] = new StringHolder();
				}
				Settings.General.DesktopNames.Value = new ObservableCollection<StringHolder>(temp);
			}
		}

		public static string GetDesktopName(int number)
		{
			var desktopNames = Settings.General.DesktopNames.Value;
			if (desktopNames == null || number < 1 || number > desktopNames.Count)
				return "";
			return desktopNames[number - 1];
		}

		public static void SetDesktopName(int number, string name)
		{
			ResizeSettingsProperties();
			var desktopNames = Settings.General.DesktopNames.Value;
			if (desktopNames == null)
				return;

			if (number < 1 || number > desktopNames.Count)
				return;
			desktopNames[number - 1] = name;
			Settings.General.DesktopNames.Value = desktopNames;
		}
	}
}
