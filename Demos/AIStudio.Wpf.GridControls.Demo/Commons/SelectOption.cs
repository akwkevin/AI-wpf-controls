using System;
using System.Collections.Generic;
using System.Text;

namespace AIStudio.Wpf.GridControls.Demo.Commons
{
    public class SelectOption
    {
        public SelectOption(string value, string text)
        {
            Value = value;
            Text = text;
        }
        public string Value
        {
            get; set;
        }
        public string Text
        {
            get; set;
        }

        public override string ToString()
        {
            return $"{Value}-{Text}";
        }
    }
}
