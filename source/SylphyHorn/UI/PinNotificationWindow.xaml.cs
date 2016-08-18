using System;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using SylphyHorn.Interop;

namespace SylphyHorn.UI
{
	partial class PinNotificationWindow
	{
		private readonly IntPtr _target;

		public PinNotificationWindow(IntPtr target)
		{
			this._target = target;
			this.InitializeComponent();
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

			RECT rect;
			if (NativeMethods.GetWindowRect(this._target, out rect))
			{
				var targetWidth = rect.Right - rect.Left;
				var targetHeight = rect.Bottom - rect.Top;

				var dpi = PerMonitorDpi.GetDpi(this._target);
				var width = this.ActualWidth * dpi.ScaleX;
				var height = this.ActualHeight * dpi.ScaleY;

				this.Left = (rect.Left + (targetWidth - width) / 2) / dpi.ScaleX;
				this.Top = (rect.Top + (targetHeight - height) / 2) / dpi.ScaleY;
			}

			base.OnSourceInitialized(e);
		}
	}
}
