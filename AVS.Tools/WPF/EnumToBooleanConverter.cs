using System;
using System.Windows.Data;

namespace AVS.Tools.WPF
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.Equals(false))
                return System.Windows.Data.Binding.DoNothing;
            else
                return parameter;
        }
    }
}