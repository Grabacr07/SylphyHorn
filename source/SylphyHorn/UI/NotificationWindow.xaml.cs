using System;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using SylphyHorn.Serialization;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI
{
	partial class NotificationWindow
	{
		private readonly Rect _area;

		public NotificationWindow(Rect area)
		{
			this._area = area;

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

				var dpi = hwndSource.GetDpi();
				var width = this.ActualWidth * dpi.ScaleX;
				var height = this.ActualHeight * dpi.ScaleY;
				var area = this._area;

				switch ((WindowPlacement)Settings.General.Placement.Value)
				{
					case WindowPlacement.TopLeft:
					case WindowPlacement.BottomLeft:
						this.Left = area.Left / dpi.ScaleX;
						break;

					case WindowPlacement.TopRight:
					case WindowPlacement.BottomRight:
						this.Left = (area.Right - width) / dpi.ScaleX;
						break;

					case WindowPlacement.Center:
					default:
						this.Left = (area.Left + (area.Width - width) / 2) / dpi.ScaleX;
						break;
				}

				switch ((WindowPlacement)Settings.General.Placement.Value)
				{
					case WindowPlacement.TopLeft:
					case WindowPlacement.TopCenter:
					case WindowPlacement.TopRight:
						this.Top = area.Top / dpi.ScaleY;
						break;

					case WindowPlacement.BottomLeft:
					case WindowPlacement.BottomCenter:
					case WindowPlacement.BottomRight:
						this.Top = (area.Bottom - height) / dpi.ScaleY;
						break;

					case WindowPlacement.Center:
					default:
						this.Top = (area.Top + (area.Height - height) / 2) / dpi.ScaleY;
						break;
				}

				switch ((WindowPlacement)Settings.General.Placement.Value)
				{
					case WindowPlacement.TopLeft:
						this.WindowBorder = Interop.AccentFlags.DrawBottomRightBorder;
						break;

					case WindowPlacement.TopCenter:
						this.WindowBorder = Interop.AccentFlags.DrawAllBorders ^ Interop.AccentFlags.DrawTopBorder;
						break;

					case WindowPlacement.TopRight:
						this.WindowBorder = Interop.AccentFlags.DrawBottomLeftBorder;
						break;

					case WindowPlacement.BottomLeft:
						this.WindowBorder = Interop.AccentFlags.DrawTopRightBorder;
						break;

					case WindowPlacement.BottomCenter:
						this.WindowBorder = Interop.AccentFlags.DrawAllBorders ^ Interop.AccentFlags.DrawBottomBorder;
						break;

					case WindowPlacement.BottomRight:
						this.WindowBorder = Interop.AccentFlags.DrawTopLeftBorder;
						break;

					case WindowPlacement.Center:
					default:
						this.WindowBorder = Interop.AccentFlags.DrawAllBorders;
						break;
				}
			}

			base.OnSourceInitialized(e);
		}
	}
}
