using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    public class FormCodeItem : FormItem
    {
        #region Constants
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        #endregion Constants

        private ContentPresenter _contentPresenter;

        #region ControlType
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register(
            "ControlType", typeof(ControlType), typeof(FormCodeItem), new PropertyMetadata(ControlType.TextBox, OnControlTypeChanged));

        public ControlType ControlType
        {
            get
            {
                return (ControlType)GetValue(ControlTypeProperty);
            }
            set
            {
                SetValue(ControlTypeProperty, value);
            }
        }
        #endregion

        #region Path
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", typeof(string), typeof(FormCodeItem), new PropertyMetadata("."));

        public string Path
        {
            get
            {
                return (string)GetValue(PathProperty);
            }
            set
            {
                SetValue(PathProperty, value);
            }
        }
        #endregion

        static FormCodeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormCodeItem), new FrameworkPropertyMetadata(typeof(FormCodeItem), FrameworkPropertyMetadataOptions.Inherits));
        }

        private static void OnControlTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.GetControl((ControlType)e.NewValue);
            }
        }

        private void GetControl(ControlType controlType)
        {
            if (_contentPresenter == null)
                return;

            switch (controlType)
            {
                case ControlType.TextBox: _contentPresenter.Content = new TextBox(); break;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentPresenter;

            GetControl(ControlType);
        }

    }
}
