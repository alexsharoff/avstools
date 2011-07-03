using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AVS.Tools.WPF
{
    public class EasyGrid : Grid
    {
        int m_nextColumn = 0;
        int m_nextRow = 0;
        protected override void OnVisualChildrenChanged(
            System.Windows.DependencyObject visualAdded,
            System.Windows.DependencyObject visualRemoved)
        {
            if (visualAdded as UIElement != null)
            {
                if (m_nextRow == RowDefinitions.Count)
                    RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(0, GridUnitType.Auto)
                    });
                if (visualAdded is EasyGridPlaceHolder)
                    Children.Remove(visualAdded as EasyGridPlaceHolder);
                else
                {
                    var e = visualAdded as UIElement;
                    SetRow(e, m_nextRow);
                    SetColumn(e, m_nextColumn);
                    int rs = Grid.GetRowSpan(e);
                    int cs = Grid.GetColumnSpan(e);
                    if (cs > 1)
                        m_nextColumn += cs - 1;
                    if (rs > 1)
                        m_nextRow += rs - 1;
                }
                m_nextColumn++;
                if (m_nextColumn >= ColumnDefinitions.Count)
                {
                    m_nextColumn = 0;
                    m_nextRow++;
                }
            }
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
