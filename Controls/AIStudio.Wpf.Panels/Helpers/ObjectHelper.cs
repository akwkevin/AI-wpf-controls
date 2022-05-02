using System.Reflection;
using System.Windows;

namespace AIStudio.Wpf.Panels.Helpers
{
    public static class ObjectHelper
    {
        private static BindingFlags _bindingFlags
        {
            get;
        }
          = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// 是否拥有某属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static bool ContainsProperty(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags) != null;
        }

        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj);
        }

        /// <summary>
        /// 设置某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName, _bindingFlags).SetValue(obj, value);
        }

        public static object GetPropertyValue(this FrameworkElement obj, string propertyName)
        {
            if (obj.DataContext.ContainsProperty(propertyName))
            {
                return obj.DataContext.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj.DataContext);
            }
            else if (obj.ContainsProperty(propertyName))
            {
                return obj.GetType().GetProperty(propertyName, _bindingFlags).GetValue(obj);
            }
            else
            {
                return null;
            }
        }

        public static void SetPropertyValue(this FrameworkElement obj, string propertyName, object value)
        {
            if (obj.DataContext.ContainsProperty(propertyName))
            {
                obj.DataContext.GetType().GetProperty(propertyName, _bindingFlags).SetValue(obj.DataContext, value);
            }
            else if (obj.ContainsProperty(propertyName))
            {
                obj.GetType().GetProperty(propertyName, _bindingFlags).SetValue(obj, value);
            }
        }

        public static bool EqualsPropertyValue(this FrameworkElement obj, string propertyName, object value)
        {
            var data = obj.GetPropertyValue(propertyName);
            if (object.Equals(data, value))
                return true;
            else
                return false;
        }
    }
}
