using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CMOVStockApp.Models
{
    public class Company
    {
        [JsonProperty("Symbol")]
        public String symbol { get; set; }
        [JsonProperty("Name")]
        public String name { get; set; }

        public Company(String sm, String nm)
        {
            symbol = sm;
            name = nm;
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

                    //JObject o1 = JObject.Parse(File.ReadAllText(@"C:\list.json"));
                    companies.Add(new Company("MSFT", "microsoft"));
                    companies.Add(new Company("GOOG", "google"));
                    companies.Add(new Company("AMZN", "Amazon"));
                    companies.Add(new Company("HPQ", "hp"));
                    companies.Add(new Company("CSCO", "Cisco"));
                    companies.Add(new Company("AAPL", "Apple"));
                }
                return companies;
            });
        }

    }

    class YahooFinances
    {
        public static List<Company> observingCompanies=new List<Company>();
        //gets stock value history
        //if
        //mode=0->last week;per day
        //mode=1->last month;per day
        //mode=2->lsat 6 months;per month
        //mode=3->last year; per month
        public async Task<List<CompanyValue>> GetCompanyHistory(int mode,String symbol)
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
                using (HttpResponseMessage response = await client.GetAsync("http://ichart.finance.yahoo.com/table.txt?a=" + (thisDay.Month - 1) + "&b=" + thisDay.Day + "&c=" + thisDay.Year + "&d=" + month + "&e=" + day + "&f=" + year + "&g="+interval+"&s="+symbol))
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
    }
}
/*
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace exampleRPG
{

    public class SummonerBasicInfo
    {

        [JsonProperty("id")]
        public String id { get; set; }

        [JsonProperty("name")]
        public String name { get; set; }

        [JsonProperty("profileIconId")]
        public String profileIconId { get; set; }

        [JsonProperty("summonerLevel")]
        public String summonerLevel { get; set; }
    }

    public class Match
    {
        [JsonProperty("gameMode")]
        public string gameMode { get; set; }

        [JsonProperty("subType")]
        public string subType { get; set; }

        [JsonProperty("fellowPlayers")]
        public List<Player> fellowPlayers { get; set; }
    }

    public class Player
    {
        [JsonProperty("summonerId")]
        public string summonerId { get; set; }

        [JsonProperty("championId")]
        public string championId { get; set; }
    }

    public class RESTCommunication
    {

        public async Task<SummonerBasicInfo> getSummonerBasicInfo(String summoner)
        {
            SummonerBasicInfo sum = new SummonerBasicInfo();

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("https://euw.api.pvp.net/api/lol/euw/v1.4/summoner/by-name/" + summoner + "?api_key=101d9000-e5e0-4aef-b97f-4e455a526e1f"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        JObject root = JObject.Parse(content);
                        JToken jtoken = root.First;
                        String info = ((JProperty)jtoken).Value.ToString();

                        sum = JsonConvert.DeserializeObject<SummonerBasicInfo>(info);
                    }
                }
            }
            return sum;
        }


        public async Task<List<Match>> getSummonerRecentMatchesInfo(string id)
        {

            string content = "test";
            List<Match> recMatches = new List<Match>();
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync("https://euw.api.pvp.net/api/lol/euw/v1.3/game/by-summoner/" + id + "/recent?api_key=101d9000-e5e0-4aef-b97f-4e455a526e1f"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();
                        JObject root = JObject.Parse(content);
                        JToken jtoken = root.Last;
                        String matches = ((JProperty)jtoken).Value.ToString();
                        recMatches = JsonConvert.DeserializeObject<List<Match>>(matches);
                    }
                }
            }
            return recMatches;
        }
    }
}
*/
