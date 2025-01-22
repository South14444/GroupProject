using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GroupProject
{
    public partial class Application_1 : Window
    {
        private DispatcherTimer timer;
        public string accessToken = "";
        public Application_1()
        {
            InitializeComponent();
            CreateGigachatToken();
            Start_timer();
            this.Closing += Window_Closing; 
        }

        private void Start_timer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(25);
            timer.Tick += Timer_tik;
            timer.Start();
        }

        private void Timer_tik(object sender, EventArgs e)
        {
            CreateGigachatToken();
        }

        public async Task CreateGigachatToken()
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth");

                request.Headers.Add("RqUID", "0e747cd9-1c39-4e3b-b44a-a9f322b5af97");
                request.Headers.Add("Authorization", "Basic MGU3NDdjZDktMWMzOS00ZTNiLWI0NGEtYTlmMzIyYjVhZjk3OjkzNDhlOTZiLTZkZGItNGE0ZC1hMWVkLTljOTRiZDA0ZmQzYg==");

                var collection = new List<KeyValuePair<string, string>>
                {
                    new("scope", "GIGACHAT_API_PERS")
                };
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseContent);
                    accessToken = json["access_token"]?.ToString();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка: " + response.StatusCode);
                    MessageBox.Show(errorContent);
                }
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show("Ошибка запроса:");
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                MessageBox.Show("Произошла непредвиденная ошибка:");
                MessageBox.Show(e.Message);
            }
        }

        List<Dictionary<string, string>> conversationHistory = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string food = ReadOnlyTextBox.Text;
            Food(food);
        }

        public async Task Food(string food)
        {
            string PromtFood = "Ты должен возращать мне только точное число (без каких либо слов) калорий в 100 граммах этого продукта ";
            var (response1, updatedHistory) = await GetChatCompletion(accessToken, PromtFood + food, conversationHistory);
            if (response1 != null)
            {
                JObject responseJson = JObject.Parse(response1);
                string assistantResponse = responseJson["choices"]?[0]?["message"]?["content"]?.ToString();
                InputTextBox.Text = ($"{assistantResponse}");
                conversationHistory = updatedHistory;
            }
            else
            {
                MessageBox.Show("Произошла ошибка при выполнении запроса.");
            }
        }

        public static async Task<(string, List<Dictionary<string, string>>)> GetChatCompletion(
            string authToken,
            string userMessage,
            List<Dictionary<string, string>> conversationHistory = null)
        {
            string url = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

            if (conversationHistory == null)
            {
                conversationHistory = new List<Dictionary<string, string>>();
            }

            conversationHistory.Add(new Dictionary<string, string>
            {
                { "role", "user" },
                { "content", userMessage }
            });

            var payload = JsonConvert.SerializeObject(new
            {
                model = "GigaChat:latest",
                messages = conversationHistory,
                temperature = 1,
                top_p = 0.1,
                n = 1,
                stream = false,
                max_tokens = 512,
                repetition_penalty = 1,
                update_interval = 0
            });

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(responseData);

                        string assistantContent = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

                        conversationHistory.Add(new Dictionary<string, string>
                        {
                            { "role", "assistant" },
                            { "content", assistantContent }
                        });

                        return (responseData, conversationHistory);
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode}");
                        return (null, conversationHistory);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                return (null, conversationHistory);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application_2 secondWindow = new Application_2();
            secondWindow.Show();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
    }
}
