using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public static class DataGridAttach
    {
        private static readonly Thickness DefaultCellPaddingThickness = new Thickness(2);
        private static readonly Thickness DefaultColumnHeaderPadding = new Thickness(4, 2, 4, 2);
        private static readonly Thickness DefaultListViewItemPadding = new Thickness(2);

        public static readonly DependencyProperty SelectionUnitProperty = DependencyProperty.RegisterAttached("SelectionUnit", typeof(DataGridSelectionUnit), typeof(DataGridAttach), new FrameworkPropertyMetadata(DataGridSelectionUnit.FullRow));

        /// <summary>
        /// Gets the value to define the DataGridRow selection behavior.
        /// </summary>
        [Category("AIStudio.Wpf.Controls")]
        [AttachedPropertyBrowsableForType(typeof(DataGridRow))]
        public static DataGridSelectionUnit GetSelectionUnit(UIElement element)
        {
            return (DataGridSelectionUnit)element.GetValue(SelectionUnitProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridRow selection behavior.
        /// </summary>
        public static void SetSelectionUnit(UIElement element, DataGridSelectionUnit value)
        {
            element.SetValue(SelectionUnitProperty, value);
        }

        public static readonly DependencyProperty IsCellOrRowHeaderProperty
         = DependencyProperty.RegisterAttached("IsCellOrRowHeader",
                                               typeof(bool),
                                               typeof(DataGridAttach),
                                               new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets the value to define the DataGridCell selection behavior.
        /// </summary>
        [Category("AIStudio.Wpf.Controls")]
        [AttachedPropertyBrowsableForType(typeof(DataGridCell))]
        public static bool GetIsCellOrRowHeader(UIElement element)
        {
            return (bool)element.GetValue(IsCellOrRowHeaderProperty);
        }

        /// <summary>
        /// Sets the value to define the DataGridCell selection behavior.
        /// </summary>
        internal static void SetIsCellOrRowHeader(UIElement element, bool value)
        {
            element.SetValue(IsCellOrRowHeaderProperty, value);
        }


        #region AttachedProperty : ColumnHeaderPaddingProperty
        public static readonly DependencyProperty ColumnHeaderPaddingProperty
            = DependencyProperty.RegisterAttached("ColumnHeaderPadding", typeof(Thickness), typeof(DataGridAttach),
                new FrameworkPropertyMetadata(DefaultColumnHeaderPadding, FrameworkPropertyMetadataOptions.Inherits));

        public static Thickness GetColumnHeaderPadding(DataGrid element)
            => (Thickness)element.GetValue(ColumnHeaderPaddingProperty);
        public static void SetColumnHeaderPadding(DependencyObject element, Thickness value)
            => element.SetValue(ColumnHeaderPaddingProperty, value);
        #endregion

        public static readonly DependencyProperty ListViewItemPaddingProperty = DependencyProperty.RegisterAttached(
            "ListViewItemPadding",
            typeof(Thickness),
            typeof(DataGridAttach),
            new FrameworkPropertyMetadata(DefaultListViewItemPadding, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetListViewItemPadding(DependencyObject element, Thickness value)
        {
            element.SetValue(ListViewItemPaddingProperty, value);
        }

        public static Thickness GetListViewItemPadding(DependencyObject element)
        {
            return (Thickness)element.GetValue(ListViewItemPaddingProperty);
        }
    }
}
