using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AIStudio.Wpf.MDIContainer.Demo
{
   public abstract class ViewModelBase : INotifyPropertyChanged
   {
      public event PropertyChangedEventHandler PropertyChanged;

      protected void RaisePropertyChanged(string propertyName)
      {
         var handler = this.PropertyChanged;
         if (handler != null)
         {
            handler(this, new PropertyChangedEventArgs(propertyName));
         }

         this.OnPropertyChanged(propertyName);
      }

      protected virtual void OnPropertyChanged(string propertyName)
      { }
   }
}
