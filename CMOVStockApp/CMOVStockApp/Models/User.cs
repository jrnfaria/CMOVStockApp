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
        public class Body
        {
            [JsonProperty("response")]
            public String response { get; set; }
        }

        public class SignResponse
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

        public async static Task<SignResponse> signIn(String username, String password)
        {
            String content = "";
            SignResponse rsp=null;

            using (HttpClient client = new HttpClient())
            {
                StringContent body = new StringContent("{\"username\":\""+ username + "\",\"password\":\"" + password +"\"}");
                body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/signin", body))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        rsp = JsonConvert.DeserializeObject<SignResponse>(content);
                        return rsp;
                    }

                    else { return rsp; }

                }
            }
        }

        public async static Task<SignResponse> signUp(String username, String password)
        {
            String content = "";
            SignResponse rsp = null;

            using (HttpClient client = new HttpClient())
            {
                StringContent body = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}");
                body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                using (HttpResponseMessage response = await client.PostAsync("http://localhost:8080/signup", body))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        rsp = JsonConvert.DeserializeObject<SignResponse>(content);
                        return rsp;
                    }

                    else { return rsp; }

                }
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
    }
}
