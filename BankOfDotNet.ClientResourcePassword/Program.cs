using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace BankOfDotNet.ClientResourcePassword
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using HttpClient client=new ();
            client.BaseAddress=new Uri("https://localhost:5201/connect/token");
            var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5201");

            if (discovery.IsError)
            {
                Console.WriteLine(discovery.Error);
                return;
            }
            //client.BaseAddress = new Uri(discovery.AuthorizeEndpoint);
            var tokenClient=new TokenClient(client,new TokenClientOptions{ClientId = "T.Client",ClientSecret = "secret"});

            var tokenResponse = await tokenClient.RequestPasswordTokenAsync("Babak", "babak@", "BankOfDotNetApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.AccessToken);
            Console.WriteLine("________________________________");

         
            client.SetBearerToken(tokenResponse.AccessToken);

            var customer = await client.GetAsync("https://localhost:5001/customer");

            var result = await customer.Content.ReadAsStringAsync();

            Console.WriteLine(JArray.Parse(result));

            Console.ReadKey();
        }
    }
}
