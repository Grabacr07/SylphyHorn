using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using SylphyHorn.Services;

namespace SylphyHorn.UI.Controls
{
	public class IndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
			  object parameter, CultureInfo culture)
		{
			return (int)value + 1;
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
