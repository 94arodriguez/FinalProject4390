using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Services.JobDataService;
using Services.JobService;
using Services.WebsiteStatusService;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Services.WebsiteService
{
    public class WebsiteServiceAPI
    {

        static WebsiteServiceAPI(){
        }
        
        public static async Task<string> GetLatestObservationAsync (string URL)
        {
            var output = await WebsiteServiceAPI.GetWebsiteStatusAsync(URL);

            return output!;
        }

        public static async Task<String> GetWebsiteStatusAsync(string URL)
        {
            string responseBody = "";
            try{
                using (var client = new HttpClient())
                {
                    string url = $"{URL}";
                    var result = await client.GetAsync(url);

                    responseBody = result.StatusCode.ToString();
                    // Console.WriteLine("The status of the website is " + result.StatusCode);
                }
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ",e.Message);
            }
            
            return responseBody;

        }

    }

    
}

