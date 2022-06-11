using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Behaviors;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.Controls.Converter;
using AIStudio.Wpf.Controls.Core;

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
        private FrameworkElement _control;


        #region ControlType
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register(
            "ControlType", typeof(FormControlType), typeof(FormCodeItem), new PropertyMetadata(FormControlType.TextBox, OnControlTypeChanged));

        private static void OnControlTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.GetControl((FormControlType)e.NewValue);
            }
        }

        [DisplayName("控件类型")]
        public FormControlType ControlType
        {
            get
            {
                return (FormControlType)GetValue(ControlTypeProperty);
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
        [DisplayName("属性名")]
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
        [DisplayName("绑定集合")]
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

        #region Span
        public static readonly DependencyProperty SpanProperty = DependencyProperty.Register(
            "Span", typeof(int), typeof(FormCodeItem), new PropertyMetadata(1, OnSpanChanged), new ValidateValueCallback(IsSpanValid));

        private static void OnSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FormCodeItem formCodeItem)
            {
                formCodeItem.SetValue(UniformGridEx.SpanProperty, formCodeItem.Span);
                formCodeItem.SetValue(WrapPanelFill.SpanProperty, formCodeItem.Span);
            }
        }

        [Browsable(true)]
        [DisplayName("Uniform跨度")]
        public int Span
        {
            get
            {
                return (int)GetValue(SpanProperty);
            }
            set
            {
                SetValue(SpanProperty, value);
            }
        }

        private static bool IsSpanValid(object value)
        {
            return ((int)value) > 0;
        }
        #endregion

        #region Extension1
        public static readonly DependencyProperty ExtField1Property = DependencyProperty.Register(
            "ExtField1", typeof(object), typeof(FormCodeItem), new PropertyMetadata(null));

        [Browsable(true)]
        [DisplayName("扩展字段1")]
        public object ExtField1
        {
            get
            {
                return (object)GetValue(ExtField1Property);
            }
            set
            {
                SetValue(ExtField1Property, value);
            }
        }
        #endregion

        #region Extension2
        public static readonly DependencyProperty ExtField2Property = DependencyProperty.Register(
            "ExtField2", typeof(object), typeof(FormCodeItem), new PropertyMetadata(null));

        [Browsable(true)]
        [DisplayName("扩展字段2")]
        public object ExtField2
        {
            get
            {
                return (object)GetValue(ExtField2Property);
            }
            set
            {
                SetValue(ExtField2Property, value);
            }
        }
        #endregion

        #region SettingField
        public static readonly DependencyProperty SettingFieldProperty = DependencyProperty.Register(
            "SettingField", typeof(object), typeof(FormCodeItem), new PropertyMetadata(null));

        [Browsable(true)]
        [DisplayName("设置字段")]
        public object SettingField
        {
            get
            {
                return (object)GetValue(SettingFieldProperty);
            }
            set
            {
                SetValue(SettingFieldProperty, value);
            }
        }
        #endregion

        #region 属性设置
        [Browsable(false)]
        [DisplayName("属性设置")]
        public Dictionary<string, string> PropertiesSetting
        {
            get
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
                {
                    {"Header", "名称" },
                    {"Path", "属性" },
                    {"Span", "跨度" }
                };

                switch (ControlType)
                {
                    case FormControlType.TextBox:
                    case FormControlType.PasswordBox:
                    case FormControlType.DatePicker:
                    case FormControlType.IntegerUpDown:
                    case FormControlType.LongUpDown:
                    case FormControlType.DoubleUpDown:
                    case FormControlType.DecimalUpDown:
                    case FormControlType.DateTimeUpDown:
                    case FormControlType.CheckBox:
                    case FormControlType.ToggleButton:
                    case FormControlType.RichTextBox:
                    case FormControlType.Query:
                    case FormControlType.Submit:
                        {
                            return keyValuePairs;
                        }
                    case FormControlType.ComboBox:
                    case FormControlType.TreeSelect:
                    case FormControlType.MultiComboBox:
                    case FormControlType.MultiTreeSelect:
                        {
                            keyValuePairs.Add("ItemsSource", "数据源");
                            return keyValuePairs;
                        }
                    case FormControlType.UploadFile:
                    case FormControlType.UploadImage:
                        {
                            keyValuePairs.Add("ExtField1", "上传地址");
                            keyValuePairs.Add("ExtField2", "上传Header");
                            return keyValuePairs;
                        }
                    case FormControlType.DataGrid:
                        {
                            keyValuePairs.Add("Height", "表格高度");
                            keyValuePairs.Add("SettingField", "表格设置");
                            return keyValuePairs;
                        }
                    default:
                        {
                            return keyValuePairs;
                        }
                }
            }
        }
        #endregion

        public double InitHeight
        {
            get; set;
        } = double.NaN;

        static FormCodeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormCodeItem), new FrameworkPropertyMetadata(typeof(FormCodeItem), FrameworkPropertyMetadataOptions.Inherits));
        }


        private void GetControl(FormControlType controlType)
        {
            if (_contentPresenter == null)
                return;

            bool hideHeader = false;

            if (ParentForm != null && ParentForm.FormMode == FormMode.ToolBar)
            {
                _control = new IconTextBlock();
                Binding binding = new Binding("Header");
                binding.Source = this;
                _control.SetBinding(IconTextBlock.ContentProperty, binding);

                var bindingIcon = new Binding();
                bindingIcon.Path = new PropertyPath(IconAttach.GeometryProperty);
                bindingIcon.Mode = BindingMode.OneWay;
                bindingIcon.Source = this;
                _control.SetBinding(IconAttach.GeometryProperty, bindingIcon);

                _control.Cursor = Cursors.SizeAll;
                _control.SetValue(IconTextBlock.PaddingProperty, new Thickness(6));
            }
            else
            {
                switch (controlType)
                {
                    case FormControlType.TextBox:
                        {
                            _control = new TextBox() { Background = Brushes.Transparent };
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
                    case FormControlType.ComboBox:
                        {
                            _control = new ComboBox() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(ComboBox.SelectedValueProperty, binding);
                            }
                            _control.SetValue(ComboBox.DisplayMemberPathProperty, "Text");
                            _control.SetValue(ComboBox.SelectedValuePathProperty, "Value");
                            break;
                        }
                    case FormControlType.PasswordBox:
                        {
                            _control = new PasswordBox() { Background = Brushes.Transparent };
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
                    case FormControlType.DatePicker:
                        {
                            _control = new DatePicker() { Background = Brushes.Transparent };
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
                    case FormControlType.TreeSelect:
                        {
                            _control = new TreeSelect() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.Mode = BindingMode.TwoWay;
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(TreeSelect.SelectedValueProperty, binding);
                            }
                            _control.SetValue(TreeSelect.DisplayMemberPathProperty, "Text");
                            _control.SetValue(TreeSelect.SelectedValuePathProperty, "Value");

                            var dataTemplate = new HierarchicalDataTemplate();
                            dataTemplate.ItemsSource = new Binding("Children");
                            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));
                            Binding bind = new Binding("Text");
                            fef.SetBinding(TextBlock.TextProperty, bind);
                            dataTemplate.VisualTree = fef;
                            (_control as TreeSelect).ItemTemplate = dataTemplate;
                            break;
                        }
                    case FormControlType.MultiComboBox:
                        {
                            _control = new MultiComboBox() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.Mode = BindingMode.TwoWay;
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(CustomeSelectionValues.SelectedValuesProperty, binding);
                            }
                            _control.SetValue(ComboBox.DisplayMemberPathProperty, "Text");
                            _control.SetValue(ComboBox.SelectedValuePathProperty, "Value");
                            break;
                        }
                    case FormControlType.MultiTreeSelect:
                        {
                            _control = new TreeSelect() { Background = Brushes.Transparent, IsMulti = true };

                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.Mode = BindingMode.TwoWay;
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(TreeSelect.SelectedValuesProperty, binding);
                            }
                            _control.SetValue(TreeSelect.DisplayMemberPathProperty, "Text");
                            _control.SetValue(TreeSelect.SelectedValuePathProperty, "Value");

                            var dataTemplate = new HierarchicalDataTemplate();
                            dataTemplate.ItemsSource = new Binding("Children");
                            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(StackPanel));
                            fef.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

                            FrameworkElementFactory checkbox_fef = new FrameworkElementFactory(typeof(CheckBox));
                            fef.AppendChild(checkbox_fef);
                            Binding checkbox_bind = new Binding("IsChecked");
                            checkbox_fef.SetBinding(CheckBox.IsCheckedProperty, checkbox_bind);

                            FrameworkElementFactory textblock_fef = new FrameworkElementFactory(typeof(TextBlock));
                            fef.AppendChild(textblock_fef);
                            Binding textblock_bind = new Binding("Text");
                            textblock_fef.SetBinding(TextBlock.TextProperty, textblock_bind);
                            textblock_fef.SetValue(TextBlock.MarginProperty, new Thickness(2, 0, 0, 0));

                            dataTemplate.VisualTree = fef;
                            (_control as TreeSelect).ItemTemplate = dataTemplate;
                            break;
                        }
                    case FormControlType.IntegerUpDown:
                        {
                            _control = new IntegerUpDown() { Background = Brushes.Transparent };
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
                    case FormControlType.LongUpDown:
                        {
                            _control = new LongUpDown() { Background = Brushes.Transparent };
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
                    case FormControlType.DoubleUpDown:
                        {
                            _control = new DoubleUpDown() { Background = Brushes.Transparent };
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
                    case FormControlType.DecimalUpDown:
                        {
                            _control = new DecimalUpDown() { Background = Brushes.Transparent };
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
                    case FormControlType.DateTimeUpDown:
                        {
                            _control = new DateTimeUpDown() { Background = Brushes.Transparent };
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
                    case FormControlType.CheckBox:
                        {
                            _control = new CheckBox() { Background = Brushes.Transparent };
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
                    case FormControlType.ToggleButton:
                        {
                            _control = new ToggleButton() { Background = Brushes.Transparent };
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
                    case FormControlType.RichTextBox:
                        {
                            _control = new RichTextBox() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(RichTextBox.TextProperty, binding);
                            }
                            break;
                        }
                    case FormControlType.UploadFile:
                        {
                            _control = new UploadFile() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(UploadFile.FileProperty, binding);
                            }
                            _control.SetValue(UploadFile.UploadUrlProperty, ExtField1);
                            _control.SetValue(UploadFile.UploadTokenProperty, ExtField2);
                            break;
                        }
                    case FormControlType.UploadImage:
                        {
                            _control = new UploadFile() { Background = Brushes.Transparent };
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(UploadFile.FileProperty, binding);
                                _control.SetValue(UploadFile.UploadFileTypeProperty, UploadFileType.Image);
                            }
                            _control.SetValue(UploadFile.UploadUrlProperty, ExtField1);
                            _control.SetValue(UploadFile.UploadTokenProperty, ExtField2);
                            break;
                        }
                    case FormControlType.DataGrid:
                        {
                            _control = new DataGrid() { Background = Brushes.Transparent };
                            (_control as DataGrid).IsReadOnly = true;
                            if (!string.IsNullOrEmpty(Path))
                            {
                                Binding binding = new Binding(Path);
                                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                                binding.ValidatesOnExceptions = true;
                                binding.ValidatesOnDataErrors = true;
                                binding.NotifyOnValidationError = true;
                                _control.SetBinding(DataGrid.ItemsSourceProperty, binding);
                            }

                            if (ExtField1 is IEnumerable<DataGridColumn> columns)
                            {
                                if (columns != null)
                                {
                                    foreach (var column in columns)
                                    {
                                        var copy = System.Windows.Markup.XamlReader.Parse(System.Windows.Markup.XamlWriter.Save(column)) as DataGridColumn;
                                        (_control as DataGrid).Columns.Add(copy);
                                    }
                                }
                            }
                            else if (ExtField1 is IEnumerable<DataGridColumnConfig> columnconfigs)
                            {
                                if (columnconfigs != null)
                                {
                                    foreach (var columnconfig in columnconfigs)
                                    {
                                        (_control as DataGrid).Columns.Add(columnconfig.GetColumn());
                                    }
                                }
                            }
                            else
                            {
                                (_control as DataGrid).Columns.Add(new DataGridTextColumn()
                                {
                                    Binding = new Binding("Field1"),
                                    Header = "字段1",
                                    Width = new DataGridLength(100, DataGridLengthUnitType.Pixel)
                                });
                                (_control as DataGrid).Columns.Add(new DataGridTextColumn()
                                {
                                    Binding = new Binding("Field2"),
                                    Header = "字段2",
                                    Width = new DataGridLength(100, DataGridLengthUnitType.Pixel)
                                });
                                (_control as DataGrid).Columns.Add(new DataGridTextColumn()
                                {
                                    Binding = new Binding("Field3"),
                                    Header = "字段3",
                                    Width = new DataGridLength(100, DataGridLengthUnitType.Pixel)
                                });
                                (_control as DataGrid).Columns.Add(new DataGridTextColumn()
                                {
                                    Binding = new Binding("Field4"),
                                    Header = "字段4",
                                    Width = new DataGridLength(100, DataGridLengthUnitType.Pixel)
                                });
                            }

                            _control.SetValue(DataGridColumnsConfigAttach.ShowConfigProperty, true);
                            break;
                        }
                    case FormControlType.Query:
                    case FormControlType.Submit:
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

                this.Height = InitHeight;
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

            SetItemsSource(ItemsSource);
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

        }

        private string GetPath()
        {
            return string.IsNullOrEmpty(Path) ? "." : Path;
        }

        public string GetSpan()
        {
            if (ParentForm != null && Span > 1)
            {
                if (ParentForm.PanelType == FormPanelType.UniformGrid)
                {
                    return $"ac:UniformGridEx.Span=\"{Span}\"";
                }
                if (ParentForm.PanelType == FormPanelType.UniformWrapPanel)
                {
                    return $"ac:UniformWrapPanel.Span=\"{Span}\"";
                }
            }

            return string.Empty;
        }

        private string GetDataGridColumns()
        {
            List<string> formColumnsList = new List<string>();
            if (_control is DataGrid datagrid)
            {

                foreach (var item in datagrid.Columns.OrderBy(p => p.DisplayIndex))
                {
                    string str = string.Empty;
//                    if (item is DataGridTextColumn dataGridTextColumn)
//                    {
//                        str =
//$"                   <DataGridTextColumn Header=\"{dataGridTextColumn.Header}\" Binding=\"{{Binding {(dataGridTextColumn.Binding as Binding).Path.Path}}}\" Width=\"{dataGridTextColumn.Width.ToString()}\"/>";
//                    }
//                    else if (item is DataGridCheckBoxColumn dataGridCheckBoxColumn)
//                    {
//                        str =
//$"                   <DataGridCheckBoxColumn Header=\"{dataGridCheckBoxColumn.Header}\" Binding=\"{{Binding {(dataGridCheckBoxColumn.Binding as Binding).Path.Path}}}\" Width=\"{dataGridCheckBoxColumn.Width.ToString()}\"/>";
//                    }
//                    else if (item is DataGridComboBoxColumn dataGridComboBoxColumn)
//                    {
//                        str =
//$"                   <DataGridComboBoxColumn Header=\"{dataGridComboBoxColumn.Header}\" SelectedValueBinding=\"{{Binding {(dataGridComboBoxColumn.SelectedValueBinding as Binding).Path.Path}}}\" DisplayMemberPath=\"Text\" SelectedValuePath=\"Value\" Width=\"{dataGridComboBoxColumn.Width.ToString()}\"/>";
//                    }
//                    else if (item is DataGridHyperlinkColumn dataGridHyperlinkColumn)
//                    {
//                        str =
//$"                   <DataGridHyperlinkColumn Header=\"{dataGridHyperlinkColumn.Header}\" Binding=\"{{Binding {(dataGridHyperlinkColumn.Binding as Binding).Path.Path}}}\" Width=\"{dataGridHyperlinkColumn.Width.ToString()}\"/>";
//                    }
//                    else
                    {
                        str =
"                   " + Regex.Replace(System.Windows.Markup.XamlWriter.Save(item), "( ClipboardContentBinding=\".*?\")|( DisplayIndex=\".*?\")|( xmlns=\".*?\")", "");
                    }
                    formColumnsList.Add(str);
                }
            }

            return string.Join("\r\n", formColumnsList);
        }

        public override string ToString()
        {
            switch (ControlType)
            {
                case FormControlType.TextBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <TextBox Text=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.TextBox.Underline}}\"></TextBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.ComboBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ComboBox SelectedValue=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ItemsSource=\"{{ac:ControlBinding {ItemsSource}}}\"  DisplayMemberPath=\"Text\" SelectedValuePath=\"Value\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.ComboBox.Underline}}\"></ComboBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.PasswordBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <PasswordBox ac:PasswordBoxBindingBehavior.Password=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.PasswordBox.Underline}}\"></PasswordBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.DatePicker:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <DatePicker SelectedDate=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.DatePicker.Underline}}\"></DatePicker>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.TreeSelect:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:TreeSelect SelectedValue=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ItemsSource=\"{{ac:ControlBinding {ItemsSource}}}\"  DisplayMemberPath=\"Text\" SelectedValuePath=\"Value\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.TreeSelect.Underline}}\">" + "\r\n" +
$"              <ac:TreeSelect.ItemTemplate>" + "\r\n" +
$"                  <HierarchicalDataTemplate ItemsSource = \"{{Binding Children}}\">" + "\r\n" +
$"                      <StackPanel Orientation = \"Horizontal\">" + "\r\n" +
$"                          <TextBlock Text = \"{{Binding Text}}\" VerticalAlignment = \"Center\" />" + "\r\n" +
$"                      </StackPanel>" + "\r\n" +
$"                  </HierarchicalDataTemplate>" + "\r\n" +
$"              </ac:TreeSelect.ItemTemplate>" + "\r\n" +
$"          </ac:TreeSelect>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.MultiComboBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:MultiComboBox ac:CustomeSelectionValues.SelectedValues=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ItemsSource=\"{{ac:ControlBinding {ItemsSource}}}\"  DisplayMemberPath=\"Text\" SelectedValuePath=\"Value\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.MultiComboBox.Underline}}\"></ac:MultiComboBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.MultiTreeSelect:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:TreeSelect SelectedValues=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ItemsSource=\"{{ac:ControlBinding {ItemsSource}}}\"  DisplayMemberPath=\"Text\" SelectedValuePath=\"Value\" IsMulti=\"True\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.TreeSelect.Underline}}\">" + "\r\n" +
$"              <ac:TreeSelect.ItemTemplate>" + "\r\n" +
$"                  <HierarchicalDataTemplate ItemsSource = \"{{Binding Children}}\">" + "\r\n" +
$"                      <StackPanel Orientation = \"Horizontal\">" + "\r\n" +
$"                          <CheckBox IsChecked=\"{{Binding IsChecked,Mode=TwoWay}}\" />" + "\r\n" +
$"                          <TextBlock Text = \"{{Binding Text}}\" VerticalAlignment = \"Center\" />" + "\r\n" +
$"                      </StackPanel>" + "\r\n" +
$"                  </HierarchicalDataTemplate>" + "\r\n" +
$"              </ac:TreeSelect.ItemTemplate>" + "\r\n" +
$"          </ac:TreeSelect>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.IntegerUpDown:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:IntegerUpDown Value=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.NumericUpDown}}\"></ac:IntegerUpDown>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.LongUpDown:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:LongUpDown Value=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.NumericUpDown}}\"></ac:LongUpDown>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.DoubleUpDown:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:DoubleUpDown Value=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.NumericUpDown}}\"></ac:DoubleUpDown>" + "\r\n" +
"        </ac:FormItem>";
                        return str;
                    }
                case FormControlType.DecimalUpDown:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:DecimalUpDown Value=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.NumericUpDown}}\"></ac:DecimalUpDown>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.CheckBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <CheckBox IsChecked=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.CheckBox}}\"></CheckBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.ToggleButton:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ToggleButton IsChecked=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.ToggleButton.Switch}}\"></ToggleButton>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.RichTextBox:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:RichTextBox Text=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.RichTextBox.Underline}}\"></ac:RichTextBox>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.UploadFile:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:UploadFile File=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" UploadUrl=\"{ExtField1?.ToString()}\" UploadToken=\"{ExtField2?.ToString().ToProviderString("xaml")}\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.UploadFile.Underline}}\"></ac:UploadFile>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.UploadImage:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <ac:UploadFile File=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" UploadUrl=\"{ExtField1?.ToString()}\" UploadToken=\"{ExtField2?.ToString().ToProviderString("xaml")}\" UploadFileType=\"Image\" ac:ControlAttach.ClearTextButton=\"True\" Style=\"{{DynamicResource AIStudio.Styles.UploadFile.Underline}}\"></ac:UploadFile>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.DataGrid:
                    {
                        string str =
$"      <ac:FormItem Header=\"{Header}\" {GetSpan()}>" + "\r\n" +
$"          <DataGrid ItemsSource=\"{{Binding {GetPath()},Mode=TwoWay,UpdateSourceTrigger=PropertyChanged, ValidatesOnExceptions=True, ValidatesOnDataErrors=True, NotifyOnValidationError=True}}\" IsReadOnly=\"True\" Style=\"{{DynamicResource AIStudio.Styles.DataGrid}}\">" + "\r\n" +
$"              <DataGrid.Columns>" + "\r\n" +
GetDataGridColumns() + "\r\n" +
$"              </DataGrid.Columns>" + "\r\n" +
$"          </DataGrid>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                case FormControlType.Query:
                case FormControlType.Submit:
                case FormControlType.Add:
                case FormControlType.Delete:
                    {
                        string str =
$"      <ac:FormItem {GetSpan()}>" + "\r\n" +
$"          <Button Content=\"{Header}\" Command=\"{{ac:ControlBinding {GetPath()}}}\" CommandParameter=\"{{Binding .}}\" Style=\"{{DynamicResource AIStudio.Styles.Button}}\"></Button>" + "\r\n" +
"       </ac:FormItem>";
                        return str;
                    }
                default: return System.Windows.Markup.XamlWriter.Save(this);
            }
        }

        public void Setting()
        {
            if (_control is DataGrid datagrid)
            {
                var window = new DataGridColumnConfigWindow(datagrid);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
            }
        }

        public void InitControl(object obj)
        {
            if (_control is DataGrid datagrid)
            {
                datagrid.Columns.Clear();
                if (obj is IEnumerable<DataGridColumn> columns)
                {
                    if (columns != null)
                    {
                        foreach (var column in columns)
                        {
                            datagrid.Columns.Add(column);
                        }
                    }
                }
                else if (obj is IEnumerable<DataGridColumnConfig> columnconfigs)
                {
                    if (columnconfigs != null)
                    {
                        foreach (var columnconfig in columnconfigs)
                        {
                            datagrid.Columns.Add(columnconfig.GetColumn());
                        }
                    }
                }
            }

        }

    }
}