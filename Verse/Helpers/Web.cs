using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Verse.Helpers
{
    public static class Web
    {
        public static string PostRequest(string url, string data, ILogger logger = null)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");
            HttpResponseMessage response = client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger?.LogWarning("POST error\n" + url + "\n" + response.ToString());
                return "";
            }
            
            byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;

            string stringResponse = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            logger?.LogInformation(stringResponse);
            return stringResponse;
        }
        
        public static string GetRequest(string url, ILogger logger = null)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            if (response.StatusCode != HttpStatusCode.OK)
            {
                logger?.LogWarning("GET error\n" + url + "\n" + response.ToString());
                return "";
            }
            
            byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;

            string stringResponse = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            logger?.LogInformation(stringResponse);
            return stringResponse;
        }
    }
}