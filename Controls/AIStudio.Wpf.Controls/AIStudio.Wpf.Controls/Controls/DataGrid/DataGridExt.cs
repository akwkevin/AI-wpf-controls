using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
    public class DataGridExt : DataGrid
    {

        #region 常量

        private static readonly double DefaultBlankRowHeight = 32d;
        private static readonly Brush DefaultBlankRowLineBrush = Brushes.Gray;
        private static readonly double DefaultBlankRowLineWidth = 0.1;

        #endregion

        #region 事件
        public event ItemsChangedEventHandler ItemsChanged = null;
        #endregion

        public DataGridExt()
        {
            this.Loaded += this.DataGridExt_Loaded;
            this.Unloaded += DataGridExt_Unloaded;
            this.ColumnHeaderDragCompleted += DataGridExt_ColumnHeaderDragCompleted;
        }

        #region 私有变量
        private double HorizontalScrollOffset = 0;

        private List<DataGridRow> dataGridRows = new List<DataGridRow>();

        #endregion

        #region 属性
        #region BlankRowHeight

        public double BlankRowHeight
        {
            get { return (double)GetValue(BlankRowHeightProperty); }
            set { SetValue(BlankRowHeightProperty, value); }
        }

        public static readonly DependencyProperty BlankRowHeightProperty =
            DependencyProperty.Register("BlankRowHeight", typeof(double), typeof(DataGridExt), new PropertyMetadata(DefaultBlankRowHeight, BlankRowHeightCallBack));

        private static void BlankRowHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual(d);
        }

        #endregion

        #region Columnheaderheight
        /// <summary>
        /// 列头高度
        /// </summary>
        public double Columnheaderheight
        {
            get { return (double)GetValue(ColumnheaderheightProperty); }
            set { SetValue(ColumnheaderheightProperty, value); }
        }

        public static readonly DependencyProperty ColumnheaderheightProperty =
            DependencyProperty.Register("Columnheaderheight", typeof(double), typeof(DataGridExt), new PropertyMetadata(DefaultBlankRowHeight, ColumnHeaderHeightCallBack));

        private static void ColumnHeaderHeightCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uielement = d as DataGridExt;
            if (uielement != null)
                uielement.ColumnHeaderHeight = uielement.Columnheaderheight;

        }
        #endregion

        #region BlankRowLineBrush
        public Brush BlankRowLineBrush
        {
            get { return (Brush)GetValue(BlankRowLineBrushProperty); }
            set { SetValue(BlankRowLineBrushProperty, value); }
        }

        public static readonly DependencyProperty BlankRowLineBrushProperty =
            DependencyProperty.Register("BlankRowLineBrush", typeof(Brush), typeof(DataGridExt), new PropertyMetadata(DefaultBlankRowLineBrush, BlankRowLineBrushCallBack));

        private static void BlankRowLineBrushCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual(d);
        }

        #endregion

        #region IsMulticolumn
        /// <summary>
        /// 是否为多列头
        /// </summary>
        public bool IsMulticolumn
        {
            get { return (bool)GetValue(IsMulticolumnProperty); }
            set { SetValue(IsMulticolumnProperty, value); }
        }

        public static readonly DependencyProperty IsMulticolumnProperty =
            DependencyProperty.Register("IsMulticolumn", typeof(bool), typeof(DataGridExt), new PropertyMetadata(false));
        #endregion

        #region BlankRowLineWidth

        public double BlankRowLineWidth
        {
            get { return (double)GetValue(BalnkRowLineWidthProperty); }
            set { SetValue(BalnkRowLineWidthProperty, value); }
        }

        public static readonly DependencyProperty BalnkRowLineWidthProperty =
            DependencyProperty.Register("BlankRowLineWidth", typeof(double), typeof(DataGridExt), new PropertyMetadata(DefaultBlankRowLineWidth, BalnkRowLineWidthCallBack));

        private static void BalnkRowLineWidthCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual(d);

        }

        #endregion

        #region AutoFillWithBlankRow
        public bool AutoFillWithBlankRow
        {
            get { return (bool)GetValue(AutoFillWithBlankRowProperty); }
            set { SetValue(AutoFillWithBlankRowProperty, value); }
        }

        public static readonly DependencyProperty AutoFillWithBlankRowProperty =
            DependencyProperty.Register("AutoFillWithBlankRow", typeof(bool), typeof(DataGridExt), new PropertyMetadata(false, AutoFillWithBlankRowCallBack));

        private static void AutoFillWithBlankRowCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual(d);
        }
        #endregion

        #region CurrentGridData
        private static readonly List<object> DefaultCurrentGridData = new List<object>();
        public List<object> CurrentGridData
        {
            get { return (List<object>)GetValue(CurrentGridProperty); }
            set { SetValue(CurrentGridProperty, value); }
        }

        public static readonly DependencyProperty CurrentGridProperty =
            DependencyProperty.Register("CurrentGridData", typeof(List<object>), typeof(DataGridExt), new PropertyMetadata(DefaultCurrentGridData));
        #endregion
        #endregion

        #region 右键菜单

        #region 常量
        private const string DG_ScrollViewer = "DG_ScrollViewer";
        private const string PART_ColumnHeadersPresenter = "PART_ColumnHeadersPresenter";
        private const string PART_RefreshMenuItem = "PART_RefreshMenuItem";
        private const string PART_AutoRefreshMenuItem = "PART_AutoRefreshMenuItem";
        private const string PART_SaveCurrentMenuItem = "PART_SaveCurrentMenuItem";
        private const string PART_SaveAllMenuItem = "PART_SaveAllMenuItem";
        private const string PART_ShowHiddenColumnsMenuItem = "PART_ShowHiddenColumnsMenuItem";
        private const string PART_HiddenColumnHeadersPopUp = "PART_HiddenColumnHeadersPopUp";
        private const string PART_AutoRefreshIntervalCombo = "PART_AutoRefreshIntervalCombo";
        private const string PART_HideCurrentColumnMenuItem = "PART_HideCurrentColumnMenuItem";
        private const string PART_ListView = "PART_ListView";
        private const string PATH_DataGridInfos = "DataGridInfos";
        #endregion

        #region Commands

        /// <summary>
        /// 确定命令
        /// </summary>
        public ICommand ConfirmCommand
        {
            get { return (ICommand)GetValue(ConfirmCommandProperty); }
            private set { SetValue(ConfirmCommandProperty, value); }
        }
        /// <summary>
        /// 确定命令依赖项属性
        /// </summary>
        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register("ConfirmCommand", typeof(ICommand), typeof(DataGridExt), new PropertyMetadata(null));

        /// <summary>
        /// 取消命令
        /// </summary>
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            private set { SetValue(CancelCommandProperty, value); }
        }
        /// <summary>
        /// 取消命令依赖项属性
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(DataGridExt), new PropertyMetadata(null));

        #endregion

        #region ColumnInfoSaveName 列信息保存文件名称

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

        public static readonly DependencyProperty LayoutNameProperty = DependencyProperty.Register("LayoutName", typeof(string), typeof(DataGridExt),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ColumnInfoSaveNameChanged));

        /// <summary>
        /// 列信息保存名称变化
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ColumnInfoSaveNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridExt grid = d as DataGridExt;
            if (grid == null || !grid.IsLoaded)
                return;

            if (!string.IsNullOrEmpty(grid.LayoutName) && File.Exists(grid.RealSavePath))
            {
                string xmlStr = File.ReadAllText(grid.RealSavePath);
                grid.SetColumnInformation(xmlStr);
            }
        }

        #endregion

        #region SaveUserLayout 是否保存布局

        /// <summary>
        /// 是否保存布局
        /// </summary>
        public bool SaveUserLayout
        {
            get { return (bool)GetValue(SaveUserLayoutProperty); }
            set { SetValue(SaveUserLayoutProperty, value); }
        }
        /// <summary>
        /// 是否保存布局依赖项属性
        /// </summary>
        public static readonly DependencyProperty SaveUserLayoutProperty =
            DependencyProperty.Register("SaveUserLayout", typeof(bool), typeof(DataGridExt), new PropertyMetadata(true));

        #endregion

        #region RefreshInterval 刷新间隔(秒)

        public static readonly DependencyProperty RefreshIntervalProperty = DependencyProperty.Register("RefreshInterval", typeof(int), typeof(DataGridExt),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RefreshIntervalChanged));


        /// <summary>
        /// 刷新间隔(秒)
        /// </summary>
        public int RefreshInterval
        {
            get
            {
                return (int)GetValue(RefreshIntervalProperty);
            }
            set
            {
                SetValue(RefreshIntervalProperty, value);
            }
        }

        /// <summary>
        /// 刷新间隔变化处理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void RefreshIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridExt grid = d as DataGridExt;
            if (grid == null)
                return;

            grid.DGInfo.RefreshInterval = grid.RefreshInterval;

            if (!grid.IsLoaded)
                return;

            grid.SaveColumnInfo();

            if ((int)e.OldValue == 0 && (int)e.NewValue > 0)
            {
                grid.refreshProcess(null);
            }
            else
            {
                grid.UpdateTimerInterval();
            }
        }

        /// <summary>
        /// 自动刷新处理过程
        /// </summary>
        /// <param name="state"></param>
        private void refreshProcess(object state)
        {
            if (IsVisible)
            {
                if (_refreshCommand != null && _refreshCommand.CanExecute(null))
                {
                    _refreshCommand.Execute(null);
                }
            }

            this.Dispatcher.Invoke(new Action(() =>
            {
                UpdateTimerInterval();
            }));
        }

        /// <summary>
        /// 更新自动刷新计时器的时间间隔
        /// </summary>
        private void UpdateTimerInterval()
        {
            if (refreshTimer == null)
                return;

            if (RefreshInterval == 0)
            {
                refreshTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
            }
            else
            {
                refreshTimer.Change(TimeSpan.FromSeconds(RefreshInterval), TimeSpan.FromMilliseconds(-1));
            }
        }

        #endregion

        #region IsShowColumnConfigMenu
        /// <summary>
        /// 配置列、隐藏列菜单是否启用
        /// </summary>
        public bool IsShowColumnConfigMenu { get; set; }
        #endregion

        #region IsShowRefreshMenu
        /// <summary>
        /// 刷新、自动刷新菜单是否启用（目前刷新直接隐藏了）
        /// </summary>
        public bool IsShowRefreshMenu { get; set; }
        #endregion

        #region IsShowSaveMenu
        /// <summary>
        /// 转存、转存所有菜单是否启用
        /// </summary>
        public bool IsShowSaveMenu { get; set; }
        #endregion

        #region IsShowSaveMenu

        private int hiddenColumRnHeadersWidth = 0;
        /// <summary>
        /// 设置菜单宽度
        /// </summary>
        public int HiddenColumRnHeadersWidth { get => hiddenColumRnHeadersWidth; set => hiddenColumRnHeadersWidth = value; }

        #endregion

        #region Title
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
        #endregion

        #region 私有变量
        /// <summary>
        /// 最外层滚动控件
        /// </summary>
        private ScrollViewer _scrollViewer;
        /// <summary>
        /// 表头控件
        /// </summary>
        private DataGridColumnHeadersPresenter _columnHeader;
        /// <summary>
        /// 列信息
        /// </summary>
        private DataGridInformation DGInfo = new DataGridInformation();
        /// <summary>
        /// 当前右键点击的表头
        /// </summary>
        private DataGridColumnHeader _currentHeader;
        /// <summary>
        /// 保存列信息的目录地址
        /// </summary>
        private string _ColumnInfoPathRoot;
        /// <summary>
        /// 刷新按钮
        /// </summary>
        private MenuItem _refreshMenuItem;
        /// <summary>
        /// 自动刷新菜单项
        /// </summary>
        private MenuItem _autoRefreshMenuItem;
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
        /// 刷新间隔下拉框
        /// </summary>
        private ComboBox _autoRefreshIntervalCombo;
        /// <summary>
        /// 冻结列
        /// </summary>
        private List<string> _frozenColumns;
        /// <summary>
        /// 隐藏列信息窗口
        /// </summary>
        private Popup _hiddenColumnHeaders;
        /// <summary>
        /// 列表默认的排序
        /// </summary>
        private List<SortDescription> defaultSortDescriptions;

        /// <summary>
        /// 自动刷新计时器
        /// </summary>
        private Timer refreshTimer;
        /// <summary>
        /// 刷新命令
        /// </summary>
        private ICommand _refreshCommand;

        /// <summary>
        /// 不参与配置的列的索引的集合
        /// key:index value:displayIndex
        /// </summary>
        private Dictionary<int, int> _noConfigColumnIndexList = new Dictionary<int, int>();
        /// <summary>
        /// 配置列中列表
        /// </summary>
        private DragDropListBox _listView;
        /// <summary>
        /// 各列上一个排序状态，为了可以恢复以前的排序
        /// </summary>
        private Dictionary<DataGridColumn, ListSortDirection?> lastSortDic = new Dictionary<DataGridColumn, ListSortDirection?>();
        /// <summary>
        /// 刷新间隔下拉列表
        /// </summary>
        private List<int> _autoRefreshIntervalSource;

        private EventHandler _widthPropertyChangedHandler;
        private DependencyPropertyDescriptor _sortDirectionPropertyDescriptor;
        private DependencyPropertyDescriptor _widthPropertyDescriptor;
        #endregion

        private static void InvalidateVisual(DependencyObject d)
        {
            var uielement = d as UIElement;
            if (uielement != null)
                uielement.InvalidateVisual();
        }

        private void OnUnloaded()
        {
            if (refreshTimer != null)
            {
                refreshTimer.Dispose();
                refreshTimer = null;
            }
            foreach (var column in Columns)
            {
                if (_sortDirectionPropertyDescriptor != null)
                    _sortDirectionPropertyDescriptor.RemoveValueChanged(column, ColumnResort);
                if (_widthPropertyDescriptor != null && _widthPropertyChangedHandler != null)
                    _widthPropertyDescriptor.RemoveValueChanged(column, _widthPropertyChangedHandler);
            }
        }

        private bool _isInit = false;
        private void InitMenu()
        {
            if (!_isInit)
            {
                GetSaveDirectory();
                _frozenColumns = new List<string>();
                for (var i = 0; i < this.FrozenColumnCount; i++)
                {
                    _frozenColumns.Add(this.Columns[i].GetID());
                }
                LoadConfig();
                OnComponentLoaded();
                InitializeComponent();

                _isInit = true;
            }
        }

        private bool _isDisposed;
        private void DisposeMenu()
        {
            if (!_isDisposed)
            {
                OnUnloaded();
                _isDisposed = true;
            }
        }

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
        #endregion


        private void LoadConfig()
        {
            if (!string.IsNullOrEmpty(LayoutName) && File.Exists(RealSavePath))
            {
                string xmlStr = File.ReadAllText(RealSavePath);
                SetColumnInformation(xmlStr);
            }
        }

        private void CheckRootDirectory()
        {
            if (!Directory.Exists(_ColumnInfoPathRoot))
            {
                Directory.CreateDirectory(_ColumnInfoPathRoot);
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

        private void OnComponentLoaded()
        {
            _scrollViewer = GetTemplateChild(DG_ScrollViewer) as ScrollViewer;
            _columnHeader = VisualHelper.FindChild<DataGridColumnHeadersPresenter>(_scrollViewer, PART_ColumnHeadersPresenter);
            if (_columnHeader != null && _columnHeader.ContextMenu != null)
            {
                ContextMenu cm = _columnHeader.ContextMenu;
                _refreshMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_RefreshMenuItem) as MenuItem;
                _autoRefreshMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_AutoRefreshMenuItem) as MenuItem;
                _saveCurrentMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_SaveCurrentMenuItem) as MenuItem;
                _saveAllMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_SaveAllMenuItem) as MenuItem;
                _showHiddenColumnsMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_ShowHiddenColumnsMenuItem) as MenuItem;
                _hideCurrentColumnMenuItem = LogicalTreeHelper.FindLogicalNode(cm, PART_HideCurrentColumnMenuItem) as MenuItem;
                _autoRefreshIntervalCombo = LogicalTreeHelper.FindLogicalNode(cm, PART_AutoRefreshIntervalCombo) as ComboBox;
                _columnHeader.PreviewMouseRightButtonUp += _columnHeader_PreviewMouseRightButtonUp;
            }
            _hiddenColumnHeaders = VisualHelper.FindChild<Popup>(_scrollViewer, PART_HiddenColumnHeadersPopUp);
            if (_hiddenColumnHeaders != null)
            {
                _listView = LogicalTreeHelper.FindLogicalNode(_hiddenColumnHeaders, PART_ListView) as DragDropListBox;
                //计算Popup的高度
                //var height = 16 * this.Columns.Where(c => c.Header != null && c.Header is string).Count() + 40 + 10 + 5;
                if (hiddenColumRnHeadersWidth > 0)
                    _hiddenColumnHeaders.Width = hiddenColumRnHeadersWidth;
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
        }

        private void InitializeComponent()
        {
            if (IsShowRefreshMenu)
            {
                if (_autoRefreshIntervalCombo != null)
                {
                    _autoRefreshIntervalSource = new List<int>() { 0, 1, 5, 10 };
                    _autoRefreshIntervalCombo.ItemsSource = _autoRefreshIntervalSource;
                    Binding selectedBinding = new Binding();
                    selectedBinding.Source = this;
                    selectedBinding.Path = new PropertyPath("RefreshInterval");
                    selectedBinding.Mode = BindingMode.TwoWay;
                    selectedBinding.Converter = new RefreshIntervalConverter();
                    _autoRefreshIntervalCombo.SetBinding(ComboBox.TextProperty, selectedBinding).UpdateTarget();
                }
                if (_refreshMenuItem != null)
                {
                    //该功能暂时设定为隐藏，所以不初始化
                    //Binding refreshBinding = new Binding();
                    //refreshBinding.Source = this;
                    //refreshBinding.Path = new PropertyPath("RefreshCommand");
                    //_refreshMenuItem.SetBinding(MenuItem.CommandProperty, refreshBinding);
                    //_refreshMenuItem.Visibility = Visibility.Visible;
                }
                if (_autoRefreshMenuItem != null)
                {
                    _autoRefreshMenuItem.Visibility = Visibility.Visible;
                }

                refreshProcess(null);
                refreshTimer = new Timer(refreshProcess, null, Timeout.Infinite, Timeout.Infinite);
                UpdateTimerInterval();
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


        #region 注册命令
        /// <summary>
        /// 注册命令
        /// </summary>
        private void RegisterConfigCommand()
        {
            ConfirmCommand = new DelegateCommand(OnConfirm);
            CancelCommand = new DelegateCommand(OnCancel);
        }

        #endregion



        #region 隐藏及显示列
        private string RealSavePath
        {
            get
            {
                try
                {
                    return Path.Combine(_ColumnInfoPathRoot, LayoutName + ".xml");

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return string.Empty;
                }
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
            _currentHeader = VisualHelper.FindParent<DataGridColumnHeader>(Mouse.DirectlyOver as UIElement);
        }

        protected override void OnColumnReordered(DataGridColumnEventArgs e)
        {
            base.OnColumnReordered(e);
            SaveColumnInfo();
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
                Debug.WriteLine("恢复列信息异常，{0}", ex.ToString());
            }
        }

        /// <summary>
        /// 更新列信息
        /// </summary>
        private void UpdateColumnInfo(bool isSave = true)
        {
            //唯一标志为null或高净值隐藏的列不参与保存
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
                    Debug.WriteLine(string.Format("写入文件{0}时报错：", RealSavePath));
                    Debug.WriteLine(ex);
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
                //没有标志或高净值时隐藏或保存列的集合中不包含该列，则认为是新添加的列
                if (string.IsNullOrWhiteSpace(column.GetID())
                    || DGInfo.ColumnInfos.FirstOrDefault(x => column.SortMemberPath.Equals(x.SortMemberPath) && column.GetID().Equals(x.Header)).Equals(default(ColumnInformation)))
                {
                    newColumns.Add(column.DisplayIndex, column);
                }
            }
            foreach (var column in DGInfo.ColumnInfos.OrderBy(x => x.DisplayIndex))
            {
                //高净值隐藏是防止之前保存过，后来业务上有修改
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
            RefreshInterval = DGInfo.RefreshInterval;
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
        }

        private void OnCancel()
        {
            _hiddenColumnHeaders.IsOpen = false;
        }
        #endregion


        #endregion

        #region Load
        private void DataGridExt_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AutoFillWithBlankRow)
                this.InvalidateVisual();

            isSupportSumRow = GetIsSupportSumRow(this);
            InitScrollPosition();
        }
        #endregion

        #region UnLoad

        private void DataGridExt_Unloaded(object sender, RoutedEventArgs e)
        {
            DisposeMenu();
        }

        #endregion

        #region DataGridExt_ColumnHeaderDragCompleted
        private void DataGridExt_ColumnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            try
            {
                if (!isSupportSumRow)
                    return;
                DataGridExt GridExt = (DataGridExt)sender;
                var orderedCol = GridExt.Columns.Where(x => x.Visibility == Visibility.Visible).OrderBy(x => x.DisplayIndex).ToArray();

                if (this.TotalRow != null && this.TotalRow.ItemsSource != null)
                    this.TotalRow.ItemsSource = null;
                dgSource = this;
                this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                SupportSumRowProperties.Clear();
                TotalRow = VisualHelper.FindChild<DataGrid>(this, "TotalRow"); //base.GetTemplateChild("TotalRow") as DataGrid;

                if (TotalRow == null)
                {
                    return;
                }

                TotalRow.FrozenColumnCount = dgSource.FrozenColumnCount;
                TotalRow.LoadingRow += TotalRow_LoadingRow;
                TotalRow.Visibility = Visibility.Visible;

                TotalRow.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));
                TotalRow.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));

                int displayindex = 0;
                if (TotalRow.Columns != null)
                    TotalRow.Columns.Clear();

                bool firstColumn = true;

                foreach (var item in orderedCol)
                {
                    DataGridTextColumn cl = new DataGridTextColumn();
                    cl.Header = item.Header;
                    cl.Width = item.Width;
                    cl.DisplayIndex = item.DisplayIndex = displayindex++;
                    cl.ElementStyle = Style_txb_rigth;

                    Binding widthBd = new Binding();
                    widthBd.Source = item;
                    widthBd.Mode = BindingMode.OneWay;
                    widthBd.Path = new PropertyPath(DataGridColumn.ActualWidthProperty);
                    BindingOperations.SetBinding(cl, DataGridTextColumn.WidthProperty, widthBd);

                    Binding visibleBd = new Binding();
                    visibleBd.Source = item;
                    visibleBd.Mode = BindingMode.OneWay;
                    visibleBd.Path = new PropertyPath(DataGridColumn.VisibilityProperty);
                    BindingOperations.SetBinding(cl, DataGridTextColumn.VisibilityProperty, visibleBd);

                    Binding indexBd = new Binding();
                    indexBd.Source = item;
                    indexBd.Mode = BindingMode.OneWay;
                    indexBd.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                    BindingOperations.SetBinding(cl, DataGridTextColumn.DisplayIndexProperty, indexBd);

                    string sumPath = DataGridExt.GetSumPath(item);
                    string sumNumFormat = DataGridExt.GetSumNumFormat(item);
                    if (firstColumn)
                    {
                        var binding = new Binding("Header");
                        var relativeSource = new RelativeSource();
                        relativeSource.Mode = RelativeSourceMode.FindAncestor;
                        relativeSource.AncestorType = typeof(DataGridRow);
                        binding.RelativeSource = relativeSource;
                        cl.Binding = binding;
                        firstColumn = false;
                    }
                    else if (!string.IsNullOrEmpty(sumPath))
                    {
                        SupportSumRowProperties.Add(sumPath);

                        Binding valueBind = new Binding();
                        valueBind.Mode = BindingMode.TwoWay;
                        valueBind.Path = new PropertyPath(sumPath);

                        if (string.IsNullOrEmpty(sumNumFormat))
                        {
                            sumNumFormat = "N2";
                        }

                        valueBind.Converter = new ConverterAmountToString() { Parameter = sumNumFormat };
                        valueBind.ConverterParameter = sumNumFormat;
                        cl.Binding = valueBind;
                    }



                    TotalRow.Columns.Add(cl);
                }

                this.TotalRow.ItemsSource = totalRowItemSource;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
        }

        #endregion

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.InvalidateVisual();
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "HorizontalScrollOffset")//记录下当前滚动偏移量
            {
                HorizontalScrollOffset = (double)e.NewValue;
                this.InvalidateVisual();//强制重绘
            }
        }

        //主要为了解决拖动列头时不能重绘的问题
        protected override void OnIsMouseCaptureWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseCaptureWithinChanged(e);

            this.InvalidateVisual();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (ItemsChanged != null)
                ItemsChanged(this, null);
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);

            //var index = e.Row.GetIndex();
            //if (index == 0)
            //{
            //    dataGridRows.Clear();

            //    if (CurrentGridData != null)
            //        CurrentGridData.Clear();
            //}

            //dataGridRows.Add(e.Row);
            //if (CurrentGridData != null)
            //    CurrentGridData.Add(e.Row.DataContext);

        }

        protected override void OnUnloadingRow(DataGridRowEventArgs e)
        {
            base.OnUnloadingRow(e);
            //if (dataGridRows.Remove(e.Row))//行卸载的时候需要手动触发重绘
            //    this.InvalidateVisual();
        }

        //主要解决在某些时候双击编辑时，行高发生变化不能重绘的问题
        protected override void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
        {
            base.OnPreparingCellForEdit(e);

            this.InvalidateVisual();
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.IsLoaded && AutoFillWithBlankRow)
            {
                //要把背景色设置为透明，不然画的线看不见
                this.Background = Brushes.Transparent;

                this.DrawBlankRows(drawingContext);
            }

            InitMenu();
        }

        #region ExportCommand 导出
        /// <summary>
        /// 导出
        /// </summary>
        public ICommand SaveAllCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "请选择您要导出文件的位置";
                    sd.DefaultExt = "xlsx";
                    sd.AddExtension = true;
                    sd.Filter = "Excel(*.xls;*.xlsx)文件|*.xls;*.xlsx";
                    // sd.FileName = string.Format("日内交易信息_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));

                    if (sd.ShowDialog() == true)
                    {
                        //数据导出到Excel中              
                        ExportToExcel(sd.SafeFileName, sd.FileName);
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
                return new DelegateCommand(() =>
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "请选择您要导出文件的位置";
                    sd.DefaultExt = "xlsx";
                    sd.AddExtension = true;
                    sd.Filter = "Excel(*.xls;*.xlsx)文件|*.xls;*.xlsx";
                    // sd.FileName = string.Format("日内交易信息_{0}_{1}", DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"));

                    if (sd.ShowDialog() == true)
                    {
                        //数据导出到Excel中              
                        ExportToExcel(sd.SafeFileName, sd.FileName, false);
                    }
                });
            }
        }
        #endregion

        #region ExportToExcel
        /// <summary>
        /// 导出为Excel文件
        /// </summary>
        /// <param name="workSheetName">工资表名称</param>
        /// <param name="fullFilePath">转存文件路径</param>
        /// <param name="exportAll">是否导出全部数据，否则仅导出选择项</param>
        public void ExportToExcel(string workSheetName, string fullFilePath, bool exportAll = true)
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
                int minColumnDisplayIndex = Columns[0].Header is CheckBox ? 1 : 0;
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
                    //// ClipboardHelper.GetClipboardContentForHtml(dataGridStringBuilders[DataFormats.CommaSeparatedValue]);
                    //IWorkbook workbook = ExcelHelper.CreateWorkbook(fullFilePath);
                    ////添加一个sheet页
                    //ISheet workSheet = workbook.CreateSheet(ExcelHelper.ConversionSheetName(workSheetName));

                    //ExcelHelper.LoadFromCSVText(workSheet, dataGridStringBuilders[DataFormats.CommaSeparatedValue].ToString());

                    //ExcelHelper.SaveWorkbook(fullFilePath, workbook);
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

        private static Dictionary<int, int[]> dicMergeRows;
        private static Dictionary<int, int[]> dicMergecolumns;
        /// <summary>
        /// 将列表数据导出为Excel
        /// </summary>
        /// <param name="workSheetName">工作表名</param>
        /// <param name="fullPath">保存文件的完整路径</param>
        /// <param name="widthDic">key 列索引 value 列宽 不设置 默认都是80像素</param>
        /// <returns></returns>
        public static void ExportToExcel(DataGridExt dg, string workSheetName, string fullPath, Dictionary<int, int> widthDic)
        {
            try
            {
                dicMergeRows = new Dictionary<int, int[]>();
                dicMergecolumns = new Dictionary<int, int[]>();
                StringBuilder dataGridStringBuilder = new StringBuilder();
                StringBuilder dataIsMulticolumnBuilder = new StringBuilder();

                int minRowIndex = 0;
                int maxRowIndex = dg.Items.Count - 1;
                int maxColumnDisplayIndex = dg.Columns.Count(x => x.Visibility == Visibility.Visible) - 1;
                var sb = new StringBuilder();

                var columns = dg.Columns
                    .Where(x => x is DataGridTemplateColumnExt && x.Visibility == Visibility.Visible)
                    .Select(x => x as DataGridTemplateColumnExt);
                int columnCount = 0;

                foreach (var column in dg.Columns)
                {
                    if (column.Header == null || column.Header.ToString() == "#" || (column.DisplayIndex == 0 && column.Header is CheckBox))
                        continue;
                    var columnd = column as DataGridTemplateColumnExt;
                    if (columnd == null || !columnd.IsVisbiles)
                        continue;
                    if (dg.IsMulticolumn)
                    {

                        if (columnd.MergeRows > 0)
                        {

                            string[] ColumnsValue = columnd.MultiColumnsName.Split('^');
                            dataGridStringBuilder.Append(column.Header);
                            for (int Z = 0; Z < columnd.MergeRows; Z++)
                            {
                                dataIsMulticolumnBuilder.Append(ColumnsValue[Z]);
                                dataIsMulticolumnBuilder.AppendFormat("&");
                                dataGridStringBuilder.AppendFormat("&");


                            }
                            int[] myArray = { columnCount, columnCount + columnd.MergeRows - 1 };
                            dicMergecolumns.Add(columnCount, myArray);
                            columnCount += Convert.ToInt32(columnd.MergeRows);
                            continue;
                        }
                        else
                        {
                            int[] myArray = { columnCount, columnCount };
                            dicMergeRows.Add(columnCount, myArray);
                        }

                    }
                    dataGridStringBuilder.Append(column.Header);
                    var isLast = columns.Last() == column;
                    if (isLast)
                    {

                        dataGridStringBuilder.Append('\r');
                        dataGridStringBuilder.Append('\n');
                        if (dg.IsMulticolumn)
                        {
                            dataGridStringBuilder.Append(dataIsMulticolumnBuilder.ToString());

                            dataGridStringBuilder.Append('\r');
                            dataGridStringBuilder.Append('\n');
                        }
                    }
                    else
                    {
                        dataGridStringBuilder.AppendFormat("&");
                        dataIsMulticolumnBuilder.AppendFormat("&");
                    }
                    columnCount++;

                }
                for (int i = minRowIndex; i <= maxRowIndex; i++)
                {
                    object row = dg.Items[i];
                    var items = dg.ItemsSource.GetType();
                    var type = row.GetType();
                    foreach (var column in columns)
                    {
                        if (!column.IsVisbiles || (column.DisplayIndex == 0 && column.Header is CheckBox))
                            continue;
                        if (column.MergeRows > 0 && !string.IsNullOrWhiteSpace(column.MultiColumnsValue))
                        {
                            string[] ColumnsValue = column.MultiColumnsValue.Split('^');
                            string[] ColumnsstrForm = column.MultiColumnsStrFrom.Split('^');
                            for (int Z = 0; Z < column.MergeRows; Z++)
                            {

                                var propertyValue = type.GetProperty(ColumnsValue[Z]);
                                if (propertyValue != null)
                                {
                                    var value = propertyValue.GetValue(row, null);
                                    if (value != null)
                                    {
                                        if (column.Converter != null)
                                        {
                                            sb.Append(column.Converter.Convert(value, value.GetType(), column.ConverterParameter, null));
                                        }
                                        else if (value.GetType() == typeof(double))
                                        {
                                            sb.Append(((double)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(float))
                                        {
                                            sb.Append(((float)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(decimal))
                                        {
                                            sb.Append(((decimal)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(int))
                                        {
                                            sb.Append(((int)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(short))
                                        {
                                            sb.Append(((short)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(long))
                                        {
                                            sb.Append(((long)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else if (value.GetType() == typeof(DateTime))
                                        {
                                            sb.Append(((DateTime)value).ToString(ColumnsstrForm[Z]));
                                        }
                                        else
                                        {
                                            sb.Append(value);
                                        }
                                    }
                                }
                                sb.AppendFormat("&");
                            }
                            continue;
                        }
                        var name = column.HeaderName;
                        //var property= type.GetProperty(name);                    
                        //if (property != null)
                        {
                            var value = ReflectorUtil.FollowPropertyPath(row, name);
                            if (value != null)
                            {
                                if (column.Converter != null)
                                {
                                    sb.Append(column.Converter.Convert(value, value.GetType(), column.ConverterParameter, null));
                                }
                                else if (value.GetType() == typeof(double))
                                {
                                    sb.Append(((double)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(float))
                                {
                                    sb.Append(((float)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(decimal))
                                {
                                    sb.Append(((decimal)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(int))
                                {
                                    sb.Append(((int)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(short))
                                {
                                    sb.Append(((short)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(long))
                                {
                                    sb.Append(((long)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(DateTime))
                                {
                                    if (((DateTime)value).Date == DateTime.MinValue.Date || ((DateTime)value).Date == DateTime.MaxValue.Date)
                                        sb.Append("待定");
                                    else
                                        sb.Append(((DateTime)value).ToString(column.StrFormat));
                                }
                                else
                                {
                                    sb.Append(value);
                                }
                            }
                        }

                        var isLast = columns.Last() == column;
                        if (isLast)
                        {

                            sb.Append('\r');
                            sb.Append('\n');
                        }
                        else
                        {
                            sb.AppendFormat("&");
                        }
                    }


                }
                if (dg.TotalRow != null && dg.TotalRow.Items.Count > 0)
                {
                    sb.Append('\r');
                    sb.Append('\n');
                    object Totalrow = dg.TotalRow.Items[0];
                    Type itemType = null;
                    foreach (var item in dg.TotalRow.ItemsSource)
                    {
                        itemType = item.GetType();
                        break;
                    }
                    if (itemType == null)
                        return;

                    PropertyInfo[] ps = itemType.GetProperties();
                    int RowNumber = 0;
                    foreach (var column in columns)
                    {
                        string sumPath = DataGridSumHelper.GetSumPath(column);

                        var property = ps.Where(x => x.Name == column.HeaderName).FirstOrDefault();
                        if (property != null)
                        {
                            string SumRowPropertname = property.Name;

                            if (!string.IsNullOrWhiteSpace(sumPath))
                            {
                                SumRowPropertname = sumPath;
                                property = ps.Where(x => x.Name == SumRowPropertname).FirstOrDefault();
                            }

                            if (!dg.SupportSumRowProperties.Any(x => x == SumRowPropertname))
                            {
                                if (RowNumber == 0)
                                    sb.AppendFormat("汇总");
                                else
                                    sb.AppendFormat(" ");
                                sb.AppendFormat("&");
                                RowNumber++;
                                continue;
                            }
                            var value = property.GetValue(Totalrow, null);
                            //var column = columns.Where(x => x.HeaderName == property.Name).FirstOrDefault();

                            if (value != null && column != null)
                            {
                                if (column.Converter != null)
                                {
                                    sb.Append(column.Converter.Convert(value, value.GetType(), column.ConverterParameter, null));
                                }
                                else if (value.GetType() == typeof(double))
                                {
                                    sb.Append(((double)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(float))
                                {
                                    sb.Append(((float)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(decimal))
                                {
                                    sb.Append(((decimal)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(int))
                                {
                                    sb.Append(((int)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(short))
                                {
                                    sb.Append(((short)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(long))
                                {
                                    sb.Append(((long)value).ToString(column.StrFormat));
                                }
                                else if (value.GetType() == typeof(DateTime))
                                {
                                    sb.Append(((DateTime)value).ToString(column.StrFormat));
                                }
                                else
                                {
                                    sb.Append(value);
                                }
                            }

                            sb.AppendFormat("&");

                        }

                    }
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    dataGridStringBuilder.Append(sb);
                }
                string data = dataGridStringBuilder.ToString();
                if (!string.IsNullOrEmpty(data))
                {
                    //IWorkbook workbook = ExcelHelper.CreateWorkbook(fullPath);

                    //ISheet workSheet = workbook.CreateSheet(ExcelHelper.ConversionSheetName(workSheetName));

                    //ExcelHelper.SetColumnWidth(workSheet, 0, maxColumnDisplayIndex, 80 / 8);
                    //if (widthDic != null && widthDic.Any())
                    //{
                    //    foreach (var dic in widthDic)
                    //    {
                    //        ExcelHelper.SetColumnWidth(workSheet, dic.Key, dic.Key, dic.Value / 8);
                    //    }
                    //}
                    //foreach (var itemRow in dicMergeRows)
                    //{

                    //    ExcelHelper.MergedCell(workSheet, 0, 1, itemRow.Value[0], itemRow.Value[1]);

                    //}
                    //foreach (var itemcolumns in dicMergecolumns)
                    //{
                    //    ExcelHelper.MergedCell(workSheet, 0, 0, itemcolumns.Value[0], itemcolumns.Value[1]);

                    //}
                    ////ExcelHelper.MergedCell(workSheet, 0, 1, 0, 0);
                    ////ExcelHelper.MergedCell(workSheet, 0, 1, 1, 1);
                    ////ExcelHelper.MergedCell(workSheet, 0, 1, 2, 2);
                    ////ExcelHelper.MergedCell(workSheet, 0, 0, 1, 2);
                    //ExcelHelper.LoadFromCSVTextDivision(workSheet, data);
                    //ExcelHelper.SaveWorkbook(fullPath, workbook);

                   MessageBox.Show("数据导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("没有可导出的数据！", "提示", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("数据导出错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        private void DrawBlankRows(DrawingContext drawingContext)
        {
            var pen = CreatePen();
            var startY = GetStartY();

            DrawBlankRowLines(startY, pen, drawingContext);
            //InitInitSumRowGridFinished = false;
            //InitSumRowGrid();
        }

        private Pen CreatePen()
        {
            var pen = new Pen();
            pen.Thickness = this.BlankRowLineWidth < 0 ? DefaultBlankRowLineWidth : this.BlankRowLineWidth;
            if (this.BlankRowLineBrush != null)
            {
                pen.Brush = this.BlankRowLineBrush;
            }
            else
            {
                if (this.HorizontalGridLinesBrush != null)
                {
                    pen.Brush = this.HorizontalGridLinesBrush;
                }
                else
                {
                    if (this.VerticalGridLinesBrush != null)
                    {
                        pen.Brush = this.VerticalGridLinesBrush;
                    }
                    else
                    {
                        pen.Brush = DefaultBlankRowLineBrush;
                    }
                }
            }
            return pen;
        }

        private double GetStartY()
        {
            var sumh = this.dataGridRows.Sum(x => x.ActualHeight);
            var curY = Math.Floor(sumh);//cury 计算结果不能带小数，不然画不出来，这是微软的BUG ？？？？
            return curY;
        }

        private void DrawBlankRowLines(double startY, Pen pen, DrawingContext drawingContext)
        {
            var endX = this.RenderSize.Width;
            var endY = this.RenderSize.Height;

            var rowHeight = DefaultBlankRowHeight;

            if (this.BlankRowHeight > 0)
                rowHeight = this.BlankRowHeight;

            var curY = startY;
            //水平线
            while ((curY += rowHeight) < endY)
            {
                var startPoint = new Point(0, curY);
                var endPoint = new Point(endX, curY);

                drawingContext.DrawLine(pen, startPoint, endPoint);
            }

            //排序主要是因为如果拖动改变了列的显示顺序可能导致重绘对不齐
            var orderedCol = this.Columns.Where(x => x.Visibility == Visibility.Visible || x.Visibility == Visibility.Hidden).OrderBy(x => x.DisplayIndex).ToArray();

            //先画被冻结的列
            var frozeColumnWidth = 0d;//被冻结的列的总宽度
            for (int i = 0; i < this.FrozenColumnCount; i++)
            {
                frozeColumnWidth += orderedCol[i].ActualWidth;

                //超出范围了后面的列都不用画了
                if (frozeColumnWidth > this.RenderSize.Width) return;

                drawingContext.DrawLine(pen, new Point(frozeColumnWidth, 0), new Point(frozeColumnWidth, this.RenderSize.Height));
            }


            #region 这段代码主要解决在拖动滚动条后，重绘对不齐的问题

            var startX = 0d;
            var startColindex = this.FrozenColumnCount;
            for (int i = this.FrozenColumnCount; i < orderedCol.Length; i++)
            {
                startX += orderedCol[i].ActualWidth;
                if (startX > HorizontalScrollOffset)
                {
                    startX = startX - HorizontalScrollOffset;
                    startColindex = i;
                    break;
                }
            }

            #endregion

            //竖直线，每一列的右侧绘制
            var curX = startX;

            for (int i = startColindex; i < orderedCol.Length; i++)
            {
                if (i > startColindex)
                    curX += orderedCol[i].ActualWidth;

                drawingContext.DrawLine(pen, new Point(Math.Round(curX + frozeColumnWidth), 0), new Point(Math.Round(curX + frozeColumnWidth), this.RenderSize.Height));
            }

            //最左边的线
            drawingContext.DrawLine(pen, new Point(0, startY), new Point(0, endY));
            //最右边的线
            drawingContext.DrawLine(pen, new Point(endX, startY), new Point(endX, endY));


        }

        #region 汇总
        private DataGrid TotalRow;
        /// <summary>
        /// 汇总行数据
        /// </summary>
        public List<object> totalRowItemSource;
        /// <summary>
        /// 支持汇总行
        /// </summary>
        bool isSupportSumRow = false;

        private Style _Style_txb_rigth;
        Style Style_txb_rigth
        {
            get
            {
                if (_Style_txb_rigth == null)
                {
                    _Style_txb_rigth = Application.Current.FindResource("TextBlock_DataGridCell_Right_Style") as Style;
                }
                return _Style_txb_rigth;
            }
        }


        ///// <summary>
        // /// 合并指定EXCEL的单元格
        // /// </summary>
        // /// <param name="mySheet">指定的EXCEL工作表</param>
        // /// <param name="startLine">起始行</param>
        // /// <param name="recCount">总行数</param>
        // /// <param name="col">要合并的列</param>
        //private void MergeCell_Second(ref Microsoft.Office.Interop.Excel.Worksheet mySheet, int startLine, int recCount, string col)
        // {
        //    string qy1 = mySheet.get_Range(col + startLine.ToString(), col + startLine.ToString()).Text.ToString();//获得起始行合并列单元格的填充内容

        //    Microsoft.Office.Interop.Excel.Range rg1;
        //    string strtemp = "";
        //    bool endCycle = false;

        //    //从起始行到终止行做循环
        //    for (int i = startLine; i <= recCount + startLine - 1&&!endCycle; )
        //    {
        //        for (int j = i + 1; j <= recCount + startLine - 1; j++)
        //        {
        //            rg1 = mySheet.get_Range(col + j.ToString(), col + j.ToString());//获得下一行的填充内容
        //            strtemp = rg1.Text.ToString().Trim();

        //            if (strtemp.Trim()==qy1.Trim() )//内容等于初始内容
        //            {
        //                rg1 = mySheet.get_Range(col + i.ToString(), col + j.ToString());//选取上条合并位置和当前行的合并区域
        //                rg1.ClearContents();//清空要合并的区域
        //                rg1.MergeCells = true;
        //                if (col == "A")
        //                    mySheet.Cells[i, 1] = qy1;
        //                else if (col == "B")
        //                    mySheet.Cells[i, 2] = qy1;

        //                if (j== recCount + startLine - 1)
        //                {
        //                    endCycle = true;
        //                }
        //            }
        //            else//内容不等于初始内容
        //            {
        //                i = j;//i获取新值
        //                qy1 = mySheet.get_Range(col + j.ToString(), col + j.ToString()).Text.ToString();
        //                break;
        //            }
        //        }
        //    }
        //}


        List<string> SupportSumRowProperties = new List<string>();

        /// <summary>
        /// 原DataGrid
        /// </summary>
        private static DataGrid dgSource;

        /// <summary>
        /// DataGrid中滚动条样式
        /// </summary>
        private const string ScrollViewerNameInTemplate = "DG_ScrollViewer";

        private bool InitInitSumRowGridFinished = false;

        private void InitSumRowGrid()
        {
            try
            {
                if (isSupportSumRow && !InitInitSumRowGridFinished)
                {
                    dgSource = this;
                    this.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    SupportSumRowProperties.Clear();
                    TotalRow = VisualHelper.FindChild<DataGrid>(this, "TotalRow"); //base.GetTemplateChild("TotalRow") as DataGrid;

                    if (TotalRow == null)
                    {
                        return;
                    }

                    TotalRow.FrozenColumnCount = dgSource.FrozenColumnCount;
                    TotalRow.LoadingRow += TotalRow_LoadingRow;
                    TotalRow.Visibility = Visibility.Visible;

                    TotalRow.RemoveHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));
                    TotalRow.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(xDataGrid_ScrollChanged));

                    int displayindex = 0;
                    if (TotalRow.Columns != null)
                        TotalRow.Columns.Clear();

                    bool firstColumn = true;

                    foreach (var item in this.Columns)
                    {
                        DataGridTextColumn cl = new DataGridTextColumn();
                        cl.Header = item.Header;
                        cl.Width = item.Width;
                        cl.DisplayIndex = item.DisplayIndex = displayindex++;
                        cl.ElementStyle = Style_txb_rigth;

                        Binding widthBd = new Binding();
                        widthBd.Source = item;
                        widthBd.Mode = BindingMode.OneWay;
                        widthBd.Path = new PropertyPath(DataGridColumn.ActualWidthProperty);
                        BindingOperations.SetBinding(cl, DataGridTextColumn.WidthProperty, widthBd);

                        Binding visibleBd = new Binding();
                        visibleBd.Source = item;
                        visibleBd.Mode = BindingMode.OneWay;
                        visibleBd.Path = new PropertyPath(DataGridColumn.VisibilityProperty);
                        BindingOperations.SetBinding(cl, DataGridTextColumn.VisibilityProperty, visibleBd);

                        Binding indexBd = new Binding();
                        indexBd.Source = item;
                        indexBd.Mode = BindingMode.OneWay;
                        indexBd.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                        BindingOperations.SetBinding(cl, DataGridTextColumn.DisplayIndexProperty, indexBd);

                        string sumPath = GetSumPath(item);
                        string sumNumFormat = GetSumNumFormat(item);
                        if (firstColumn)
                        {
                            var binding = new Binding("Header");
                            var relativeSource = new RelativeSource();
                            relativeSource.Mode = RelativeSourceMode.FindAncestor;
                            relativeSource.AncestorType = typeof(DataGridRow);
                            binding.RelativeSource = relativeSource;
                            cl.Binding = binding;
                            firstColumn = false;
                        }
                        else if (!string.IsNullOrEmpty(sumPath))
                        {
                            SupportSumRowProperties.Add(sumPath);

                            Binding valueBind = new Binding();
                            valueBind.Mode = BindingMode.TwoWay;
                            valueBind.Path = new PropertyPath(sumPath);

                            if (string.IsNullOrEmpty(sumNumFormat))
                            {
                                sumNumFormat = "N2";
                            }

                            valueBind.Converter = new ConverterAmountToString() { Parameter = sumNumFormat };
                            valueBind.ConverterParameter = sumNumFormat;
                            cl.Binding = valueBind;
                        }



                        TotalRow.Columns.Add(cl);
                    }

                    this.TotalRow.ItemsSource = totalRowItemSource;
                    InitInitSumRowGridFinished = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void xDataGrid_ScrollChanged(object sender, RoutedEventArgs eBase)
        {
            InitScrollPosition();
        }

        private void InitScrollPosition()
        {
            try
            {
                if (TotalRow == null || this == null)
                {
                    return;
                }
                ScrollViewer sourceScrollViewer = (ScrollViewer)dgSource.Template.FindName(ScrollViewerNameInTemplate, this);
                ScrollViewer associatedScrollViewer = (ScrollViewer)TotalRow.Template.FindName(ScrollViewerNameInTemplate, TotalRow);
                sourceScrollViewer.ScrollToHorizontalOffset(associatedScrollViewer.HorizontalOffset);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void TotalRow_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            try
            {
                var row = (DataGridRow)TotalRow.ItemContainerGenerator.ContainerFromIndex(0);
                if (row != null)
                {
                    row.Header = "汇总：";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void CalculateSum()
        {
            try
            {

                if (isSupportSumRow)
                {
                    InitSumRowGrid();

                    Type itemType = null;
                    totalRowItemSource = new List<object>();
                    object obj = null;
                    if (this.TotalRow != null)
                        this.TotalRow.ItemsSource = null;

                    if (this.ItemsSource == null)
                    {
                        return;
                    }

                    var viewItems = CollectionViewSource.GetDefaultView(this.ItemsSource);

                    if (viewItems == null)
                    {
                        return;
                    }


                    foreach (var item in this.ItemsSource)
                    {
                        itemType = item.GetType();

                        obj = Activator.CreateInstance(itemType, true);
                        break;
                    }
                    if (itemType == null)
                        return;

                    PropertyInfo[] ps = itemType.GetProperties();
                    foreach (var item in viewItems)
                    {

                        foreach (PropertyInfo property in ps)
                        {
                            if (!SupportSumRowProperties.Any(x => x == property.Name))
                            {
                                continue;
                            }

                            object tmpValue = property.GetValue(item, null);

                            object[] attributes = property.GetCustomAttributes(typeof(SumExAttribute), true);
                            if (attributes != null && attributes.Length > 0)
                            {
                                SumExAttribute attr = attributes[0] as SumExAttribute;
                                var propertyEx = ps.FirstOrDefault(x => x.Name.ToUpper() == attr.SumExName.ToUpper());
                                if (propertyEx != null)
                                {
                                    tmpValue = propertyEx.GetValue(item, null);
                                }
                            }

                            object totalValue = property.GetValue(obj, null);


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

                    totalRowItemSource.Add(obj);
                    if (TotalRow != null)
                    {
                        this.TotalRow.ItemsSource = totalRowItemSource;
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }



        #region 是否支持汇总行
        public static bool GetIsSupportSumRow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSupportSumRowProperty);
        }
        public static void SetIsSupportSumRow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSupportSumRowProperty, value);
        }
        public static readonly DependencyProperty IsSupportSumRowProperty = DependencyProperty.RegisterAttached("IsSupportSumRow", typeof(bool), typeof(DataGridExt), new UIPropertyMetadata(false, (d, e) =>
        {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion

        #region 汇总的列
        public static string GetSumPath(DependencyObject obj)
        {
            return (string)obj.GetValue(SumPathProperty);
        }
        public static void SetSumPath(DependencyObject obj, string value)
        {
            obj.SetValue(SumPathProperty, value);
        }
        public static readonly DependencyProperty SumPathProperty = DependencyProperty.RegisterAttached("SumPath", typeof(string), typeof(DataGridExt), new UIPropertyMetadata(null, (d, e) =>
        {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion

        #region 汇总的列数字格式
        public static string GetSumNumFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(SumNumFormatProperty);
        }
        public static void SetSumNumFormat(DependencyObject obj, string value)
        {
            obj.SetValue(SumNumFormatProperty, value);
        }
        public static readonly DependencyProperty SumNumFormatProperty = DependencyProperty.RegisterAttached("SumNumFormat", typeof(string), typeof(DataGridExt), new UIPropertyMetadata("N2", (d, e) =>
        {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion

        #endregion
    }

    public static class ReflectorUtil
    {
        public static object FollowPropertyPath(object value, string path)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (path == null) throw new ArgumentNullException("path");

            Type currentType = value.GetType();

            object obj = value;
            foreach (string propertyName in path.Split('.'))
            {
                if (currentType != null)
                {
                    PropertyInfo property = null;
                    int brackStart = propertyName.IndexOf("[");
                    int brackEnd = propertyName.IndexOf("]");

                    property = currentType.GetProperty(brackStart > 0 ? propertyName.Substring(0, brackStart) : propertyName);

                    if (property == null)
                    {
                        continue;
                    }

                    obj = property.GetValue(obj, null);

                    if (brackStart > 0)
                    {
                        string index = propertyName.Substring(brackStart + 1, brackEnd - brackStart - 1);
                        foreach (Type iType in obj.GetType().GetInterfaces())
                        {
                            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                            {
                                obj = typeof(ReflectorUtil).GetMethod("GetDictionaryElement")
                                                     .MakeGenericMethod(iType.GetGenericArguments())
                                                     .Invoke(null, new object[] { obj, index });
                                break;
                            }
                            if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                            {
                                obj = typeof(ReflectorUtil).GetMethod("GetListElement")
                                                     .MakeGenericMethod(iType.GetGenericArguments())
                                                     .Invoke(null, new object[] { obj, index });
                                break;
                            }
                        }
                    }

                    currentType = obj != null ? obj.GetType() : null; //property.PropertyType;
                }
                else return null;
            }
            return obj;
        }

        public static TValue GetDictionaryElement<TKey, TValue>(IDictionary<TKey, TValue> dict, object index)
        {
            TKey key = (TKey)Convert.ChangeType(index, typeof(TKey), null);
            return dict[key];
        }

        public static T GetListElement<T>(IList<T> list, object index)
        {
            return list[Convert.ToInt32(index)];
        }

    }

    /// <summary>
    /// 用于标识某实体中用指定只读属性做汇总计算使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SumExAttribute : Attribute
    {
        public string SumExName
        {
            get; set;
        }
    }

    /// <summary>
    /// 统计汇总行、列
    /// </summary>
    public class DataGridSumHelper
    {
        #region 是否支持汇总行
        public static bool GetIsSupportSumRow(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSupportSumRowProperty);
        }
        public static void SetIsSupportSumRow(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSupportSumRowProperty, value);
        }
        public static readonly DependencyProperty IsSupportSumRowProperty = DependencyProperty.RegisterAttached("IsSupportSumRow", typeof(bool), typeof(DataGridSumHelper), new UIPropertyMetadata(false, (d, e) => {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion

        #region 汇总的列
        public static string GetSumPath(DependencyObject obj)
        {
            return (string)obj.GetValue(SumPathProperty);
        }
        public static void SetSumPath(DependencyObject obj, string value)
        {
            obj.SetValue(SumPathProperty, value);
        }
        public static readonly DependencyProperty SumPathProperty = DependencyProperty.RegisterAttached("SumPath", typeof(string), typeof(DataGridSumHelper), new UIPropertyMetadata(null, (d, e) => {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion

        #region 汇总的列数字格式
        public static string GetSumNumFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(SumNumFormatProperty);
        }
        public static void SetSumNumFormat(DependencyObject obj, string value)
        {
            obj.SetValue(SumNumFormatProperty, value);
        }
        public static readonly DependencyProperty SumNumFormatProperty = DependencyProperty.RegisterAttached("SumNumFormat", typeof(string), typeof(DataGridSumHelper), new UIPropertyMetadata("N2", (d, e) => {
            if (DesignerHelper.IsInDesignMode)
                return;
        }));
        #endregion
    }

    /// <summary>
    /// 刷新间隔时间转换器
    /// </summary>
    public class RefreshIntervalConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "不刷新";
            int interval = 0;
            int.TryParse(value.ToString(), out interval);
            if (interval > 0)
            {
                return string.Format("{0}秒", interval);
            }
            return "不刷新";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            int interval = 0;
            int.TryParse(value.ToString().Replace("秒", ""), out interval);
            return interval;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new RefreshIntervalConverter();
        }
    }

    /// <summary>
    /// 数字转字符串
    /// </summary>
    public class ConverterAmountToString : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///Console.WriteLine("{0:C5}", i); // ￥123,456.00 
        ///Console.WriteLine("{0:D5}", i); // 123456 
        ///Console.WriteLine("{0:E5}", i); // 1.23456E+005 
        ///Console.WriteLine("{0:F5}", i); // 123456.00000 
        ///Console.WriteLine("{0:G5}", i); // 1.23456E5 
        ///Console.WriteLine("{0:N5}", i); // 123,456.00000 
        ///Console.WriteLine("{0:P5}", i); // 12,345,600.00000 % 
        ///Console.WriteLine("{0:X5}", i); // 1E240 
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //确保value与parameter为数字类型或者可以转换成数字类型
            if (value == null)
                return DependencyProperty.UnsetValue;

            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
                return value.ToString();

            //10000^N2
            if (parameter.ToString().Contains("^"))
            {
                string[] paras = parameter.ToString().Split('^');
                if (paras.Length == 2)
                {
                    double m;
                    if (double.TryParse(value.ToString(), out m) == false)
                    {
                        return value;
                    }
                    else
                    {
                        double unit = 1;
                        double.TryParse(paras[0], out unit);
                        return string.Format("{0:" + paras[1] + "}", m / unit); ;
                    }
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            else
            {
                //对数字类型进行转换
                return string.Format("{0:" + parameter.ToString() + "}", value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double tempInt;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out tempInt))
                {
                    return tempInt;
                }
                else
                {
                    return 0D;
                }
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ConverterAmountToString
            {
                FromType = this.FromType ?? typeof(double),
                TargetType = this.TargetType ?? typeof(string),
                Parameter = this.Parameter
            };
        }

        public object Parameter
        {
            get;
            set;
        }
        public Type TargetType
        {
            get;
            set;
        }
        public Type FromType
        {
            get;
            set;
        }
    }
}
