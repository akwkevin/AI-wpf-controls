namespace AIStudio.Wpf.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The HamburgerTreeMenu is based on a SplitView control. By default it contains a PART_HamburgerButton and a ListView to display menu items.
    /// </summary>
    [TemplatePart(Name = "PART_HamburgerButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ButtonsListView", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_OptionsListView", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_MenuListView", Type = typeof(Menu))]
    [TemplatePart(Name = "PART_OptionsMenuListView", Type = typeof(Menu))]
    public partial class HamburgerTreeMenu : ContentControl
    {
        private Button _hamburgerButton;
        private TreeView _buttonsListView;
        private TreeView _optionsListView;
        private Menu _menuListView;
        private Menu _optionsMenuListView;

        /// <summary>
        /// Initializes a new instance of the <see cref="HamburgerTreeMenu"/> class.
        /// </summary>
        public HamburgerTreeMenu()
        {
            //DefaultStyleKey = typeof(HamburgerTreeMenu);
        }

        static HamburgerTreeMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerTreeMenu), new FrameworkPropertyMetadata(typeof(HamburgerTreeMenu)));
        }

        /// <summary>
        /// Override default OnApplyTemplate to capture children controls
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_hamburgerButton != null)
            {
                _hamburgerButton.Click -= HamburgerButton_Click;
            }

            if (_buttonsListView != null)
            {
                _buttonsListView.MouseUp -= ButtonsListView_ItemClick;
            }

            if (_optionsListView != null)
            {
                _optionsListView.MouseUp -= OptionsListView_ItemClick;
            }

            if (_menuListView != null)
            {
                _menuListView.RemoveHandler(MenuItem.MouseUpEvent, new RoutedEventHandler(OnMouseUp));
            }

            if (_optionsMenuListView != null)
            {
                _optionsMenuListView.RemoveHandler(MenuItem.MouseUpEvent, new RoutedEventHandler(OnMouseUp));
            }

            _hamburgerButton = (Button)GetTemplateChild("PART_HamburgerButton");
            _buttonsListView = (TreeView)GetTemplateChild("PART_ButtonsListView");
            _optionsListView = (TreeView)GetTemplateChild("PART_OptionsListView");
            _menuListView = (Menu)GetTemplateChild("PART_MenuListView");
            _optionsMenuListView = (Menu)GetTemplateChild("PART_OptionsMenuListView");

            if (_hamburgerButton != null)
            {
                _hamburgerButton.Click += HamburgerButton_Click;
            }

            if (_buttonsListView != null)
            {
                _buttonsListView.MouseUp += ButtonsListView_ItemClick;
            }

            if (_optionsListView != null)
            {
                _optionsListView.MouseUp += OptionsListView_ItemClick;
            }

            if (_menuListView != null)
            {
                _menuListView.AddHandler(MenuItem.MouseUpEvent, new RoutedEventHandler(OnMouseUp), true);
            }

            if (_optionsMenuListView != null)
            {
                _optionsMenuListView.AddHandler(MenuItem.MouseUpEvent, new RoutedEventHandler(OnMouseUp), true);
            }

            this.Loaded -= HamburgerMenu_Loaded;
            this.Loaded += HamburgerMenu_Loaded;

            base.OnApplyTemplate();
        }
    }

    public class SimpleCommand2 : ICommand
    {
        public Predicate<object> CanExecuteDelegate
        {
            get; set;
        }
        public Action<object> ExecuteDelegate
        {
            get; set;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true; // if there is no can execute default to true
        }

        public event EventHandler CanExecuteChanged
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

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }


    public class TomWrightsUtils
    {
        public static void ClearTreeViewSelection(TreeView tv)
        {
            if (tv != null)
                ClearTreeViewItemsControlSelection(tv.Items, tv.ItemContainerGenerator);
        }
        private static void ClearTreeViewItemsControlSelection(ItemCollection ic, ItemContainerGenerator icg)
        {
            if ((ic != null) && (icg != null))
                for (int i = 0; i < ic.Count; i++)
                {
                    TreeViewItem tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (tvi != null)
                    {
                        ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
                        tvi.IsSelected = false;
                    }
                }
        }

        public static void ClearTreeViewExpander(TreeView tv)
        {
            if (tv != null)
                ClearTreeViewItemsControlExpander(tv.Items, tv.ItemContainerGenerator);
        }
        private static void ClearTreeViewItemsControlExpander(ItemCollection ic, ItemContainerGenerator icg)
        {
            if ((ic != null) && (icg != null))
                for (int i = 0; i < ic.Count; i++)
                {
                    TreeViewItem tvi = icg.ContainerFromIndex(i) as TreeViewItem;
                    if (tvi != null)
                    {
                        ClearTreeViewItemsControlExpander(tvi.Items, tvi.ItemContainerGenerator);
                        tvi.IsExpanded = false;
                    }
                }
        }
    }


}
