using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = "PART_InnerHyperlink", Type = typeof(Hyperlink))]
    public class LinkTextBlock : Label
    {
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(Uri), typeof(LinkTextBlock));

        [Category("Common Properties"), Bindable(true)]
        public Uri Url
        {
            get
            {
                return GetValue(UrlProperty) as Uri;
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        public static readonly DependencyProperty ViewInBrowerProperty =
        DependencyProperty.Register("ViewInBrower", typeof(bool),
            typeof(LinkTextBlock));

        public bool ViewInBrower
        {
            get
            {
                return (bool)GetValue(ViewInBrowerProperty);
            }
            set
            {
                SetValue(ViewInBrowerProperty, value);
            }
        }

        public static readonly DependencyProperty HyperlinkStyleProperty =
            DependencyProperty.Register("HyperlinkStyle", typeof(Style),
                typeof(LinkTextBlock));

        public Style HyperlinkStyle
        {
            get
            {
                return GetValue(HyperlinkStyleProperty) as Style;
            }
            set
            {
                SetValue(HyperlinkStyleProperty, value);
            }
        }

        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush),
                typeof(LinkTextBlock));

        [Category("Brushes"), Bindable(true)]
        public Brush MouseOverForeground
        {
            get
            {
                return GetValue(MouseOverForegroundProperty) as Brush;
            }
            set
            {
                SetValue(MouseOverForegroundProperty, value);
            }
        }

        public static readonly DependencyProperty LinkLabelBehaviorProperty =
            DependencyProperty.Register("LinkLabelBehavior",
                typeof(LinkLabelBehavior),
                typeof(LinkTextBlock));

        [Category("Common Properties"), Bindable(true)]
        public LinkLabelBehavior LinkLabelBehavior
        {
            get
            {
                return (LinkLabelBehavior)GetValue(LinkLabelBehaviorProperty);
            }
            set
            {
                SetValue(LinkLabelBehaviorProperty, value);
            }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(LinkTextBlock));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(LinkTextBlock));

        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(LinkTextBlock));

        [Localizability(LocalizationCategory.NeverLocalize), Bindable(true), Category("Action")]
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }
            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        [Localizability(LocalizationCategory.NeverLocalize), Bindable(true), Category("Action")]
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandParameterProperty);
            }
            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        [Bindable(true), Category("Action")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(CommandTargetProperty);
            }
            set
            {
                this.SetValue(CommandTargetProperty, value);
            }
        }

        [Category("Behavior")]
        public static readonly RoutedEvent ClickEvent;

        [Category("Behavior")]
        public static readonly RoutedEvent RequestNavigateEvent;

        static LinkTextBlock()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LinkTextBlock),
                new FrameworkPropertyMetadata(typeof(LinkTextBlock)));

            ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LinkTextBlock));
            RequestNavigateEvent = EventManager.RegisterRoutedEvent("RequestNavigate", RoutingStrategy.Bubble, typeof(RequestNavigateEventHandler), typeof(LinkTextBlock));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Hyperlink innerHyperlink = GetTemplateChild("PART_InnerHyperlink") as Hyperlink;
            if (innerHyperlink != null)
            {
                innerHyperlink.Click += new RoutedEventHandler(InnerHyperlink_Click);
                innerHyperlink.RequestNavigate += new RequestNavigateEventHandler(InnerHyperlink_RequestNavigate);
            }
        }

        void InnerHyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (ViewInBrower)
            {
                // 激活的是当前默认的浏览器
                Process.Start("explorer.exe", Url.AbsoluteUri);
            }
            else
            {
                RequestNavigateEventArgs args = new RequestNavigateEventArgs(e.Uri, String.Empty);
                args.Source = this;
                args.RoutedEvent = LinkTextBlock.RequestNavigateEvent;
                RaiseEvent(args);
            }
        }


        void InnerHyperlink_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(LinkTextBlock.ClickEvent, this));
        }

        public event RoutedEventHandler Click
        {
            add
            {
                base.AddHandler(ClickEvent, value);
            }
            remove
            {
                base.RemoveHandler(ClickEvent, value);
            }
        }

        public event RequestNavigateEventHandler RequestNavigate
        {
            add
            {
                base.AddHandler(RequestNavigateEvent, value);
            }
            remove
            {
                base.RemoveHandler(RequestNavigateEvent, value);
            }
        }
    }

    public enum LinkLabelBehavior
    {
        SystemDefault,
        AlwaysUnderline,
        HoverUnderline,
        NeverUnderline
    }
}
