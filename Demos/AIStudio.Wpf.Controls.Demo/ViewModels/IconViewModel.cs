using System;
using System.Collections.Generic;
using System.Linq;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class IconViewModel : BindableBase
    {
        public IconViewModel()
        {
            _packIconKinds = Icon.DataIndex.Value.Select(p => new Tuple<string, string, string>(p.Key.Item1, p.Key.Item2, p.Value)).ToList();
        }

        private readonly List<Tuple<string, string, string>> _packIconKinds;
        private IEnumerable<Tuple<string, string, string>> _kinds;
        public IEnumerable<Tuple<string, string, string>> Kinds
        {
            get { return _kinds ?? (_kinds = _packIconKinds); }
            set
            {
                _kinds = value;
                RaisePropertyChanged("Kinds");
            }
        }

        private Tuple<string, string, string> _selectedKind;
        public Tuple<string, string, string> SelectedKind
        {
            get { return _selectedKind; }
            set
            {
                _selectedKind = value;
                RaisePropertyChanged("SelectedKind");

                OnSelectedKindChanged(_selectedKind);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");

                Search(_searchText);
            }
        }


        private void OnSelectedKindChanged(Tuple<string, string, string> selectedKind)
        {
            WindowBase.ShowMessageQueue($"<ac:Icon Kind=\"{selectedKind.Item2}\" /> copied 🎉", "RootWindow");

            try
            {
                System.Windows.Clipboard.SetDataObject($"<ac:Icon Kind=\"{selectedKind.Item2}\" />");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        private void Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                Kinds = _packIconKinds;
            else
                Kinds = _packIconKinds.Where(x => x.Item2.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

    }
}
