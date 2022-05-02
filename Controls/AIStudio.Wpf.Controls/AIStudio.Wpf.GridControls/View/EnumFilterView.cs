using System.Windows.Controls;

namespace AIStudio.Wpf.GridControls.View
{
    /// <summary>
    /// Defile View control for IMultiValueFilter model.
    /// </summary>
    [ModelView]
    public class EnumFilterView : MultiValueFilterView
    {
        // static EnumFilterView() {
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumFilterView),
        //        new FrameworkPropertyMetadata(typeof(EnumFilterView)));
        //}
        private bool isModelAttached;
        private ListBox _itemsCtrl;
        /// <summary>
        /// Create new instance of EnumFilterView;
        /// </summary>
        public EnumFilterView()
            : base()
        {

        }
        /// <summary>
        /// Create new instance of EnumFilterView and accept model.
        /// </summary>
        /// <param name="model">IMultiValueFilter model</param>
        public EnumFilterView(object model)
            : base(model)
        {

        }
    }
}
