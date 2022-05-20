using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.GridControls
{
    /// <summary>
    /// DragDropListBox.xaml 的交互逻辑
    /// </summary>
    public partial class DragDropListBox : UserControl
    {
        /// <summary>
        /// 不可操作的数据源
        /// </summary>
        public ObservableCollection<DragDropListViewData> ItemsSource
        {
            get
            {
                return (ObservableCollection<DragDropListViewData>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// 不可操作的数据源
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<DragDropListViewData>), typeof(DragDropListBox), new PropertyMetadata(new ObservableCollection<DragDropListViewData>()));

        /// <summary>
        /// 可操作的数据源
        /// </summary>
        public ObservableCollection<DragDropListViewData> OperationItemsSource
        {
            get
            {
                return (ObservableCollection<DragDropListViewData>)GetValue(OperationItemsSourceProperty);
            }
            set
            {
                SetValue(OperationItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// 可操作的数据源
        /// </summary>
        public static readonly DependencyProperty OperationItemsSourceProperty =
            DependencyProperty.Register("OperationItemsSource", typeof(ObservableCollection<DragDropListViewData>), typeof(DragDropListBox), new PropertyMetadata(new ObservableCollection<DragDropListViewData>()));

        /// <summary>
        /// 不可排序的列表是否显示
        /// </summary>
        public Visibility DisabledListViewVisibility
        {
            get
            {
                return (Visibility)GetValue(DisabledListViewVisibilityProperty);
            }
            set
            {
                SetValue(DisabledListViewVisibilityProperty, value);
            }
        }
        /// <summary>
        /// 不可排序的列表是否显示
        /// </summary>
        public static readonly DependencyProperty DisabledListViewVisibilityProperty =
            DependencyProperty.Register("DisabledListViewVisibility", typeof(Visibility), typeof(DragDropListBox), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 列宽
        /// </summary>
        public double ColumnWidth
        {
            get
            {
                return (double)GetValue(ColumnWidthProperty);
            }
            set
            {
                SetValue(ColumnWidthProperty, value);
            }
        }
        /// <summary>
        /// 列宽
        /// </summary>
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(DragDropListBox), new PropertyMetadata(100.0));


        private ListBoxDragDropManager<DragDropListViewData> dragMgr;

        private IEnumerable<DragDropListViewData> _orignalDataSource;

        public DragDropListBox()
        {
            InitializeComponent();

            this.dragMgr = new ListBoxDragDropManager<DragDropListViewData>(this.listView);
            this.dragMgr.ProcessDropBeforeMove += OnProcessDropBeforeMove;

            this.DataContext = this;
        }

        private void OnProcessDropBeforeMove(object sender, ProcessDropBeforeMoveEventArgs e)
        {
            var old = OperationItemsSource[e.OldIndex];
            var @new = OperationItemsSource[e.NewIndex];
            var oldDisplayIndex = old.DisplayIndex;
            var newDisplayIndex = @new.DisplayIndex;
            OperationItemsSource[e.OldIndex].DisplayIndex = newDisplayIndex;
            if (e.OldIndex > e.NewIndex)
            {
                for (int i = e.NewIndex; i < e.OldIndex; i++)
                {
                    OperationItemsSource[i].DisplayIndex++;
                }
            }
            else//等于不会进该事件
            {
                for (int i = e.OldIndex + 1; i < e.NewIndex + 1; i++)
                {
                    OperationItemsSource[i].DisplayIndex--;
                }
            }
        }

        public void SetOrignalDataSource(IEnumerable<DragDropListViewData> dataSource)
        {
            _orignalDataSource = dataSource;
            var maxWidth = _orignalDataSource.Select(x => x.DisplayName.Length).Max() * 18;
            ColumnWidth = maxWidth > 100 ? maxWidth : 100;
            ItemsSource = new ObservableCollection<DragDropListViewData>(_orignalDataSource.Where(d => !d.IsEnabled));
            OperationItemsSource = new ObservableCollection<DragDropListViewData>(_orignalDataSource.Where(d => d.IsEnabled));
            DisabledListViewVisibility = ItemsSource.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public IEnumerable<DragDropListViewData> GetResult()
        {
            return ItemsSource.Concat(OperationItemsSource);
        }
    }
}
