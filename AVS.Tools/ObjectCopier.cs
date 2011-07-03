using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AVS.Tools
{
    public static class ObjectCopier
    {
        public static T Clone<T>(this T source)
        {
            if (Object.ReferenceEquals(source, null))
                return default(T);

            using (var writer = new StringWriter())
            {
                Serializer.Serialize(source, writer);
                using (var reader = new StringReader(writer.ToString()))
                {
                    return (T)Serializer.Deserialize<T>(reader);
                }
            }
        }
    }    

}
