using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace AIStudio.Wpf.Controls
{
    class IdiomsText
    {
        public static Lazy<Dictionary<string, string>> Idioms = new Lazy<Dictionary<string, string>>(GetIdioms);

        public static Dictionary<string, string> GetIdioms()
        {
            var stream = Application.GetResourceStream(new Uri("/AIStudio.Wpf.Controls;component/Controls/Verify/Idioms.txt", UriKind.Relative))?.Stream;
            if (stream == null) return new Dictionary<string, string> { };

#if  NETCOREAPP
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

            Dictionary<string, string> dic = new Dictionary<string, string>();
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("GB2312")))
            {
                string line;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            dic.Add(line.Substring(0, line.IndexOf(" ")), line.IndexOf("释义") >= 0 ? line.Substring(line.IndexOf("释义")).Trim() : "");


                        }
                        catch
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }

            return dic;
        }
    }
}
