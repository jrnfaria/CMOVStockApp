using CMOVStockApp.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CMOVStockApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static PushNotificationChannel channel = null;

        private static string channelUri = null;

        private static string accessToken = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            CreatePushNotificationChannel();

            GetAccessToken();

            SendPushNotification();
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

        public static async void SendPushNotification()
        {
            await Task.Delay(1000);

            string xml = String.Format("<?xml version='1.0' encoding='utf-8'?><toast><visual>< binding template =\"ToastImageAndText01\"><image id=\"1\" src=\"{0}\" alt=\"Placeholder image\"/>< text id =\"1\">{1}</text></binding></visual></toast>", "World", "Hello");

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

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(Login), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }


    }
}
