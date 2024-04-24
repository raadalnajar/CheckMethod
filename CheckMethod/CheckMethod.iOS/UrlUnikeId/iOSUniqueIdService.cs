using App6.GeneratUrl;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace GANIOS.iOS.URLunikeID
{
    public class iOSUniqueIdService : IUniqueIdService
    {
        private readonly NSDictionary launchOptions;

        public iOSUniqueIdService(NSDictionary launchOptions)
        {
            this.launchOptions = launchOptions;
        }

        public string GetUniqueId()
        {
            string uniqueId = string.Empty;

            NSUrl url = launchOptions?.ValueForKey(UIApplication.LaunchOptionsUrlKey) as NSUrl;
            if (url != null)
            {
                // Replace this with your code to extract the unique ID from the URL
                uniqueId = ExtractUniqueIdFromUrl(url);
            }

            return uniqueId;
        }

        private string ExtractUniqueIdFromUrl(NSUrl url)
        {
            string uniqueId = string.Empty;

            // Extract the unique ID from the URL
            // For example, if the URL is "yourapp://deeplink?id=12345", you can extract the ID using:
            var urlComponents = new NSUrlComponents(url, false);
            var queryItems = urlComponents?.QueryItems;
            var uniqueIdItem = queryItems?.FirstOrDefault(item => item.Name == "id");

            if (uniqueIdItem != null)
            {
                uniqueId = uniqueIdItem.Value;
            }

            return uniqueId;
        }

    }

}