using System;
using System.Drawing;
using System.Windows.Controls;
using SylphyHorn.Interop;

namespace SylphyHorn.UI
{
	public class DynamicInfoTrayIcon : IDisposable
	{
		private const string _defaultFontFamilyName = "Segoe UI";
		private const float _horizontalFontSize = 7;
		private const float _verticalFontSize = 6;

		private readonly FontFamily _fontFamily;
		private readonly Brush _brush;
		private Font _font;
		private Orientation _lastOrientation;

		public DynamicInfoTrayIcon(int totalDesktopCount, FontFamily fontFamily = null, Color? color = null)
		{
			var currentOrientation = this._lastOrientation = GetOrientation(totalDesktopCount);
			var fontSize = GetFontSize(currentOrientation);

			this._fontFamily = fontFamily ?? new FontFamily(_defaultFontFamilyName);
			this._font = new Font(this._fontFamily, fontSize, FontStyle.Bold);

			if (!color.HasValue)
			{
				color = Color.White;
			}

			this._brush = new SolidBrush(color.Value);
		}

		public Icon GetDesktopInfoIcon(int currentDesktop, int totalDesktopCount)
		{
			var currentOrientation = GetOrientation(totalDesktopCount);

			if (currentOrientation != this._lastOrientation)
			{
				this.UpdateFontSize(currentOrientation);
			}

			this._lastOrientation = currentOrientation;

			using (var iconBitmap = currentOrientation == Orientation.Horizontal
				? this.DrawHorizontalInfo(currentDesktop, totalDesktopCount)
				: this.DrawVerticalInfo(currentDesktop, totalDesktopCount))
			{
				return iconBitmap.ToIcon();
			}
		}

		private void UpdateFontSize(Orientation newOrientation)
		{
			var fontSize = GetFontSize(newOrientation);

			this._font?.Dispose();
			this._font = new Font(this._fontFamily, fontSize, FontStyle.Bold);
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
				graphics.DrawString(stringToDraw, this._font, this._brush, offset);
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
				graphics.DrawString(firstString, this._font, this._brush, firstOffset);
				graphics.DrawString(secondString, this._font, this._brush, secondOffset);
			}

			return bitmap;
		}

		public void Dispose()
		{
			this._fontFamily?.Dispose();
			this._font?.Dispose();
			this._brush?.Dispose();
		}

		private static Orientation GetOrientation(int totalDesktopCount)
		{
			return totalDesktopCount >= 10 ? Orientation.Vertical : Orientation.Horizontal;
		}

		private static float GetFontSize(Orientation orientation)
		{
			return orientation == Orientation.Horizontal ? _horizontalFontSize : _verticalFontSize;
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

			offset.Y += bitmapHeight / 2f;

			return offset;
		}
	}
}
