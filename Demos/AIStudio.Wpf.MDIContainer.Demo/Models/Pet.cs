using AIStudio.Wpf.MDIContainer.Demo;

namespace AIStudio.Wpf.MDIContainer.Demo.Models
{
   public class Pet : ViewModelBase
   {
      public Pet(string name, string owner)
      {
         this.Name = name;
         this.Owner = owner;
      }

      public string Name { get; set; }
      public string Owner { get; set; }
   }
}
