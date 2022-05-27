using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using AIStudio.Wpf.Controls.Behaviors;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Converter;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_Header, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    public class FormCodeItem : FormItem
    {
        #region Constants
        private const string PART_Header = "PART_Header";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        #endregion Constants

        private ContentPresenter _header;
        private ContentPresenter _contentPresenter;
        private Control _control;


        #region ControlType
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register(
            "ControlType", typeof(ControlType), typeof(FormCodeItem), new PropertyMetadata(ControlType.TextBox, OnControlTypeChanged));

        private static void OnControlTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.GetControl((ControlType)e.NewValue);
            }
        }

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
            "Path", typeof(string), typeof(FormCodeItem), new PropertyMetadata("", OnPathChanged));

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.GetControl(formCodeItem.ControlType);
            }
        }

        [Browsable(true)]
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

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(string), typeof(FormCodeItem), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.SetItemsSource((string)e.NewValue);
            }
        }

        [Browsable(true)]
        public string ItemsSource
        {
            get
            {
                return (string)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        #endregion

        static FormCodeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormCodeItem), new FrameworkPropertyMetadata(typeof(FormCodeItem), FrameworkPropertyMetadataOptions.Inherits));
        }


        private void GetControl(ControlType controlType)
        {
            if (_contentPresenter == null)
                return;

            bool hideHeader = false;
            switch (controlType)
            {
                case ControlType.TextBox:
                    {
                        _control = new TextBox();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(TextBox.TextProperty, binding);
                        }
                        break;
                    }
                case ControlType.ComboBox:
                    {
                        _control = new ComboBox(); 
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(ComboBox.SelectedValueProperty, binding);
                            _control.SetValue(ComboBox.DisplayMemberPathProperty, "Text");
                            _control.SetValue(ComboBox.SelectedValuePathProperty, "Value");
                        }
                        break;
                    }
                case ControlType.PasswordBox:
                    {
                        _control = new PasswordBox();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(PasswordBoxBindingBehavior.PasswordProperty, binding);
                        }
                        break;
                    }
                case ControlType.DatePicker:
                    {
                        _control = new DatePicker();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(DatePicker.SelectedDateProperty, binding);
                        }
                        break;
                    }
                case ControlType.TreeSelect:
                    {
                        _control = new TreeSelect();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.Mode = BindingMode.TwoWay;
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(TreeSelect.SelectedValueProperty, binding);
                            _control.SetValue(TreeSelect.DisplayMemberPathProperty, "Text");
                            _control.SetValue(TreeSelect.SelectedValuePathProperty, "Value");
                        }
                        var dataTemplate = new HierarchicalDataTemplate();
                        dataTemplate.ItemsSource = new Binding("Children");
                        FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
                        Binding bind = new Binding("Text");
                        fef.SetBinding(TextBlock.TextProperty, bind);
                        dataTemplate.VisualTree = fef;
                        (_control as TreeSelect).ItemTemplate = dataTemplate;
                        break;
                    }
                case ControlType.MultiComboBox:
                    {
                        _control = new MultiComboBox();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.Mode = BindingMode.TwoWay;
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(CustomeSelectionValues.SelectedValuesProperty, binding);
                            _control.SetValue(ComboBox.DisplayMemberPathProperty, "Text");
                            _control.SetValue(ComboBox.SelectedValuePathProperty, "Value");
                        }
                        break;
                    }
                case ControlType.IntegerUpDown:
                    {
                        _control = new IntegerUpDown();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    }
                case ControlType.LongUpDown:
                    {
                        _control = new LongUpDown();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    }
                case ControlType.DoubleUpDown:
                    {
                        _control = new DoubleUpDown();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    }
                case ControlType.DecimalUpDown:
                    {
                        _control = new DecimalUpDown();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    }
                case ControlType.DateTimeUpDown:
                    {
                        _control = new DateTimeUpDown();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(IntegerUpDown.ValueProperty, binding);
                        }
                        break;
                    }
                case ControlType.CheckBox:
                    {
                        _control = new CheckBox();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(CheckBox.IsCheckedProperty, binding);
                        }
                        break;
                    }
                case ControlType.ToggleButton:
                    {
                        _control = new ToggleButton();
                        if (!string.IsNullOrEmpty(Path))
                        {
                            Binding binding = new Binding(Path);
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                            binding.ValidatesOnExceptions = true;
                            binding.ValidatesOnDataErrors = true;
                            binding.NotifyOnValidationError = true;
                            _control.SetBinding(ToggleButton.IsCheckedProperty, binding);
                        }
                        break;
                    }
                case ControlType.Query:
                case ControlType.Submit:
                    {
                        hideHeader = true;

                        _control = new Button();
                        Binding binding = new Binding($"DataContext.{Path}");
                        binding.RelativeSource = new RelativeSource { AncestorType = typeof(UserControl), Mode = RelativeSourceMode.FindAncestor };
                        _control.SetBinding(Button.CommandProperty, binding);
                        Binding binding2 = new Binding(".");
                        _control.SetBinding(Button.CommandParameterProperty, binding2);
                        Binding binding3 = new Binding("Header");
                        binding3.Source = this;
                        _control.SetBinding(Button.ContentProperty, binding3);
                        break;
                    }
                default:
                    {
                        _control = null;
                        break;
                    }
            }

            if (_header != null)
            {
                _header.Visibility = hideHeader ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_control != null)
            {
                _control.SetCurrentValue(ControlAttach.ClearTextButtonProperty, true);
                _contentPresenter.Content = _control;
            }
        }

        private void SetItemsSource(string itemsSource)
        {
            if (_control != null)
            {
                var binding = new Binding($"DataContext.{itemsSource}");
                binding.RelativeSource = new RelativeSource { AncestorType = typeof(UserControl), Mode = RelativeSourceMode.FindAncestor };
                if (_control is ComboBox comboBox)
                {
                    comboBox.SetBinding(ComboBox.ItemsSourceProperty, binding);
                }
                else if (_control is TreeSelect treeSelect)
                {
                    treeSelect.SetBinding(TreeSelect.ItemsSourceProperty, binding);
                }
                else if (_control is MultiComboBox multiComboBox)
                {
                    multiComboBox.SetBinding(MultiComboBox.ItemsSourceProperty, binding);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentPresenter;
            _header = GetTemplateChild(PART_Header) as ContentPresenter;

            GetControl(ControlType);
            SetItemsSource(ItemsSource);
        }

    }
}