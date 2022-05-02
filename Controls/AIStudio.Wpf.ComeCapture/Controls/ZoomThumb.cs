using AIStudio.Wpf.ComeCapture.Models;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.ComeCapture.Controls
{
    public class ZoomThumb : Thumb
    {
        #region Direction DependencyProperty
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        public static readonly DependencyProperty DirectionProperty =
                DependencyProperty.Register("Direction", typeof(Direction), typeof(ZoomThumb),
                new PropertyMetadata(Direction.Null, new PropertyChangedCallback(ZoomThumb.OnDirectionPropertyChanged)));

        private static void OnDirectionPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ZoomThumb)
            {
                (obj as ZoomThumb).OnDirectionValueChanged();
            }
        }

        protected void OnDirectionValueChanged()
        {
            if (Direction == Direction.Move)
            {
                MouseDoubleClick += (sender, e) =>
                {
                    MainWindow.Current.OnOK();
                };
            }
        }
        #endregion
    }
}
