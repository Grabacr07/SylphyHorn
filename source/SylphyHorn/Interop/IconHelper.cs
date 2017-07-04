using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;
using SylphyHorn.Services;
using System.Windows;

namespace SylphyHorn.Interop
{
    public static class IconHelper
	{
        private static bool _lastOrientationVertical;
        private static Color? _lastTrayColor = null;

        private static FontFamily _trayFontFamily;
        private static Font _trayFont;
        private static Brush _trayBrush;

        private const string TrayFontFamilyName = "Segoe UI";
        private const float TrayHorizontalFontSize = 7;
        private const float TrayVerticalFontSize = 6;

		public static Icon GetIconFromResource(Uri uri)
		{
			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null) throw new ArgumentException("Resource not found.", nameof(uri));

			using (var stream = streamResourceInfo.Stream)
			{
                var icon = new Icon(stream);

                return ScaleIconToDpi(icon);
			}
		}

        public static Icon GetDesktopInfoIcon(int currentDesktop, int totalDesktopCount, Color color)
        {
            var orientationVertical = totalDesktopCount >= 10;

            if (_trayFontFamily == null)
            {
                _trayFontFamily = new FontFamily(TrayFontFamilyName);
            }

            if (orientationVertical != _lastOrientationVertical || _trayFont == null)
            {
                var size = orientationVertical ? TrayVerticalFontSize : TrayHorizontalFontSize;
                _trayFont?.Dispose();
                _trayFont = new Font(_trayFontFamily, size, System.Drawing.FontStyle.Bold);
            }

            if (!_lastTrayColor.HasValue || _lastTrayColor != color)
            {
                _trayBrush?.Dispose();
                _trayBrush = new SolidBrush(color);
            }

            _lastOrientationVertical = orientationVertical;

            using (var iconBitmap = !orientationVertical
                ? DrawHorizontalInfo(currentDesktop, totalDesktopCount)
                : DrawVerticalInfo(currentDesktop, totalDesktopCount))
            {
                return iconBitmap.ToIcon();
            }

        }

        private static Icon ToIcon(this Bitmap bitmap)
        {
            IntPtr iconHandle = bitmap.GetHicon();
            var icon = Icon.FromHandle(iconHandle);

            return icon;
        }
        
        private static Bitmap DrawHorizontalInfo(int currentDesktop, int totalDesktopCount)
        {
            var iconSize = GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var stringToDraw = $"{currentDesktop}/{totalDesktopCount}";

            var offset = GetHorizontalStringOffset();

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawString(stringToDraw, _trayFont, _trayBrush, offset);
            }

            return bitmap;
        }

        private static Bitmap DrawVerticalInfo(int currentDesktop, int totalDesktops)
        {
            var iconSize = GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var firstOffset = GetFirstVerticalStringOffset(currentDesktop);
            var secondOffset = GetSecondVerticalStringOffset(totalDesktops, bitmap.Height);

            var firstString = currentDesktop.ToString();
            var secondString = totalDesktops.ToString();

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawString(firstString, _trayFont, _trayBrush, firstOffset);
                graphics.DrawString(secondString, _trayFont, _trayBrush, secondOffset);
            }

            return bitmap;
        }

        private static PointF GetHorizontalStringOffset()
        {
            return new PointF(-2, 0);
        }

        private static PointF GetFirstVerticalStringOffset(int value)
        {
            var offset = new PointF(-2, -2);

            if (value < 10)
            {
                offset.X += 7;
            }
            else if (value < 100)
            {
                offset.X += 4;
            }

            return offset;
        }

        private static PointF GetSecondVerticalStringOffset(int value, int bitmapHeight)
        {
            var offset = GetFirstVerticalStringOffset(value);

            offset.Y += bitmapHeight / 2;

            return offset;
        }

        private static Icon ScaleIconToDpi(Icon targetIcon)
        {
            var dpi = GetMonitorDpi();

            return new Icon(targetIcon, new System.Drawing.Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
        }

        private static System.Windows.Size GetIconSize()
        {
            var dpi = GetMonitorDpi();

            return new System.Windows.Size(SystemParameters.SmallIconWidth * dpi.ScaleX, SystemParameters.SmallIconHeight * dpi.ScaleY);
        }
        
        private static Dpi GetMonitorDpi()
        {
            return PerMonitorDpi.GetDpi(IntPtr.Zero);
        }
	}
}
