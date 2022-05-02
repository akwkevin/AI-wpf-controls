using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace AIStudio.Wpf.Controls
{
    public static class EmojiDataFactory
    {
        internal static Func<IDictionary<string, string>> Create
        {
            get
            {
                return new Func<IDictionary<string, string>>(() => {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    try
                    {
                        //获得正在运行类所在的名称空间
                        Type type = MethodBase.GetCurrentMethod().DeclaringType;
                        string _namespace = type.Namespace;
                        //获得当前运行的Assembly
                        Assembly _assembly = Assembly.GetExecutingAssembly();
                        //根据名称空间和文件名生成资源名称
                        string resourceName = _namespace + ".Emoji.emoji.json";
                        //根据资源名称从Assembly中获取此资源的Stream
                        Stream stream = _assembly.GetManifestResourceStream(resourceName);
                        StreamReader reader = new StreamReader(stream);
                        string text = reader.ReadToEnd();
                        dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                    }
                    catch { }
                    return dic;
                });
            }
        }

        private static Lazy<IDictionary<string, string>> _dataIndex;
        public static Lazy<IDictionary<string, string>> DataIndex
        {
            get
            {
                if (_dataIndex == null)
                    _dataIndex = new Lazy<IDictionary<string, string>>(EmojiDataFactory.Create);

                return _dataIndex;
            }
        }

        private static string _dataPattern;// = "/::\\)|/::~|/::B";
        public static string DataPattern
        {
            get
            {
                if (_dataPattern == null)
                    _dataPattern = "(" + string.Join(")|(",
                        DataIndex.Value.Keys.Select(p => p
                        .Replace("$", "\\$")
                        .Replace("(", "\\(")
                        .Replace(")", "\\)")
                        .Replace("*", "\\*")
                        .Replace("+", "\\+")
                        .Replace(".", "\\.")
                        .Replace("?", "\\?")
                        .Replace("|", "\\|")
                    )) + ")";

                return _dataPattern;
            }
        }
    }
}
