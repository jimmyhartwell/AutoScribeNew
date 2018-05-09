using AutoScribeClient;
using AutoScribeClient.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SpeakerListPage : BasePage {
        public SpeakerListPage()
        {
            this.InitializeComponent();
            ViewModel = SpeakerListViewModel.GetSpeakerListViewModel();
            Add_Command = new RelayCommand(() => Add());
            Refresh_Command = new RelayCommand(() => Refresh());
        }

        private SpeakerListViewModel ViewModel { get; set; }

        private void Add() {
            Frame.Navigate(typeof(NewSpeakerPage), new SpeakerViewModel());
        }

        private void Refresh()
        {
            ViewModel.AddViewModels();
        }

        private void Delete() {

        }

        private void SpeakerTile_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((Grid)sender).Children.Last().Fade(value: 1.0f).SetDurationForAll(500).Start();
        }

        private void SpeakerTile_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((Grid)sender).Children.Last().Fade(value: 0.1f).SetDurationForAll(500).Start();
        }

        private void ViewSpeaker_Click(object sender, ItemClickEventArgs e)
        {
            base.Frame.Navigate(typeof(SpeakerPage), e.ClickedItem);
        }

        private void EditSpeaker(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SpeakerPage), ((Button)sender).DataContext);
            (Frame.Content as SpeakerPage).ToggleEditMode();
        }

        private async void DeleteSpeaker(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDialog = new ContentDialog {
                Title = "Delete Protocol",
                Content = (((Button)sender).DataContext as SpeakerViewModel).Name + " will be deleted.",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel"
            };
            if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary) {
                await SpeakerListViewModel.GetSpeakerListViewModel().DeleteSpeaker(((Button)sender).DataContext as SpeakerViewModel);
            } else {
                deleteDialog.Hide();
            }
        }
    }
}
