using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Thumb, Type = typeof(Thumb))]
    public class FormItem : HeaderedContentControl
    {
        private const string PART_Thumb = "PART_Thumb";
        private Thumb _thumb;

        static FormItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormItem), new FrameworkPropertyMetadata(typeof(FormItem), FrameworkPropertyMetadataOptions.Inherits));
        }

        public FormItem()
        {
            this.Drop += FormItem_Drop;
            this.AddHandler(Form.AllowDropEvent, new RoutedEventHandler(OnAllDropChanged));
        }

        private void OnAllDropChanged(object sender, RoutedEventArgs e)
        {
            if (_thumb != null)
            {
                if (AllowDrop)
                {
                    _thumb.Visibility = Visibility.Visible;
                }
                else
                {
                    _thumb.Visibility = Visibility.Collapsed;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_thumb != null)
            {
                _thumb.DragStarted -= ThumbOnDragStarted;
                _thumb.DragDelta -= ThumbOnDragDelta;
                _thumb.DragCompleted -= ThumbOnDragCompleted;
            }

            _thumb = GetTemplateChild(PART_Thumb) as Thumb;
            if (_thumb != null)
            {
                _thumb.DragStarted += ThumbOnDragStarted;
                _thumb.DragDelta += ThumbOnDragDelta;
                _thumb.DragCompleted += ThumbOnDragCompleted;
                if (AllowDrop)
                {
                    _thumb.Visibility = Visibility.Visible;
                }
                else
                {
                    _thumb.Visibility = Visibility.Collapsed;
                }
            }
        }



        private void ThumbOnDragStarted(object sender, DragStartedEventArgs dragStartedEventArgs)
        {

        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
        }

        private void ThumbOnDragCompleted(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {

        }

        private Form parent => System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(this) as Form;

        private void FormItem_Drop(object sender, DragEventArgs e)
        {
            if (parent == null) return;

            //查找元数据
            var sourceItem = e.Data.GetData(typeof(FormItem)) as FormItem;
            if (sourceItem == null)
            {
                return;
            }

            var targetItem = this;
            if (sourceItem == targetItem)
            {
                return;
            }

            var source = parent.ItemContainerGenerator.ItemFromContainer(sourceItem);
            var target = parent.ItemContainerGenerator.ItemFromContainer(targetItem);

            var list = parent.GetActualList();

            int indexTarget = list.IndexOf(target);

            list.Remove(source);
            list.Insert(indexTarget, source);
        }

    }
}
