using System;
using System.Collections.Generic;
using System.Text;

namespace App6.GeneratUrl
{
    public class GenerateunikkIDinURL
    {
        string GenerateSmartBannerLink(string uniqueId)
        {
            // Replace "yourapp://deeplink" with your actual deep link or custom URI scheme
            string deepLink = "yourapp://deeplink";

            // Replace "YOUR_APP_ID" with your actual iOS app ID
            string iOSAppId = "YOUR_APP_ID";

            // Replace "your.package.name" with your actual Android package name
            string androidPackageName = "your.package.name";

            // Generate the smart banner link with the unique ID
            string smartBannerLink = $"<a href=\"{deepLink}?id={uniqueId}\">Open the App</a>\n\n" +
                                     $"<meta name=\"apple-itunes-app\" content=\"app-id={iOSAppId}, app-argument={deepLink}?id={uniqueId}\">\n\n" +
                                     $"<link rel=\"alternate\" href=\"https://play.google.com/store/apps/details?id={androidPackageName}&id={uniqueId}\">";

            return smartBannerLink;
        }

    }
}
