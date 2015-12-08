using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace CMOVStockApp.Models
{
    public class Company
    {
        [JsonProperty("SYMBOL")]
        public String symbol { get; set; }
        [JsonProperty("NAME")]
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
            min = 0;
            max = 10000;
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

        public static string companyName = "";

        public static string companySymbol = "";

        public static Task<List<Company>> getList()
        {
            return Task.Run(() =>
            {
                if (companies.Count < 1)
                {
                    companies.Add(new Company("AMZN", "Amazon.com, Inc."));
                    companies.Add(new Company("AAPL", "Apple Inc."));
                    companies.Add(new Company("CSCO", "Cisco Systems, Inc."));
                    companies.Add(new Company("GOOG", "Alphabet Inc."));
                    companies.Add(new Company("MSFT", "Microsoft Corporation"));
                    companies.Add(new Company("ORCL", "Oracle Corporation"));
                }

                return companies;
            });
        }

        public static Task<List<Company>> getSearchList(string companySymbol)
        {
            return Task.Run(async () =>
            {
                List<Company> companies = new List<Company>();

                var client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(new Uri("http://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22" + companySymbol + "%22)%0A%09%09&diagnostics=true&env=http%3A%2F%2Fdatatables.org%2Falltables.env&format=json"));

                var jsonString = await response.Content.ReadAsStringAsync();

                JsonObject root = Windows.Data.Json.JsonValue.Parse(jsonString).GetObject();
                JsonObject query = root.GetNamedObject("query");
                JsonObject results = query.GetNamedObject("results");
                JsonObject quote = results.GetNamedObject("quote");
                if (quote.GetNamedValue("Name").ValueType == JsonValueType.Null)
                {
                    
                }
                else
                {
                    companies.Add(new Company(companySymbol, quote.GetNamedString("Name")));
                }

                return companies;
            });
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
        public static async Task<List<CompanyValue>> GetCompanyHistory(int mode, String symbol)
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


        public async static Task<List<Company>> getQuotes()
        {
            String content = "";
            String query = "";

            for (int i = 0; i < observingCompanies.Count; i++)
            {
                query += observingCompanies.ElementAt(i).symbol;
                if (i < observingCompanies.Count - 1)
                {
                    query += ",";
                }
            }
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("http://finance.yahoo.com/d/quotes?f=sl1d1t1v&s=" + query))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4

                        string[] quotes = content.Split('\n');


                        for (int i = 0; i < observingCompanies.Count; i++)
                        {
                            observingCompanies.ElementAt(i).quote = Convert.ToDouble(quotes[i].Split(',')[1]);
                        }
                        return observingCompanies;
                    }
                    else { return observingCompanies; }

                }
            }
        }
    }
}