using App4.Services.LoginService.OTPVerfication.AutomaticVerfication;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace OTPVerificationSample
{
    public partial class MainPage : ContentPage
    {
        private HttpClient _httpClient = new HttpClient();

        int otpNo = 0;
        public MainPage()
        {
            InitializeComponent();
            // ios
            otp.Keyboard = Keyboard.Numeric;

            otp.On<iOS>().SetAdjustsFontSizeToFitWidth(true);
            // KeyboardAppearance.Dark is not directly available, but you can set the Keyboard property directly
            otp.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeNone);

            // Check if the device platform is iOS
            if (Device.RuntimePlatform == Device.iOS)
            {
                // For setting return type and cursor color, we don't have direct platform-specific methods
                // You can set them directly on the Entry control
                otp.ReturnType = ReturnType.Done;
            }
            MessagingCenter.Subscribe<string>(this, "ReceivedOTP", (message) =>
             {
                 string[] words = message.Split();
                 foreach (string item in words.ToList())
                 {
                     var isNumeric = int.TryParse(item, out int n);
                     if (isNumeric)
                     {
                         otp.IsVisible = true;
                         mobileNumber.IsVisible = false;
                         otp.Text = item;
                         btnNext.Text = "Login";
                         //DisplayAlert("Message", $"OTP is {item}", "Ok");
                         break;
                     }
                 }
             });
        }

        private async void SendOTP(object sender, EventArgs e)
        {

            // send a GET request  
            string countryCode = "91";  // two digit only

            OTPModel model = new OTPModel();
            model.sender = "SOCKET";
            model.route = "4";
            model.country = countryCode;
            string hashKey = System.Web.HttpUtility.UrlEncode(DependencyService.Get<IHashService>().GenerateHashkey());
            otpNo = GenerateOTP();
            string message = $"<#> Your OTP is {otp} {hashKey}";
            List<string> numbers = new List<string> { mobileNumber.Text };
            model.sms.Add(new Sms { message = message, to = numbers });

            var client = new RestClient($"https://api.msg91.com/api/v2/sendsms?country={countryCode}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authkey", "281817ABVlLRzJt5d0a30e2");
            string jsonData = JsonConvert.SerializeObject(model);
            request.AddParameter("application/json", jsonData, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                DependencyService.Get<IHashService>().StartSMSRetriverReceiver();
                OTPResponse resp = JsonConvert.DeserializeObject<OTPResponse>(response.Content);
                if (resp.type == "success")
                {
                    await DisplayAlert("Message", $"An OTP send to {numbers[0]}", "Ok");
                }
                else
                {
                    // handle if sms failed to send
                    await DisplayAlert("Message", resp.message, "Ok");
                }
            }
            else
            {
                DisplayAlert("Message", response.ErrorMessage, "Ok");
            }
        }
        public int GenerateOTP()
        {
            return new Random().Next(1000, 9999);
        }
        private async Task<bool> SendOTPAsync(string countryCode, string mobile, int otp)
        {
            var apiUrl = "https://your-backend-api.com/sendotp"; // URL of your backend API
            var payload = new
            {
                CountryCode = countryCode,
                Mobile = mobile,
                OTP = otp
            };

            try
            {
                // Serialize the payload to JSON
                string jsonPayload = JsonConvert.SerializeObject(payload);

                // Create HttpContent
                HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send a POST request to the specified Uri
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                // Check the result
                if (response.IsSuccessStatusCode)
                {
                    // You can log the response body if needed for debugging
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Assuming the backend sends a JSON with a boolean field 'success'
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return result.success;
                }
                else
                {
                    // Log error message or handle exceptions
                    Console.WriteLine($"Failed to send OTP. Status Code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle potential exceptions such as network errors
                Console.WriteLine($"Exception in SendOTPAsync: {ex.Message}");
                return false;
            }
        }
    }

}

