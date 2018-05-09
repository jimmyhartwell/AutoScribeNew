using AutoScribeClient.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProtocolListPage : BasePage {
        
        private ProtocolListViewModel ViewModel { get; set; }

        public ProtocolListPage()
        {
            this.InitializeComponent();
            ViewModel = ProtocolListViewModel.GetProtocolListViewModel();
            Add_Command = new RelayCommand(() => Add());
            Refresh_Command = new RelayCommand(() => Refresh());
        }

        private void Add() {
            Frame.Navigate(typeof(NewProtocolPage), new ProtocolViewModel());
        }
        private void Refresh() {
            ViewModel.AddViewModels();
        }

        private void ProtocolTile_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Grid)sender).Children.Last().Fade(value: 1.0f).SetDurationForAll(500).Start();
        }

        private void ProtocolTile_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Grid)sender).Children.Last().Fade(value: 0.1f).SetDurationForAll(500).Start();
        }

        private void ViewProtocol_Click(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ProtocolPage), e.ClickedItem);
        }

        private void EditProtocol(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProtocolPage), ((Button)sender).DataContext);
            (Frame.Content as ProtocolPage).ToggleEditMode();
        }

        private async void DeleteProtocol(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog {
                Title = "Delete Protocol",
                Content = (((Button)sender).DataContext as ProtocolViewModel).Name + " will be deleted.",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel"
            };
            if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary) {
                await ProtocolListViewModel.GetProtocolListViewModel().DeleteProtocol(((Button)sender).DataContext as ProtocolViewModel);
            } else {
                deleteDialog.Hide();
            }
        }
    }
}
