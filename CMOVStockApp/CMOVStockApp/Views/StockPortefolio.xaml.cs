using CMOVStockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class StockPortefolio : Page
    {
        private ObservableCollection<Company> observingList { get; set; }//companies selected

        public StockPortefolio()
        {
            this.InitializeComponent();
            observingList = new ObservableCollection<Company>();
        }


        private async void loadQuotes()
        {
            await YahooFinance.getQuotes();
        }

   
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadQuotes();
            observingList = new ObservableCollection<Company>(YahooFinance.observingCompanies);
            observingCompanyList.ItemsSource = observingList;
        }

    }
}
