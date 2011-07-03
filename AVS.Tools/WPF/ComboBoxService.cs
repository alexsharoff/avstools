using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AVS.Tools.WPF
{
	public static class ComboBoxService
	{
		public static readonly DependencyProperty ComboBoxDropDownClosedCommandProperty =
			DependencyProperty.RegisterAttached("DropDownClosedCommand", typeof ( ICommand ), typeof ( ComboBoxService ),
			                                    new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDropDownClosedEvent)));

		public static ICommand GetDropDownClosedCommand(DependencyObject obj)
		{
			return (ICommand) obj.GetValue(ComboBoxDropDownClosedCommandProperty);
		}

		public static void SetDropDownClosedCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(ComboBoxDropDownClosedCommandProperty, value);
		}
		
		public static void AttachOrRemoveDropDownClosedEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ComboBox cb = obj as ComboBox;
			if ( cb != null )
			{
				ICommand cmd = (ICommand) args.NewValue;
				if ( args.OldValue == null && args.NewValue != null )
					cb.DropDownClosed += ExecuteDropDownClosed;
				else if ( args.OldValue != null && args.NewValue == null )
					cb.DropDownClosed -= ExecuteDropDownClosed;
			}
		}

		private static void ExecuteDropDownClosed(object sender, EventArgs e)
		{
			DependencyObject obj = sender as DependencyObject;
			ICommand cmd = (ICommand) obj.GetValue(ComboBoxDropDownClosedCommandProperty);
			if ( cmd != null )
			{
				if ( cmd.CanExecute(obj) )
				{
					cmd.Execute(obj);
				}
			}
		}

		
	}
}
