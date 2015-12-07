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
        private const int DATE_WITH_MONTH = 0;
        private const int DATE_WITH_DAY_AND_MONTH = 1;
        public StockHistory()
        {
            options = new ObservableCollection<String>();

            this.InitializeComponent();

        }

        protected override  void OnNavigatedTo(NavigationEventArgs e)
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
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await YahooFinance.GetCompanyHistory(0, DecideCompany.SelectedItem as String);
                changeDatesOnStocks(stockHistory, DATE_WITH_DAY_AND_MONTH);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLastMonthClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await YahooFinance.GetCompanyHistory(1, DecideCompany.SelectedItem as String);
                changeDatesOnStocks(stockHistory, DATE_WITH_DAY_AND_MONTH);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLast6MonthClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await YahooFinance.GetCompanyHistory(2, DecideCompany.SelectedItem as String);
                changeDatesOnStocks(stockHistory, DATE_WITH_MONTH);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private async void buttonGetLastYearClick(object sender, RoutedEventArgs e)
        {
            if (DecideCompany.SelectedItem != null)
            {
                List<CompanyValue> stockHistory = new List<CompanyValue>();
                stockHistory = await YahooFinance.GetCompanyHistory(3, DecideCompany.SelectedItem as String);
                changeDatesOnStocks(stockHistory, DATE_WITH_MONTH);
                (LineChart.Series[0] as LineSeries).ItemsSource = stockHistory;
            }
        }

        private void changeDatesOnStocks(List<CompanyValue> stockHistory, int type)
        {
            for (int i = 0; i < stockHistory.Count; i++)
            {
                string[] tokens = stockHistory[i].date.Split('-');
                string year = tokens[0];
                string month = tokens[1];
                string day = tokens[2];
                switch(type)
                {
                    case DATE_WITH_MONTH:
                        stockHistory[i].date = month;
                        break;
                    case DATE_WITH_DAY_AND_MONTH:
                        stockHistory[i].date = day + "-" + month;
                        break;
                }
                
            }
        }
    }
}

