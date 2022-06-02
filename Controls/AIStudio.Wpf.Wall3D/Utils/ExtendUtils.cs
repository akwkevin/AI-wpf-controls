using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Wall3D.Utils
{
    public class ExtendUtils // 扩展类
    {
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}
