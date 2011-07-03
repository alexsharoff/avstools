using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Collections;

namespace AVS.Tools.WPF
{
    public class StringToArrayConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enumerable.Aggregate((value as IEnumerable<string>), string.Empty, (str, a) =>
            {
                if (str.Length > 0) str += ","; return str + a;
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Activator.CreateInstance(targetType,
                (value as string).
                Split(new char[] { ',' }).
                Select(a => a.Trim()).
                Where(a => a.Length > 0));
        }

        #endregion
    }
}
