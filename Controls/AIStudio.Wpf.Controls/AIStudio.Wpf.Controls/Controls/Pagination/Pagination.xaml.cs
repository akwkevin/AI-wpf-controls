using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{

    public class Pagination : Control
    {
        static Pagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pagination), new FrameworkPropertyMetadata(typeof(Pagination)));
        }

        public Pagination()
        {
            UpdatePaginationItems();
            TotalText = $"总数:{Total}";
        }

        private readonly static ObservableCollection<int> DefaultPageSizeSource = new ObservableCollection<int>(new int[] { 10, 50, 100, 500, 1000 });
        #region Routed Event
        public static readonly RoutedEvent CurrentIndexChangedEvent = EventManager.RegisterRoutedEvent("CurrentIndexChanged", RoutingStrategy.Bubble, typeof(CurrentIndexChangedEventHandler), typeof(Pagination));
        public event CurrentIndexChangedEventHandler CurrentIndexChanged
        {
            add
            {
                AddHandler(CurrentIndexChangedEvent, value);
            }
            remove
            {
                RemoveHandler(CurrentIndexChangedEvent, value);
            }
        }
        void RaiseCurrentIndexChanged(int index)
        {
            var arg = new CurrentIndexChangedEventArgs(index, CurrentIndexChangedEvent);
            RaiseEvent(arg);
        }
        #endregion

        #region Property
        /// <summary>
        /// Current index.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return (int)GetValue(CurrentIndexProperty);
            }
            set
            {
                SetValue(CurrentIndexProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register(nameof(CurrentIndex), typeof(int), typeof(Pagination), new PropertyMetadata(1, OnCurrentIndexChanged));


        /// <summary>
        /// Total index.
        /// </summary>
        public int TotalIndex
        {
            get
            {
                return (int)GetValue(TotalIndexProperty);
            }
            set
            {
                SetValue(TotalIndexProperty, value);
            }
        }

        public static readonly DependencyProperty TotalIndexProperty =
            DependencyProperty.Register(nameof(TotalIndex), typeof(int), typeof(Pagination), new PropertyMetadata(1));

        /// <summary>
        ///PageSize.
        /// </summary>
        public int PageSize
        {
            get
            {
                return (int)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(Pagination),
                new FrameworkPropertyMetadata(100, OnPageSizeChanged));

        /// <summary>
        ///PageSize.
        /// </summary>
        public ObservableCollection<int> PageSizeSource
        {
            get
            {
                return (ObservableCollection<int>)GetValue(PageSizeSourceProperty);
            }
            set
            {
                SetValue(PageSizeSourceProperty, value);
            }
        }

        public static readonly DependencyProperty PageSizeSourceProperty =
            DependencyProperty.Register(nameof(PageSizeSource), typeof(ObservableCollection<int>), typeof(Pagination), new PropertyMetadata(DefaultPageSizeSource, OnPageSizeSourceChanged));


        /// <summary>
        /// Theme brush.
        /// </summary>
        public Brush HoverBrush
        {
            get
            {
                return (Brush)GetValue(HoverBrushProperty);
            }
            set
            {
                SetValue(HoverBrushProperty, value);
            }
        }

        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register(nameof(HoverBrush), typeof(Brush), typeof(Pagination), new PropertyMetadata(default(Brush)));


        /// <summary>
        /// Theme brush.
        /// </summary>
        public Brush SelectedBrush
        {
            get
            {
                return (Brush)GetValue(SelectedBrushProperty);
            }
            set
            {
                SetValue(SelectedBrushProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register(nameof(SelectedBrush), typeof(Brush), typeof(Pagination), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Pagination style.
        /// </summary>
        public ControlStyle ControlStyle
        {
            get
            {
                return (ControlStyle)GetValue(ControlStyleProperty);
            }
            set
            {
                SetValue(ControlStyleProperty, value);
            }
        }

        public static readonly DependencyProperty ControlStyleProperty =
            DependencyProperty.Register(nameof(ControlStyle), typeof(ControlStyle), typeof(Pagination), new PropertyMetadata(ControlStyle.Standard));

        /// <summary>
        /// Spacing
        /// </summary>
        public double Spacing
        {
            get
            {
                return (double)GetValue(SpacingProperty);
            }
            set
            {
                SetValue(SpacingProperty, value);
            }
        }

        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(Pagination), new PropertyMetadata(default(double)));


        /// <summary>
        /// CornerRadius
        /// </summary>
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

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Pagination), new PropertyMetadata(default(CornerRadius)));

        /// <summary>
        /// Total
        /// </summary>
        public int Total
        {
            get
            {
                return (int)GetValue(TotalProperty);
            }
            set
            {
                SetValue(TotalProperty, value);
            }
        }

        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register(nameof(Total), typeof(int), typeof(Pagination), new PropertyMetadata(0, OnTotalChanged));

        /// <summary>
        /// TotalText
        /// </summary>
        public string TotalText
        {
            get
            {
                return (string)GetValue(TotalTextProperty);
            }
            set
            {
                SetValue(TotalTextProperty, value);
            }
        }

        public static readonly DependencyProperty TotalTextProperty =
            DependencyProperty.Register(nameof(TotalText), typeof(string), typeof(Pagination), new PropertyMetadata(default(string)));
        #endregion

        #region Internal Property
        internal ObservableCollection<PaginationItem> PaginationItems
        {
            get
            {
                return (ObservableCollection<PaginationItem>)GetValue(PaginationItemsProperty);
            }
            set
            {
                SetValue(PaginationItemsProperty, value);
            }
        }

        internal static readonly DependencyProperty PaginationItemsProperty =
            DependencyProperty.Register("PaginationItems", typeof(ObservableCollection<PaginationItem>), typeof(Pagination));

        internal ICommand PreviousCommand
        {
            get
            {
                return (ICommand)GetValue(PreviousCommandProperty);
            }
            set
            {
                SetValue(PreviousCommandProperty, value);
            }
        }

        internal static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(Pagination), new PropertyMetadata(new PreviousCommand()));



        internal ICommand NextCommand
        {
            get
            {
                return (ICommand)GetValue(NextCommandProperty);
            }
            set
            {
                SetValue(NextCommandProperty, value);
            }
        }

        internal static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(Pagination), new PropertyMetadata(new NextCommand()));



        internal ICommand IndexCommand
        {
            get
            {
                return (ICommand)GetValue(IndexCommandProperty);
            }
            set
            {
                SetValue(IndexCommandProperty, value);
            }
        }

        internal static readonly DependencyProperty IndexCommandProperty =
            DependencyProperty.Register("IndexCommand", typeof(ICommand), typeof(Pagination), new PropertyMetadata(new IndexCommand()));


        #endregion

        #region OnCurrentIndexChanged
        private static void OnCurrentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pagination = d as Pagination;

            if (pagination.CurrentIndex > pagination.TotalIndex)
            {
                pagination.CurrentIndex = Math.Max(1, pagination.TotalIndex);
                return;
            }
            else if (pagination.CurrentIndex < 1)
            {
                pagination.CurrentIndex = 1;
            }

            pagination.UpdatePaginationItems();
            pagination.RaiseCurrentIndexChanged(pagination.CurrentIndex);
        }

        private static void OnTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pagination = d as Pagination;

            pagination.TotalIndex = pagination.SetTotalIndex();

            if (pagination.CurrentIndex > pagination.TotalIndex)
            {
                pagination.CurrentIndex = pagination.TotalIndex;
            }

            pagination.UpdatePaginationItems();
            pagination.TotalText = $"总数:{pagination.Total}";
        }

        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pagination = d as Pagination;

            pagination.TotalIndex = pagination.SetTotalIndex();

            if (pagination.CurrentIndex > pagination.TotalIndex)
            {
                pagination.CurrentIndex = pagination.TotalIndex;
            }

            pagination.UpdatePaginationItems();
            pagination.RaiseCurrentIndexChanged(pagination.CurrentIndex);

        }

        private static void OnPageSizeSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pagination = d as Pagination;

            if (!pagination.PageSizeSource.Any(p => p == pagination.PageSize))
            {
                pagination.PageSize = pagination.PageSizeSource.FirstOrDefault();
                pagination.RaiseCurrentIndexChanged(pagination.CurrentIndex);
            }
        }

        private int SetTotalIndex()
        {
            if (PageSize == 0 || Total == 0)
                return 1;
            int pages = Total / PageSize;
            pages = Total % PageSize == 0 ? pages : pages + 1;
            return pages;

        }
        #endregion

        #region Function
        private void UpdatePaginationItems()
        {
            if (PaginationItems == null)
                PaginationItems = new ObservableCollection<PaginationItem>();

            PaginationItems.Clear();

            if (TotalIndex <= 7)
            {
                for (var i = 1; i <= TotalIndex; i++)
                {
                    PaginationItems.Add(new PaginationItem(i, CurrentIndex == i));
                }
            }
            else
            {
                PaginationItems.Add(new PaginationItem(1, CurrentIndex == 1));
                PaginationItems.Add(new PaginationItem(2, CurrentIndex == 2));


                if (CurrentIndex == 1 || CurrentIndex == 2 || CurrentIndex == 3 || CurrentIndex == 4)
                {
                    PaginationItems.Add(new PaginationItem(3, CurrentIndex == 3));
                    PaginationItems.Add(new PaginationItem(4, CurrentIndex == 4));
                    PaginationItems.Add(new PaginationItem(5, CurrentIndex == 5));
                }

                PaginationItems.Add(new PaginationItem(null));

                if (CurrentIndex >= TotalIndex - 3)
                {
                    PaginationItems.Add(new PaginationItem(null));

                    for (var i = TotalIndex - 4; i <= TotalIndex; i++)
                    {
                        PaginationItems.Add(new PaginationItem(i, CurrentIndex == i));
                    }
                    return;
                }
                if (CurrentIndex != 1 && CurrentIndex != 2 && CurrentIndex != 3 && CurrentIndex != 4)
                {
                    for (var i = CurrentIndex - 1; i <= (CurrentIndex + 1); i++)
                    {
                        PaginationItems.Add(new PaginationItem(i, CurrentIndex == i));
                    }
                }
                PaginationItems.Add(new PaginationItem(null));
                for (var i = TotalIndex - 1; i <= TotalIndex; i++)
                {
                    PaginationItems.Add(new PaginationItem(i, CurrentIndex == i));
                }
            }
        }
        #endregion
    }

    public class CurrentIndexChangedEventArgs : RoutedEventArgs
    {
        public CurrentIndexChangedEventArgs(int currentIndex, RoutedEvent routedEvent) : base(routedEvent)
        {
            CurrentIndex = currentIndex;
        }

        public int CurrentIndex
        {
            get; set;
        }
    }

    public delegate void CurrentIndexChangedEventHandler(object sender, CurrentIndexChangedEventArgs e);

    internal class PaginationItem
    {
        public PaginationItem(int? value)
        {
            Value = value;
        }

        public PaginationItem(int? value, bool isSelected)
        {
            Value = value;
            IsSelected = isSelected;
        }

        public int? Value
        {
            get; set;
        }

        public bool IsSelected
        {
            get; set;
        }
    }

    internal class PreviousCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var pagination = (parameter as Pagination);

            if (pagination.CurrentIndex - 1 < 0)
                return;

            pagination.CurrentIndex--;
        }
    }

    internal class NextCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var pagination = (parameter as Pagination);

            if (pagination.CurrentIndex + 1 > pagination.TotalIndex)
                return;

            pagination.CurrentIndex++;
        }
    }

    internal class IndexCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var objs = parameter as object[];

            var pagination = objs[0] as Pagination;
            var index = (int)objs[1];


            pagination.CurrentIndex = index;
        }
    }
}
