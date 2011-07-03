using System.ComponentModel;
using System;

namespace AVS.Tools.WPF
{
    public abstract class ObservableObject : BindableObject
    {
        public ObservableObject()
        {
            PropertyChanged += HasChangedHandler;
        }
        bool m_isRemoved;
        public bool IsRemoved
        {
            get { return m_isRemoved; }
            set
            {
                m_isRemoved = value;
                OnPropertyChanged("IsRemoved");
            }
        }
        bool m_hasChanged;
        public bool HasChanged
        {
            get
            {
                return m_hasChanged;
            }
            set
            {
                m_hasChanged = value;
                OnPropertyChanged("HasChanged");
            }
        }

        public void HasChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (HasChanged != true && e.PropertyName != "HasChanged")
                HasChanged = true;
        }
    }
}
