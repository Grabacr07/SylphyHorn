using SylphyHorn.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SylphyHorn.UI
{
    // wip
    public class DynamicInfoTrayIcon : IDisposable
    {
        private Orientation _lastOrientation;

        private FontFamily _fontFamily;
        private Font _font;
        private Brush _brush;

        private const string DefaultFontFamilyName = "Segoe UI";
        private const float HorizontalFontSize = 7;
        private const float VerticalFontSize = 6;

        public DynamicInfoTrayIcon(int totalDesktopCount, FontFamily fontFamily = null, Color? color = null)
        {
            if (_fontFamily == null)
            {
                _fontFamily = new FontFamily(DefaultFontFamilyName);
            }

            var currentOrientation = _lastOrientation = GetOrientation(totalDesktopCount);
            var fontSize = GetFontSize(currentOrientation);

            _font = new Font(_fontFamily, fontSize, FontStyle.Bold);

            if (!color.HasValue)
            {
                color = Color.White;
            }

            _brush = new SolidBrush(color.Value);
        }

        public Icon GetDesktopInfoIcon(int currentDesktop, int totalDesktopCount)
        {
            var currentOrientation = GetOrientation(totalDesktopCount);

            if (currentOrientation != _lastOrientation)
            {
                UpdateFontSize(currentOrientation);
            }

            _lastOrientation = currentOrientation;

            using (var iconBitmap = currentOrientation == Orientation.Horizontal
                ? DrawHorizontalInfo(currentDesktop, totalDesktopCount)
                : DrawVerticalInfo(currentDesktop, totalDesktopCount))
            {
                return iconBitmap.ToIcon();
            }
        }

        private Orientation GetOrientation(int totalDesktopCount)
        {
            return totalDesktopCount >= 10 ? Orientation.Vertical : Orientation.Horizontal;
        }

        private float GetFontSize(Orientation orientation)
        {
            return orientation == Orientation.Horizontal ? HorizontalFontSize : VerticalFontSize;
        }

        private void UpdateFontSize(Orientation newOrientation)
        {
            var fontSize = GetFontSize(newOrientation);

            _font?.Dispose();
            _font = new Font(_fontFamily, fontSize, FontStyle.Bold);
        }

        private void UpdateBrush(Color newColor)
        {
            _brush?.Dispose();
            _brush = new SolidBrush(newColor);
        }

        // consolidate two methods below?
        private Bitmap DrawHorizontalInfo(int currentDesktop, int totalDesktopCount)
        {
            var iconSize = IconHelper.GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var stringToDraw = $"{currentDesktop}/{totalDesktopCount}";

            var offset = GetHorizontalStringOffset();

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawString(stringToDraw, _font, _brush, offset);
            }

            return bitmap;
        }

        private Bitmap DrawVerticalInfo(int currentDesktop, int totalDesktops)
        {
            var iconSize = IconHelper.GetIconSize();
            var bitmap = new Bitmap((int)iconSize.Width, (int)iconSize.Height);

            var firstOffset = GetFirstVerticalStringOffset(currentDesktop);
            var secondOffset = GetSecondVerticalStringOffset(totalDesktops, bitmap.Height);

            var firstString = currentDesktop.ToString();
            var secondString = totalDesktops.ToString();

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawString(firstString, _font, _brush, firstOffset);
                graphics.DrawString(secondString, _font, _brush, secondOffset);
            }

            return bitmap;
        }

        private PointF GetHorizontalStringOffset()
        {
            return new PointF(-2, 0);
        }

        private PointF GetFirstVerticalStringOffset(int value)
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

        private PointF GetSecondVerticalStringOffset(int value, int bitmapHeight)
        {
            var offset = GetFirstVerticalStringOffset(value);

            offset.Y += bitmapHeight / 2;

            return offset;
        }

        public void Dispose()
        {
            _fontFamily?.Dispose();
            _font?.Dispose();
            _brush?.Dispose();
        }
    }
}
