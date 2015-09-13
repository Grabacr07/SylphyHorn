using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Interop;
using MetroRadiance.Core.Win32;
using SylphyHorn.Models;

namespace SylphyHorn.Views
{
	public class TransparentWindow : RawWindow
	{
		public override void Show()
		{
			var parameters = new HwndSourceParameters(ProductInfo.Title)
			{
				Width = 1,
				Height = 1,
				WindowStyle = (int)WS.BORDER,
			};

			this.Show(parameters);
		}

		protected override IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.DWMCOLORIZATIONCOLORCHANGED)
			{
				VisualHelper.ForceChangeAccent((long)wParam);
			}

			return base.WindowProc(hwnd, msg, wParam, lParam, ref handled);
		}
	}

	public class RawWindow
	{
		public HwndSource Source { get; private set; }

		public IntPtr Handle => this.Source?.Handle ?? IntPtr.Zero;

		public virtual void Show()
		{
			this.Show(new HwndSourceParameters());
		}

		protected void Show(HwndSourceParameters parameters)
		{
			this.Source = new HwndSource(parameters);
			this.Source.AddHook(this.WindowProc);
		}

		protected virtual IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			return IntPtr.Zero;
		}
	}
}
