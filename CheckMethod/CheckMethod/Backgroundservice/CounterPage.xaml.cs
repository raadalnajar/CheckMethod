using App2.Helper.Background.Alarm.interfaces;
using App2.Helper.Background.Alarm.Model;
using NBitcoin.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Appbackgroundtest
{
   public partial class CounterPage : ContentPage
    {
        public CounterPage()
        {
            InitializeComponent();
            var config = new NotificationConfig
            {
                IntervalInSeconds = 10, // Notify every 60 seconds
                Title = "Reminder",
                Message = messagenotify.Text
            };

            var backgroundService = DependencyService.Get<IBackgroundService>();
            backgroundService.StartService(config);


        }
        private void btnForegroundService_Clicked(object sender, EventArgs e)
        {
            if (DependencyService.Resolve<IForegroundService>().IsForeGroundServiceRunning())
            {
                App2.App.Current.MainPage.DisplayAlert("Opps", "Foreground Service Is Already Running", "OK");
            }
            else
            {
                DependencyService.Resolve<IForegroundService>().StartMyForegroundService();
            }
              }

        private void btnStopForegroundService_Clicked(object sender, EventArgs e)
        {
            DependencyService.Resolve<IForegroundService>().StopMyForegroundService();
        }
    }
}