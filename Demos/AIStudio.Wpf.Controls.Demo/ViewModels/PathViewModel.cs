using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using AIStudio.Wpf.Controls.Bindings;

namespace AIStudio.Wpf.Controls.Demo.ViewModels
{
    class PathViewModel : BindableBase
    {
        public PathViewModel()
        {
            _packIconKinds = PathIcon.DataIndex.Value.Select(p => new Tuple<string, string, Geometry>(p.Key.Item1, p.Key.Item2, p.Value)).ToList();
        }

        private readonly List<Tuple<string, string, Geometry>> _packIconKinds;
        private IEnumerable<Tuple<string, string, Geometry>> _kinds;
        public IEnumerable<Tuple<string, string, Geometry>> Kinds
        {
            get
            {
                return _kinds ?? (_kinds = _packIconKinds);
            }
            set
            {
                _kinds = value;
                RaisePropertyChanged("Kinds");
            }
        }

        private Tuple<string, string, Geometry> _selectedKind;
        public Tuple<string, string, Geometry> SelectedKind
        {
            get
            {
                return _selectedKind;
            }
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
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                RaisePropertyChanged("SearchText");

                Search(_searchText);
            }
        }


        private void OnSelectedKindChanged(Tuple<string, string, Geometry> selectedKind)
        {
            WindowBase.ShowMessageQueue($"<ac:PathIcon Kind=\"{selectedKind.Item2}\" /> copied 🎉", "RootWindow");

            try
            {
                System.Windows.Clipboard.SetDataObject($"<ac:PathIcon Kind=\"{selectedKind.Item2}\" />");
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
