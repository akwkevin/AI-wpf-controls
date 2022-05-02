using System;

namespace AIStudio.Wpf.Controls
{
    public interface IBusyBox
    {
        string WaitInfo
        {
            get; set;
        }
        double Percent
        {
            get; set;
        }
        Action CancelAction
        {
            get; set;
        }
    }
}
