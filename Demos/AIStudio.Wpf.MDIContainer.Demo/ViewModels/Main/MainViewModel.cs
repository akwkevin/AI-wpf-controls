using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using AIStudio.Wpf.MDIContainer.Demo.Commands;
using AIStudio.Wpf.MDIContainer.Demo.Models;
using AIStudio.Wpf.MDIContainer.Demo.Interfaces;
using AIStudio.Wpf.MDIContainer.Enums;
using AIStudio.Wpf.MDIContainer.Extensions;

namespace AIStudio.Wpf.MDIContainer.Demo.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IContent> Items
        {
            get; private set;
        }

        public ObservableCollection<Person> People
        {
            get; private set;
        }

        public ObservableCollection<Pet> Pets
        {
            get; private set;
        }

        public RelayCommand ShowCommand
        {
            get; private set;
        }
        public RelayCommand ShowPetCommand
        {
            get; private set;
        }
        public RelayCommand CascadeCommand
        {
            get; private set;
        }
        public RelayCommand TileHorizontalCommand
        {
            get; private set;
        }
        public RelayCommand TileVerticalCommand
        {
            get; private set;
        }

        private IContent _selectedWindow = null;
        public IContent SelectedWindow
        {
            get
            {
                return this._selectedWindow;
            }
            set
            {
                this._selectedWindow = value;
                this.RaisePropertyChanged("SelectedWindow");
            }
        }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<IContent>();
            this.People = new ObservableCollection<Person>();
            this.Pets = new ObservableCollection<Pet>();

            this.ShowCommand = new RelayCommand(Show, p => p != null);
            this.CascadeCommand = new RelayCommand(Cascade);
            this.TileHorizontalCommand = new RelayCommand(TileHorizontal);
            this.TileVerticalCommand = new RelayCommand(TileVertical);

            this.People.Add(new Person("John Texas", new System.DateTime(1978, 12, 3), "NYC"));
            this.People.Add(new Person("Margareth Smith", new System.DateTime(1996, 4, 2), "Dallas"));
            this.People.Add(new Person("Jenny Happyday", new System.DateTime(1991, 5, 5), "TX"));
            this.People.Add(new Person("William Box", new System.DateTime(1966, 7, 3), "CA"));

            this.Pets.Add(new Pet("Rex", "Aunt Mary"));
            this.Pets.Add(new Pet("Rusty", "Oncle Bill"));
        }

        private void TileHorizontal(object obj)
        {
            var container = obj as MDIContainer;
            if (container != null)
            {
                container.Rank(MdiLayout.TileHorizontal);
            }
        }

        private void TileVertical(object obj)
        {
            var container = obj as MDIContainer;
            if (container != null)
            {
                container.Rank(MdiLayout.TileVertical);
            }
        }

        private void Cascade(object obj)
        {
            var container = obj as MDIContainer;
            if (container != null)
            {
                container.Rank(MdiLayout.Cascade);
            }
        }

        private void Show(object p)
        {
            if (p is Person person)
            {
                var item = new PersonWindow(person);
                item.Closing += (s, e) => this.Items.Remove(item);
                this.Items.Add(item);
            }
            else if (p is Pet pet)
            {
                var item = new PetWindow(pet);
                this.Items.Add(item);
            }
        }
    }
}
