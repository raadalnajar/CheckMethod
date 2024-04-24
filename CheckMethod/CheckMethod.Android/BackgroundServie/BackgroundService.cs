using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App2.Droid;
using Appbackgroundtest.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

using AndroidX.Core.App;

using System.Threading;
using ForegroundService = Appbackgroundtest.Droid.ForegroundService;
using App2.Helper.Background.Alarm.interfaces;
using App2.Helper.Background.Alarm.Model;

[assembly: Dependency(typeof(ForegroundService))]
namespace Appbackgroundtest.Droid
{
    [Service]
    public class ForegroundService : Service, IForegroundService
    {
        private Timer _timer;
       private NotificationConfig _config;
        public static bool IsForegroundServiceRunning;

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _config = new NotificationConfig(); // Initialize with default settings or load from saved state.
            CreateNotificationChannel();
            StartForeground(1001, BuildNotification("Service is running..."));
            UpdateTimer(); // Start or update the timer based on current config.
            IsForegroundServiceRunning = true;
            return StartCommandResult.Sticky;
        }

        private void UpdateTimer()
        {
            int intervalMilliseconds = (int)(_config.IntervalInSeconds * 1000); // Convert to milliseconds and cast to int
            _timer?.Dispose(); // Dispose the old timer if it exists
            _timer = new Timer(NotifyTimerCallback, null, 0, 10000);  // Adjusted to every 10 seconds
        }

       
        private async void NotifyTimerCallback(object state)
        {
            var messageService = DependencyService.Get<IMessageService>();
            if (messageService != null)
            {
                string newMessage = await messageService.FetchLatestMessageAsync();
                SendNotification("New Message", newMessage, 1002);
            }
        }
       // SendNotification(_config.Title, _config.Message, 1002);
      //  }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string channelId = "my_service_channel_id";
                string channelName = "My Foreground Service";
                NotificationChannel channel = new NotificationChannel(channelId, channelName, NotificationImportance.High)
                {
                    Description = "Notifications from My Service"
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        private Notification BuildNotification(string text)
        {
            var channelId = "my_service_channel_id";
            var notificationBuilder = new NotificationCompat.Builder(this, channelId)
                .SetContentTitle("Foreground Service")
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.notification_icon_background)
                .SetOngoing(true);

            return notificationBuilder.Build();
        }

     
        private void SendNotification(string title, string content, int notificationId)
        {
            var notificationManager = NotificationManagerCompat.From(this);
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("navigateTo", "CounterPage");

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var builder = new NotificationCompat.Builder(this, "my_service_channel_id")
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Mipmap.icon)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            notificationManager.Notify(notificationId, builder.Build());
        }
       
        public override void OnDestroy()
        {
            _timer?.Dispose();
            IsForegroundServiceRunning = false;
            base.OnDestroy();
        }

        public void StartMyForegroundService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Android.App.Application.Context.StartForegroundService(intent);
            }
            else
            {
                Android.App.Application.Context.StartService(intent);
            }
        }

        public void StopMyForegroundService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(ForegroundService));
            Android.App.Application.Context.StopService(intent);
        }

        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }

        public void UpdateConfig(NotificationConfig config)
        {
            _config = config;
            UpdateTimer();
        }
    }
}