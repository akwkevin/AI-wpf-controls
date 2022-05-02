using System;
using AIStudio.Wpf.MDIContainer.Demo;

namespace AIStudio.Wpf.MDIContainer.Demo.Models
{


    public class Person : ViewModelBase
   {
      public event EventHandler Changed;

      public Person(string name, DateTime birthDate, string address)
      {
         this.Name = name;
         this.BirthDate = birthDate;
         this.Address = address;
      }

      private string _name = string.Empty;
      public string Name
      {
         get
         {
            return this._name;
         }
         set
         {
            this._name = value;
            this.RaisePropertyChanged("Name");
         }
      }

      private DateTime _birthDate;
      public DateTime BirthDate
      {
         get
         {
            return this._birthDate;
         }
         set
         {
            this._birthDate = value;
            this.RaisePropertyChanged("BirthDate");
         }
      }

      private string _address = string.Empty;
      public string Address
      {
         get
         {
            return this._address;
         }
         set
         {
            this._address = value;
            this.RaisePropertyChanged("Address");
         }
      }

      protected override void OnPropertyChanged(string propertyName)
      {
         var hander = this.Changed;
         if (hander != null)
         {
            hander(this, EventArgs.Empty);
         }
      }
   }
}
