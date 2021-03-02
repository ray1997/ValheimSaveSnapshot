using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ValheimSaveSnapshot.Converters
{
	public class ItemCountToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IList)
			{
				int count = (value as IList).Count;
				if (parameter is Inverter)
				{
					return count < 1 ? Visibility.Visible : Visibility.Collapsed;
				}
				else
				{
					return count < 1 ? Visibility.Collapsed : Visibility.Visible;
				}
			}
			else if (value is null)
				return Visibility.Collapsed;
			return parameter is Inverter ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
