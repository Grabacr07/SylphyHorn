using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Livet;
using Microsoft.Win32;
using SylphyHorn.Interop;

namespace SylphyHorn.Models
{
	public static class KeyHelper
	{
		public static bool IsModifyKey(this Key key)
		{
			// System.Windows.Forms.KeyEventArgs.Modifiers が LWin と RWin を含めてくれないので…
			// あと Left と Right 区別できたらいいなって…
			// _:(´ཀ`」 ∠):_

			return key == Key.LeftAlt
				   || key == Key.LeftCtrl
				   || key == Key.LeftShift
				   || key == Key.LWin
				   || key == Key.RightAlt
				   || key == Key.RightCtrl
				   || key == Key.RightShift
				   || key == Key.RWin;
		}
	}

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
	}

	internal static class VisualHelper
	{
		public static void InvokeOnUIDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(action, priority);
		}

		public static bool IsDarkTheme()
		{
			const string keyName = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
			const string valueName = "AppsUseLightTheme";

			var value = Registry.GetValue(keyName, valueName, null);
			return value as int? == 0;
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

		public static bool CreateLink(string path)
		{
			var type = Type.GetTypeFromCLSID(new Guid("00021401-0000-0000-C000-000000000046"));
			var psl = (IShellLink)Activator.CreateInstance(type);
			psl.SetPath(Assembly.GetExecutingAssembly().Location);
			var ppf = (IPersistFile)psl;
			ppf.Save(path, false);
			return true;
		}

		public static bool CreateStartup(string name)
		{
			return CreateLink(GetStartupFileName(name));
		}

		public static bool CreateStartup()
		{
			return CreateStartup(GetExecutingAssemblyFileNameWithoutExtension());
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
