using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ObjectDictionary.Converter
{
    class BooleanToCheckConverter : IValueConverter
    {
        private const string PositiveString = "✓";
        private const string NegativeString = "◯";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return PositiveString;
            }

            return NegativeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string) == PositiveString;
        }
    }
}
