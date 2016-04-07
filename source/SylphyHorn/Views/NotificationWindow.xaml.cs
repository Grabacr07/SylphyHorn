using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop.Win32;

namespace SylphyHorn.Views
{
	partial class NotificationWindow
	{
		public NotificationWindow()
		{
			this.InitializeComponent();

			// var workingArea = System.Windows.Forms.Screen.GetWorkingArea(System.Drawing.Point.Empty);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (hwndSource != null)
			{
				var style = User32.GetWindowLongEx(hwndSource.Handle);
				style |= WindowExStyles.WS_EX_TOOLWINDOW | WindowExStyles.WS_EX_NOACTIVATE | WindowExStyles.WS_EX_TRANSPARENT;
				User32.SetWindowLongEx(hwndSource.Handle, style);
			}

			base.OnSourceInitialized(e);
		}
	}
}
