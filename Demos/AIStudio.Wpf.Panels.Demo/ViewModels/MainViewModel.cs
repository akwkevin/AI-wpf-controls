using System;
using System.Collections.Generic;
using System.Text;

namespace AIStudio.Wpf.Panels.Demo.ViewModels
{
    class MainViewModel
    {
        public List<IResizableItemData> Datas
        {
            get; set;
        }
        public List<int> Lists
        {
            get; set;
        }

        public MainViewModel()
        {
            List<IResizableItemData> datas = new List<IResizableItemData>()
            {
                new class1(){Number=1 , WidthPix = 2, HeightPix = 2, Title = "标题1"},
                new class2(){Number=2 , CanClose = true, Title = "标题2"},
                new class3(){Number=3, HeightPix = 2, Title = "标题3" },
                new class1(){Number=4, Title = "标题4" },
                new class2(){Number=5, Title = "标题5" },
                new class3(){Number=6, Title = "标题6" },
                new class1(){Number=7, Title = "标题7" },
                new class2(){Number=8, Title = "标题8" },
                new class3(){Number=9, Title = "标题9" },
                new class1(){Number=10, Title = "标题10" },
                new class2(){Number=11, Title = "标题11" },
                new class3(){Number=12, Title = "标题12" },
                new class1(){Number=13, Title = "标题13" },
                new class2(){Number=14, Title = "标题14" },
                new class3(){Number=15, Title = "标题15" },
                new class1(){Number=16, Title = "标题16" },
                new class2(){Number=17, Title = "标题17" },
                new class3(){Number=18, Title = "标题18" },
                new class1(){Number=19, Title = "标题19" },
                new class2(){Number=20, Title = "标题20" },
                new class3(){Number=21, Title = "标题21" },
                new class1(){Number=22, Title = "标题22" },
                new class2(){Number=23, Title = "标题23" },
                new class3(){Number=24, Title = "标题24" },
                new class1(){Number=25, Title = "标题25" },
                new class2(){Number=26, Title = "标题26" },
                new class3(){Number=27, Title = "标题27" },
                new class1(){Number=28, Title = "标题28" },
                new class2(){Number=29, Title = "标题29" },
                new class3(){Number=30, Title = "标题30" },
            };
            //resizableItemsControl.ItemsSource = datas;

            List<int> list = new List<int>();
            for (int i = 0; i < 500; i++)
            {
                list.Add(i);
            }

            Datas = datas;
            Lists = list;
        }

    }

    public interface Iclass : IResizableItemData
    {
        int Number
        {
            get; set;
        }

    }


    public class class1 : MDIItemData
    {

    }

    public class class2 : MDIItemData
    {
    }

    public class class3 : MDIItemData
    {
    }
}
