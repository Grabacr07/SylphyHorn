using System;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop;
using SylphyHorn.Serialization;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI
{
	partial class SwitchWindow
	{
		private readonly Rect _area;

		public SwitchWindow(Rect area)
		{
			this._area = area;
			this.InitializeComponent();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source == null) throw new InvalidOperationException();

			var dpi = source.GetDpi();
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
		}
	}
}
