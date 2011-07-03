using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AVS.Tools.WPF
{
    public class DataGridDateTimeColumn : DataGridBoundColumn
    {
        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            DatePicker dp = editingElement as DatePicker;
            if (dp != null)
            {
                dp.SelectedDate = DateTime.Parse(uneditedValue.ToString());
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            DatePicker dp = new DatePicker();
            dp.SetBinding(DatePicker.SelectedDateProperty, this.Binding);
            return dp;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            TextBlock txt = new TextBlock();
            txt.SetBinding(TextBlock.TextProperty, this.Binding);
            return txt;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            DatePicker dp = editingElement as DatePicker;
            if (dp != null)
            {
                DateTime? dt = dp.SelectedDate;
                if (dt.HasValue)
                    return dt.Value;
            }
            return DateTime.Today;
        }
    }
}
