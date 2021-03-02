using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ValheimSaveSnapshot.Converters
{
	/// <summary>
	/// Convert from file name to display name
	/// </summary>
	public class NameDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is null || string.IsNullOrEmpty(value.ToString()))
				return "";
			if (value.ToString().Length == 1)
				return value.ToString().ToUpper();
			return $"{value.ToString().Substring(0,1).ToUpper()}{value.ToString().Substring(1, value.ToString().IndexOf(".") - 1)}";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
