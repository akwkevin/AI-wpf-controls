using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Wall3D.Demo.Models;
using AIStudio.Wpf.Wall3D.Wall;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace AIStudio.Wpf.Wall3D.Demo.ViewModels
{
    class Wall3DViewModel
    {
        public List<WallItemData> Datas
        {
            get; set;
        }

        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get
            {
                return this._openCommand ?? (this._openCommand = new DelegateCommand<object>(para => this.Open(para)));
            }
        }

        public Wall3DViewModel()
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

        //如果想弹窗实现。
        private void Open(object para)
        {
            WallControl.ItemclickEventArg e = para as WallControl.ItemclickEventArg;
            if (e != null && e.Data is WallItemData item)
            {
                var Picture = new Popwindow();
                Picture.HorizontalAlignment = HorizontalAlignment.Center;
                Picture.VerticalAlignment = VerticalAlignment.Center;

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(item.Source));
                Picture.Content = image;

                e.Sender.PopupElement(Picture);
            }
        }
    }
}
