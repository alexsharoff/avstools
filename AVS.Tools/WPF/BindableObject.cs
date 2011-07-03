using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace AVS.Tools.WPF
{
    public abstract class BindableObject : INotifyPropertyChanged
    {
        public bool PreventPropertyChangedEvents
        {
            get;
            set;
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PreventPropertyChangedEvents)
                return;
#if DEBUG
            if (GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName) == null)
            {
                throw new Exception(
                    string.Format("Property {0} does not exist on object of type {1}.",
                    propertyName, GetType()));
            }
#endif
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
