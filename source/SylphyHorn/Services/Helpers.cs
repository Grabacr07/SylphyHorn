using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Livet;
using MetroRadiance.Interop.Win32;
using MetroTrilithon.Desktop;
using Microsoft.Win32;
using SylphyHorn.Interop;

namespace SylphyHorn.Services
{
	internal static class InteropHelper
	{
		private const string consoleWindowClass = "ConsoleWindowClass";

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
			return className.ToString() == consoleWindowClass;
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


	internal static class ShellLinkHelper
	{
		private static string GetExecutingAssemblyFileNameWithoutExtension()
		{
			return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
		}

		private static string GetStartupFileName(string name)
		{
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			return Path.Combine(dir, name + ".lnk");
		}

		public static void CreateStartup(string name)
		{
			ShellLink.Create(GetStartupFileName(name));
		}

		public static void CreateStartup()
		{
			CreateStartup(GetExecutingAssemblyFileNameWithoutExtension());
		}

		public static bool RemoveStartup(string name)
		{
			if (!ExistsStartup(name)) return false;
			File.Delete(GetStartupFileName(name));
			return true;
		}

		public static bool RemoveStartup()
		{
			return RemoveStartup(GetExecutingAssemblyFileNameWithoutExtension());
		}

		public static bool ExistsStartup(string name)
		{
			return File.Exists(GetStartupFileName(name));
		}

		public static bool ExistsStartup()
		{
			return ExistsStartup(GetExecutingAssemblyFileNameWithoutExtension());
		}
	}
}
