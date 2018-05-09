using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        public SplashPage(SplashScreen splashscreen, bool loadState)
        {
            this.InitializeComponent();
            LoadingText.Text = "";
            splash = splashscreen;
            if (splash != null) {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, Object>(DismissedEventHandler);
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();
        }

        /// <summary>
        /// Loading app resources when on extended splash screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;

            // Updating the loading text depends on loading operation. This call is performed safely using the main UI thread.
            // If there's any error, ignore anh proceed to main page. Errors will be shown specifically in corresponding pages.

            // Load protocols
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingText.Text = "Loading Protocols...");
            try {
                ProtocolListViewModel.GetProtocolListViewModel();
            } catch { }

            // Load speakers
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingText.Text = "Loading Speakers...");
            try {
                SpeakerListViewModel.GetSpeakerListViewModel();
            } catch { }            

            // (Fake) Finalizing
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadingText.Text = "Finalizing Initialization...");
            Thread.Sleep(2000);

            // Dismiss and go to main page.
            DismissExtendedSplash();
        }

        /// <summary>
        /// Dismiss this extended splash screen and go to main page.
        /// </summary>
        async void DismissExtendedSplash()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                rootFrame = new Frame {
                    Content = new MainPage()
                };
                Window.Current.Content = rootFrame;
            });
        }
    }
}
