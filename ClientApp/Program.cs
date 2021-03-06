﻿using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApp
{
    public class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();


        private static async Task MainAsync()
        {
            //pronalazenje endpoint-a
            var disco = await DiscoveryClient.GetAsync("http://localhost:57216");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            //slanje zahteva za token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");

            //pomocu client credentials
            //  var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            //pomocu client password-a
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            //pozivanje API-ja
            var client = new HttpClient();

            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:57216/identity");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(JArray.Parse(content));
            }

        }
    }


}

