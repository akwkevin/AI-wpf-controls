using System;
using System.Windows;
using AIStudio.Wpf.MDIContainer.Demo.Commands;
using AIStudio.Wpf.MDIContainer.Demo.Models;
using AIStudio.Wpf.MDIContainer.Demo.Interfaces;

namespace AIStudio.Wpf.MDIContainer.Demo.ViewModels
{
    public class PersonWindow : ViewModelBase, IContent
   {
      public string Title
      {
         get
         {
            return Person.Name;
         }
      }

      public event EventHandler Closing;

      public RelayCommand CloseCommand { get; private set; }      

      public Person Person { get; private set; }

      private bool IsDirty { get; set; }

      public PersonWindow(Person person)
      {
         this.Person = person;
         this.Person.Changed += (s, e) => this.IsDirty = true;

         this.CloseCommand = new RelayCommand(CloseWindow);
      }

      private void CloseWindow(object p)
      {
         if (this.CanClose)
         {
            var hander = this.Closing;
            if (hander != null)
            {
               hander(this, EventArgs.Empty);
            }
         }
      }      

      public bool CanClose
      {
         get 
         {
            return this.IsDirty == false || MessageBox.Show("Changes were made. Do you want to close this window?", "Close", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
         }
      }
   }
}
