using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CMOVStockApp.Models
{
    class User
    {
        private static String savedUsername;
        public class Body
        {
            [JsonProperty("response")]
            public String response { get; set; }
        }

        public class Response
        {
            [JsonProperty("status")]
            public String status { get; set; }
            [JsonProperty("body")]
            public Body body { get; set; }
        }

        public class MycompaniesResponse
        {
            [JsonProperty("status")]
            public String status { get; set; }
            [JsonProperty("body")]
            public MycompaniesResponseBody body { get; set; }
        }

        public class MycompaniesResponseBody
        {
            [JsonProperty("response")]
            public List<Company> response { get; set; }
        }

        public async static Task<Response> signIn(String username, String password)
        {
            try
            {
                String content = "";
                Response rsp = null;

                using (HttpClient client = new HttpClient())
                {
                    StringContent body = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}");
                    body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                    using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/signin", body))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            content = await response.Content.ReadAsStringAsync();//0,4
                            rsp = JsonConvert.DeserializeObject<Response>(content);
                            savedUsername = username;
                            return rsp;
                        }

                        else { return rsp; }

                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static Task<Response> signUp(String username, String password)
        {
            try
            {
                String content = "";
                Response rsp = null;

                using (HttpClient client = new HttpClient())
                {
                    StringContent body = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}");
                    body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                    using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/signup", body))
                    {

                        if (response.IsSuccessStatusCode)
                        {
                            content = await response.Content.ReadAsStringAsync();//0,4
                            rsp = JsonConvert.DeserializeObject<Response>(content);
                            return rsp;
                        }

                        else { return rsp; }

                    }
                }
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async static Task<MycompaniesResponse> myCompanies(String username)
        {
            String content = "";
            MycompaniesResponse rsp = null;

            using (HttpClient client = new HttpClient())
            {
                StringContent body = new StringContent("{\"username\":\"" + username + "\"}");
                body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/mycompanies", body))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        rsp = JsonConvert.DeserializeObject<MycompaniesResponse>(content);
                        return rsp;
                    }

                    else { return rsp; }

                }
            }
        }

        public async static Task<Boolean> addCompanies()
        {
            String content = "{\"companies\":[";
            Response rsp = null;

            for (int i = 0; i < YahooFinance.observingCompanies.Count; i++)
            {
                content += "{\"NAME\":\"" + YahooFinance.observingCompanies.ElementAt(i).name + "\",\"SYMBOL\":\"" + YahooFinance.observingCompanies.ElementAt(i).symbol + "\",\"MIN\":" + YahooFinance.observingCompanies.ElementAt(i).min + ",\"MAX\":" + YahooFinance.observingCompanies.ElementAt(i).max + "}";
                if (i < YahooFinance.observingCompanies.Count - 1)
                {
                    content += ",";
                }
            }
            content += "],\"username\":\"" + savedUsername + "\"}";


            using (HttpClient client = new HttpClient())
            {

                StringContent body = new StringContent(content);
                body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/addcompanies", body))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        rsp = JsonConvert.DeserializeObject<Response>(content);
                        return true;
                    }

                    else { return false; }

                }
            }
        }
    }
}
