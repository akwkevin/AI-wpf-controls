using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AIStudio.Wpf.GridControls.ViewModel
{
    /// <summary>
    /// Provide INotifyPropertyChanged implementation for derived class.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {

        #region Члены INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            var myType = this.GetType();

#if NETFX_CORE
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetTypeInfo().GetDeclaredProperty(propertyName) == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
#else
            if (!string.IsNullOrEmpty(propertyName)
                && myType.GetProperty(propertyName) == null)
            {
#if !SILVERLIGHT
                var descriptor = this as ICustomTypeDescriptor;

                if (descriptor != null)
                {
                    if (descriptor.GetProperties()
                        .Cast<PropertyDescriptor>()
                        .Any(property => property.Name == propertyName))
                    {
                        return;
                    }
                }
#endif

                throw new ArgumentException("Property not found", propertyName);
            }
#endif
        }
        #endregion

    }
}
