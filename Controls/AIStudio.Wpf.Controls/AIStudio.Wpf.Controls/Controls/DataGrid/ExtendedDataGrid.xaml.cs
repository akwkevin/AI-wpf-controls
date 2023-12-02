using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;
using AIStudio.Wpf.Controls.Commands;
using AIStudio.Wpf.Controls.Helper;
using Microsoft.Win32;

namespace AIStudio.Wpf.Controls
{
    [TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_ColumnHeadersPresenter, Type = typeof(DataGridColumnHeadersPresenter))]
    [TemplatePart(Name = PART_ColumnHeadersContextMenu, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_RefreshMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_SaveCurrentMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_SaveAllMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_ShowHiddenColumnsMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_HiddenColumnHeadersPopUp, Type = typeof(Popup))]
    [TemplatePart(Name = PART_AutoRefreshIntervalCombo, Type = typeof(ComboBox))]
    [TemplatePart(Name = PART_HideCurrentColumnMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_ListBox, Type = typeof(DragDropListBox))]
    [TemplatePart(Name = PART_TotalRow, Type = typeof(DataGrid))]
    public class ExtendedDataGrid : DataGrid
    {
        #region 常量
        private const string PART_ScrollViewer = "PART_ScrollViewer";
        private const string PART_ColumnHeadersPresenter = "PART_ColumnHeadersPresenter";
        private const string PART_ColumnHeadersContextMenu = "PART_ColumnHeadersContextMenu";
        private const string PART_RefreshMenuItem = "PART_RefreshMenuItem";
        private const string PART_SaveCurrentMenuItem = "PART_SaveCurrentMenuItem";
        private const string PART_SaveAllMenuItem = "PART_SaveAllMenuItem";
        private const string PART_ShowHiddenColumnsMenuItem = "PART_ShowHiddenColumnsMenuItem";
        private const string PART_HiddenColumnHeadersPopUp = "PART_HiddenColumnHeadersPopUp";
        private const string PART_AutoRefreshIntervalCombo = "PART_AutoRefreshIntervalCombo";
        private const string PART_HideCurrentColumnMenuItem = "PART_HideCurrentColumnMenuItem";
        private const string PART_ListBox = "PART_ListBox";
        private const string PART_TotalRow = "PART_TotalRow";
        private const string PATH_DataGridInfos = "DataGridInfos";
        #endregion

        #region 公开属性
        #region RefreshCommand 刷新命令

        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(ExtendedDataGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 刷新命令
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return (ICommand)GetValue(RefreshCommandProperty);
            }
            set
            {
                SetValue(RefreshCommandProperty, value);
            }
        }

        #endregion

        #region RefreshAction 刷新动作

        public static readonly DependencyProperty RefreshActionProperty = DependencyProperty.Register("RefreshAction", typeof(Action), typeof(ExtendedDataGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RefreshActionChanged));

        private static void RefreshActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedDataGrid dg = d as ExtendedDataGrid;
            if (dg == null)
                return;

            if (dg.RefreshAction != null)
            {
                dg.RefreshCommand = new RelayCommand(dg.RefreshAction);
            }
            else
            {
                dg.RefreshCommand = null;
            }
        }

        /// <summary>
        /// 刷新动作
        /// </summary>
        public Action RefreshAction
        {
            get
            {
                return (Action)GetValue(RefreshActionProperty);
            }
            set
            {
                SetValue(RefreshActionProperty, value);
            }
        }

        #endregion

        #region LayoutName 列信息保存文件名称

        public static readonly DependencyProperty LayoutNameProperty = DependencyProperty.Register("LayoutName", typeof(string), typeof(ExtendedDataGrid),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColumnInfoSaveNameChanged));

        /// <summary>
        /// 列信息保存文件名称,为空不保存
        /// </summary>
        public string LayoutName
        {
            get
            {
                return (string)GetValue(LayoutNameProperty);
            }
            set
            {
                SetValue(LayoutNameProperty, value);
            }
        }
        #endregion

        #region SaveUserLayout 是否保存布局

        /// <summary>
        /// 是否保存布局
        /// </summary>
        public bool SaveUserLayout
        {
            get
            {
                return (bool)GetValue(SaveUserLayoutProperty);
            }
            set
            {
                SetValue(SaveUserLayoutProperty, value);
            }
        }
        /// <summary>
        /// 是否保存布局依赖项属性
        /// </summary>
        public static readonly DependencyProperty SaveUserLayoutProperty =
            DependencyProperty.Register("SaveUserLayout", typeof(bool), typeof(ExtendedDataGrid), new PropertyMetadata(true));

        #endregion

        /// <summary>
        /// 刷新、自动刷新菜单是否启用（目前刷新直接隐藏了）
        /// </summary>
        public bool IsShowRefreshMenu
        {
            get; set;
        }

        /// <summary>
        /// 转存、转存所有菜单是否启用
        /// </summary>
        public bool IsShowSaveMenu
        {
            get; set;
        }

        /// <summary>
        /// 配置列、隐藏列菜单是否启用
        /// </summary>
        public bool IsShowColumnConfigMenu
        {
            get; set;
        }

        /// <summary>
        /// 汇总列是否启用
        /// </summary>
        public bool IsShowTotalRow
        {
            get; set;
        }


        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return LayoutName + "概览设置(去掉勾选后列表将不展示此项)";
            }
        }

        #region Commands

        /// <summary>
        /// 确定命令
        /// </summary>
        public ICommand ConfirmCommand
        {
            get
            {
                return (ICommand)GetValue(ConfirmCommandProperty);
            }
            private set
            {
                SetValue(ConfirmCommandProperty, value);
            }
        }
        /// <summary>
        /// 确定命令依赖项属性
        /// </summary>
        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register("ConfirmCommand", typeof(ICommand), typeof(ExtendedDataGrid), new PropertyMetadata(null));

        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                return (ICommand)GetValue(CancelCommandProperty);
            }
            private set
            {
                SetValue(CancelCommandProperty, value);
            }
        }
        /// <summary>
        /// 取消命令依赖项属性
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(ExtendedDataGrid), new PropertyMetadata(null));

        #endregion

        #endregion

        #region 私有属性及域
        private string RealSavePath
        {
            get
            {
                try
                {
                    return Path.Combine(_ColumnInfoPathRoot, LayoutName + ".xml");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 列信息
        /// </summary>
        private DataGridInformation DGInfo = new DataGridInformation();
        /// <summary>
        /// 最外层滚动控件
        /// </summary>
        private ScrollViewer _scrollViewer;
        /// <summary>
        /// 表头控件
        /// </summary>
        private DataGridColumnHeadersPresenter _columnHeader;
        /// <summary>
        /// 刷新按钮
        /// </summary>
        private MenuItem _refreshMenuItem;
        /// <summary>
        /// 保存当前
        /// </summary>
        private MenuItem _saveCurrentMenuItem;
        /// <summary>
        /// 保存所有
        /// </summary>
        private MenuItem _saveAllMenuItem;
        /// <summary>
        /// 显示隐藏列信息
        /// </summary>
        private MenuItem _showHiddenColumnsMenuItem;
        /// <summary>
        /// 隐藏当前列
        /// </summary>
        private MenuItem _hideCurrentColumnMenuItem;
        /// <summary>
        /// 隐藏列信息窗口
        /// </summary>
        private Popup _hiddenColumnHeaders;
        /// <summary>
        /// 保存列信息的目录地址
        /// </summary>
        private string _ColumnInfoPathRoot;
        /// <summary>
        /// 当前右键点击的表头
        /// </summary>
        private DataGridColumnHeader _currentHeader;
        /// <summary>
        /// 汇总列
        /// </summary>
        private DataGrid _tolalRow;

        /// <summary>
        /// 配置列中列表
        /// </summary>
        private DragDropListBox _listView;

        private EventHandler _widthPropertyChangedHandler;
        private DependencyPropertyDescriptor _sortDirectionPropertyDescriptor;
        private DependencyPropertyDescriptor _widthPropertyDescriptor;

        private bool _isDisposed;
        /// <summary>
        /// 不参与配置的列的索引的集合
        /// key:index value:displayIndex
        /// </summary>
        private Dictionary<int, int> _noConfigColumnIndexList = new Dictionary<int, int>();
        /// <summary>
        /// 列表默认的排序
        /// </summary>
        private List<SortDescription> defaultSortDescriptions;
        /// <summary>
        /// 各列上一个排序状态，为了可以恢复以前的排序
        /// </summary>
        Dictionary<DataGridColumn, ListSortDirection?> lastSortDic = new Dictionary<DataGridColumn, ListSortDirection?>();

        /// <summary>
        /// 冻结列
        /// </summary>
        private List<string> _frozenColumns;

        private List<object> totalRowItemSource;
        #endregion

        #region ctor
        static ExtendedDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedDataGrid),
                new FrameworkPropertyMetadata(typeof(ExtendedDataGrid)));
        }

        public ExtendedDataGrid()
        {
            Unloaded += ExtendedDataGrid_Unloaded;
            LoadingRow += ExtendedDataGrid_LoadingRow;
        }

        private void ExtendedDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void ExtendedDataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
        #endregion

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (_hiddenColumnHeaders == null || _hiddenColumnHeaders.IsOpen)
            {
                return;
            }
            base.OnMouseDoubleClick(e);
        }

        private bool _isRendered = false;
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (!_isRendered)
            {
                _isRendered = true;
                GetSaveDirectory();
                _frozenColumns = new List<string>();
                for (var i = 0; i < this.FrozenColumnCount; i++)
                {
                    _frozenColumns.Add(this.Columns[i].GetID());
                }
                LoadConfig();
                OnComponentLoaded();
                InitializeComponent();
            }

            _isRendered = true;
        }


        private void OnUnloaded()
        {
            foreach (var column in Columns)
            {
                if (_sortDirectionPropertyDescriptor != null)
                    _sortDirectionPropertyDescriptor.RemoveValueChanged(column, ColumnResort);
                if (_widthPropertyDescriptor != null && _widthPropertyChangedHandler != null)
                    _widthPropertyDescriptor.RemoveValueChanged(column, _widthPropertyChangedHandler);
            }
        }

        private void OnComponentLoaded()
        {
            _scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
            _columnHeader = VisualHelper.FindChild<DataGridColumnHeadersPresenter>(_scrollViewer, PART_ColumnHeadersPresenter);
            if (_columnHeader != null && _columnHeader.ContextMenu != null)
            {
                ContextMenu cm = _columnHeader.ContextMenu;
                _refreshMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_RefreshMenuItem) as MenuItem;
                _saveCurrentMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_SaveCurrentMenuItem) as MenuItem;
                _saveAllMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_SaveAllMenuItem) as MenuItem;
                _showHiddenColumnsMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_ShowHiddenColumnsMenuItem) as MenuItem;
                _hideCurrentColumnMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_HideCurrentColumnMenuItem) as MenuItem;
                _columnHeader.PreviewMouseRightButtonUp += _columnHeader_PreviewMouseRightButtonUp;
            }
            _hiddenColumnHeaders = VisualHelper.FindChild<Popup>(_scrollViewer, PART_HiddenColumnHeadersPopUp);
            if (_hiddenColumnHeaders != null)
            {
                _listView = LogicalTreeHelper.FindLogicalNode(_hiddenColumnHeaders, PART_ListBox) as DragDropListBox;
                //计算Popup的高度
                //var height = 16 * this.Columns.Where(c => c.Header != null && c.Header is string).Count() + 40 + 10 + 5;
                //_hiddenColumnHeaders.Height = height > 405 ? 405 : height;
            }
            _widthPropertyChangedHandler = (sender, x) => UpdateColumnInfo();
            _sortDirectionPropertyDescriptor =
                DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
            _widthPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

            foreach (var column in Columns)
            {
                _sortDirectionPropertyDescriptor.AddValueChanged(column, ColumnResort);
                _widthPropertyDescriptor.AddValueChanged(column, _widthPropertyChangedHandler);
            }


            if (IsShowTotalRow)
            {
                _tolalRow = GetTemplateChild(PART_TotalRow) as DataGrid;
                _tolalRow.LoadingRow += _tolalRow_LoadingRow;
                SetTotalRow();
            }
        }

        private void _tolalRow_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = "合计";
        }

        private void SetTotalRow()
        {
            if (_tolalRow != null)
            {
                _tolalRow.Visibility = Visibility.Visible;

                if (_tolalRow.Columns != null)
                    _tolalRow.Columns.Clear();

                foreach (var item in this.Columns.OrderBy(p => p.DisplayIndex))
                {

                    DataGridTextColumn cl = new DataGridTextColumn();
                    cl.Header = item.Header;
                    cl.Width = item.Width;
                    cl.DisplayIndex = item.DisplayIndex;

                    Binding widthBd = new Binding();
                    widthBd.Source = item;
                    widthBd.Mode = BindingMode.OneWay;
                    widthBd.Path = new PropertyPath(DataGridColumn.WidthProperty);
                    BindingOperations.SetBinding(cl, DataGridTextColumn.WidthProperty, widthBd);

                    Binding visibleBd = new Binding();
                    visibleBd.Source = item;
                    visibleBd.Mode = BindingMode.OneWay;
                    visibleBd.Path = new PropertyPath(DataGridColumn.VisibilityProperty);
                    BindingOperations.SetBinding(cl, DataGridTextColumn.VisibilityProperty, visibleBd);

                    //每次重排了，再次生成好了。
                    //Binding indexBd = new Binding();
                    //indexBd.Source = item;
                    //indexBd.Mode = BindingMode.OneWay;
                    //indexBd.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                    //BindingOperations.SetBinding(cl, DataGridTextColumn.DisplayIndexProperty, indexBd);

                    if (item is DataGridTextColumn)
                    {
                        cl.Binding = (item as DataGridTextColumn).Binding;
                    }
                    else if (item is DataGridBoundColumn)
                    {
                        cl.Binding = (item as DataGridBoundColumn).Binding;
                    }

                    _tolalRow.Columns.Add(cl);
                }
                OnItemsSourceChanged(null, Items);
            }
        }

        private void ColumnResort(object s, EventArgs e)
        {
            var column = s as DataGridColumn;
            if (column == null) return;
            if (lastSortDic.ContainsKey(column))
            {
                var dir = lastSortDic[column];
                if (dir == ListSortDirection.Descending)
                {
                    column.SortDirection = null;
                    Items.SortDescriptions.Clear();
                    if (defaultSortDescriptions != null)
                    {
                        defaultSortDescriptions.ForEach(o => Items.SortDescriptions.Add(o));
                    }
                    Items.Refresh();
                }
            }
            if (!lastSortDic.ContainsKey(column))
            {
                lastSortDic.Add(column, null);
            }
            lastSortDic[column] = column.SortDirection;

            UpdateColumnInfo();
        }

        private void InitializeComponent()
        {
            if (IsShowRefreshMenu)
            {
                if (_refreshMenuItem != null)
                {
                    _refreshMenuItem.Command = RefreshCommand;
                    _refreshMenuItem.Visibility = Visibility.Visible;
                }
            }

            if (IsShowSaveMenu)
            {
                if (_saveAllMenuItem != null)
                {
                    _saveAllMenuItem.Command = SaveAllCommand;
                    _saveAllMenuItem.Visibility = Visibility.Visible;
                }

                if (_saveCurrentMenuItem != null)
                {
                    _saveCurrentMenuItem.Command = SaveCurrentCommand;
                    _saveCurrentMenuItem.Visibility = Visibility.Visible;
                }
            }

            if (IsShowColumnConfigMenu)
            {
                if (_showHiddenColumnsMenuItem != null)
                {
                    _showHiddenColumnsMenuItem.Click -= ShowHiddenPopup;
                    _showHiddenColumnsMenuItem.Click += ShowHiddenPopup;
                    _showHiddenColumnsMenuItem.Visibility = Visibility.Visible;
                }

                if (_hideCurrentColumnMenuItem != null)
                {
                    _hideCurrentColumnMenuItem.PreviewMouseLeftButtonUp -= HideColumnHeader;
                    _hideCurrentColumnMenuItem.PreviewMouseLeftButtonUp += HideColumnHeader;
                    _hideCurrentColumnMenuItem.Visibility = Visibility.Visible;
                }

                RegisterConfigCommand();
            }




        }


        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (_tolalRow != null)
            {
                Type itemType = null;
                totalRowItemSource = new List<object>();
                object obj = null;
                if (newValue == null)
                {
                    return;
                }

                foreach (var item in newValue)
                {

                    itemType = item.GetType();
                    obj = Activator.CreateInstance(itemType, true);
                    break;
                }
                if (itemType == null)
                    return;

                PropertyInfo[] ps = itemType.GetProperties();
                foreach (var item in newValue)
                {

                    foreach (PropertyInfo property in ps)
                    {
                        object tmpValue = property.GetValue(item, null);
                        object totalValue = property.GetValue(obj, null);

                        if (property.CanRead && property.CanWrite)
                        {
                            if (property.PropertyType == typeof(int))
                            {
                                totalValue = (int)tmpValue + (int)totalValue;
                                property.SetValue(obj, totalValue, null);
                            }
                            else if (property.PropertyType == typeof(double))
                            {
                                totalValue = (double)tmpValue + (double)totalValue;
                                property.SetValue(obj, totalValue, null);
                            }
                        }
                    }
                }
                totalRowItemSource.Add(obj);

                _tolalRow.ItemsSource = totalRowItemSource;
            }
        }

        private void GetSaveDirectory()
        {
            //这行代码是为了控制在设计阶段和使用多线程进行加载Xaml优化时，不执行任何实质性的代码
            if (DesignerHelper.IsInDesignMode || !DesignerHelper.IsInMainThread)
                return;

            _ColumnInfoPathRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_DataGridInfos);
            CheckRootDirectory();
        }

        private void CheckRootDirectory()
        {
            if (!Directory.Exists(_ColumnInfoPathRoot))
            {
                Directory.CreateDirectory(_ColumnInfoPathRoot);
            }
        }

        private void LoadConfig()
        {
            if (!string.IsNullOrEmpty(LayoutName) && File.Exists(RealSavePath))
            {
                string xmlStr = File.ReadAllText(RealSavePath);
                SetColumnInformation(xmlStr);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        #region 隐藏及显示列
        /// <summary>
        /// 列信息保存名称变化
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ColumnInfoSaveNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedDataGrid grid = d as ExtendedDataGrid;
            if (grid == null || !grid.IsLoaded)
                return;

            if (!string.IsNullOrEmpty(grid.LayoutName) && File.Exists(grid.RealSavePath))
            {
                string xmlStr = File.ReadAllText(grid.RealSavePath);
                grid.SetColumnInformation(xmlStr);
            }
        }

        /// <summary>
        /// 双击隐藏列列表项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = Mouse.DirectlyOver as TextBlock;
            if (tb != null)
            {
                DataGridColumn col = tb.DataContext as DataGridColumn;
                if (col != null)
                {
                    col.Visibility = Visibility.Visible;
                    SaveColumnInfo();
                }
            }
        }

        /// <summary>
        /// 右键点击表头事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _columnHeader_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _currentHeader = VisualHelper.TryFindParent<DataGridColumnHeader>(Mouse.DirectlyOver as UIElement);
        }

        protected override void OnColumnReordered(DataGridColumnEventArgs e)
        {
            base.OnColumnReordered(e);
            SaveColumnInfo();
            SetTotalRow();
        }

        /// <summary>
        /// 隐藏当前列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideColumnHeader(object sender, MouseButtonEventArgs e)
        {
            if (_currentHeader == null || string.IsNullOrWhiteSpace(_currentHeader.Column.GetID()))
                return;
            if (!_currentHeader.Column.IsConfig() || _frozenColumns.Contains(_currentHeader.Column.GetID()))
            {
                MessageBox.Show("该列不允许隐藏！");
                return;
            }
            if (Columns.Where(c => c.Visibility == Visibility.Visible && c.IsConfig()).Count() == 1)
            {
                MessageBox.Show("只剩最后一个,不能再隐藏了！");
                return;
            }
            if (_currentHeader.Column.Visibility == Visibility.Visible)
            {
                _currentHeader.Column.Visibility = Visibility.Hidden;
            }
            else
            {
                _currentHeader.Column.Visibility = Visibility.Visible;
            }
            SaveColumnInfo();
        }

        /// <summary>
        /// 获取当前列信息的序列化数据
        /// </summary>
        /// <returns></returns>
        public string GetColumnInformation()
        {
            UpdateColumnInfo(false);
            return SerializeObjectToXML(DGInfo);
        }

        /// <summary>
        /// 从序列化数据恢复当前列信息
        /// </summary>
        /// <param name="xmlString"></param>
        public void SetColumnInformation(string xmlString)
        {
            try
            {
                DGInfo = DeserializeFromXml<DataGridInformation>(xmlString);
                InvalidateColumnInfo();
                if (this.ItemsSource != null)
                {
                    var cvs = CollectionViewSource.GetDefaultView(this.ItemsSource);
                    cvs.Refresh();//刷新CollectionViewSource的排序、筛选、分组等
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 更新列信息
        /// </summary>
        private void UpdateColumnInfo(bool isSave = true)
        {
            //唯一标志为null不参与保存
            DGInfo.ColumnInfos = new ObservableCollection<ColumnInformation>(Columns.Where(c => !string.IsNullOrWhiteSpace(c.GetID())).Select(x => new ColumnInformation(x)));
            //UpdateLayout();
            if (isSave)
                SaveColumnInfo();
        }

        /// <summary>
        /// 保存列表信息
        /// </summary>
        public void SaveColumnInfo(string layoutName)
        {
            if (this.LayoutName != layoutName)
            {
                this.LayoutName = layoutName;
            }

            if (!string.IsNullOrEmpty(RealSavePath) && !File.Exists(RealSavePath))
            {
                SaveColumnInfo();
            }

        }

        /// <summary>
        /// 保存列信息
        /// </summary>
        private void SaveColumnInfo()
        {
            //这行代码是为了控制在设计阶段和使用多线程进行加载Xaml优化时，不执行任何实质性的代码
            if (DesignerHelper.IsInDesignMode || !DesignerHelper.IsInMainThread) return;

            if (!string.IsNullOrEmpty(LayoutName) && SaveUserLayout)
            {
                string columnInfo = GetColumnInformation();
                CheckRootDirectory();
                try
                {
                    File.WriteAllText(RealSavePath, columnInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 根据列信息重新设置列属性
        /// </summary>
        private void InvalidateColumnInfo()
        {
            defaultSortDescriptions = Items.SortDescriptions.ToList();
            //用dictionary是因为后续的操作会导致displayIndex发生改变
            var newColumns = new Dictionary<int, DataGridColumn>();
            foreach (var column in Columns)
            {
                //保存列的集合中不包含该列，则认为是新添加的列
                if (string.IsNullOrWhiteSpace(column.GetID())
                    || DGInfo.ColumnInfos.FirstOrDefault(x => column.SortMemberPath.Equals(x.SortMemberPath) && column.GetID().Equals(x.Header)).Equals(default(ColumnInformation)))
                {
                    newColumns.Add(column.DisplayIndex, column);
                }
            }
            foreach (var column in DGInfo.ColumnInfos.OrderBy(x => x.DisplayIndex))
            {
                var realColumn = Columns.FirstOrDefault(x => column.SortMemberPath.Equals(x.SortMemberPath)
                && column.Header.Equals(x.GetID()));
                if (realColumn == null)
                {
                    continue;
                }
                column.Apply(realColumn, Columns.Count);
            }
            //最后将新列的index重新赋值
            foreach (var column in newColumns)
            {
                var index = column.Key;
                column.Value.DisplayIndex = Columns.Count - 1;
                column.Value.DisplayIndex = index;
            }
        }

        /// <summary>
        /// 显示隐藏列列表弹窗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHiddenPopup(object sender, RoutedEventArgs e)
        {
            _hiddenColumnHeaders.IsOpen = true;
            OnConfigColumn();
        }

        private void OnConfigColumn()
        {
            _hiddenColumnHeaders.IsOpen = false;
            _hiddenColumnHeaders.IsOpen = true;
            _noConfigColumnIndexList.Clear();
            var temp = new List<DataGridColumn>(Columns);
            temp.Sort(new DataGridColumnComparer());
            var index = -1;
            var showList = new List<DragDropListViewData>();
            foreach (var item in temp)
            {
                index++;
                if (!item.IsConfig())
                {
                    _noConfigColumnIndexList.Add(index, item.DisplayIndex);
                    continue;
                }
                showList.Add(new DragDropListViewData()
                {
                    IsEnabled = !_frozenColumns.Contains(item.GetID()),
                    IsChecked = item.Visibility == Visibility.Visible,
                    DisplayName = item.GetID(),
                    DisplayIndex = item.DisplayIndex,
                });
            }
            _listView.SetOrignalDataSource(showList);
        }

        private void OnConfirm()
        {
            var index = 0;
            var result = _listView.GetResult();
            if (!result.Any(x => x.IsChecked))
            {
                _hiddenColumnHeaders.StaysOpen = true;
                MessageBox.Show("至少保留一个可显示的列！");
                _hiddenColumnHeaders.StaysOpen = false;
                return;
            }
            foreach (var item in result)
            {
                var column = Columns.FirstOrDefault(c => c.GetID() == item.DisplayName);
                if (column != null)
                {
                    if (item.IsChecked)
                    {
                        column.Visibility = Visibility.Visible;
                        column.DisplayIndex = item.DisplayIndex;
                    }
                    else
                    {
                        column.Visibility = Visibility.Collapsed;
                    }
                }
                index++;
            }
            foreach (var item in _noConfigColumnIndexList)
            {
                Columns[item.Key].DisplayIndex = item.Value;
            }
            _hiddenColumnHeaders.IsOpen = false;
            UpdateColumnInfo();
            SetTotalRow();
        }

        private void OnCancel()
        {
            _hiddenColumnHeaders.IsOpen = false;
        }
        #endregion

        #region 注册命令
        /// <summary>
        /// 注册命令
        /// </summary>
        private void RegisterConfigCommand()
        {
            ConfirmCommand = new RelayCommand(OnConfirm);
            CancelCommand = new RelayCommand(OnCancel);
        }

        #endregion

        #region 保存（导出）

        #region ExportCommand 导出
        /// <summary>
        /// 导出
        /// </summary>
        public ICommand SaveAllCommand
        {
            get
            {
                return new RelayCommand(() => {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "请选择您要导出文件的位置";
                    sd.DefaultExt = "csv";
                    sd.AddExtension = true;
                    sd.Filter = "CSV(*.csv)文件|*.csv";

                    if (sd.ShowDialog() == true)
                    {
                        //数据导出到Excel中              
                        ExportToSvg(sd.SafeFileName, sd.FileName);
                    }
                });
            }
        }
        #endregion

        #region SaveCurrentCommand 导出当前
        /// <summary>
        /// 导出当前
        /// </summary>
        public ICommand SaveCurrentCommand
        {
            get
            {
                return new RelayCommand(() => {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "请选择您要导出文件的位置";
                    sd.DefaultExt = "csv";
                    sd.AddExtension = true;
                    sd.Filter = "CSV(*.csv)文件|*.csv";

                    if (sd.ShowDialog() == true)
                    {
                        //数据导出到Excel中              
                        ExportToSvg(sd.SafeFileName, sd.FileName, false);
                    }
                });
            }
        }
        #endregion

        /// <summary>
        /// 导出为Excel文件
        /// </summary>
        /// <param name="workSheetName">工资表名称</param>
        /// <param name="fullFilePath">转存文件路径</param>
        /// <param name="exportAll">是否导出全部数据，否则仅导出选择项</param>
        public void ExportToSvg(string workSheetName, string fullFilePath, bool exportAll = true)
        {
            bool originalCanSelectMultipleItems = CanSelectMultipleItems;
            try
            {
                //Select all rows 
                Focus();
                if (exportAll)
                {
                    CanSelectMultipleItems = true;
                    SelectAllCells();
                }

                // Supported default formats: Html, Text, UnicodeText and CSV
                Collection<string> formats = new Collection<string>(new string[] { DataFormats.Html, DataFormats.Text, DataFormats.UnicodeText, DataFormats.CommaSeparatedValue });
                Dictionary<string, StringBuilder> dataGridStringBuilders = new Dictionary<string, StringBuilder>(formats.Count);
                foreach (string format in formats)
                {
                    dataGridStringBuilders[format] = new StringBuilder();
                }

                int minRowIndex = 0;
                int maxRowIndex = Items.Count - 1;
                int minColumnDisplayIndex = 0;
                int maxColumnDisplayIndex = Columns.Count(x => x.Visibility == Visibility.Visible) - 1;

                // Add column headers if enabled
                DataGridRowClipboardEventArgs preparingRowClipboardContentEventArgs = new DataGridRowClipboardEventArgs(null, minColumnDisplayIndex, maxColumnDisplayIndex, true /*IsColumnHeadersRow*/);
                OnCopyingRowClipboardContent(preparingRowClipboardContentEventArgs);

                foreach (string format in formats)
                {
                    dataGridStringBuilders[format].Append(preparingRowClipboardContentEventArgs.FormatClipboardCellValues(format));
                }

                // Add each selected row
                for (int i = minRowIndex; i <= maxRowIndex; i++)
                {
                    object row = Items[i];

                    preparingRowClipboardContentEventArgs = new DataGridRowClipboardEventArgs(row, minColumnDisplayIndex, maxColumnDisplayIndex, false /*IsColumnHeadersRow*/);
                    OnCopyingRowClipboardContent(preparingRowClipboardContentEventArgs);

                    foreach (string format in formats)
                    {
                        dataGridStringBuilders[format].Append(preparingRowClipboardContentEventArgs.FormatClipboardCellValues(format));
                    }
                }
                string data = dataGridStringBuilders[DataFormats.CommaSeparatedValue].ToString();

                if (!string.IsNullOrEmpty(data))
                {
                    System.IO.StreamWriter stream = new System.IO.StreamWriter(fullFilePath);
                    stream.WriteLine(data);
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();

                    MessageBox.Show("数据导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("没有可导出的数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            finally
            {
                if (exportAll)
                {
                    UnselectAllCells();
                    CanSelectMultipleItems = originalCanSelectMultipleItems;
                }
            }

        }

        #endregion

        #region 序列化
        /// <summary>
        /// 序列化为xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        private string SerializeObjectToXML<T>(T item)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                xs.Serialize(stringWriter, item);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 从xml反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        private T DeserializeFromXml<T>(string xml)
        {
            T result;
            var ser = new XmlSerializer(typeof(T));
            using (TextReader tr = new StringReader(xml))
            {
                result = (T)ser.Deserialize(tr);
            }
            return result;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                OnUnloaded();
                _isDisposed = true;
            }
        }

        #endregion
    }


    public struct ColumnInformation
    {
        public ColumnInformation(DataGridColumn column)
        {
            Header = column.GetID();
            if (!(column is DataGridTemplateColumn))
            {
                try
                {
                    if (column is DataGridComboBoxColumn)
                        PropertyPath = column.SortMemberPath;
                    else
                        PropertyPath = ((Binding)((DataGridBoundColumn)column).Binding).Path.Path;
                }
                catch
                {
                    PropertyPath = string.Empty;
                }
            }
            else
            {
                PropertyPath = column.SortMemberPath;
            }
            WidthValue = column.Width.DisplayValue;
            WidthType = column.Width.UnitType;
            SortDirection = column.SortDirection;
            DisplayIndex = column.DisplayIndex;
            SortMemberPath = column.SortMemberPath;
            IsVisible = column.Visibility == Visibility.Visible;
        }
        public void Apply(DataGridColumn column, int gridColumnCount)
        {
            column.Width = new DataGridLength(WidthValue, WidthType, WidthValue, WidthValue);
            column.SortDirection = SortDirection;
            if (column.DisplayIndex != DisplayIndex)
            {
                var maxIndex = (gridColumnCount == 0) ? 0 : gridColumnCount - 1;
                column.DisplayIndex = (DisplayIndex <= maxIndex) ? DisplayIndex : maxIndex;
            }
            //if (!column.VisibilityIsBinding())
            {
                column.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            column.SortMemberPath = SortMemberPath;
        }
        public string Header;
        public string PropertyPath;
        public ListSortDirection? SortDirection;
        public int DisplayIndex;
        public double WidthValue;
        public DataGridLengthUnitType WidthType;
        public string SortMemberPath;
        public bool IsVisible;
    }

    public class DataGridInformation
    {
        public int RefreshInterval;
        public ObservableCollection<ColumnInformation> ColumnInfos;
    }

    internal class DataGridColumnComparer : IComparer<DataGridColumn>
    {
        public int Compare(DataGridColumn x, DataGridColumn y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            if (x.DisplayIndex < y.DisplayIndex)
            {
                return -1;
            }
            else if (x.DisplayIndex < y.DisplayIndex)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    internal static class DataGridColumnExtend
    {
        /// <summary>
        /// 判断该类是否参与配置
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsConfig(this DataGridColumn column)
        {
            if (string.IsNullOrWhiteSpace(column.GetID()))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取列的唯一标志
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetID(this DataGridColumn column)
        {
            if (column.Header is string && !string.IsNullOrWhiteSpace(column.Header.ToString()))
            {
                return column.Header.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 判断该列的Visibility属性是否被绑定过
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool VisibilityIsBinding(this DataGridColumn column)
        {
            var expression = BindingOperations.GetBindingExpression(column, DataGridColumn.VisibilityProperty);
            return expression != null;
        }
    }


}
