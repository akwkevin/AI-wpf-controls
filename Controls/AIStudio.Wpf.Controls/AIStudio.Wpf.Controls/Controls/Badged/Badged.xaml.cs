using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_BadgeContainer, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_RectWave, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_ScaleTransform, Type = typeof(ScaleTransform))]
    public partial class Badged : BadgedEx
    {
        public static readonly DependencyProperty BadgeChangedStoryboardProperty = DependencyProperty.Register(
            nameof(BadgeChangedStoryboard), typeof(Storyboard), typeof(Badged), new PropertyMetadata(default(Storyboard)));

        public Storyboard BadgeChangedStoryboard
        {
            get
            {
                return (Storyboard)this.GetValue(BadgeChangedStoryboardProperty);
            }
            set
            {
                this.SetValue(BadgeChangedStoryboardProperty, value);
            }
        }

        static Badged()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Badged), new FrameworkPropertyMetadata(typeof(Badged)));
        }

        public override void OnApplyTemplate()
        {
            this.BadgeChanged -= this.OnBadgeChanged;

            base.OnApplyTemplate();

            this.BadgeChanged += this.OnBadgeChanged;
        }

        private void OnBadgeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var sb = this.BadgeChangedStoryboard;
            if (this._badgeContainer != null && sb != null)
            {
                try
                {
                    this._badgeContainer.BeginStoryboard(sb);
                }
                catch (Exception exc)
                {
                    Trace.WriteLine("Error during Storyboard execution, exception will be rethrown.");
                    Trace.WriteLine($"{exc.Message} ({exc.GetType().FullName})");
                    Trace.WriteLine(exc.StackTrace);

                    throw;
                }
            }
        }
    }
}
