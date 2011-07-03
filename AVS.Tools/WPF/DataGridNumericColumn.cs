using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Windows.Controls;
using AVS.Tools.WPF;

namespace AVS.Tools.WPF
{
    public class DataGridNumericColumn : DataGridBoundColumn
    {
        public DataGridNumericColumn()
        {
            FormatString = "F0";
        }
        public double Maximum
        {
            get;
            set;
        }
        public double Minimum
        {
            get;
            set;
        }
        public double Increment
        {
            get;
            set;
        }
        public Type TargetType
        {
            get;
            set;
        }
        public string FormatString
        {
            get;
            set;
        }
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var numericEditor = new DoubleUpDown();
            numericEditor.Minimum = Minimum;
            numericEditor.Maximum = Maximum;
            numericEditor.Increment = Increment;
            numericEditor.FormatString = FormatString;
            var sourceBinding = this.Binding as System.Windows.Data.Binding;
            var binding = new System.Windows.Data.Binding();
            binding.Path = sourceBinding.Path;
            binding.Converter = new TargetTypeConverter();
            numericEditor.SetBinding(DoubleUpDown.ValueProperty, binding);
            return numericEditor;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            TextBlock txt = new TextBlock();
            txt.SetBinding(TextBlock.TextProperty, this.Binding);
            return txt;
        }
    }
}
