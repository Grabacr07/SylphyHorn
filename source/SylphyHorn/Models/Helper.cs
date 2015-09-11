using System;
using System.Linq;
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

	internal static class NativeHelper
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
		public static void InvokeOnUIDispatcher(Action action)
		{
			DispatcherHelper.UIDispatcher.BeginInvoke(action, DispatcherPriority.Normal);
		}
	}
}
