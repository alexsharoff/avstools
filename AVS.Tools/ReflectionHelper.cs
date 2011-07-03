using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace AVS.Tools
{
    public static class ReflectionHelper
    {
        public static List<object> GetUniquePropertyValues<T>(IEnumerable<T> o, string path)
        {
            var p = typeof(T).GetProperty(path);
            return new List<object>(o.Select(i => p.GetValue(i, null)).Where(i => i != null).Distinct());
        }
        
        public static List<string> GetDescriptions(Type enumType)
        {
            List<string> descr = new List<string>();
            foreach (var memInfo in enumType.GetMembers())
            {
                object[] attrs = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    descr.Add(((DescriptionAttribute)attrs[0]).Description);
                }
            }
            return descr;
        }

        public static string GetDescription(Enum en)
        {
            if (en == null)
                return null;
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return en.ToString();
        }

        public static string DescribeObject(object o)
        {
            string description = string.Empty;
            if (o == null)
                return description;
            string fullClassName = TypeDescriptor.GetClassName(o);
            string[] split = fullClassName.Split(new char[1] { '.' });
            string objectName = split[split.Length - 1];

            PropertyDescriptor defaultDescriptor = TypeDescriptor.GetDefaultProperty(o);
            if (defaultDescriptor != null)
            {
                objectName += " " + defaultDescriptor.GetValue(o);
            }
            description += string.Format("{0}: ", objectName);
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(o))
            {
                string value = GetTextValue(descriptor.GetValue(o));
                if (value.Length > 0)
                {
                    string name = descriptor.Name;
                    description += string.Format("{0}={1}; ", name, value);
                }
            }

            return description;
        }

        public static object InvokeStaticMethod<T>(string methodName, object[] args)
        {
            return typeof(T).InvokeMember(methodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                null, null, args);
        }

        public static object GetStaticProperty<T>(string propertyName)
        {
            return typeof(T).InvokeMember(propertyName,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty,
                    null, null, null);
        }

        public static bool EqualityByProperties<T>(T a, T b, params string[] ignore)
        {
            if (a != null && b != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object aValue = type.GetProperty(pi.Name).GetValue(a, null);
                        object bValue = type.GetProperty(pi.Name).GetValue(b, null);

                        if ((aValue == null || bValue == null) && aValue != bValue)
                        {
                            return false;
                        }

                        if (!(aValue is IEnumerable) || aValue is string)
                        {
                            if (aValue != bValue && !aValue.Equals(bValue))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private static string GetTextValue(object value)
        {
            string printValue = string.Empty;
            if (value == null)
            {
                return string.Empty;
            }
            if (value is string)
            {
                return value.ToString();
            }
            else
                if (value is IEnumerable)
                {
                    foreach (object o in (value as IEnumerable))
                    {
                        string p = GetTextValue(o);
                        if (p.Length > 0)
                        {
                            if (printValue.Length > 0)
                            {
                                printValue += ", ";
                            }
                            printValue += p;
                        }
                    }
                    if (printValue.Length > 0)
                    {
                        printValue = string.Format("[{0}]", printValue);
                    }
                }
                else
                {
                    printValue += value.ToString();
                }
            return printValue;
        }

        public static object ReadValue(object item, string property_name)
        {
            return ReadValue<object>(item, property_name);
        }

        public static object ReadValue<T>(object item, string property_name)
        {
            return (T)item.GetType().GetProperty(property_name).GetValue(item, null);
        }

        public static PropertyInfo[] AllProperties(object item)
        {
            return item.GetType().GetProperties();
        }

        public static object NewObject(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static Type[] GetGenericParameters(Type type)
        {
            return type.GetGenericArguments();
        }

        public static object AddNewItemToCollection(object collection)
        {
            Type itemType = GetGenericParameters(collection.GetType())[0];
            object newObject = NewObject(itemType);
            InvokeMethod(collection, "Add", newObject);
            return newObject;
        }

        public static bool PropertyExists(Type type, string property_name)
        {
            return type.GetProperty(property_name) != null;
        }

        public static object InvokeMethod(object item, string method, params object[] parameters)
        {
            return InvokeMethod<object>(item, method, parameters);
        }

        public static T InvokeMethod<T>(object item, string method, params object[] parameters)
        {
            return (T)item.GetType().GetMethod(method).Invoke(item, parameters);
        }

        public static bool HasMethod(Type type, string method)
        {
            return type.GetMethod(method) != null;
        }
    }
}
