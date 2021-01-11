using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace BankOfDotNet.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
           using HttpClient httpClient = new HttpClient();
            var discovery = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5201");

            if (discovery.IsError)
            {
                Console.WriteLine(discovery.Error);
                return;
            }

            var token=new TokenClient(httpClient,new TokenClientOptions{ClientId = "Client",Address = discovery.TokenEndpoint,ClientSecret = "Secret"});

            var tokenResponse = await token.RequestClientCredentialsTokenAsync("BankOfDotNetApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.AccessToken);
            Console.WriteLine("____________________________");

            httpClient.SetBearerToken(tokenResponse.AccessToken);

            var customer =await httpClient.GetAsync("https://localhost:44337/customer");

            var customerJson = await customer.Content.ReadAsStringAsync();

            Console.WriteLine(JArray.Parse(customerJson));
        }
    }
}
