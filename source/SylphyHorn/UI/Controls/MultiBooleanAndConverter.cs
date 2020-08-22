using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SylphyHorn.UI.Controls
{
	public sealed class MultiBooleanAndConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			=> !values.Select(System.Convert.ToBoolean).Any(flag => !flag);

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
