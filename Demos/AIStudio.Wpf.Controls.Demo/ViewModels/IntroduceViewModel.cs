using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Commands;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class IntroduceViewModel : BindableBase
    {
        private ICommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                return this._showCommand ?? (this._showCommand = new DelegateCommand<List<object>>(para => this.ShowBox(para)));
            }
        }

        public void ShowBox(List<object> para)
        {
            List<GuidVo> list = new List<GuidVo>();
            foreach (var obj in para.OfType<FrameworkElement>())
            {
                var item = new GuidVo()
                {
                    Uc = obj,
                    Content = obj.Tag?.ToString(),
                };
                list.Add(item);
            }
            GuideWindow win = new GuideWindow(Window.GetWindow(list[0].Uc), list);

            win.ShowDialog();
        }
    }
}
