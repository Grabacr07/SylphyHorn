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
}
