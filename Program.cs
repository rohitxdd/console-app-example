using Microsoft.Extensions.Configuration;
using Reconsile.Models;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Transactions;
using System.Xml;

namespace Reconsile
{
    internal class Program
    {
        private static readonly string BaseUrl = "https://example.in/";
        private static string token = string.Empty;
        private static readonly DataAccess da = new();
        static async Task Main(string[] args)
        {
            LogManager.Logger.Information("Application started");
            string basePath = AppContext.BaseDirectory;
            string logDirectory = Path.Combine(basePath, "logs");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var res = await da.ListOfCandidateWithSuccessButNotdate();

            LogManager.Logger.Information($"Candidates with dummy number but no transdate, Total Candidates : {res.Count()}");

            foreach (var candidate in res) 
            {
                await GetFeeDetail(candidate.applid.ToString());
            }

            LogManager.Logger.Information("Application finished");
        }

        static async Task GetFeeDetail(string applid)
        {
            HttpClient client = new();
            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            if (string.IsNullOrEmpty(token))
            {
                token = await GetAuthToken();
            }
            client.DefaultRequestHeaders.Add("Authorization", token);

            var fromdate = ""; 
            var todate = ""; 

            var formData = new StringContent($"applid={applid}&fromdate={fromdate}&todate={todate}", Encoding.UTF8, "application/x-www-form-urlencoded");

            try
            {
                HttpResponseMessage response = await client.PostAsync("ReverifyPaymentStatus.asmx/getConciledata", formData);
                response.EnsureSuccessStatusCode(); 
                string responseXml = await response.Content.ReadAsStringAsync();
                var responseBody = ExtractResponseBody(responseXml);
                if(responseBody.Trim() == "Authentication token is expired")
                {
                    token = await GetAuthToken();
                    await GetFeeDetail(applid);
                }
                if (!string.IsNullOrEmpty(responseBody)) 
                { 
                    List<PaymentTransaction>? transactions = JsonSerializer.Deserialize<List<PaymentTransaction>>(responseBody);
                    //handle response
                    LogManager.Logger.Information($"Done with applid : {applid}");
                }
                Console.WriteLine(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                LogManager.Logger.Error($"Request error: {e.Message}");
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.ToString());
                LogManager.Logger.Error($"error: {e.Message}");
            }
        }

        static async Task<string> GetAuthToken()
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri(BaseUrl)
            };
            try
            {
                HttpResponseMessage response = await client.GetAsync("ReverifyPaymentStatus.asmx/GetToken");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                return ExtractTokenValue(responseBody);
            }
            catch (HttpRequestException e)
            {
                return e.Message;
            }
        }

        static string ExtractTokenValue(string xmlStr)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlStr);

            if (xmlDoc.DocumentElement != null)
            {
                string tokenValue = xmlDoc.DocumentElement.InnerText.Trim('"');
                LogManager.Logger.Information($"Access Token Generated ${tokenValue}");
                return tokenValue;
            }
            throw new Exception($"Token is empty: {xmlStr}");
        }

        static string ExtractResponseBody(string xmlStr)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlStr);
            if (xmlDoc.DocumentElement != null)
            {
                string json = xmlDoc.DocumentElement.InnerText;
                return json;
            }
            throw new Exception($"Body is empty: {xmlStr}");
        }
    }
}
