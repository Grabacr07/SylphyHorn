using SylphyHorn.UI.Bindings;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsDesktop;

namespace SylphyHorn.UI
{
	partial class SettingsWindow
	{
		public static SettingsWindow Instance { get; set; }

		public SettingsWindow()
		{
			this.InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			this.UpdatePreviewBlurImageMargin();
			DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Grid))
				.AddValueChanged(this.PreviewRoot, this.OnPreviewRootCanvasPositionChanged);
			DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Grid))
				.AddValueChanged(this.PreviewRoot, this.OnPreviewRootCanvasPositionChanged);
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			this.Pin();
		}

		private void OnPreviewRootCanvasPositionChanged(object sender, EventArgs e)
			=> this.UpdatePreviewBlurImageMargin();

		private void UpdatePreviewBlurImageMargin()
		{
			var previewLeft = Canvas.GetLeft(this.PreviewRoot);
			if (double.IsNaN(previewLeft)) previewLeft = 0.0;
			var previewTop = Canvas.GetTop(this.PreviewRoot);
			if (double.IsNaN(previewTop)) previewTop = 0.0;

			this.PreviewBlurImage.Margin = new Thickness(
				-previewLeft,
				-previewTop,
				-(this.CanvasRoot.Width - previewLeft - this.PreviewRoot.Width),
				-(this.CanvasRoot.Height - previewTop - this.PreviewRoot.Height));
		}

		#region Implement PreviewRoot dragging

		private const double _snappingDeltaX = 30;
		private const double _snappingDeltaY = 45;

		private bool _isDrag = false;
		private Point _positionOffset = new Point(0, 0);

		private void OnPreviewRootMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this._positionOffset = e.GetPosition(this.PreviewRoot);
				this._isDrag = true;
			}
		}

		private void OnCanvasRootMouseMove(object sender, MouseEventArgs e)
		{
			if (!this._isDrag || e.LeftButton != MouseButtonState.Pressed) return;

			var viewModel = this.DataContext as SettingsWindowViewModel;
			if (viewModel == null) return;

			var left = 0.0;
			var top = 0.0;
			var width = this.CanvasRoot.ActualWidth - this.PreviewRoot.ActualWidth;
			var height = this.CanvasRoot.ActualHeight - this.PreviewRoot.ActualHeight;
			switch (viewModel.PreviewTaskbarPosition)
			{
				case "Left":
					left = this.PreviewTaskbar.ActualWidth;
					break;

				case "Top":
					top = this.PreviewTaskbar.ActualHeight;
					break;

				case "Right":
					width -= this.PreviewTaskbar.ActualWidth;
					break;

				case "Bottom":
					height -= this.PreviewTaskbar.ActualHeight;
					break;
			}

			var position = Point.Subtract(e.GetPosition(this.CanvasRoot), _positionOffset);
			Canvas.SetLeft(this.PreviewRoot, Math.Max(left, Math.Min(width, position.X)));
			Canvas.SetTop(this.PreviewRoot, Math.Max(top, Math.Min(height, position.Y)));

			var halfAvaiableWidth = width * 0.5;
			if (position.Y <= top + _snappingDeltaY)
			{
				if (position.X <= left + _snappingDeltaX)
				{
					viewModel.Placement = WindowPlacement.TopLeft;
				}
				else if (ContainsRange(position.X, halfAvaiableWidth, _snappingDeltaX))
				{
					viewModel.Placement = WindowPlacement.TopCenter;
				}
				else if (position.X >= width - _snappingDeltaX)
				{
					viewModel.Placement = WindowPlacement.TopRight;
				}
			}
			else if (ContainsRange(position.Y, height * 0.5, _snappingDeltaY))
			{
				viewModel.Placement = WindowPlacement.Center;
			}
			else if (position.Y >= height - _snappingDeltaY)
			{
				if (position.X <= left + _snappingDeltaX)
				{
					viewModel.Placement = WindowPlacement.BottomLeft;
				}
				else if (ContainsRange(position.X, halfAvaiableWidth, _snappingDeltaX))
				{
					viewModel.Placement = WindowPlacement.BottomCenter;
				}
				else if (position.X >= width - _snappingDeltaX)
				{
					viewModel.Placement = WindowPlacement.BottomRight;
				}
			}
		}

		private void OnCanvasRootMouseUp(object sender, MouseButtonEventArgs e)
			=> this.ResetDragState();

		private void OnCanvasRootMouseLeave(object sender, MouseEventArgs e)
			=> this.ResetDragState();

		private void ResetDragState()
		{
			if (this._isDrag)
			{
				this.PreviewRoot.ClearValue(Canvas.LeftProperty);
				this.PreviewRoot.ClearValue(Canvas.TopProperty);
				this._isDrag = false;
			}
		}

		private static bool ContainsRange(double current, double length, double delta)
			=> current >= length - delta && current <= length + delta;

		#endregion
	}
}
