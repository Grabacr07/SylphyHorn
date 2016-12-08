using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SylphyHorn.UI.Controls
{
	public class UnlockImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return null;

			using (var stream = new FileStream((string)value, FileMode.Open))
			{
				var decoder = BitmapDecoder.Create(
					stream,
					BitmapCreateOptions.None,
					BitmapCacheOption.OnLoad);

				var bitmap = new WriteableBitmap(decoder.Frames[0]);
				bitmap.Freeze();

				return bitmap;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
