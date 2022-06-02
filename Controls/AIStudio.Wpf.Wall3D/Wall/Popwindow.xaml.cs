using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AIStudio.Wpf.Wall3D.Wall
{
    /// <summary>
    /// Interaction logic for Popwindow.xaml
    /// </summary>
    public partial class Popwindow : UserControl
    {
        public Popwindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += Popwindow_Loaded;
        }

        void Popwindow_Loaded(object sender, RoutedEventArgs e)
        {

            Loaded -= Popwindow_Loaded;
            SetSize();
            Appear();
        }


        private void Appear()//出现
        {
            DoubleAnimation Ani = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = new Duration(TimeSpan.FromSeconds(.5))
            };

            DoubleAnimation Rni = new DoubleAnimation()
            {
                From = 0,
                To = 720,
                Duration = new Duration(TimeSpan.FromSeconds(.5))

            };
            _scale.BeginAnimation(ScaleTransform.ScaleXProperty, Ani);
            _scale.BeginAnimation(ScaleTransform.ScaleYProperty, Ani);
            _rot.BeginAnimation(RotateTransform.AngleProperty, Rni);
        }
        #region <<属性    

        public double X
        {
            set
            {
                Canvas.SetLeft(this, value);
            }
            get
            {
                return Canvas.GetLeft(this);
            }
        }
        public double Y
        {
            set
            {
                Canvas.SetTop(this, value);
            }
            get
            {
                return Canvas.GetTop(this);
            }
        }
        #endregion
        private void SetSize()
        {

        }

    }
}
