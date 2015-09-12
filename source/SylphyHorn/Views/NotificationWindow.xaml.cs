using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Core.Win32;

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
				var wsex = hwndSource.Handle.GetWindowLongEx() | WSEX.TOOLWINDOW | WSEX.NOACTIVATE | WSEX.TRANSPARENT;
				hwndSource.Handle.SetWindowLongEx(wsex);
			}

			base.OnSourceInitialized(e);
		}
	}
}
