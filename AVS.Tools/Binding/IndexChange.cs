
namespace AVS.Tools.Binding
{
    public class IndexChange
    {
        public object Value
        {
            get;
            protected set;
        }

        public string PropertyName
        {
            get;
            protected set;
        }

        public IndexChange(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }
    }
}
