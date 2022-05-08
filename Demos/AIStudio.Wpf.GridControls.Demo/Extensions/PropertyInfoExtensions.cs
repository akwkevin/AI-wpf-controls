using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace AIStudio.Wpf.GridControls.Demo.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static IDictionary<string, object> ModelToDic(this object o)
        {
            IDictionary<string, object> map = new Dictionary<string, object>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, p.GetValue(o, null));
                }
            }

            return map;
        }

        public static ExpandoObject DicToExpandoObject(this IDictionary<string, object> map)
        {
            ExpandoObject expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var keyvalue in map)
            {
                dictionary.Add(keyvalue.Key, keyvalue.Value);
            }

            return expando;
        }


        /// <summary>
        /// 实体属性反射
        /// </summary>
        /// <typeparam name="S">赋值对象</typeparam>
        /// <typeparam name="T">被赋值对象</typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public static void CopyTo<S, T>(this S s, T t)
        {
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);

                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
        }

        public static void CopyTo<T>(this Dictionary<string, object> s, T t)
        {
            PropertyInfo[] pps = GetPropertyInfos(t.GetType());

            foreach (var pp in pps)
            {
                object value = s[pp.Name];
                if (value != null)
                {
                    pp.SetValue(t, value, null);
                }
            }
        }
    }
}
