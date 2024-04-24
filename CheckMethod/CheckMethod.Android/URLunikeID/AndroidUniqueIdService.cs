using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App4.Droid;
using App6.GeneratUrl;
using GANM.Droid.URLunikeID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
[assembly: Dependency(typeof(AndroidUniqueIdService))]

namespace GANM.Droid.URLunikeID
{
    public class AndroidUniqueIdService : IUniqueIdService
    {
        public string GetUniqueId()
        {
            string uniqueId = string.Empty;

            MainActivity activity = MainActivity.Instance;
            if (activity != null && activity.Intent?.Data != null)
            {
                Android.Net.Uri uri = activity.Intent.Data;
                uniqueId = ExtractUniqueIdFromUri(uri);
            }

            return uniqueId;
        }

        private string ExtractUniqueIdFromUri(Android.Net.Uri uri)
        {
            string uniqueId = string.Empty;

            // Extract the unique ID from the URI
            // For example, if the URI is "yourapp://deeplink?id=12345", you can extract the ID using:
            string id = uri.GetQueryParameter("id");

            if (!string.IsNullOrEmpty(id))
            {
                uniqueId = id;
            }

            return uniqueId;
        }

    }
}