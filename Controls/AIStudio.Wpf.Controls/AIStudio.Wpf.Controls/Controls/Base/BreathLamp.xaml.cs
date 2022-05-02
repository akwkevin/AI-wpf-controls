using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace AIStudio.Wpf.Controls
{
    public enum LampEffect
    {
        OuterGlow,
        Eclipse,
        Ripple
    }

    [TemplatePart(Name = PART_Lamp, Type = typeof(Border))]
    public class BreathLamp : ContentControl
    {
        private const string PART_Lamp = "PART_Lamp";
        private Border _border;

        static BreathLamp()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreathLamp), new FrameworkPropertyMetadata(typeof(BreathLamp)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _border = GetTemplateChild(PART_Lamp) as Border;
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(BreathLamp), new PropertyMetadata(new CornerRadius(60d)));

        public LampEffect LampEffect
        {
            get
            {
                return (LampEffect)GetValue(LampEffectProperty);
            }
            set
            {
                SetValue(LampEffectProperty, value);
            }
        }
        // Using a DependencyProperty as the backing store for LampEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LampEffectProperty =
            DependencyProperty.Register("LampEffect", typeof(LampEffect), typeof(BreathLamp), new PropertyMetadata(default(LampEffect), OnLampEffectPropertyChangedCallBack));


        private static void OnLampEffectPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public bool IsLampStart
        {
            get
            {
                return (bool)GetValue(IsLampStartProperty);
            }
            set
            {
                SetValue(IsLampStartProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsLampStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLampStartProperty =
            DependencyProperty.Register("IsLampStart", typeof(bool), typeof(BreathLamp), new PropertyMetadata(true));

        public Color EffectColor
        {
            get
            {
                return (Color)GetValue(EffectColorProperty);
            }
            set
            {
                SetValue(EffectColorProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for IsLampStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EffectColorProperty =
            DependencyProperty.Register("EffectColor", typeof(Color), typeof(BreathLamp), new PropertyMetadata(default(Color)));

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsLampStart == false)
            {
                var sb = (Storyboard)this.Template.Resources["EaseOut"];
                if (sb != null)
                {
                    sb = sb.Clone();

                    EventHandler completionHandler = null;
                    completionHandler = (sender, args) => {
                        _border.Effect = null;
                        sb.Completed -= completionHandler;
                    };

                    sb.Completed += completionHandler;

                    _border.Effect = new DropShadowEffect() { BlurRadius = 25, Color = EffectColor, ShadowDepth = 0, };
                    _border.BeginStoryboard(sb);
                }
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }
    }
}
