namespace AIStudio.Wpf.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public sealed class ItemClickEventArgs : RoutedEventArgs
    {
        public ItemClickEventArgs(object clickedObject)
        {
            this.ClickedItem = clickedObject;
        }

        public object ClickedItem
        {
            get; private set;
        }
    }

    public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);


    /// <summary>
    /// The HamburgerTreeMenu is based on a SplitView control. By default it contains a PART_HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerTreeMenu
    {
        /// <summary>
        /// Event raised when an item is clicked
        /// </summary>
        public event ItemClickEventHandler ItemClick;

        /// <summary>
        /// Event raised when an options' item is clicked
        /// </summary>
        public event ItemClickEventHandler OptionsItemClick;

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !IsPaneOpen;
        }

        private void ButtonsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            if (_optionsListView != null)
            {
                TomWrightsUtils.ClearTreeViewSelection(_optionsListView);
            }

            SelectedItem = _buttonsListView.SelectedItem;

            ItemClick?.Invoke(this, new ItemClickEventArgs(SelectedItem));

        }

        private void OptionsListView_ItemClick(object sender, MouseButtonEventArgs e)
        {
            if (_buttonsListView != null)
            {
                TomWrightsUtils.ClearTreeViewSelection(_buttonsListView);
            }

            OptionsItemClick?.Invoke(this, new ItemClickEventArgs(_optionsListView.SelectedItem));
        }

        /// <summary>
        /// 左侧菜单点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseUp(object sender, RoutedEventArgs e)
        {
            var menu = sender as Menu;
            MenuItem menuItem = null;

            if (e.Source is MenuItem)
            {
                menuItem = e.Source as MenuItem;
            }
            else
            {
                menuItem = GetItemFromChild(menu, e.OriginalSource as UIElement);
            }

            if (menuItem != null)
            {
                SelectedItem = menuItem.DataContext;
                if (sender == _menuListView)
                {
                    ItemClick?.Invoke(this, new ItemClickEventArgs(SelectedItem));
                }
                else
                {
                    OptionsItemClick?.Invoke(this, new ItemClickEventArgs(SelectedItem));
                }
            }
        }

        private static MenuItem GetItemFromChild(Menu treeView, UIElement child)
        {
            try
            {
                UIElement target = child;

                while ((target != null) && !(target is MenuItem))
                    target = System.Windows.Media.VisualTreeHelper.GetParent(target) as UIElement;

                return target as MenuItem;
            }
            catch
            {
                return null;
            }

        }

        private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetCurrentValue(ContentProperty, _buttonsListView?.SelectedItem ?? _optionsListView?.SelectedItem);
        }

    }


}
