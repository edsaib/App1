using App1.Services;
using App1.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    public partial class App : Application
    {

        private static ContactDatabase _contactDatabase;

        public static ContactDatabase ContactDatabase
        {
            get
            {
                // Singleton: If the connection is already open, don't reopen it.
                if(_contactDatabase == null)
                {
                    _contactDatabase = new ContactDatabase(Constants.DatabasePath, Constants.Flags);
                }

                return _contactDatabase;
            }
        }

        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();
            DependencyService.Register<ContactDatabase>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
