using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AIStudio.Wpf.Controls
{
    public class IconTextBlock : ContentControl
    {
        static IconTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconTextBlock), new FrameworkPropertyMetadata(typeof(IconTextBlock)));
        }
    }
}
