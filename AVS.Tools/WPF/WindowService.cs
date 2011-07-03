using System;
using System.Windows;

namespace AVS.Tools.WPF
{
	public class WindowService : DependencyObject
	{
		public static readonly DependencyProperty IsMainWindowProperty = DependencyProperty.Register(
			"IsMainWindow",
			typeof(bool),
			typeof(Window),
			new FrameworkPropertyMetadata(false,
			                              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static bool GetIsMainWindowProperty(DependencyObject d)
		{
			return (bool)d.GetValue(IsMainWindowProperty);
		}
		public static void SetIsMainWindowProperty(DependencyObject d, bool value)
		{
			d.SetValue(IsMainWindowProperty, value);
		}
		
		public bool IsMainWindow
		{
			get { return (bool)GetValue(IsMainWindowProperty); }
			set { SetValue(IsMainWindowProperty, value); }
		}
	}
}
