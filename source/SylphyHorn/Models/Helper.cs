using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Livet;
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
	}

	internal static class ShellLinkHelper
	{
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
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			return CreateLink(dir + "\\" + name + ".lnk");
		}

		public static bool CreateStartup()
		{
			var name = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
			return CreateStartup(name);
		}

		public static bool RemoveStartup(string name)
		{
			var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
			var path = dir + "\\" + name + ".lnk";
			if (!File.Exists(path)) return false;
			File.Delete(path);
			return true;
		}

		public static bool RemoveStartup()
		{
			var name = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
			return RemoveStartup(name);
		}
	}
}
