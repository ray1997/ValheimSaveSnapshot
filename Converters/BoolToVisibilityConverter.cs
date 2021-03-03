using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ValheimSaveSnapshot.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				if (parameter is Inverter)
				{
					return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
				}
				return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				if (parameter is Inverter)
					return ((Visibility)value) == Visibility.Visible ? false : true;
				return ((Visibility)value) == Visibility.Visible ? true : false;
			}
			return false;
		}
	}
}
