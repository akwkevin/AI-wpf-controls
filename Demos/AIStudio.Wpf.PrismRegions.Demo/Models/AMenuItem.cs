using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AIStudio.Wpf.PrismRegions.Demo.Models
{
    public class AMenuItem : BindableBase
    {
        private string _glyph;
        public string Glyph
        {
            get { return _glyph; }
            set
            {
                SetProperty(ref _glyph, value);
            }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                SetProperty(ref _label, value);
            }
        }

        public string Code { get; set; }//区分点击的菜单
        public int Type { get; set; }//区分菜单还是目录

        private ICommand _command;
        public ICommand Command
        {
            get { return _command; }
            set
            {
                SetProperty(ref _command, value);
            }
        }

        private ObservableCollection<AMenuItem> _children = new ObservableCollection<AMenuItem>();

        public ObservableCollection<AMenuItem> Children
        {
            get { return _children; }
            set
            {
                SetProperty(ref _children, value);
            }
        }

        public void AddChildren(AMenuItem child)
        {
            this.Children.Add(child);
        }
    }
}
