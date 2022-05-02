using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace AIStudio.Wpf.Controls
{
    class IconDataFactory
    {
        readonly static Dictionary<string, string> icons = new Dictionary<string, string>() {
            {"Awesome", "/AIStudio.Wpf.Controls;component/Resources/variables.less" },
        };
        internal static Func<IDictionary<Tuple<string, string>, string>> Create
        {
            get
            {
                return new Func<IDictionary<Tuple<string, string>, string>>(() => {
                    Dictionary<Tuple<string, string>, string> dic = new Dictionary<Tuple<string, string>, string>();
                    try
                    {
                        foreach (var icon in icons)
                        {
                            var stream = Application.GetResourceStream(new Uri(icon.Value, UriKind.Relative))?.Stream;

                            string str;
                            using (var reader = new StreamReader(stream))
                            {
                                str = reader.ReadToEnd();
                            }

                            var datas = str.Split(new string[] { "@fa-var-" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var data in datas)
                            {
                                var items = data.Split(new string[] { ": ", ";", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                                if (items.Length == 2)
                                {
                                    dic.Add(new Tuple<string, string>(icon.Key, items[0]), Regex.Unescape("\\u" + items[1].Replace("\\", "").Replace("\"", "")));
                                }
                            }
                            //if ((file.Item1 == "fill" || file.Item1 == "outline" || file.Item1 == "twotone") && !dic.ContainsKey(new Tuple<string, string>(file.Item1, file.Item2)))
                            //    dic.Add(new Tuple<string, string>(file.Item1, file.Item2), file.Item3);
                            //else if (!dic.ContainsKey(new Tuple<string, string>("", file.Item2)))
                            //    dic.Add(new Tuple<string, string>("", file.Item2), file.Item3);
                        }
                    }
                    catch { }
                    return dic;
                });
            }
        }
    }
}
