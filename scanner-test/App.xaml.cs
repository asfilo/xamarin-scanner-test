using System;
using scannertest.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace scanner_test
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            TabbedPage tabbedPage = new TabbedPage();
            NavigationPage navigationPage = new NavigationPage(new MainPage());
            tabbedPage.Children.Add(navigationPage);
            //tabbedPage.Children.Add(new PowerView());
            tabbedPage.Children.Add(new DeviceView());

            MainPage = new DeviceView();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
