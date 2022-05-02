using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;

namespace AIStudio.Wpf.PrintDialog.UserControls
{
    internal class NoKeypadDocumentViewer : DocumentViewer
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }
    }
}
