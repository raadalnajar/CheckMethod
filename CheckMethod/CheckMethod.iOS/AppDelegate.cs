using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace CheckMethod.iOS
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
            LoadApplication(new App());
            // Set up background fetch
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            return base.FinishedLaunching(app, options);
        }
        // background services
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Implement your background fetch logic here
            // This method is called when the app is woken up in the background to perform a fetch
            // Remember to call the completion handler when done
            completionHandler(UIBackgroundFetchResult.NewData);
        }
        //open deep link url 
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            // Handle the deep link URL here
            if (url != null && url.AbsoluteString.StartsWith("yourdeeplink://"))
            {
                // Extract any parameters from the URL if needed
                // For example, if your URL is "yourdeeplink://page?id=123", you can extract the id parameter like this:
                var id = url.GetComponents(NSURLComponents.UrlQueryKey, NSURLComponents.UrlComponentDecoded, out _);

                // Handle the deep link based on the URL and its parameters
                // You might want to navigate to a specific page or perform some action based on the deep link
            }

            return true;
        }
    }
}
