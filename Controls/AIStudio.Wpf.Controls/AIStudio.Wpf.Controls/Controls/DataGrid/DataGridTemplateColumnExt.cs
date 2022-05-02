using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class CustomCommand : ICommand
    {
        private Action<object> commandExeAction;

        public CustomCommand(Action<object> action)
        {
            this.commandExeAction = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (commandExeAction != null)
                commandExeAction.Invoke(parameter);
        }
    }

    public class DataGridTemplateColumnExt : DataGridTemplateColumn, IFilteringSupportColumn
    {
        private const string NULLVALUE_DISPLAY_STR = "<空>";

        Popup popup = null;
        OptionList Option;
        ToggleButton button = null;
        private bool hasNoFilter = true;
        public ICommand BtnCommand
        {
            get { return new CustomCommand(ColCmdExeAction); }
        }

        private static SolidColorBrush defaultBorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#848D9C"));
        public Brush DefaultFillBrush
        {
            get { return (Brush)GetValue(DefaultFillBrushProperty); }
            set { SetValue(DefaultFillBrushProperty, value); }
        }


        public static readonly DependencyProperty DefaultFillBrushProperty =
            DependencyProperty.Register("DefaultFillBrush", typeof(Brush), typeof(DataGridTemplateColumnExt), new PropertyMetadata(defaultBorderBrush));

        /// <summary>
        /// 按钮是否显示
        /// </summary>
        public string btnVisbile
        {
            get { return (string)GetValue(IsVisbileProperty); }
            set { SetValue(IsVisbileProperty, value); }
        }

        public static readonly DependencyProperty IsVisbileProperty =
            DependencyProperty.Register("btnVisbile", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata("1"));


        public bool IsVisbiles
        {
            get { return (bool)GetValue(IsVisbilesProperty); }
            set { SetValue(IsVisbilesProperty, value); }
        }

        public static readonly DependencyProperty IsVisbilesProperty =
            DependencyProperty.Register("IsVisbiles", typeof(bool), typeof(DataGridTemplateColumnExt), new PropertyMetadata(true));


        /// <summary>
        /// 需要合并的列的数量
        /// </summary>
        public int MergeRows
        {
            get { return (int)GetValue(MergeRowsProperty); }
            set { SetValue(MergeRowsProperty, value); }
        }

        public static readonly DependencyProperty MergeRowsProperty =
            DependencyProperty.Register("MergeRows", typeof(int), typeof(DataGridTemplateColumnExt), new PropertyMetadata(0));

        /// <summary>
        /// 存储多列的名称(^隔开)
        /// </summary>
        public string MultiColumnsName
        {
            get { return (string )GetValue(MultiColumnsnameProperty); }
            set { SetValue(MultiColumnsnameProperty, value); }
        }

        public static readonly DependencyProperty MultiColumnsnameProperty =
            DependencyProperty.Register("MultiColumnsName", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata(""));

        /// <summary>
        /// 存储多列绑定的值(^隔开)
        /// </summary>
        public string MultiColumnsValue
        {
            get { return (string)GetValue(MultiColumnsValueProperty); }
            set { SetValue(MultiColumnsValueProperty, value); }
        }

        public static readonly DependencyProperty MultiColumnsValueProperty =
            DependencyProperty.Register("MultiColumnsValue", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata(""));

        /// <summary>
        /// 存储多列定的格式化方式(^隔开)
        /// </summary>
        public string MultiColumnsStrFrom
        {
            get { return (string)GetValue(MultiColumnsStrFromProperty); }
            set { SetValue(MultiColumnsStrFromProperty, value); }
        }

        public static readonly DependencyProperty MultiColumnsStrFromProperty =
            DependencyProperty.Register("MultiColumnsStrFrom", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata(""));



        /// <summary>
        /// 格式化内容
        /// </summary>
        public string StrFormat
        {
            get { return (string)GetValue(StrFormatProperty); }
            set { SetValue(StrFormatProperty, value); }
        }

        public static readonly DependencyProperty StrFormatProperty =
            DependencyProperty.Register("StrFormat", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata(""));


        private void ColCmdExeAction(object obj)
        {
            try
            {
                btnVisbile = "0";
                if (obj != null)
                {
                    this.CanUserSort = false;
                    optionOrdts = new List<OptionOrdt>();
                    if (this.DataGridOwner == null || this.DataGridOwner.ItemsSource == null)
                        return;
                    Predicate<object> filter = null;
                    var view = CollectionViewSource.GetDefaultView(this.DataGridOwner.ItemsSource);
                    if (view != null)
                    {
                        filter = view.Filter;
                    }

                    Itemsoure = this.DataGridOwner.ItemsSource;
                    optionOrdts.Clear();

                    foreach (var item in Itemsoure)
                    {
                        bool check = true;
                        var name = GetThisFilterPropValue(item);

                        string viewName = FormatValue(name);

                        if (FromFilteredSource)
                            if (filter != null && !filter(item))
                            {
                                check = false;
                            }

                        var exitOpt = optionOrdts.FirstOrDefault(X => X.Name == viewName);

                        if (exitOpt == null)
                        {
                            //if (check == true)
                            {
                                optionOrdts.Add(new OptionOrdt { Name = viewName, Host = check });
                            }
                        }
                        else
                        {
                            if (exitOpt.Host == false)
                            {
                                exitOpt.Host = check;
                            }
                        }

                    }

                    var uiElemen = obj as UIElement;
                    button = obj as ToggleButton;
                    popup.PlacementTarget = uiElemen;
                    popup.MinWidth = 160;
                    popup.StaysOpen = false;
                    Option.HearName = this.HeaderName;
                    Option.SetValuesToCheck(optionOrdts.Distinct());
                    Option.SaveEvent += Option_SaveEvent;
                    popup.Child = Option;
                    popup.IsOpen = true;

                    popup.Closed -= Popup_Closed;
                    popup.Closed += Popup_Closed;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private string FormatValue(object value)
        {
            if (value == null) return NULLVALUE_DISPLAY_STR;
            if (string.IsNullOrWhiteSpace(value.ToString())) return NULLVALUE_DISPLAY_STR;

            var valueType = value.GetType();

            if (valueType == typeof(DateTime))
                return ((DateTime)value).ToString(this.StrFormat);
            if (valueType == typeof(DateTime?))
                return ((DateTime?)value).Value.ToString(this.StrFormat);

            if (valueType == typeof(short))
                return ((short)value).ToString(this.StrFormat);
            if (valueType == typeof(short?))
                return ((short?)value).Value.ToString(this.StrFormat);

            if (valueType == typeof(int))
                return ((int)value).ToString(this.StrFormat);
            if (valueType == typeof(int?))
                return ((int?)value).Value.ToString(this.StrFormat);

            if (valueType == typeof(long))
                return ((long)value).ToString(this.StrFormat);
            if (valueType == typeof(long?))
                return ((long?)value).Value.ToString(this.StrFormat);

            if (valueType == typeof(double))
                return ((double)value).ToString(this.StrFormat);
            if (valueType == typeof(double?))
                return ((double?)value).Value.ToString(this.StrFormat);

            return value.ToString();
        }


        private void Popup_Closed(object sender, EventArgs e)
        {
            this.CanUserSort = true;
            if (strState == "ColorNotAll_State")
                btnVisbile = "0";
            else
                btnVisbile = "1";

            if (FilterAction != null)
            {
                FilterAction(this);
            }
        }

        public DataGridTemplateColumnExt() : base()
        {
            popup = new Popup();
            Option = new OptionList();
            popup.Child = Option;
        }

        public List<OptionOrdt> optionOrdts
        {
            get { return (List<OptionOrdt>)GetValue(optionOrdtsProperty); }
            set { SetValue(optionOrdtsProperty, value); }
        }

        public static readonly DependencyProperty optionOrdtsProperty =
            DependencyProperty.Register("optionOrdts", typeof(List<OptionOrdt>), typeof(DataGridTemplateColumnExt), new PropertyMetadata(null, null));


        System.Collections.IEnumerable Itemsoure { get; set; }
        /// <summary>
        /// 需要筛选的属性名称( the path of property to be filtered)
        /// </summary>
        public string HeaderName
        {
            get {

                if (string.IsNullOrEmpty((string)GetValue(HeaderNameProperty)))
                {
                    return (string)GetValue(SortMemberPathProperty);
                }
                else
                {
                    return (string)GetValue(HeaderNameProperty);
                }

            }
            set { SetValue(HeaderNameProperty, value); }
        }

        public bool FromFilteredSource { get; set; }

        public static readonly DependencyProperty HeaderNameProperty =
            DependencyProperty.Register("HeaderName", typeof(string), typeof(DataGridTemplateColumnExt), new PropertyMetadata(""));

        private bool isEnsoured = false;
        /// <summary>
        /// 按钮执行的状态
        /// </summary>
        private string strState = string.Empty;

        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(DataGridTemplateColumnExt), new PropertyMetadata(null));

        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(DataGridTemplateColumnExt), new PropertyMetadata(null));

        /// <summary>
        /// 进行数据筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Option_SaveEvent(object sender, EventArgs e)
        {
            isEnsoured = true;

            optionOrdts = Option != null ? Option.OptionOrdtS.ToList() : new List<OptionOrdt>();

            if (Option != null && Option.OptionOrdtS.Any())
            {
                if (Option.OptionOrdtS.All(x => x.Host))
                {

                    try
                    {
                        if (button != null)
                        {
                            var colHeader = button.TemplatedParent as DataGridColumnHeader;
                            VisualStateManager.GoToState(colHeader, "ColorAll_State", true);
                            strState = "ColorAll_State";
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {

                    try
                    {
                        if (button != null)
                        {
                            var colHeader = button.TemplatedParent as DataGridColumnHeader;
                            VisualStateManager.GoToState(colHeader, "ColorNotAll_State", true);
                            strState = "ColorNotAll_State";
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                }

            }
            
            SetFilter();

            DataGridExt dataGridExt = this.DataGridOwner as DataGridExt;

            var view = CollectionViewSource.GetDefaultView(this.DataGridOwner.ItemsSource);
            if (view != null)
            {
                dataGridExt.ItemsChanged -= DataGridExt_ItemsChanged;
                view.Refresh();
                dataGridExt.ItemsChanged += DataGridExt_ItemsChanged;
            }


            if (dataGridExt != null)
            {
                dataGridExt.CalculateSum();
                dataGridExt.ItemsChanged -= DataGridExt_ItemsChanged;
                dataGridExt.ItemsChanged += DataGridExt_ItemsChanged;
            }

            if (popup != null)
                popup.IsOpen = false;

            if (FilterChanged != null)
            {
                FilterChanged(this, strState);
            }
        }
        
        private void DataGridExt_ItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            DataGridExt dataGridExt = sender as DataGridExt;
            dataGridExt.ItemsChanged -= DataGridExt_ItemsChanged;
            SetFilter();
            var view = CollectionViewSource.GetDefaultView(dataGridExt.ItemsSource);
            if (view != null)
                view.Refresh();
            dataGridExt.CalculateSum();

            //数据源改变后，重置条件
            //ClearFilter();

            if (FilterChanged != null)
            {
                FilterChanged(this, "ColorAll_State");
            }
            dataGridExt.ItemsChanged += DataGridExt_ItemsChanged;
        }

        /// <summary>
        /// 设置条件过滤
        /// </summary>
        private void SetFilter()
        {
            var view = CollectionViewSource.GetDefaultView(this.DataGridOwner.ItemsSource);

            if (view != null)
            {
                if (view.Filter == null || view.Filter.GetInvocationList()[0].Method.Name != "BankDataGridTemplateColumn_CloumnFilter")
                {
                    outerFilter = view.Filter;
                    view.Filter = BankDataGridTemplateColumn_CloumnFilter;
                }

                hasNoFilter = false;
            }
        }

        private Predicate<object> outerFilter;

        public event Action<IFilteringSupportColumn, string> FilterChanged;

        public Action<IFilteringSupportColumn> FilterAction
        {
            get { return (Action<IFilteringSupportColumn>)GetValue(FilterActionProperty); }
            set { SetValue(FilterActionProperty, value); }
        }

        public static readonly DependencyProperty FilterActionProperty =
          DependencyProperty.Register("FilterAction", typeof(Action<IFilteringSupportColumn>), typeof(DataGridTemplateColumnExt));

        /// <summary>
        /// /设置列的过滤
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool BankDataGridTemplateColumn_CloumnFilter(object obj)
        {
            if (outerFilter != null && !outerFilter(obj)) return false;
            foreach (var item in this.DataGridOwner.Columns)
            {
                var col = item as DataGridTemplateColumnExt;
                if (col == null) continue;
                if (!col.RecordDisplayable(obj))
                    return false;
            }
            return true;
        }

        private bool RecordDisplayable(object obj)
        {
            if (!isEnsoured) return true;
            if (hasNoFilter) return true;

            var checkedValues = Option.GetCheckedValues().ToList();
            if (!checkedValues.Any()) return false;

            var value = GetThisFilterPropValue(obj);
            var valueStr = FormatValue(value);

            return checkedValues.Any(x => x == valueStr);
        }

        public string[] GetFilteredDescribe()
        {
            var checkedOpts = Option.GetCheckedValues();
            if (checkedOpts == null || !checkedOpts.Any()) return new string[] { "<全不>" };

            return checkedOpts.ToArray();
        }

        public void ClearFilter()
        {
            DataGridExt dataGridExt = this.DataGridOwner as DataGridExt;
            if (dataGridExt != null)
            {
                dataGridExt.ItemsChanged -= DataGridExt_ItemsChanged;
            }
            if (Option != null)
            {
                Option.IsCheck = true;
                hasNoFilter = true;
                Option.SaveAction();

                var view = CollectionViewSource.GetDefaultView(this.DataGridOwner.ItemsSource);
                if (view != null)
                    view.Refresh();

                if (dataGridExt != null)
                {
                    dataGridExt.CalculateSum();
                }
            }

            if (FilterAction != null)
            {
                FilterAction(this);
            }
        }

        public string GetFilterTitle()
        {
            return this.Header.ToString();
        }


        private static readonly Dictionary<DataGrid, Dictionary<string, FilterPropertyPathPropInfos>> FilterPropInfoIndatagrid = new Dictionary<DataGrid, Dictionary<string, FilterPropertyPathPropInfos>>();

        private object GetThisFilterPropValue(object item)
        {

            var datagrid = this.DataGridOwner;
            if (datagrid == null) return null;

            Dictionary<string, FilterPropertyPathPropInfos> filterPathPropInfo;
            if (!FilterPropInfoIndatagrid.TryGetValue(datagrid, out filterPathPropInfo))
            {
                FilterPropInfoIndatagrid[datagrid] = filterPathPropInfo = new Dictionary<string, FilterPropertyPathPropInfos>();
            }

            FilterPropertyPathPropInfos propertyPathPropInfos;
            if (!filterPathPropInfo.TryGetValue(this.HeaderName, out propertyPathPropInfos))
            {
                propertyPathPropInfos = new FilterPropertyPathPropInfos(this.HeaderName);
                propertyPathPropInfos.InitPropertyInfosWithObject(item.GetType());

                filterPathPropInfo[this.HeaderName] = propertyPathPropInfos;
            }

            var obj = propertyPathPropInfos.GetFilterPropValue(item);
            if (Converter != null)
            {
                obj = Converter.Convert(obj, item.GetType(), ConverterParameter, null);
            }
            return obj;
        }


        private class FilterPropertyPathPropInfos
        {
            private PropertyInfo[] _propertyInfos;
            private string[] _proppertyNames;

            public FilterPropertyPathPropInfos(string filterPropertyPath)
            {
                if (string.IsNullOrWhiteSpace(filterPropertyPath))
                    throw new ArgumentNullException("filterPropertyPath");

                this.FilterPropertyPath = filterPropertyPath;
                this._proppertyNames = this.FilterPropertyPath.Split('.');
                this._propertyInfos = new PropertyInfo[this._proppertyNames.Length];
            }
            public string FilterPropertyPath { get; private set; }

            public void InitPropertyInfosWithObject(Type objValueType)
            {
                if (objValueType == null)
                    throw new ArgumentNullException("objValueType");

                var type = objValueType;

                for (int i = 0; i < _proppertyNames.Length; i++)
                {
                    var propInfo = type.GetProperty(_proppertyNames[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                    this._propertyInfos[i] = propInfo;

                    if (propInfo == null) return;

                    type = propInfo.PropertyType;
                }
            }

            public object GetFilterPropValue(object item)
            {
                var curObj = item;
                for (int i = 0; i < this._proppertyNames.Length; i++)
                {
                    if (curObj == null) return null;
                    if (this._propertyInfos[i] == null) return null;
                    curObj = this._propertyInfos[i].GetValue(curObj, null);
                }

                return curObj;
            }
        }

    }
    /// <summary>
    /// 筛选类
    /// </summary>
    public class OptionOrdt
    {
        public string Name { get; set; }
        public bool Host { get; set; }
    }
}

