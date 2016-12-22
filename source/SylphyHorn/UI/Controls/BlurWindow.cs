using SylphyHorn.Interop;
using System;
using System.Windows;

namespace SylphyHorn.UI.Controls
{
	public class BlurWindow : Window
	{
		private bool _isBlured = false;

		#region IsBlured 依存関係プロパティ

		public bool IsBlured
		{
			get { return (bool)this.GetValue(IsBluredProperty); }
			set { this.SetValue(IsBluredProperty, value); }
		}
		public static readonly DependencyProperty IsBluredProperty =
			DependencyProperty.Register("IsBlured", typeof(bool), typeof(BlurWindow), new UIPropertyMetadata(false, IsBluredChangedCallback));

		private static void IsBluredChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as BlurWindow;
			if (window == null) return;

			window.ChangeIsBlured((bool)e.NewValue);
		}

		#endregion

		protected override void OnSourceInitialized(EventArgs e)
		{
			this.ChangeIsBlured(this.IsBlured);

			base.OnSourceInitialized(e);
		}

		private void ChangeIsBlured(bool blured)
		{
			if (!this.IsInitialized) return;

			var highContrast = SystemParameters.HighContrast;
			var transparency = this.AllowsTransparency;
			if (!highContrast && transparency && blured && !this._isBlured)
			{
				this.EnableBlur();
				this._isBlured = true;
			}
			else if ((!transparency || !blured) && this._isBlured)
			{
				this.DisableBlur();
				this._isBlured = false;
			}
		}

		private void EnableBlur()
		{
			WindowCompositionHelper.SetWindowComposition(this, AccentState.ACCENT_ENABLE_BLURBEHIND, AccentFlags.DrawAllBorders);
		}

		private void DisableBlur()
		{
			WindowCompositionHelper.SetWindowComposition(this, AccentState.ACCENT_DISABLED, 0);
		}
	}
}