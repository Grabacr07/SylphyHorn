using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using MetroRadiance.UI.Controls;
using SylphyHorn.Interop;
using SylphyHorn.Services;
using SylphyHorn.UI.Bindings;

namespace SylphyHorn.UI
{
	public class NotificationWindow : BlurWindow
	{
		static NotificationWindow()
		{
			ResizeModeProperty.OverrideMetadata(typeof(NotificationWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
			ShowInTaskbarProperty.OverrideMetadata(typeof(NotificationWindow), new FrameworkPropertyMetadata(false));
			TopmostProperty.OverrideMetadata(typeof(NotificationWindow), new FrameworkPropertyMetadata(true));
		}

		protected WindowPlacement Placement { get; }

		private IntPtr _hWnd = IntPtr.Zero;

		#region NativeOpacity dependency property

		public static readonly DependencyProperty NativeOpacityProperty = DependencyProperty.Register(
			nameof(NativeOpacity), typeof(double), typeof(NotificationWindow), new PropertyMetadata(default(double), HandleNativeOpacityChanged));

		public double NativeOpacity
		{
			get { return (double)this.GetValue(NativeOpacityProperty); }
			set { this.SetValue(NativeOpacityProperty, value); }
		}

		private static void HandleNativeOpacityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			((NotificationWindow)sender).ChangeOpacity((double)args.NewValue);
		}

		#endregion
		
		public NotificationWindow()
		{
			this.Placement = WindowPlacement.Center;
		}

		public NotificationWindow(WindowPlacement placement)
		{
			this.Placement = placement;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source == null) throw new InvalidOperationException();
			this._hWnd = source.Handle;

			var style = User32.GetWindowLongEx(this._hWnd);
			style |= WindowExStyles.WS_EX_TOOLWINDOW | WindowExStyles.WS_EX_NOACTIVATE | WindowExStyles.WS_EX_TRANSPARENT;
			User32.SetWindowLongEx(this._hWnd, style);

			this.UpdateWindowPosition();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			this.UpdateWindowPosition();
		}

		protected virtual bool TryGetTargetRect(out RECT rect, out RECT? capableRect)
			=> throw new NotImplementedException();

		private void UpdateWindowPosition()
		{
			if (this._hWnd == IntPtr.Zero) return;
			if (!NativeMethods.GetWindowRect(this._hWnd, out var rect)) return;
			if (!TryGetTargetRect(out var area, out var outside)) return;

			var placementWithoutFlags = this.Placement & ~WindowPlacement.OutsideY;

			int left;
			switch (placementWithoutFlags)
			{
				case WindowPlacement.TopLeft:
				case WindowPlacement.BottomLeft:
					left = area.Left;
					break;

				case WindowPlacement.TopRight:
				case WindowPlacement.BottomRight:
					left = area.Right - (rect.Right - rect.Left);
					break;

				case WindowPlacement.TopCenter:
				case WindowPlacement.Center:
				case WindowPlacement.BottomCenter:
				default:
					left = area.Left + (area.Right - area.Left - (rect.Right - rect.Left)) / 2;
					break;
			}

			int top;
			switch (placementWithoutFlags)
			{
				case WindowPlacement.TopLeft:
				case WindowPlacement.TopCenter:
				case WindowPlacement.TopRight:
					if (outside.HasValue)
					{
						top = Math.Max(outside.Value.Top, area.Top - (rect.Bottom - rect.Top));
					}
					else
					{
						top = area.Top;
					}
					break;

				case WindowPlacement.BottomLeft:
				case WindowPlacement.BottomCenter:
				case WindowPlacement.BottomRight:
					if (outside.HasValue)
					{
						top = Math.Min(outside.Value.Bottom - (rect.Bottom - rect.Top), area.Bottom);
					}
					else
					{
						top = area.Bottom - (rect.Bottom - rect.Top);
					}
					break;

				case WindowPlacement.Center:
				default:
					top = area.Top + (area.Bottom - area.Top - (rect.Bottom - rect.Top)) / 2;
					break;
			}

			User32.SetWindowPos(
				this._hWnd,
				IntPtr.Zero,
				left,
				top,
				0,
				0,
				SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOOWNERZORDER);
		}

		protected static bool TryGetWorkAreaFromHmonitor(IntPtr hMonitor, out RECT rect)
		{
			try
			{
				var monitor = MonitorHelper.GetAreaFromHmonitor(hMonitor);
				rect = monitor.WorkArea;
			}
			catch (Win32Exception)
			{
				rect = new RECT();
				return false;
			}
			return true;
		}

		protected static bool TryGetWorkAreaFromHwnd(IntPtr hWnd, out RECT rect)
		{
			try
			{
				var monitor = MonitorHelper.GetAreaFromHwnd(hWnd);
				rect = monitor.WorkArea;
			}
			catch (Win32Exception)
			{
				rect = new RECT();
				return false;
			}
			return true;
		}

		protected static bool TryGetWindowRectFromHwnd(IntPtr hWnd, out RECT rect)
		{
			return NativeMethods.GetWindowRect(hWnd, out rect);
		}

		private void ChangeOpacity(double opacity)
		{
			if (this._hWnd == IntPtr.Zero) return;

			var bAlpha = (byte)(opacity * 255.0);
			NativeMethods.SetLayeredWindowAttributes(this._hWnd, 0, bAlpha, LayeredWindowAttributes.Alpha);
		}
	}
}
