using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop.Win32;
using MetroRadiance.UI.Controls;
using SylphyHorn.Interop;

namespace SylphyHorn.UI
{
	public class InteractableNotificationWindow : BlurWindow
	{
		#region NativeOpacity dependency property

		public static readonly DependencyProperty NativeOpacityProperty = DependencyProperty.Register(
			nameof(NativeOpacity), typeof(double), typeof(InteractableNotificationWindow), new PropertyMetadata(default(double), HandleNativeOpacityChanged));

		public double NativeOpacity
		{
			get { return (double)this.GetValue(NativeOpacityProperty); }
			set { this.SetValue(NativeOpacityProperty, value); }
		}

		private static void HandleNativeOpacityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			((InteractableNotificationWindow)sender).ChangeOpacity((double)args.NewValue);
		}

		#endregion

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source == null) throw new InvalidOperationException();

			// Reset styles to make it interactive
			User32.SetWindowLong(source.Handle, 0);
		}

		private void ChangeOpacity(double opacity)
		{
			var source = (HwndSource)PresentationSource.FromVisual(this);
			if (source == null) return;

			var bAlpha = (byte)(opacity * 255.0);
			NativeMethods.SetLayeredWindowAttributes(source.Handle, 0, bAlpha, LayeredWindowAttributes.Alpha);
		}
	}
}
