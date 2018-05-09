using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP.Helpers;
using UWP.View;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public MainPage()
        {
            this.InitializeComponent();
            // Hide default title bar.
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            ApplicationViewTitleBar appTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
            appTitleBar.ButtonBackgroundColor = appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            //Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            // Set default header.
            NavView.HeaderTemplate = Application.Current.Resources["DefaultNavigationViewHeader"] as DataTemplate;

            ContentFrame.DataContextChanged += SetNavViewHeaderDataContext;

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => On_BackRequested();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible) {
                AppTitleBar.Visibility = Visibility.Visible;
            } else {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void NavView_Loading(FrameworkElement sender, object args)
        {
            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavView.MenuItems) {
                if (item is NavigationViewItem && item.Tag.ToString() == "home") {
                    NavView.SelectedItem = item;
                    break;
                }
            }

        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {            
            ContentFrame.Navigated += On_Navigated;
            // add keyboard accelerators for backwards navigation
            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;

        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked) {
                ContentFrame.Navigate(typeof(SettingsPage));
            } else {
                // find NavigationViewItem with Content that equals InvokedItem
                var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag) {
                case "home":
                    ContentFrame.Navigate(typeof(WelcomePage));
                    break;
                case "protocols":
                    ContentFrame.Navigate(typeof(ProtocolListPage));
                    break;
                case "speakers":
                    ContentFrame.Navigate(typeof(SpeakerListPage));
                    break;
            }
        }

        //private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        //{
        //    On_BackRequested();
        //}

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private bool On_BackRequested()
        {
            bool navigated = false;

            // don't go back if the nav pane is overlayed
            if (NavView.IsPaneOpen && (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal)) {
                return false;
            } else {
                if (ContentFrame.CanGoBack) {
                    ContentFrame.GoBack();
                    LoadHeader();
                    navigated = true;
                }
            }
            return navigated;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            if (ContentFrame.SourcePageType == typeof(SettingsPage)) {
                NavView.SelectedItem = NavView.SettingsItem as NavigationViewItem;
            } else {
                Dictionary<Type, string> lookup = new Dictionary<Type, string>()
                {
                    {typeof(WelcomePage), "home"},
                    {typeof(ProtocolListPage), "protocols"},
                    {typeof(SpeakerListPage), "speakers"},
                    {typeof(ProtocolPage), "protocol"},
                    {typeof(SpeakerPage), "speaker" },
                    {typeof(NewProtocolPage), "newprotocol" },
                    {typeof(NewSpeakerPage), "newspeaker" },
                    {typeof(SettingsPage), "settings" }
                };

                String stringTag = lookup[ContentFrame.SourcePageType];
                if (stringTag == "protocol" || stringTag == "speaker" || stringTag == "newprotocol" || stringTag == "newspeaker" || stringTag == "settings") {
                    //NavView.IsBackEnabled = true;
                    //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                } else {
                    //NavView.IsBackEnabled = false;
                    //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                LoadHeader();
                
                // set the new SelectedItem  
                foreach (NavigationViewItemBase item in NavView.MenuItems) {
                    if (item is NavigationViewItem && item.Tag.Equals(stringTag)) {
                        item.IsSelected = true;
                        break;
                    }
                }
            }
        }

        private void SetNavViewHeaderDataContext(object sender, DataContextChangedEventArgs e) {
            ((Grid)(NavView.HeaderTemplate.LoadContent())).DataContext = ContentFrame.Content;
        }

        private void OnBackgroundImageOpened(object sender, RoutedEventArgs e) =>
            BackgroundImage.Visibility = Visibility.Visible;

        private string GetMenuItemName(object selectedItem) {
            if (selectedItem is NavigationViewItem navViewItem) {
                string content = ((NavigationViewItem)selectedItem).Content as string;
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(content.ToLower());
            }
            return string.Empty;
        }

        private void LoadHeader() {
            if (ContentFrame.SourcePageType == typeof(ProtocolListPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["ProtocolListPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            if (ContentFrame.SourcePageType == typeof(SpeakerListPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["SpeakerListPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            if (ContentFrame.SourcePageType == typeof(ProtocolPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["ProtocolPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            if (ContentFrame.SourcePageType == typeof(SpeakerPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["SpeakerPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            if (ContentFrame.SourcePageType == typeof(NewProtocolPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["NewProtocolPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            if (ContentFrame.SourcePageType == typeof(NewSpeakerPage)) {
                NavView.HeaderTemplate = Application.Current.Resources["NewSpeakerPageNavigationViewHeader"] as DataTemplate;
                return;
            }
            NavView.HeaderTemplate = Application.Current.Resources["DefaultNavigationViewHeader"] as DataTemplate;
        }

        private void NavigationViewItem_ContextCanceled(UIElement sender, RoutedEventArgs args)
        {

        }
    }
}
