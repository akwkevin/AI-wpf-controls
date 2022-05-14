using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Helper;

namespace AIStudio.Wpf.Controls
{
    public partial class MessageBoxDialog : BaseDialog
    {
        public MessageBoxDialog()
        {
            InitializeComponent();
        }

        #region Property
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessageBoxDialog));

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(MessageBoxDialog));


        public Geometry Path
        {
            get
            {
                return (Geometry)GetValue(PathProperty);
            }
            set
            {
                SetValue(PathProperty, value);
            }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register(nameof(Path), typeof(Geometry), typeof(MessageBoxDialog));

        public ControlStatus Status
        {
            get
            {
                return (ControlStatus)GetValue(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ControlStatus), typeof(MessageBoxDialog));

        #endregion

        /// <summary>
        /// 信息提示
        /// </summary>
        /// <param name="msg"></param>
        public static async Task<BaseDialogResult> Info(string message, string title, string identifier = "RootWindow")
        {
            return await Show(message, title, ControlStatus.Info, identifier);
        }

        /// <summary>
        /// 真香警告
        /// </summary>
        /// <param name="msg"></param>
        public static async Task<BaseDialogResult> Warning(string message, string title, string identifier = "RootWindow")
        {
            return await Show(message, title, ControlStatus.Warning, identifier);
        }

        /// <summary>
        /// 真香询问
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task<BaseDialogResult> Success(string message, string title, string identifier = "RootWindow")
        {
            return await Show(message, title, ControlStatus.Success, identifier);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg"></param>
        public static async Task<BaseDialogResult> Error(string message, string title, string identifier = "RootWindow")
        {
            return await Show(message, title, ControlStatus.Danger, identifier);
        }

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <param name="notify">类型</param>
        /// <param name="msg">文本信息</param>
        /// <returns></returns>
        public static async Task<BaseDialogResult> Show(string message, string title, ControlStatus status = ControlStatus.Mid, string identifier = "RootWindow")
        {
            string path = string.Empty;
            switch (status)
            {
                case ControlStatus.Light:
                case ControlStatus.Mid:
                case ControlStatus.Dark:
                    path = "InfoGeometry"; break;
                case ControlStatus.Secondary:
                    path = "InfoGeometry"; break;
                case ControlStatus.Success:
                    path = "SuccessGeometry"; break;
                case ControlStatus.Info:
                    path = "InfoGeometry"; break;
                case ControlStatus.Warning:
                    path = "WarningGeometry"; break;
                case ControlStatus.Danger:
                    path = "DangerGeometry"; break;
                case ControlStatus.Plain:
                    path = "InfoGeometry"; break;
                default:
                    path = "InfoGeometry"; break;
            }
            BaseDialog baseDialog = new MessageBoxDialog()
            {
                Message = message,
                Title = title,
                Status = status,
                Path = ResourceHelper.GetResource<Geometry>(path),
            };

            var result = await WindowBase.ShowDialogAsync(baseDialog, identifier); //位于顶级窗口
            return (BaseDialogResult)result;
        }
    }
}
