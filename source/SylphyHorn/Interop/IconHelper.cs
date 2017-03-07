using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;
using SylphyHorn.Services;
using System.Windows;

namespace SylphyHorn.Interop
{
    // handle exception thrown when font is not found
    // reuse objects if possible (fontfamily, font, brush?)
    // scale string positions too?
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
        
        private static Bitmap DrawHorizontalInfo(int currentDesktop, int totalDesktopCount, Color color)
        {
            var iconSize = GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var stringToDraw = $"{currentDesktop}/{totalDesktopCount}";

            var position = new PointF(-2, 0);

            using (var fontFamily = new FontFamily("Segoe UI"))
            {
                using (var font = new Font(fontFamily, (float)iconSize.Height * 0.375f, System.Drawing.FontStyle.Bold))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        using (var brush = new SolidBrush(color))
                        {
                            graphics.DrawString(stringToDraw, font, brush, position);
                        }
                    }
                }
            }

            return bitmap;
        }

        // one string with \n instead of two?
        private static Bitmap DrawVerticalInfo(int currentDesktop, int totalDesktops, Color color)
        {
            var iconSize = GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var firstPosition = new PointF(-2, -2);
            var secondPosition = new PointF(-2, bitmap.Height / 2 - 2);

            if (currentDesktop < 10)
            {
                firstPosition.X += 7;
            }
            else if (currentDesktop < 100)
            {
                firstPosition.X += 4;
            }

            if (totalDesktops < 100)
            {
                secondPosition.X += 4;
            }

            var firstString = currentDesktop.ToString();
            var secondString = totalDesktops.ToString();

            using (var fontFamily = new FontFamily("Segoe UI"))
            {
                using (var font = new Font(fontFamily, (float)iconSize.Height * 0.325f, System.Drawing.FontStyle.Bold))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        using (var brush = new SolidBrush(color))
                        {
                            graphics.DrawString(firstString, font, brush, firstPosition);
                            graphics.DrawString(secondString, font, brush, secondPosition);
                        }
                    }
                }
            }

            return bitmap;
        }

        private static Icon ScaleIconToDpi(Icon targetIcon)
        {
            var dpi = GetMonitorDpi();

            return new Icon(targetIcon, new System.Drawing.Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
        }

        private static Dpi GetMonitorDpi()
        {
            return PerMonitorDpi.GetDpi(IntPtr.Zero);
        }

        private static System.Windows.Size GetIconSize()
        {
            var dpi = GetMonitorDpi();

            return new System.Windows.Size(SystemParameters.SmallIconWidth * dpi.ScaleX, SystemParameters.SmallIconHeight * dpi.ScaleY);
        }

        // move to appropriate file
        // not used but useful
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
