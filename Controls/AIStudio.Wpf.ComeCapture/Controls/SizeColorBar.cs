using AIStudio.Wpf.ComeCapture.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    [TemplatePart(Name = "PART_RectangleTool", Type = typeof(RectangleTool))]
    [TemplatePart(Name = "PART_EllipseTool", Type = typeof(EllipseTool))]
    [TemplatePart(Name = "PART_ArrowTool", Type = typeof(ArrowTool))]
    [TemplatePart(Name = "PART_LineTool", Type = typeof(LineTool))]
    [TemplatePart(Name = "PART_TextTool", Type = typeof(TextTool))]
    public class SizeColorBar : Control
    {
        private RectangleTool _RectangleTool;
        private EllipseTool _EllipseTool;
        private LineTool _LineTool;
        private TextTool _TextTool;
        private ArrowTool _ArrowTool;

        static SizeColorBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SizeColorBar), new FrameworkPropertyMetadata(typeof(SizeColorBar)));
        }

        public SizeColorBar()
        {
            _Current = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _RectangleTool = GetTemplateChild("PART_RectangleTool") as RectangleTool;
            _EllipseTool = GetTemplateChild("PART_EllipseTool") as EllipseTool;
            _ArrowTool = GetTemplateChild("PART_ArrowTool") as ArrowTool;
            _LineTool = GetTemplateChild("PART_LineTool") as LineTool;
            _TextTool = GetTemplateChild("PART_TextTool") as TextTool;
        }

        public void ChangeColor(SolidColorBrush brush)
        {
            switch (Selected)
            {
                case Tool.Rectangle:
                    _RectangleTool.LineBrush = brush;
                    break;
                case Tool.Ellipse:
                    _EllipseTool.LineBrush = brush;
                    break;
                case Tool.Arrow:
                    _ArrowTool.LineBrush = brush;
                    break;
                case Tool.Line:
                    _LineTool.LineBrush = brush;
                    break;
                case Tool.Text:
                    _TextTool.LineBrush = brush;
                    break;
                default:
                    break;
            }
        }

        #region 属性 Current
        private static SizeColorBar _Current = null;
        public static SizeColorBar Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }
        #endregion

        #region 属性 ColorBars
        private static List<SolidColorBrush> _ColorBars;
        public static List<SolidColorBrush> ColorBars
        {
            get
            {
                if (_ColorBars == null)
                {
                    _ColorBars = new List<SolidColorBrush>()
                    {
                        new SolidColorBrush(Colors.Black),
                        new SolidColorBrush(Colors.Gray),
                        new SolidColorBrush(Colors.Firebrick),
                        new SolidColorBrush(Colors.Orange),
                        new SolidColorBrush(Colors.ForestGreen),
                        new SolidColorBrush(Colors.Blue),
                        new SolidColorBrush(Colors.Maroon),
                        new SolidColorBrush(Colors.CadetBlue),
                        new SolidColorBrush(Colors.HotPink),

                        new SolidColorBrush(Colors.White),
                        new SolidColorBrush(Colors.LightGray),
                        new SolidColorBrush(Colors.Red),
                        new SolidColorBrush(Colors.Yellow),
                        new SolidColorBrush(Colors.YellowGreen),
                        new SolidColorBrush(Colors.DodgerBlue),
                        new SolidColorBrush(Colors.Magenta),
                        new SolidColorBrush(Colors.Cyan),
                        new SolidColorBrush(Colors.MistyRose)
                    };
                }
                return _ColorBars;
            }
            set
            {
                _ColorBars = value;
            }
        }
        #endregion

        #region 定位
        public void ResetCanvas()
        {
            ResetCanvasLeft();
            ResetCanvasTop();
        }
        #endregion

        #region CanvasLeft DependencyProperty
        public double CanvasLeft
        {
            get { return (double)GetValue(CanvasLeftProperty); }
            set { SetValue(CanvasLeftProperty, value); }
        }
        public static readonly DependencyProperty CanvasLeftProperty =
                DependencyProperty.Register("CanvasLeft", typeof(double), typeof(SizeColorBar),
                new PropertyMetadata(0.0, new PropertyChangedCallback(SizeColorBar.OnCanvasLeftPropertyChanged)));

        private static void OnCanvasLeftPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SizeColorBar)
            {
                (obj as SizeColorBar).OnCanvasLeftValueChanged();
            }
        }

        protected void OnCanvasLeftValueChanged()
        {

        }

        private void ResetCanvasLeft()
        {
            CanvasLeft = ImageEditBar.Current.CanvasLeft;
        }
        #endregion

        #region CanvasTop DependencyProperty
        public double CanvasTop
        {
            get { return (double)GetValue(CanvasTopProperty); }
            set { SetValue(CanvasTopProperty, value); }
        }
        public static readonly DependencyProperty CanvasTopProperty =
                DependencyProperty.Register("CanvasTop", typeof(double), typeof(SizeColorBar),
                new PropertyMetadata(0.0, new PropertyChangedCallback(SizeColorBar.OnCanvasTopPropertyChanged)));

        private static void OnCanvasTopPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SizeColorBar)
            {
                (obj as SizeColorBar).OnCanvasTopValueChanged();
            }
        }

        protected void OnCanvasTopValueChanged()
        {

        }

        private void ResetCanvasTop()
        {
            CanvasTop = AppModel.Current.MaskBottomHeight >= 67 ? MainWindow.ScreenHeight - AppModel.Current.MaskBottomHeight + 32
                : AppModel.Current.MaskTopHeight >= 67 ? AppModel.Current.MaskTopHeight - 67
                : AppModel.Current.MaskTopHeight + 30;
        }
        #endregion

        #region Selected DependencyProperty
        public Tool Selected
        {
            get { return (Tool)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }
        public static readonly DependencyProperty SelectedProperty =
                DependencyProperty.Register("Selected", typeof(Tool), typeof(SizeColorBar),
                new PropertyMetadata(Tool.Null, new PropertyChangedCallback(SizeColorBar.OnSelectedPropertyChanged)));

        private static void OnSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is SizeColorBar)
            {
                (obj as SizeColorBar).OnSelectedValueChanged();
            }
        }

        protected void OnSelectedValueChanged()
        {
            ImageEditBar.Current.ResetCanvas();
            ResetCanvas();
            if (Selected == Tool.Null)
            {
                MainWindow.Current.MainImage.MoveCursor = Cursors.SizeAll;
            }
            else
            {
                MainWindow.Current.MainImage.MoveCursor = Cursors.Pen;
            }
        }
        #endregion
    }
}
