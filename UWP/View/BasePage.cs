using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace UWP.View
{
    public class BasePage : Page, INotifyPropertyChanged
    {
        public virtual string Header { get; set; }
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

        public ICommand Add_Command { get; set; }
        public ICommand Search_Command { get; set; }
        public ICommand Refresh_Command { get; set; }
        public ICommand Info_Command { get; set; }
        public ICommand Edit_Command { get; set; }
        public ICommand Cancel_Command { get; set; }
        public ICommand Save_Command { get; set; }
        public ICommand Delete_Command { get; set; }
        public ICommand StartRecording_Command { get; set; }
        public ICommand StopRecording_Command { get; set; }
        public ICommand SaveAudio_Command { get; set; }
        public ICommand CancelAudio_Command { get; set; }
    }
}
