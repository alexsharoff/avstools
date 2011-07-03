using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVS.Tools.WPF
{
    public class EnumWithDescription
    {
        public object Value
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
    }
    public class EnumHelper
    {
        public string NullValueDescription
        {
            get;
            set;
        }
        public IEnumerable<EnumWithDescription> GetValuesWithDescription(Type enumType)
        {
            List<EnumWithDescription> list = new List<EnumWithDescription>();
            foreach (Enum value in Enum.GetValues(enumType))
            {
                list.Add(new EnumWithDescription()
                {
                     Description = ReflectionHelper.GetDescription(value),
                     Value = value
                });
            }
            return list;
        }
        public IEnumerable<EnumWithDescription> GetNullableValuesWithDescription(Type enumType)
        {
            List<EnumWithDescription> list = new List<EnumWithDescription>();
            list.Add(new EnumWithDescription()
            {
                Value = null,
                Description = NullValueDescription
            });
            list.AddRange(GetValuesWithDescription(enumType));
            return list;
        }
    }
}
