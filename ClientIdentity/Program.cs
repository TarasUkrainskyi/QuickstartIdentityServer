using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientIdentity
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ClientIdentity", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("ApiIdentity");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.WriteLine("\n\n");
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");
            Console.ReadLine();

            // request token
            var tokenClient1 = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse1 = await tokenClient1.RequestResourceOwnerPasswordAsync("alice", "password", "ApiIdentity");

            if (tokenResponse1.IsError)
            {
                Console.WriteLine(tokenResponse1.Error);
                return;
            }

            Console.WriteLine(tokenResponse1.Json);
            Console.WriteLine("\n\n");
            Console.ReadLine();

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                Console.ReadLine();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
                Console.ReadLine();
            }
        }
    }
}
