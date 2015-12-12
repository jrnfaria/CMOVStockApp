using CMOVStockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private ObservableCollection<Company> observingList { get; set; }//companies selected

        private Company cmp;
        public Shares()
        {
            this.InitializeComponent();
            observingList = new ObservableCollection<Company>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadCompanies();
            observingList = new ObservableCollection<Company>();
            if (YahooFinance.observingCompanies.Count > 0)
                foreach (var company in YahooFinance.observingCompanies)
                    observingList.Add(company);
            observingCompanyList.ItemsSource = observingList;
        }

        private async void loadCompanies()
        {
            companyList = await StockNameLoad.getList();
            searchedList = companyList;
            searchList.ItemsSource = companyList;
        }

        //show modal window ativated by add stock button
        private async void addStock(object sender, RoutedEventArgs e)
        {
            await contentDialog.ShowAsync();
        }

        //searchs the companies from a list activates every time user writes something in the modal window
        private async void searchCompanies(object sender, TextChangedEventArgs e)
        {
            searchText.Text = SearchBar.Text;
            searchedList = new List<Company>();

            foreach (Company cmp in companyList)
            {
                if (cmp.symbol.IndexOf(searchText.Text.ToUpper()) != -1)
                {
                    searchedList.Add(cmp);
                }
            }

            if (searchedList.Count == 0)
            {
                searchedList = await StockNameLoad.getSearchList(searchText.Text.ToUpper());
            }

            searchList.ItemsSource = searchedList;
        }

        //adds company to observing list activated by add button in modal window
        private async void addCompany(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Company cmp = searchedList.ElementAt(searchList.SelectedIndex);
            Boolean found = false;

            for (int i = 0; i < YahooFinance.observingCompanies.Count; i++)
            {
                if (YahooFinance.observingCompanies.ElementAt(i).name == cmp.name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                observingList.Add(cmp);
                YahooFinance.observingCompanies.Add(cmp);
                observingCompanyList.ItemsSource = observingList;
            }


            await User.addCompanies();
        }

        private async void minTextBox(object sender, RoutedEventArgs e)
        {
            TextBox min = (TextBox)sender;
            float value;
            Single.TryParse(min.Text, out value);
            if (value < cmp.max)
            {
                cmp.min = value;
            }
            min.Text = cmp.min.ToString();
            await User.addCompanies();
        }

        private async void maxTextBox(object sender, RoutedEventArgs e)
        {
            TextBox max = (TextBox)sender;
            float value;
            Single.TryParse(max.Text, out value);
            if (value > cmp.min)
            {
                cmp.max = value;
            }
            max.Text = cmp.max.ToString();
            await User.addCompanies();
        }

        private async void removeCompany(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var item = button.DataContext;
            Company cmp = (Company)item;

            for (int i = 0; i < YahooFinance.observingCompanies.Count; i++)
            {
                if (YahooFinance.observingCompanies.ElementAt(i).name == cmp.name)
                {
                    YahooFinance.observingCompanies.RemoveAt(i);
                    observingList.RemoveAt(i);
                    observingCompanyList.ItemsSource = observingList;
                    break;
                }
            }
            await User.addCompanies();
        }
        private async void OpenMinMaxDialog(object sender, SelectionChangedEventArgs e)
        {
            ListView lvi = (ListView)sender;
            if (lvi.SelectedIndex > -1)
            {
               
            var item = lvi.SelectedIndex;
                cmp = observingList.ElementAt(item);
                minBlock.Text = cmp.min.ToString();
                maxBlock.Text = cmp.max.ToString();
            
            await MinMaxDialog.ShowAsync();
            }
        }

        private void reloadList(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            observingCompanyList.ItemsSource = observingList;
            observingCompanyList.SelectedIndex = -1;
        }
    }
}
