using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

namespace CMOVStockApp.Models
{
    class Observer
    {
        private static PushNotificationChannel channel = null;

        private static string channelUri = null;

        private static string accessToken = null;


        public static async void checkIntervals()
        {
            CreatePushNotificationChannel();

            GetAccessToken();


            await YahooFinance.getQuotes();

            for (int i=0;i<YahooFinance.observingCompanies.Count;i++)
            {
                if(YahooFinance.observingCompanies.ElementAt(i).quote < YahooFinance.observingCompanies.ElementAt(i).min)
                {
                    SendPushNotification(0, YahooFinance.observingCompanies.ElementAt(i).name);

                    YahooFinance.observingCompanies.ElementAt(i).min = 0;
                }
                else if (YahooFinance.observingCompanies.ElementAt(i).quote > YahooFinance.observingCompanies.ElementAt(i).max)
                {
                    SendPushNotification(1, YahooFinance.observingCompanies.ElementAt(i).name);

                    YahooFinance.observingCompanies.ElementAt(i).max = 1000000;
                }
            }
        }

        public static async void CreatePushNotificationChannel()
        {
            try
            {
                channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

                channelUri = channel.Uri.ToString();
            }
            catch
            {

            }
        }

        public static async void GetAccessToken()
        {
            HttpClient client = new HttpClient();
            var body = String.Format("grant_type=client_credentials&client_id=ms-app://s-1-15-2-1714645876-651989582-552947948-2933913919-3002687657-2755528455-2492861792&client_secret=4a0egKDo5N3rQBRoH4qJE5rpQxlCyeLx&scope=notify.windows.com");
            StringContent content = new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage response = await client.PostAsync(new Uri("https://login.live.com/accesstoken.srf"), content);
            string responseString = await response.Content.ReadAsStringAsync();
            JObject obj = (JObject)JsonConvert.DeserializeObject(responseString);
            accessToken = (string)obj.GetValue("access_token");
        }

        public static async void SendPushNotification(int limit, string companyName)
        {
            if(channelUri != null && accessToken != null)
            {
                // if limit == 0 -> min limit surpassed
                // if limit == 1 -> max limit surpassed

                await Task.Delay(2000);

                string xml = "";

                if (limit == 0)
                {
                    xml = String.Format("<toast launch=\"\"><visual lang=\"en-US\"><binding template=\"ToastImageAndText01\"><image id=\"1\" src=\"\"/><text id=\"1\">" + companyName + ": minimum value limit surpassed!</text></binding></visual></toast>");
                }
                else
                {
                    if (limit == 1)
                    {
                        xml = String.Format("<toast launch=\"\"><visual lang=\"en-US\"><binding template=\"ToastImageAndText01\"><image id=\"1\" src=\"\"/><text id=\"1\">" + companyName + ": maximum value limit surpassed!</text></binding></visual></toast>");
                    }
                }

                StringContent xmlContent = new StringContent(xml);
                xmlContent.Headers.Add("X-WNS-Type", "wns/toast");
                xmlContent.Headers.Add("X-WNS-RquestForStatus", "true");
                xmlContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                xmlContent.Headers.ContentLength = xml.Length;

                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = new HttpMethod("POST");
                message.Content = xmlContent;
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken + "=");
                message.RequestUri = new Uri(channelUri, UriKind.Absolute);

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage httpReponseMessage = await httpClient.SendAsync(message);
            }
        }
    }
}

