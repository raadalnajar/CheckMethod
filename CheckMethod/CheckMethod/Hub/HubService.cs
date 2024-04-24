
using GAN1.ModelController.Chat;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GANM.ChatServices
{
    public class HubService
    {
        private HubConnection _hubConnection;
        private HttpClient _httpClient;
        public event Action<string, string> MessageReceived;
        public event Action<string, string> PrivateMessageReceived;

        public HubService()
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Be cautious with this in production
            };

            _httpClient = new HttpClient(httpClientHandler);
            _httpClient.BaseAddress = new Uri("https://192.168.1.103:45455/api/testValues/");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://<your-hub-url>")
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", OnMessageReceived);
            _hubConnection.On<string, string>("ReceivePrivateMessage", OnPrivateMessageReceived);
            _hubConnection.On<byte[]>("ReceiveVoiceMessage", async (audioChunk) =>
            {
                // Play the received audio chunk
                // You can use Xamarin's audio playback mechanism here
            });
            _hubConnection.On<string, ChatMessage>("ReceivePrivateMessage", (user, messageModel) =>
            {
                // Handle the received message
                string message = messageModel.Content;
                byte[] receivedImageData = messageModel.ImageData;
                byte[] receivedVoiceData = messageModel.VoiceData;

                // Do something with the received data
            });

        }

        public async Task SendFlexibleMessage(string receiverEmail, string message = null, byte[] imageData = null, byte[] voiceData = null)
        {
            try
            {
                await _hubConnection.SendAsync("SendFlexibleMessage", receiverEmail, message, imageData, voiceData);
            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary
            }
        }

      

        private void OnMessageReceived(string sender, string message)
        {
            MessageReceived?.Invoke(sender, message);
        }

        private void OnPrivateMessageReceived(string sender, string message)
        {
            PrivateMessageReceived?.Invoke(sender, message);
        }
        // Add methods to send messages to the ChatHub
        public async Task SendPrivateMessage(string userEmail, string message , byte[] image, byte[]voice)
        {
            await _hubConnection.SendAsync("SendPrivateMessageWithSaveDatabase", userEmail, message,image,voice);
        }
        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(int chatRoomId, string sender, string message, byte[] imageData)
        {
            await _hubConnection.SendAsync("SendMessage", chatRoomId, sender, message, imageData);
        }

        public async Task SendPrivateMessage(string userEmail, string message)
        {
            await _hubConnection.SendAsync("SendPrivateMessage", userEmail, message);
        }

        public async Task LoadConversationHistory(int senderId, int receiverId)
        {
            var conversationHistory = await _hubConnection.InvokeAsync<List<ChatMessage>>("LoadConversationHistory", senderId, receiverId);

            // Process the retrieved conversation history as needed
            // For example, update the UI with the retrieved messages
        }
        public void SavePrivateConversation(List<ChatMessage> conversation, string userEmail)
        {
            string json = JsonSerializer.Serialize(conversation);
            string filename = $"{userEmail}_conversation.json";
            File.WriteAllText(filename, json);
        }

        // Load a private conversation from a local file
        public List<ChatMessage> LoadPrivateConversation(string userEmail)
        {
            string filename = $"{userEmail}_conversation.json";
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                return JsonSerializer.Deserialize<List<ChatMessage>>(json);
            }
            return new List<ChatMessage>();
        }

        // Example usage when sending a private message
        public async Task SendPrivateMessagenew(string userEmail, string message)
        {
            await _hubConnection.SendAsync("SendPrivateMessage", userEmail, message);

            // Update the local conversation history when sending a private message
            var conversation = LoadPrivateConversation(userEmail);
            conversation.Add(new ChatMessage { Sender = 3, Content = message }); // Modify to fit your ChatMessage class
            SavePrivateConversation(conversation, userEmail);
        }

        // Example usage when loading a private conversation
        public void LoadPrivateConversationHistory(string userEmail)
        {
            var conversationHistory = LoadPrivateConversation(userEmail);

            // Process the loaded conversation history
            // For example, display it in the UI
        }
     
        async void StartConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://<your-hub-url>")
                .Build();
            try
            {
                await _hubConnection.StartAsync();
                _hubConnection.On<string, string>("ReceiveMessage", (user,message) =>
                {
                    Label lable = new Label { Text = $"{user}:{message}", HorizontalOptions = LayoutOptions.Start, FontSize = 20 };

                   
                    lable.Text = message;
                      
                    
                });

            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary
            }
        }
    }
}
