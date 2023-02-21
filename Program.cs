using Ayyeka;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RestAPIExample
{
    internal static class Program
    {
        private const string AUTH_URL = "https://restapi.ayyeka.com/auth/token";
        private const string BASE_URL = "https://restapi.ayyeka.com/v2.0";
        /// <summary>
        /// Fill here the right API key and secret
        /// </summary>
        private const string API_KEY = "YOUR_API_KEY";
        private const string API_SECRET = "YOUR_API_SECRET";

        public static async Task Main(string[] args)
        {

            try
            {
                var tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET);

                var httpClient = new HttpClient();
               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStr);

                var apiClient = new APIClient(httpClient);                
                apiClient.BaseUrl = BASE_URL;

                var sites = await apiClient.SiteGetAsync();

                if (sites != null && sites.Count > 0)
                {
                    foreach (var site in sites)
                    {
                        await Console.Out.WriteLineAsync($"Found Sites: Id: { site.Id}, DisplayName: { site.Display_name}");
                    }
                }


                var streams = await  apiClient.StreamGetAsync();

                if (streams != null && streams.Count > 0)
                {
                    foreach (var stream in streams)
                    {
                        await Console.Out.WriteLineAsync($"Found Streams: Id: { stream.Id}, DisplayName: { stream.Display_name}, TypeId: {stream.Type_id}, TypeDisplayName: {stream.Type_display_name}, Unit: {stream.Units}");
                    }

                }

                while (true)
                {
                    StoredSampleScalarBatch? response = null;

                    var tokenRefreshed = false;
                    try
                    {
                        /*
                         * The first time you call this API, specify a sampleID or backfillHours to define the starting
                         * point from which to provide the batch of samples. In addition, set enableAck to true so that the
                         * system will not update its internal last delivered sample id field based on the last sample
                         * sent in this batch.
                         */
                        response = await apiClient.SampleBatchAsync(false,null,2);
                    }
                    catch (ApiException e)
                    {
                        if (e.StatusCode == (int)HttpStatusCode.Unauthorized)
                        {

                            tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET, tokenStr);
                            response = null;
                            tokenRefreshed = true;
                        }
                        else
                        {
                            throw;
                        }

                    }


                    if (response?.Samples != null)
                    {
                        /*
                         * Printing Out the new Samples
                         */
                        foreach (var sample in response.Samples)
                        {
                            await Console.Out.WriteLineAsync($"Found New Sample: ID: { sample.Id}, Value: { sample.Value}, SampleDate: {sample.Sample_date}, StreamId: {sample.Stream_id}");
                        }
                    }

                    /*
                     * If the batch consists of over 10,000 samples, it is broken down into chunks of no more than 10,000
                     * samples each, sent consecutively. The hasMore return flag indicates whether a given chunk is the
                     * last chunk.
                     */
                    if (tokenRefreshed || response == null || !response.HasMore)
                    {
                        //Going to sleep
                        Thread.Sleep(new TimeSpan(0, 15, 0));
                    }

                    tokenStr = await FetchOrValidateAccessToken(API_KEY, API_SECRET, tokenStr);

                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception when calling RestAPI: " + e.Message);
                Environment.Exit(1);
            }
        }

        private static async Task<string> FetchOrValidateAccessToken(string apiKey, string apiSecret, string tokenStr = null)
        {

            if (string.IsNullOrEmpty(tokenStr))
            {
                tokenStr = await GetAccessToken(apiKey, apiSecret);
            }


            if (DateTime.UtcNow > new JwtSecurityTokenHandler().ReadJwtToken(tokenStr).ValidTo)
            {
                tokenStr = await GetAccessToken(apiKey, apiSecret);
            }


            return tokenStr;
        }


        /// <summary>
        /// Method that will return an AccessToken from Ayyeka REST-API oAuth2 Service
        /// </summary>
        /// <param name="apiKey">APIKey from Ayyeka FAI Platform</param>
        /// <param name="apiSecret">API Secret from Ayyeka FAI Platform </param>
        /// <returns>Returns access token</returns>
        /// <exception cref="Exception"></exception>
        private static async Task<string> GetAccessToken(string apiKey, string apiSecret)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(AUTH_URL);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}")));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var result = await client.PostAsync(AUTH_URL, content);

                if (!result.IsSuccessStatusCode)
                    throw new Exception($"Failed to receive access token: ret code {result.StatusCode}");

                var resultContent = await result.Content.ReadAsStringAsync();
                var json = JObject.Parse(resultContent);

                return (string)json["access_token"];
            }
        }
    }
}