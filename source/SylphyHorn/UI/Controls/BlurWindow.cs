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

		#region WindowBorder 依存関係プロパティ

		public AccentFlags WindowBorder
		{
			get { return (AccentFlags)this.GetValue(WindowBorderProperty); }
			set { this.SetValue(WindowBorderProperty, value); }
		}
		public static readonly DependencyProperty WindowBorderProperty =
			DependencyProperty.Register("WindowBorder", typeof(AccentFlags), typeof(BlurWindow), new UIPropertyMetadata(AccentFlags.DrawAllBorders, WindowBorderChangedCallback));

		private static void WindowBorderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = d as BlurWindow;
			if (window == null) return;

			window.ChangeWindowBorder((AccentFlags)e.NewValue);
		}

		#endregion


		protected override void OnSourceInitialized(EventArgs e)
		{
			this.ChangeIsBlured(this.IsBlured);

			base.OnSourceInitialized(e);
		}

		private void ChangeIsBlured(bool blured, bool force = false)
		{
			if (!this.IsInitialized) return;

			var highContrast = SystemParameters.HighContrast;
			var transparency = this.AllowsTransparency;
			if (!highContrast && transparency && (force || (blured && !this._isBlured)))
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

		private void ChangeWindowBorder(AccentFlags flags)
		{
			if (this.IsBlured)
			{
				this.ChangeIsBlured(true, true);
			}
		}

		private void EnableBlur()
		{
			WindowCompositionHelper.SetWindowComposition(this, AccentState.ACCENT_ENABLE_BLURBEHIND, this.WindowBorder);
		}

		private void DisableBlur()
		{
			WindowCompositionHelper.SetWindowComposition(this, AccentState.ACCENT_DISABLED, 0);
		}
	}
}