using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Bor, Type = typeof(Border))]
    [TemplatePart(Name = PART_Can, Type = typeof(Canvas))]
    /// <summary>
    /// GuideWindow.xaml 的交互逻辑
    /// </summary>
    public class GuideWindow : WindowBase
    {
        private const string PART_Bor = "PART_Bor";
        private const string PART_Can = "PART_Can";

        Border bor;
        Canvas canvas;

        PathGeometry borGeometry = new PathGeometry();
        List<GuidVo> list;
        int index = 0;

        static GuideWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuideWindow), new FrameworkPropertyMetadata(typeof(GuideWindow)));
        }

        public GuideWindow(Window win, List<GuidVo> gl)
        {
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.ShowInTaskbar = false;

            //设置弹出的窗体
            this.Height = win.ActualHeight;
            this.Width = win.ActualWidth;
            this.WindowState = win.WindowState;
            this.Left = win.Left;
            this.Top = win.Top;
            this.Owner = win;
            list = gl;

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            bor = GetTemplateChild(PART_Bor) as Border;
            canvas = GetTemplateChild(PART_Can) as Canvas;

            show(index + 1, list[index].Uc, list[index].Content, list[index].Width, list[index].Height);
        }


        private void show(int xh, FrameworkElement fe, string con, int? width = null, int? height = null, bool first = false, bool last = false)
        {
            Point point = fe.TransformToAncestor(Window.GetWindow(fe)).Transform(new Point(0, 0));//获取控件坐标点

            RectangleGeometry rg = new RectangleGeometry();
            rg.Rect = new Rect(0, 0, this.Width, this.Height);
            borGeometry = Geometry.Combine(borGeometry, rg, GeometryCombineMode.Union, null);
            bor.Clip = borGeometry;

            RectangleGeometry rg1 = new RectangleGeometry();
            rg1.Rect = new Rect(point.X - 5, point.Y - 5, fe.ActualWidth + 10, fe.ActualHeight + 10);
            borGeometry = Geometry.Combine(borGeometry, rg1, GeometryCombineMode.Exclude, null);

            bor.Clip = borGeometry;

            HintUC hit = new HintUC(this, point, fe, xh.ToString(), con, width, height, first, last);
            hit.nextHintEvent -= Hit_nextHintEvent;
            hit.nextHintEvent += Hit_nextHintEvent;
            hit.preHintEvent -= Hit_preHintEvent;
            hit.preHintEvent += Hit_preHintEvent;
            canvas.Children.Add(hit);
        }

        private void Hit_nextHintEvent()
        {
            canvas.Children.Clear();
            if (index >= list.Count - 1)
                return;

            index++;

            if (index == list.Count - 1)
                show(index + 1, list[index].Uc, list[index].Content, list[index].Width, list[index].Height, false, true);
            else
                show(index + 1, list[index].Uc, list[index].Content, list[index].Width, list[index].Height);
        }

        private void Hit_preHintEvent()
        {
            canvas.Children.Clear();
            if (index == 0)
                return;

            index--;

            if (index == 0)
                show(index + 1, list[index].Uc, list[index].Content, list[index].Width, list[index].Height, true, false);
            else
                show(index + 1, list[index].Uc, list[index].Content, list[index].Width, list[index].Height);
        }

    }
}
