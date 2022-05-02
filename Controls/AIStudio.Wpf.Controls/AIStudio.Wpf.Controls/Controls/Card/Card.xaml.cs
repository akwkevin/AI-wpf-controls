using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// A card is a content control, styled according to Material Design guidelines.
    /// </summary>
    [TemplatePart(Name = PART_ClipBorder, Type = typeof(Border))]
    public class Card : ContentControl
    {
        public const string PART_ClipBorder = "PART_ClipBorder";

        private Border _clipBorder;

        static Card()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Card), new FrameworkPropertyMetadata(typeof(Card)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _clipBorder = Template.FindName(PART_ClipBorder, this) as Border;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (_clipBorder == null) return;

            var farPoint = new Point(
                Math.Max(0, _clipBorder.ActualWidth),
                Math.Max(0, _clipBorder.ActualHeight));

            var clipRect = new Rect(
                new Point(),
                new Point(farPoint.X, farPoint.Y));

            //ContentClip = new RectangleGeometry(clipRect, UniformCornerRadius, UniformCornerRadius);
        }

        //public static readonly DependencyProperty UniformCornerRadiusProperty = DependencyProperty.Register(
        //    nameof(UniformCornerRadius), typeof(double), typeof(Card), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        //public double UniformCornerRadius
        //{
        //    get { return (double)GetValue(UniformCornerRadiusProperty); }
        //    set { SetValue(UniformCornerRadiusProperty, value); }
        //}

        //private static readonly DependencyPropertyKey ContentClipPropertyKey =
        //    DependencyProperty.RegisterReadOnly(
        //        nameof(ContentClip), typeof(Geometry), typeof(Card),
        //        new PropertyMetadata(default(Geometry)));

        //public static readonly DependencyProperty ContentClipProperty =
        //    ContentClipPropertyKey.DependencyProperty;

        //public Geometry ContentClip
        //{
        //    get { return (Geometry)GetValue(ContentClipProperty); }
        //    private set { SetValue(ContentClipPropertyKey, value); }
        //}

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
           nameof(Header), typeof(object), typeof(Card), new PropertyMetadata(default(object)));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            nameof(HeaderTemplate), typeof(DataTemplate), typeof(Card), new PropertyMetadata(default(DataTemplate)));

        [Bindable(true), Category("Content")]
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register(
            nameof(HeaderTemplateSelector), typeof(DataTemplateSelector), typeof(Card), new PropertyMetadata(default(DataTemplateSelector)));

        [Bindable(true), Category("Content")]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty);
            set => SetValue(HeaderTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty HeaderStringFormatProperty = DependencyProperty.Register(
            nameof(HeaderStringFormat), typeof(string), typeof(Card), new PropertyMetadata(default(string)));

        [Bindable(true), Category("Content")]
        public string HeaderStringFormat
        {
            get => (string)GetValue(HeaderStringFormatProperty);
            set => SetValue(HeaderStringFormatProperty, value);
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            nameof(Footer), typeof(object), typeof(Card), new PropertyMetadata(default(object)));

        public object Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register(
            nameof(FooterTemplate), typeof(DataTemplate), typeof(Card), new PropertyMetadata(default(DataTemplate)));

        [Bindable(true), Category("Content")]
        public DataTemplate FooterTemplate
        {
            get => (DataTemplate)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateSelectorProperty = DependencyProperty.Register(
            nameof(FooterTemplateSelector), typeof(DataTemplateSelector), typeof(Card), new PropertyMetadata(default(DataTemplateSelector)));

        [Bindable(true), Category("Content")]
        public DataTemplateSelector FooterTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(FooterTemplateSelectorProperty);
            set => SetValue(FooterTemplateSelectorProperty, value);
        }

        public static readonly DependencyProperty FooterStringFormatProperty = DependencyProperty.Register(
            nameof(FooterStringFormat), typeof(string), typeof(Card), new PropertyMetadata(default(string)));

        [Bindable(true), Category("Content")]
        public string FooterStringFormat
        {
            get => (string)GetValue(FooterStringFormatProperty);
            set => SetValue(FooterStringFormatProperty, value);
        }
    }
}
