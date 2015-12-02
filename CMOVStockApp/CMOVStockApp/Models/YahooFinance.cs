using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CMOVStockApp.Models
{
    public class Company
    {
        [JsonProperty("Symbol")]
        public String symbol { get; set; }
        [JsonProperty("Name")]
        public String name { get; set; }
        [JsonProperty("Min")]
        public float min { get; set; }
        [JsonProperty("Max")]
        public float max { get; set; }
        public double quote { get; set; }

        public Company(String sm, String nm)
        {
            symbol = sm;
            name = nm;
            min = -1;
            max = 1000000;
            quote = 0;
        }
    }

    public class CompanyValue
    {
        public String value { get; set; }
        public String date { get; set; }

        public CompanyValue(String v, String d)
        {
            value = v;
            date = d;
        }
    }

    class StockNameLoad
    {
        public static List<Company> companies = new List<Company>();

        public static Task<List<Company>> getList()
        {
            return Task.Run(() =>
            {
                if (companies.Count < 1)
                {
                    companies.Add(new Company("IBM", "IBM"));
                    companies.Add(new Company("MSFT", "Microsoft"));
                    companies.Add(new Company("CSCO", "Cisco"));
                    companies.Add(new Company("AMZN", "Amazon"));
                    companies.Add(new Company("HPQ", "HP"));
                    companies.Add(new Company("GOOG", "Google"));
                    companies.Add(new Company("AAPL", "Apple"));
                    companies.Add(new Company("ORCL", "Oracle"));
                }

                return companies;
            });
        }

        public static async void getCompanyName(string companySymbol)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22" + companySymbol + "%22)%0A%09%09&diagnostics=true&env=http%3A%2F%2Fdatatables.org%2Falltables.env&format=json"));

            var jsonString = await response.Content.ReadAsStringAsync();

            JsonObject root = Windows.Data.Json.JsonValue.Parse(jsonString).GetObject();
            JsonObject query = root.GetNamedObject("query");
            JsonObject results = query.GetNamedObject("results");
            JsonObject quote = results.GetNamedObject("quote");
            if (quote.GetNamedValue("Name").ValueType == JsonValueType.Null)
            {
                Debug.WriteLine("Error: company symbol does not exist!");
            }
            else
            {
                string name = quote.GetNamedString("Name");
                Debug.WriteLine(name);
            }
        }
    }

    class StockPriceLoad
    {
        public static async void getLastTradePrice(string companySymbol)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22" + companySymbol + "%22)%0A%09%09&diagnostics=true&env=http%3A%2F%2Fdatatables.org%2Falltables.env&format=json"));
            var jsonString = await response.Content.ReadAsStringAsync();

            JsonObject root = Windows.Data.Json.JsonValue.Parse(jsonString).GetObject();
            JsonObject query = root.GetNamedObject("query");
            JsonObject results = query.GetNamedObject("results");
            JsonObject quote = results.GetNamedObject("quote");

            string lastTradePriceOnly = quote.GetNamedString("LastTradePriceOnly");
            Debug.WriteLine(lastTradePriceOnly);
        }
    }

    class YahooFinance
    {
        public static List<Company> observingCompanies = new List<Company>();
        //gets stock value history
        //if
        //mode=0->last week;per day
        //mode=1->last month;per day
        //mode=2->last 6 months;per month
        //mode=3->last year; per month
        public async Task<List<CompanyValue>> GetCompanyHistory(int mode, String symbol)
        {
            string content = null;
            List<CompanyValue> rsp = new List<CompanyValue>();
            DateTime thisDay = DateTime.Today;

            int month = thisDay.Month - 1;
            int year = thisDay.Year;
            int day = thisDay.Day;
            String interval = "d";

            if (mode == 0)
            {
                thisDay = thisDay.AddDays(-7);
            }
            else if (mode == 1)
            {
                thisDay = thisDay.AddMonths(-1);
                interval = "w";
            }
            else if (mode == 2)
            {
                thisDay = thisDay.AddMonths(-6);
                interval = "m";
            }
            else if (mode == 3)
            {
                thisDay = thisDay.AddYears(-1);
                interval = "m";
            }


            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("http://ichart.finance.yahoo.com/table.txt?a=" + (thisDay.Month - 1) + "&b=" + thisDay.Day + "&c=" + thisDay.Year + "&d=" + month + "&e=" + day + "&f=" + year + "&g=" + interval + "&s=" + symbol))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        string[] history = content.Split('\n');
                        for (int i = history.Length - 2; i > 0; i--)
                        {
                            string[] info = history[i].Split(',');
                            rsp.Add(new CompanyValue(info[4], info[0]));
                        }
                        return rsp;
                    }
                    else { return rsp; }

                }
            }

        }


        public async Task<List<Company>> getQuotes()
        {
            String content = "";
            String query="";

            for(int i=0;i<observingCompanies.Count;i++)
            {
                query += observingCompanies.ElementAt(i).symbol;
                if(i< observingCompanies.Count-1)
                {
                    query += ",";
                }
            }
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("http://finance.yahoo.com/d/quotes?f=sl1d1t1v&s="+query))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4

                        string[] quotes = content.Split('\n');


                        for (int i = 0; i < observingCompanies.Count; i++)
                        {
                            observingCompanies.ElementAt(i).quote = Convert.ToDouble( quotes[i].Split(',')[1]);
                        }
                        return observingCompanies;
                    }
                    else {return observingCompanies; }

                }
            }
        }
    }
}