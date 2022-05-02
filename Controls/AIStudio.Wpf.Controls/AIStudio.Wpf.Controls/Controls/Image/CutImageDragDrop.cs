using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = ThumbTemplateName, Type = typeof(Thumb))]
    [TemplatePart(Name = ThumbRightBottomTemplateName, Type = typeof(Thumb))]
    public class CutImageDragDrop : Control
    {
        private const string ThumbTemplateName = "PART_Rect";
        private const string ThumbRightBottomTemplateName = "PART_RectRightBottom";

        private Thumb _thumb;
        private Thumb _rectRightBottomThumb;

        public event Action UpdateImageEvent;
        #region 依赖属性
        public static readonly DependencyProperty ParentMaxWidthProperty =
            DependencyProperty.Register(nameof(ParentMaxWidth), typeof(double), typeof(CutImageDragDrop), new PropertyMetadata(null));
        public double ParentMaxWidth
        {
            get
            {
                return (double)this.GetValue(ParentMaxWidthProperty);
            }
            set
            {
                this.SetValue(ParentMaxWidthProperty, value);
            }
        }

        public static readonly DependencyProperty ParentMaxHeightProperty =
            DependencyProperty.Register(nameof(ParentMaxHeight), typeof(double), typeof(CutImageDragDrop), new PropertyMetadata(null));
        public double ParentMaxHeight
        {
            get
            {
                return (double)this.GetValue(ParentMaxHeightProperty);
            }
            set
            {
                this.SetValue(ParentMaxHeightProperty, value);
            }
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _thumb = GetTemplateChild(ThumbTemplateName) as Thumb;
            _rectRightBottomThumb = GetTemplateChild(ThumbRightBottomTemplateName) as Thumb;
            RegisterEventListener();
        }
        protected virtual void RegisterEventListener()
        {
            _thumb.DragDelta += OnDragDeltaHandler;
            _rectRightBottomThumb.DragDelta += OnRightBottomDragDeltaHandler;
        }
        static CutImageDragDrop()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CutImageDragDrop), new FrameworkPropertyMetadata(typeof(CutImageDragDrop)));
        }
        public System.Drawing.Rectangle GetCutRectangle()
        {
            return new System.Drawing.Rectangle((int)Canvas.GetLeft(this), (int)Canvas.GetTop(this), (int)this.ActualWidth, (int)this.ActualHeight);
        }


        #region 中间拖动
        private void OnDragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double left = Canvas.GetLeft(this) + e.HorizontalChange;
            double top = Canvas.GetTop(this) + e.VerticalChange;

            if (left < 0) left = 0;
            if (top < 0) top = 0;

            if (left + this.Width > this.ParentMaxWidth) left = this.ParentMaxWidth - this.Width;
            if (top + this.Height > this.ParentMaxHeight) top = this.ParentMaxHeight - this.Height;

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            if (UpdateImageEvent != null)
            {
                UpdateImageEvent();
            }
        }
        #endregion

        #region 右下点拖动
        private void OnRightBottomDragDeltaHandler(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 0) this.Width += e.HorizontalChange;
            double left = Canvas.GetLeft(this);
            if (left + this.Width > this.ParentMaxWidth)
            {
                this.Width = this.ParentMaxWidth - left;
            }

            if (this.Height + e.VerticalChange > 0) this.Height += e.VerticalChange;
            double top = Canvas.GetTop(this);
            if (top + this.Height > this.ParentMaxHeight)
            {
                this.Height = this.ParentMaxHeight - top;
            }
            if (UpdateImageEvent != null)
            {
                UpdateImageEvent();
            }
        }
        #endregion
    }
}
