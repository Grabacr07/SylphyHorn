using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;
using SylphyHorn.Services;

namespace SylphyHorn.Interop
{
    // test for various scaling and resolution
	public static class IconHelper
	{
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
            using (var iconBitmap = totalDesktopCount < 10
                ? DrawHorizontalInfo(currentDesktop, totalDesktopCount, color)
                : DrawVerticalInfo(currentDesktop, totalDesktopCount, color))
            {
                return iconBitmap.ToIcon();
            }
        }

        // move to appropriate file
        private static Icon ToIcon(this Bitmap bitmap)
        {
            IntPtr iconHandle = bitmap.GetHicon();
            var icon = Icon.FromHandle(iconHandle);

            return icon;
        }

        private static Icon ScaleIconToDpi(Icon targetIcon)
        {
            var dpi = GetMonitorDpi();

            return new Icon(targetIcon, new Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
        }

        private static Bitmap DrawHorizontalInfo(int currentDesktop, int totalDesktopCount, Color color)
        {
            var dpi = GetMonitorDpi();
            var bitmap = GetEmptyIconBitmap(dpi);

            var stringToDraw = $"{currentDesktop}/{totalDesktopCount}";
            var font = new Font(new FontFamily("Arial"), 6 * (float) dpi.ScaleY, FontStyle.Bold);
            var position = new PointF(0, 0);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                using (var brush = new SolidBrush(color))
                {
                    graphics.DrawString(stringToDraw, font, brush, position);
                }
            }

            return bitmap;
        }

        private static Bitmap DrawVerticalInfo(int currentDesktop, int totalDesktops, Color color)
        {
            var dpi = GetMonitorDpi();
            var bitmap = GetEmptyIconBitmap(dpi);

            var font = new Font(new FontFamily("Arial"), 5 * (float) dpi.ScaleY, FontStyle.Bold);
            var firstPosition = new PointF(0, 0);
            var secondPosition = new PointF(0, bitmap.Height / 2);

            var firstString = currentDesktop.ToString();
            var secondString = totalDesktops.ToString();

            using (var graphics = Graphics.FromImage(bitmap))
            {
                using (var brush = new SolidBrush(color))
                {
                    graphics.DrawString(firstString, font, brush, firstPosition);
                    graphics.DrawString(secondString, font, brush, secondPosition);
                }
            }

            return bitmap;
        }

        private static Dpi GetMonitorDpi()
        {
            return PerMonitorDpi.GetDpi(IntPtr.Zero);
        }

        private static Bitmap GetEmptyIconBitmap(Dpi dpi)
        {
            var width = Convert.ToInt32(16 * dpi.ScaleX);
            var height = Convert.ToInt32(16 * dpi.ScaleY);

            return new Bitmap(width, height);
        }

        // move to appropriate file
        public static List<int> ToDigits(this int value)
        {
            var result = new List<int>();

            while (value >= 10)
            {
                result.Add(value % 10);

                value /= 10;
            }

            result.Add(value);
            return result;
        }
	}
}
