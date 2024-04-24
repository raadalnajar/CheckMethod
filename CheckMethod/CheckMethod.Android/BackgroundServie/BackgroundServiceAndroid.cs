using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App2.Helper.Background.Alarm.interfaces;
using App2.Helper.Background.Alarm.Model;
using Appbackgroundtest.Droid;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Application = Android.App.Application;
[assembly: Dependency(typeof(BackgroundServiceAndroid))]

namespace Appbackgroundtest.Droid
{
    public class BackgroundServiceAndroid : IBackgroundService
    {
        private Timer _timer;
        private NotificationConfig _config;

        public void StartService(NotificationConfig config)
        {
            _config = config;
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StartForegroundService(intent);
        }

        private void NotifyTimerCallback(object state)
        {
            SendNotification(_config.Title, _config.Message, 1002);
            // Reset timer with new interval if needed
            _timer.Change(TimeSpan.FromSeconds(_config.IntervalInSeconds), TimeSpan.FromMilliseconds(-1));
        }

        private void SendNotification(string title, string content, int notificationId)
        {
            // Notification setup code
        }

        // Additional methods...
    }

}