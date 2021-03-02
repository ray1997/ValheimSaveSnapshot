using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ValheimSaveSnapshot.Converters
{
	public class ItemCountToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IList)
			{
				int count = (value as IList).Count;
				if (parameter is Inverter)
				{
					return count < 1 ? true : false;
				}
				else
				{
					return count < 1 ? false : true;
				}
			}
			else if (value is null)
				return false;
			return parameter is Inverter ? true : false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
