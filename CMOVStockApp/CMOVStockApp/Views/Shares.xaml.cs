using CMOVStockApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class Shares : Page
    {
        private List<Company> companyList { get; set; }//all companies

        public List<Company> searchedList { get; set; }//companies in the search field

        private List<Company> observingList { get; set; }//companies selected



        public Shares()
        {
            this.InitializeComponent();

            observingList = new List<Company>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadCompanies();

        }

        private async void loadCompanies()
        {
            companyList = await StockNameLoad.getList();
            searchedList = companyList;
            searchList.ItemsSource = companyList;
        }


        private async void addStock(object sender, RoutedEventArgs e)
        {

            await contentDialog.ShowAsync();
        }

        private void searchCompanies(object sender, TextChangedEventArgs e)
        {
            searchText.Text = SearchBar.Text;
            searchedList = new List<Company>();

            foreach (Company cmp in companyList)
            {
                if (cmp.symbol.IndexOf(searchText.Text)!=-1)
                {
                    searchedList.Add(cmp);
                }
            }
            searchList.ItemsSource = searchedList;
        }

        private void addCompany(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Company cmp = searchedList.ElementAt(searchList.SelectedIndex);
            observingList.Add(cmp);
            observingCompanyList.ItemsSource = observingList;
        }
    }
}
