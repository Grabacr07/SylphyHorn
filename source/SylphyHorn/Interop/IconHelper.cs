using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MetroRadiance.Interop;
using SylphyHorn.Services;

namespace SylphyHorn.Interop
{
	public static class IconHelper
	{
        private static DigitDrawer _digitDrawer;

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
            if (_digitDrawer.Color != color || _digitDrawer == null)
            {
                _digitDrawer?.Dispose();

                _digitDrawer = new DigitDrawer(1, color);
            }



            IntPtr hIcon = bitmap.GetHicon();
            var icon = Icon.FromHandle(hIcon);

            return ScaleIconToDpi(icon);


            //
            using (var bitmap = new Bitmap(16, 16))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    if (totalDesktopCount < 10)
                    {
                        DrawHorizontalInfo(graphics, currentDesktop, totalDesktopCount, color);
                    }
                    else
                    {
                        DrawVerticalInfo(graphics, currentDesktop, totalDesktopCount, color);
                    }

                    IntPtr hIcon = bitmap.GetHicon();
                    var icon = Icon.FromHandle(hIcon);

                    return ScaleIconToDpi(icon);
                }
            }
        }

        private static Icon ScaleIconToDpi(Icon targetIcon)
        {
			var dpi = PerMonitorDpi.GetDpi(IntPtr.Zero); // get desktop dpi

            return new Icon(targetIcon, new Size((int)(16 * dpi.ScaleX), (int)(16 * dpi.ScaleY)));
        }

        private static Image DrawHorizontalInfo(int currentDesktop, int totalDesktopCount)
        {
            var digitSize = new Size(4, 13);

            var currentDesktopImage = _digitDrawer.GetDigit(currentDesktop, digitSize);
            var totalDesktopsImage = _digitDrawer.GetDigit(totalDesktopCount, digitSize);

            using (var image = new Bitmap(16, 16))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.DrawImage(currentDesktopImage, new Point(1, 1));
                    graphics.DrawImage(totalDesktopsImage, new Point(9, 1));

                    DrawSeparator(graphics, Color.DimGray);
                }

                return image;
            }
        }

        private static void DrawSeparator(Graphics targetImage, Color color)
        {
            using (var pen = new Pen(color))
            {
                targetImage.DrawLine(pen, new Point(7, 15), new Point(8, 0));
            }
        }

        private static void DrawVerticalInfo(Graphics targetImage, int currentDesktop, int totalDesktops, Color color)
        {
            using (var pen = new Pen(color))
            {
                DrawNumber(targetImage, new Point(12, 0), currentDesktop, pen);
                DrawNumber(targetImage, new Point(12, 8), totalDesktops, pen);
            }
        }

        private static void DrawNumber(Graphics targetImage, Point startPoint, int number, Pen pen, int digitWidth = 3, int digitHeight = 6)
        {
            var digits = ExtractDigits(number);

            for (int index = 0; index <= digits.Count-1; index++)
            {
                var offset = index * (digitWidth + 2);
                var startPointWithOffset = new Point(startPoint.X - offset, startPoint.Y);

                DrawDigit(targetImage, startPointWithOffset, digits[index], pen, digitWidth, digitHeight);
            }
        }

        public static List<int> ExtractDigits(int value)
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

        private static void DrawDigit(Graphics targetImage, Point startPoint, int digit, Pen pen, int width, int height)
        {
            /* Common positions for digit drawing
             * 0 1
             * 2 3
             * 4 5
             */
            var positions = new Point[]
            {
                startPoint,
                new Point(startPoint.X + width, startPoint.Y),
                new Point(startPoint.X, startPoint.Y + height/2),
                new Point(startPoint.X + width, startPoint.Y + height/2),
                new Point(startPoint.X, startPoint.Y + height),
                new Point(startPoint.X + width, startPoint.Y + height),
            };

            switch (digit)
            {
                case 0:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[0],
                        positions[1],
                        positions[5],
                        positions[4],
                        positions[0],
                    });
                    break;
                case 1:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[1],
                        positions[5],
                    });
                    break;
                case 2:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[0],
                        positions[1],
                        positions[3],
                        positions[2],
                        positions[4],
                        positions[5],
                    });
                    break;
                case 3:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[0],
                        positions[1],
                        positions[3],
                        positions[2],
                        positions[3],
                        positions[5],
                        positions[4],
                    });
                    break;
                case 4:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[0],
                        positions[2],
                        positions[3],
                        positions[1],
                        positions[5],
                    });
                    break;
                case 5:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[1],
                        positions[0],
                        positions[2],
                        positions[3],
                        positions[5],
                        positions[4],
                    });
                    break;
                case 6:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[1],
                        positions[0],
                        positions[4],
                        positions[5],
                        positions[3],
                        positions[2],
                    });
                    break;
                case 7:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[0],
                        positions[1],
                        positions[5],
                    });
                    break;
                case 8:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[2],
                        positions[3],
                        positions[1],
                        positions[0],
                        positions[4],
                        positions[5],
                        positions[3],
                    });
                    break;
                case 9:
                    targetImage.DrawLines(pen, new Point[]
                    {
                        positions[4],
                        positions[5],
                        positions[1],
                        positions[0],
                        positions[2],
                        positions[3],
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(digit));
            }
        }
	}
}
