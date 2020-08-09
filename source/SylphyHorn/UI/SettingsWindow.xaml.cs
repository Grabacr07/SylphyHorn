using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
	}
}
