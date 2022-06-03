using System.Collections.Generic;
using AIStudio.Wpf.Wall3D.Demo.Models;

namespace AIStudio.Wpf.Wall3D.Demo.ViewModels
{

    class CarouselViewModel
    {
        public List<WallItemData> Datas { get; set; }

        public CarouselViewModel()
        {
            List<WallItemData> datas = new List<WallItemData>()
            {
                new WallItemData(){ Thumb = "1.jpg"},
                new WallItemData(){ Thumb = "2.jpg"},
                new WallItemData(){ Thumb = "3.jpg"},
                new WallItemData(){ Thumb = "4.jpg"},
                new WallItemData(){ Thumb = "5.jpg"},
                new WallItemData(){ Thumb = "6.jpg"},
                new WallItemData(){ Thumb = "7.jpg"},
                new WallItemData(){ Thumb = "8.jpg"},
                new WallItemData(){ Thumb = "9.jpg"},
                new WallItemData(){ Thumb = "10.jpg"},
            };
            Datas = datas;
        }
    }


}
