using CheckMethod.iOS.AutoOtpVerfication;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]

namespace CheckMethod.iOS.AutoOtpVerfication
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Set the keyboard return type
                Control.ReturnKeyType = UIReturnKeyType.Done; // Example: Done, Next, etc.

                // Set cursor color
                Control.TintColor = UIColor.Red; // Example: set to red
            }
        }

    }
}