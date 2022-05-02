using AIStudio.Wpf.MDIContainer.Demo;
using AIStudio.Wpf.MDIContainer.Demo.Models;
using AIStudio.Wpf.MDIContainer.Demo.Interfaces;

namespace AIStudio.Wpf.MDIContainer.Demo.ViewModels
{
    public class PetWindow : ViewModelBase, IContent
   {
      public string Title
      {
         get { return string.Format("{0} - {1}", Pet.Name, Pet.Owner); }
      }

      public PetWindow(Pet pet)
      {
         this.Pet = pet;
      }

      public Pet Pet { get; private set; }

      public bool CanClose
      {
         get { return true; }
      }
   }
}
