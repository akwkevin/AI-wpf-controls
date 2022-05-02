using AIStudio.Wpf.Controls.Demo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AIStudio.Wpf.Controls.Demo.Services
{
    internal class DataService
    {
        public static List<DemoInfoModel> GetDemoInfo()
        {
            var infoList = new List<DemoInfoModel>();

            var stream = Application.GetResourceStream(new Uri("/AIStudio.Wpf.Controls.Demo;component/DemoInfo.json", UriKind.Relative))?.Stream;
            if (stream == null) return infoList;

            string jsonStr;
            using (var reader = new StreamReader(stream))
            {
                jsonStr = reader.ReadToEnd();
            }

            var jsonObj = JsonConvert.DeserializeObject<List<DemoInfo>>(jsonStr);
            foreach (var item in jsonObj)
            {
                var titleKey = item.title;
                var imageName = item.image;
                var title = titleKey;
                var list = Convert2DemoItemList(item.demoItemList);
                infoList.Add(new DemoInfoModel
                {
                    Key = titleKey,
                    Label = title,
                    Glyph = imageName,
                    Children = list,
                    Level = 0,
                });
            }

            return infoList;
        }

        private static List<DemoInfoModel> Convert2DemoItemList(List<List<string>> list)
        {
            var resultList = new List<DemoInfoModel>();

            foreach (var item in list)
            {
                var name = item[0];
                string targetCtlName = item[1];
                string imageName = item[2];
                var isNew = !string.IsNullOrEmpty((string)item[3]);

                resultList.Add(new DemoInfoModel
                {
                    Label = name,
                    TargetCtlName = targetCtlName,
                    Glyph = imageName,
                    IsNew = isNew,
                    Level = 1,
                });
            }

            return resultList;
        }


        //F:\AIStudio.Wpf.Controls\Demos\AIStudio.Wpf.Controls.Demo\Views
        public static string GetCodeByFile(string demoName)
        {
            var directory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()).ToString()).ToString()).ToString();
            using (Stream s = new FileStream(Path.Combine(directory, "Demos", "AIStudio.Wpf.Controls.Demo", demoName), FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(s))
                {
                    var code = reader.ReadToEnd();
                    return code;
                }
            }
        }

        public static ObservableCollection<DemoDataModel> GetDemoDataList()
        {
            var list = new ObservableCollection<DemoDataModel>();
            for (var i = 1; i <= 20; i++)
            {
                var dataList = new ObservableCollection<DemoDataModel>();
                for (var j = 0; j < 3; j++)
                {
                    dataList.Add(new DemoDataModel
                    {
                        Index = j,
                        IsSelected = j % 2 == 0,
                        Name = $"SubName{j}",
                        ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{j % 5 + 1}.jpg",
                    });
                    for (var k = 0; k < 3; k++)
                    {
                        dataList[j].AddChild(new DemoDataModel
                        {
                            Index = k,
                            IsSelected = k % 2 == 0,
                            Name = $"SubSubName{k}",
                            ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{k % 5 + 1}.jpg",
                        });
                    }
                }
                var model = new DemoDataModel
                {
                    Index = i,
                    IsSelected = i % 2 == 0,
                    Name = $"Name{i}",                   
                    ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{i % 5 + 1}.jpg",
                    Remark = new string(i.ToString()[0], 10)
                };
                model.AddChildRange(dataList);
                list.Add(model);
            }

            return list;
        }

        public static ObservableCollection<DemoDataModel> GetDemoDataList(int count)
        {
            var list = new ObservableCollection<DemoDataModel>();
            for (var i = 1; i <= count; i++)
            {
                var index = i % 6 + 1;

                var dataList = new List<DemoDataModel>();
                for (var j = 0; j < 3; j++)
                {
                    dataList.Add(new DemoDataModel
                    {
                        Index = j,
                        IsSelected = j % 2 == 0,
                        Name = $"SubName{j}",
                        ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{j % 5 + 1}.jpg",
                    });
                    for (var k = 0; k < 3; k++)
                    {
                        dataList[j].AddChild(new DemoDataModel
                        {
                            Index = k,
                            IsSelected = k % 2 == 0,
                            Name = $"SubSubName{k}",
                            ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{k % 5 + 1}.jpg",
                        });
                    }
                }
                var model = new DemoDataModel
                {
                    Index = i,
                    IsSelected = i % 2 == 0,
                    Name = $"Name{i}",
                    ImgPath = $"/AIStudio.Wpf.Controls.Demo;component/Resources/Images/{i % 5 + 1}.jpg",
                    Remark = new string(i.ToString()[0], 10)
                };
                model.AddChildRange(dataList);
                list.Add(model);
            }

            return list;
        }
    }

    public class DemoInfo
    {
        public string title
        {
            get;set;
        }

        public string image
        {
            get; set;
        }

        public List<List<string>> demoItemList
        {
            get; set;
        }
    }
}
