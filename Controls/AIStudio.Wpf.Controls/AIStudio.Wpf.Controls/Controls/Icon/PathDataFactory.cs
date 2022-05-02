using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    class PathDataFactory
    {
        readonly static Dictionary<string, string> icons = new Dictionary<string, string>() {
            {"Geometries", "/AIStudio.Wpf.Controls;component/Themes/Geometries.xaml" },
        };
        internal static Func<IDictionary<Tuple<string, string>, Geometry>> Create
        {
            get
            {
                return new Func<IDictionary<Tuple<string, string>, Geometry>>(() => {
                    Dictionary<Tuple<string, string>, Geometry> dic = new Dictionary<Tuple<string, string>, Geometry>();
                    try
                    {
                        foreach (var icon in icons)
                        {
                            ResourceDictionary rd = new ResourceDictionary();
                            rd.Source = new Uri(icon.Value, UriKind.Relative);


                            foreach (var key in rd.Keys)
                            {
                                var geometry = rd[key] as System.Windows.Media.Geometry;
                                if (geometry != null)
                                {
                                    dic.Add(new Tuple<string, string>(icon.Key, key.ToString().Replace("Geometry", "")), geometry);
                                }
                            }
                        }
                    }
                    catch { }
                    return dic;
                });
            }
        }
    }
}
