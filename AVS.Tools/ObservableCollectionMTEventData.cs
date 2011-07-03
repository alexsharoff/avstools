using System;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Collections;

namespace AVS.Tools
{

    internal class ObservableCollectionMTCollectionRangeAddedEventData
    {
        #region Public Properties

        public Dispatcher Dispatcher
        {
            get;
            set;
        }

        public Action<IEnumerable> Action
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Constructor

        public ObservableCollectionMTCollectionRangeAddedEventData(Dispatcher dispatcher, Action<IEnumerable> action)
        {
            Dispatcher = dispatcher;
            Action = action;
        }

        #endregion Constructor
    }

    internal class ObservableCollectionMTCollectionChangedEventData
    {
        #region Public Properties

        public Dispatcher Dispatcher
        {
            get;
            set;
        }

        public Action<NotifyCollectionChangedEventArgs> Action
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Constructor

        public ObservableCollectionMTCollectionChangedEventData(Dispatcher dispatcher, Action<NotifyCollectionChangedEventArgs> action)
        {
            Dispatcher = dispatcher;
            Action = action;
        }

        #endregion Constructor
    }
}
