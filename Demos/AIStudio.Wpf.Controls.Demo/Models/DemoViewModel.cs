using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls.Demo.Models
{
    public class DemoViewModel : BindableBase
    {
        private object _subContent;
        /// <summary>
        ///     子内容
        /// </summary>
        public object SubContent
        {
            get => _subContent;
            set => SetProperty(ref _subContent, value);
        }

        private object _title;
        /// <summary>
        ///     内容标题
        /// </summary>
        public object Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _icon;
        /// <summary>
        ///     图标
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        private string _targetCtlName;
        /// <summary>
        ///     内容标题
        /// </summary>
        public string TargetCtlName
        {
            get => _targetCtlName;
            set => SetProperty(ref _targetCtlName, value);
        }

        private string _xamlText;
        public string XamlText
        {
            get => _xamlText;
            set => SetProperty(ref _xamlText, value);
        }

        private string _csText;
        public string CsText
        {
            get => _csText;
            set => SetProperty(ref _csText, value);
        }

        private string _vmText;
        public string VmText
        {
            get => _vmText;
            set => SetProperty(ref _vmText, value);
        }
    }
}
