using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// A base class for all view models used in this project.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Represent change of a property in this ViewModel.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Alert to the elements which use this property.
        /// </summary>
        /// <param name="propertyName">Name of used property.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual bool HasError { get; set; }
        public virtual string Error { get; set; }

        
    }
}
