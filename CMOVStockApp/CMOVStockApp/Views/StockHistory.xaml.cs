using CMOVStockApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CMOVStockApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class StockHistory : Page
    {
        private ObservableCollection<String> options { get; set; }
        public StockHistory()
        {
            options = new ObservableCollection<String>();

            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            options = new ObservableCollection<String>();
            if (YahooFinance.observingCompanies.Count > 0)
                foreach (var company in YahooFinance.observingCompanies)
                    options.Add(company.symbol);
            DecideCompany.ItemsSource = options;
        }

        //get last week history
        private async void buttonGetLastWeekClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                YahooFinance y = new YahooFinance();
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await y.GetCompanyHistory(0, DecideCompany.SelectedItem as String);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLastMonthClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                YahooFinance y = new YahooFinance();
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await y.GetCompanyHistory(1, DecideCompany.SelectedItem as String);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLast6MonthClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                YahooFinance y = new YahooFinance();
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await y.GetCompanyHistory(2, DecideCompany.SelectedItem as String);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLastYearClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                YahooFinance y = new YahooFinance();
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await y.GetCompanyHistory(3, DecideCompany.SelectedItem as String);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }
    }
}

