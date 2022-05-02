using AIStudio.Wpf.GridControls.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace AIStudio.Wpf.GridControls.View
{
    /// <summary>
    /// Defile View control for IStringFilter model.
    /// </summary>
    [ModelView]
    [TemplatePart(Name = StringFilterView.PART_FilterType, Type = typeof(Selector))]
    public class StringFilterView : FilterViewBase<IStringFilter>, IFilterView
    {
        internal const string PART_FilterType = "PART_FilterType";
        static StringFilterView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StringFilterView),
                new FrameworkPropertyMetadata(typeof(StringFilterView)));

        }
        /// <summary>
        /// Instance of a selector allowing to choose the filtering mode
        /// </summary>
        private Selector _selectorFilterType;

        /// <summary>
        /// Create new instance of StringFilterView.
        /// </summary>
        public StringFilterView() : base() { }
        /// <summary>
        /// Create new instance of StringFilterView and accept IStringFilter model.
        /// </summary>
        /// <param name="model"></param>
        public StringFilterView(object model)
        {
            base.Model = model as IStringFilter;
        }

        /// <summary>
        /// Called when the control template is applied to this control
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _selectorFilterType = GetTemplateChild(PART_FilterType) as Selector;
            if (_selectorFilterType != null)
            {
                _selectorFilterType.ItemsSource = GetFilterModes();
            }
        }
        private IEnumerable<StringFilterMode> GetFilterModes()
        {
            foreach (var item in typeof(StringFilterMode).GetEnumValues())
            {
                yield return (StringFilterMode)item;
            }
        }
    }
}
