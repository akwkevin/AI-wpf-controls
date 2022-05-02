using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class TreeDataGridExpander : UserControl
    {
        static TreeDataGridExpander()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridExpander), new FrameworkPropertyMetadata(typeof(TreeDataGridExpander)));
        }

        public TreeDataGridExpander()
        {

        }
    }
}
