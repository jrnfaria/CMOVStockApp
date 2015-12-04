using CMOVStockApp.Models;
using System.Diagnostics;
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
using Windows.ApplicationModel.Background;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CMOVStockApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class StockPortefolio : Page
    {
        private DispatcherTimer dispatch;

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

        private void loadQuotesTask(object sender, object e)
        {
            loadQuotes();
            observingList = new ObservableCollection<Company>(YahooFinance.observingCompanies);
            observingCompanyList.ItemsSource = observingList;
        }
   
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadQuotesTask(null,null);
            dispatch = new DispatcherTimer();
            dispatch.Interval = new TimeSpan(0, 0, 1);
            dispatch.Tick += loadQuotesTask;
            dispatch.Start();
        }

    }
}
