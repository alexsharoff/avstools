using System;
using System.Globalization;
using System.Windows.Data;

namespace AVS.Tools.WPF
{
    public class TargetTypeConverter : IValueConverter
    {
        object ChangeType(object value, Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
                return value;

            if (targetType == typeof(string))
            {
                try
                {
                    return value.ToString();
                }
                catch { }
            }
            if (value != DBNull.Value)
            {
                try
                {
                    return System.Convert.ChangeType(value, targetType);
                }
                catch { }
            }
            if (targetType != typeof(object))
            {
                try
                {
                    return Activator.CreateInstance(targetType);
                }
                catch { }
            }

            return null;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ChangeType(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ChangeType(value, targetType);
        }
    }
}
