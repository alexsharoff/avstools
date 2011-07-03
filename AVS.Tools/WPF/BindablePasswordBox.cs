using System.Windows;
using System.Windows.Controls;

namespace AVS.Tools.WPF
{
    public class BindablePasswordBox : Decorator
    {
        internal volatile bool Lock = false;

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PasswordPropertyChangedCallback));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public BindablePasswordBox()
        {
            Child = new PasswordBox();
            ((PasswordBox)Child).PasswordChanged += PasswordBox_PasswordChanged;
        }

        void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Lock = true;
            Password = ((PasswordBox)Child).Password;
            Lock = false;
        }
        static void PasswordPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bpx = d as BindablePasswordBox;
            if (!bpx.Lock)
                (bpx.Child as PasswordBox).Password = bpx.Password;

        }
    }
}
