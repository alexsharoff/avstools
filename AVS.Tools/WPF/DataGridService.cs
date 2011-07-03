using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AVS.Tools.WPF
{
	public static class DataGridService
	{
		public static readonly DependencyProperty DataGridDoubleClickProperty =
			DependencyProperty.RegisterAttached("DataGridDoubleClickCommand", typeof ( ICommand ), typeof ( DataGridService ),
			                            new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDataGridDoubleClickEvent)));

		public static ICommand GetDataGridDoubleClickCommand(DependencyObject obj)
		{
			return (ICommand) obj.GetValue(DataGridDoubleClickProperty);
		}

		public static void SetDataGridDoubleClickCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(DataGridDoubleClickProperty, value);
		}
		
		public static void AttachOrRemoveDataGridDoubleClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			DataGrid dataGrid = obj as DataGrid;
			if ( dataGrid != null )
			{
				ICommand cmd = (ICommand) args.NewValue;

				if ( args.OldValue == null && args.NewValue != null )
				{
					dataGrid.MouseDoubleClick += ExecuteDataGridDoubleClick;
				}
				else if ( args.OldValue != null && args.NewValue == null )
				{
					dataGrid.MouseDoubleClick -= ExecuteDataGridDoubleClick;
				}
			}
		}

		private static void ExecuteDataGridDoubleClick(object sender, MouseButtonEventArgs args)
		{
			DependencyObject obj = sender as DependencyObject;
			ICommand cmd = (ICommand) obj.GetValue(DataGridDoubleClickProperty);
			if ( cmd != null )
			{
				if ( cmd.CanExecute(obj) )
				{
					cmd.Execute(obj);
				}
			}
		}

		
		
		public static readonly DependencyProperty PreviousSelectedItemProperty = DependencyProperty.RegisterAttached(
			"PreviousSelectedItem",
			typeof(object),
			typeof(DataGridService));

		public static object GetPreviousSelectedItem(DependencyObject d)
		{
			return (object)d.GetValue(PreviousSelectedItemProperty);
		}
		public static void SetPreviousSelectedItem(DependencyObject d, object value)
		{
			d.SetValue(PreviousSelectedItemProperty, value);
		}




		/// <summary>
		/// SelectedItems Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty NavigateToSelectedItemProperty = DependencyProperty.RegisterAttached(
			"NavigateToSelectedItem",
			typeof(bool),
			typeof(DataGridService),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnNavigateToSelectedItemChanged)));

		/// <summary>
		/// Gets the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
		/// <returns>The value of the SelectedItems property</returns>
		public static bool GetNavigateToSelectedItem(DependencyObject d)
		{
			return (bool)d.GetValue(NavigateToSelectedItemProperty);
		}

		/// <summary>
		/// Sets the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
		/// <param name="value">value of the property</param>
		public static void SetNavigateToSelectedItem(DependencyObject d, bool value)
		{
			d.SetValue(NavigateToSelectedItemProperty, value);
		}

		/// <summary>
		/// Handles changes to the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
		/// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
		private static void OnNavigateToSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = (DataGrid)d;
			if (GetNavigateToSelectedItem(grid) == true)
				grid.SelectionChanged += (s, arg) =>
			{
				var g = s as DataGrid;
				if (g.SelectedItem != null && g.SelectedItem != GetPreviousSelectedItem(d))
					g.ScrollIntoView(g.SelectedItem);

				SetPreviousSelectedItem(d, g.SelectedItem);
			};
		}


		#region SelectedItems

		/// <summary>
		/// SelectedItems Attached Dependency Property
		/// </summary>
		public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
			"SelectedItems",
			typeof(IList),
			typeof(DataGridService),
			new FrameworkPropertyMetadata((IList)null, new PropertyChangedCallback(OnSelectedItemsChanged)));

		/// <summary>
		/// Gets the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> to get the property from</param>
		/// <returns>The value of the SelectedItems property</returns>
		public static IList GetSelectedItems(DependencyObject d)
		{
			return (IList)d.GetValue(SelectedItemsProperty);
		}

		/// <summary>
		/// Sets the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> to set the property on</param>
		/// <param name="value">value of the property</param>
		public static void SetSelectedItems(DependencyObject d, IList value)
		{
			d.SetValue(SelectedItemsProperty, value);
		}

		/// <summary>
		/// Handles changes to the SelectedItems property.
		/// </summary>
		/// <param name="d"><see cref="DependencyObject"/> that fired the event</param>
		/// <param name="e">A <see cref="DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
		private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var grid = (DataGrid)d;
			ReSetSelectedItems(grid);
			grid.SelectionChanged += delegate
			{
				ReSetSelectedItems(grid);
			};
		}
		
		

		/// <summary>
		/// Sets the selected items collection for the specified <see cref="DataGrid"/>
		/// </summary>
		/// <param name="grid"><see cref="DataGrid"/> to use for setting the selected items</param>
		private static void ReSetSelectedItems(DataGrid grid)
		{
			IList selectedItems = GetSelectedItems(grid);
			if (selectedItems != null)
			{
				selectedItems.Clear();
				if (grid.SelectedItems != null)
				{
					foreach (var item in grid.SelectedItems)
					{
						selectedItems.Add(item);
					}
				}
			}
		}

		#endregion
	}

}
