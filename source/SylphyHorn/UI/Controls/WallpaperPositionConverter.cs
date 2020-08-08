using SylphyHorn.Services;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SylphyHorn.UI.Controls
{
	public class WallpaperPositionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var position = (WallpaperPosition)value;
			switch (position)
			{
				case WallpaperPosition.Center:
					return Stretch.None;

				case WallpaperPosition.Stretch:
					return Stretch.Fill;

				case WallpaperPosition.Fill:
				case WallpaperPosition.Span:
					return Stretch.UniformToFill;

				case WallpaperPosition.Fit:
				default:
					return Stretch.Uniform;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
