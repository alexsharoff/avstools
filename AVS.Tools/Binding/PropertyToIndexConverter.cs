using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Reflection;

namespace AVS.Tools.Binding
{
    /*
     * USAGE:
     * <DataGridTextColumn Header="Name" 
     * Binding="{Binding Path=Dictionary, 
     * Converter={StaticResource PropertyToIndexConverter}, ConverterParameter=Name}"/>
     * */
    public class PropertyToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var row = value as IDictionary<string, object>;
            var key = parameter.ToString();
            return row.Keys.Contains(key) ? row[key] : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new IndexChange(parameter as string, value);
        }
    }
}
