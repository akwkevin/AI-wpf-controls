/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace AIStudio.Wpf.Controls
{
    public class RichTextBox : System.Windows.Controls.RichTextBox
    {
        #region Private Members

        private bool _preventDocumentUpdate;
        private bool _preventTextUpdate;

        #endregion //Private Members

        #region Constructors

        public RichTextBox()
        {
        }

        public RichTextBox(System.Windows.Documents.FlowDocument document)
          : base(document)
        {

        }

        #endregion //Constructors

        #region Properties

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RichTextBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextPropertyChanged, CoerceTextProperty, true, UpdateSourceTrigger.LostFocus));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBox)d).OnTextPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateDocumentFromText();
        }

        private static object CoerceTextProperty(DependencyObject d, object value)
        {
            return value ?? "";
        }

        #endregion //Text

        #endregion //Properties

        #region Methods

        protected override void OnTextChanged(System.Windows.Controls.TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            this.UpdateTextFromDocument();
        }

        private void UpdateTextFromDocument()
        {
            if (_preventTextUpdate)
                return;

            _preventDocumentUpdate = true;
            Text = new TextRange(Document.ContentStart, Document.ContentEnd).Text;
            _preventDocumentUpdate = false;
        }

        private void UpdateDocumentFromText()
        {
            if (_preventDocumentUpdate)
                return;

            _preventTextUpdate = true;
            new TextRange(Document.ContentStart, Document.ContentEnd).Text = Text;
            _preventTextUpdate = false;
        }

        /// <summary>
        /// Clears the content of the RichTextBox.
        /// </summary>
        public void Clear()
        {
            Document.Blocks.Clear();
        }

        public override void BeginInit()
        {
            base.BeginInit();
            // Do not update anything while initializing. See EndInit
            _preventTextUpdate = true;
            _preventDocumentUpdate = true;
        }

        public override void EndInit()
        {
            base.EndInit();
            _preventTextUpdate = false;
            _preventDocumentUpdate = false;
            // Possible conflict here if the user specifies 
            // the document AND the text at the same time 
            // in XAML. Text has priority.
            if (!string.IsNullOrEmpty(Text))
                UpdateDocumentFromText();
            else
                UpdateTextFromDocument();
        }

        #endregion //Methods
    }
}
