using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace CheckMethod.Droid
{
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
  DataScheme = "http",
  DataHost = "yourdomain",
  DataPathPrefix = "/",
  AutoVerify = true,
  Categories = new[] { Android.Content.Intent.ActionView, Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]


    [Activity(Label = "CheckMethod", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        // this for deep link handling 
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        private void HandleIntent(Intent intent)
        {
            if (intent.HasExtra("navigateTo"))
            {
                string page = intent.GetStringExtra("navigateTo");
                if (page == "CounterPage")
                {
                    // Assume NavigateToCounterPage is a method that handles the navigation
                    NavigateToCounterPage();
                }
            }
        }
    }
}