using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CMOVStockApp.Models
{
    class User
    {
        public async static Task<String> login(String username, String password)
        {
            String content = "";

            using (HttpClient client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                 {
                     { "username", username},
                     { "password", password }
                 };

                var body = new FormUrlEncodedContent(values);

                using (HttpResponseMessage response = await client.PostAsync("http://www.example.com/recepticle.aspx", body))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();//0,4
                        return content;
                    }
                    else { return content; }

                }
            }
        }
    }
}
