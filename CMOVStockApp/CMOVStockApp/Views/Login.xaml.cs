using CMOVStockApp.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            User.Response rsp = await User.signIn(usernameTextBox.Text, passwordTextBox.Password);
            if (rsp != null)
            {
                if (rsp.status == "ok")
                {
                    User.MycompaniesResponse companies = await User.myCompanies(usernameTextBox.Text);
                    YahooFinance.observingCompanies = companies.body.response;
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
                var dialog = new Windows.UI.Popups.MessageDialog("Cant connect to server");
                await dialog.ShowAsync();
            }
        }

        private async void signUpButtonClick(object sender, RoutedEventArgs e)
        {
            User.Response rsp = await User.signUp(usernameTextBox.Text, passwordTextBox.Password);
            
            if (rsp != null)
            {
                if (rsp.status == "ok")
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("User registered");
                    await dialog.ShowAsync();
                }
                else
                {
                    var dialog = new Windows.UI.Popups.MessageDialog(rsp.body.response);
                    await dialog.ShowAsync();
                
                }
            }
            else
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Cant connect to server");
                await dialog.ShowAsync();
            }
        }

    }
}

