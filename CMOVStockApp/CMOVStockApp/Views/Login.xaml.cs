using CMOVStockApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CMOVStockApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }


        private async void signInButtonClick(object sender, RoutedEventArgs e)
        {
            User.LoginResponse rsp = await User.login(usernameTextBox.Text, passwordTextBox.Password); 
            if(rsp!=null)
            {
                if(rsp.status=="ok")
                {
                    this.Frame.Navigate(typeof(Menu));
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog(rsp.body.response);

                    await dialog.ShowAsync();
                }
            }
            else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("You dont have connection to the internet");
                await dialog.ShowAsync();
            }
        }
    }
}
