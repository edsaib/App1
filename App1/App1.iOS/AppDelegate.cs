
using App1.Views;
using Foundation;
using ObjCRuntime;
using System.Linq;
using UIKit;
using Xamarin.Forms;

namespace App1.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();      // Initialize Maps on iOS (it just works!) 
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }


        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, [Transient] UIWindow forWindow)
        {
            if (Xamarin.Forms.Application.Current == null || Xamarin.Forms.Application.Current.MainPage == null)
            {
                return UIInterfaceOrientationMask.All;
            }

            var mainPage = Xamarin.Forms.Application.Current.MainPage;

            if (mainPage is MapPage ||
                    (mainPage is NavigationPage && ((NavigationPage)mainPage).CurrentPage is MapPage) ||
                    (mainPage.Navigation != null && mainPage.Navigation.ModalStack.LastOrDefault() is MapPage))
            {
                return UIInterfaceOrientationMask.Landscape;
            }

            return UIInterfaceOrientationMask.All;
        }
    }
}
