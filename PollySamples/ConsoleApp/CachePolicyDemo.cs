using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Polly;
using Polly.Caching.Memory;

using ConsoleApp.Models;

namespace ConsoleApp
{
    public static class CachePolicyDemo
    {
        // cache provider should be a singleton
        static readonly MemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        static readonly MemoryCacheProvider memoryCacheProvider = new MemoryCacheProvider(memoryCache);
        static readonly Random rand = new Random();

        public static async Task Run(HttpClient httpClient)
        {
            Utils.OutputSectionHeader("Cache Policy");

            // define the policy
            var cachePolicy = Policy.CacheAsync(
                    memoryCacheProvider, 
                    TimeSpan.FromMinutes(5),
                    onCacheGet: (context, str) => Utils.WriteSuccess("\tCache Hit!"),
                    onCacheMiss: (context, str) => Utils.WriteWarning("\tCache miss"),
                    onCachePut: (context, str) => Utils.WriteSuccess("\tItem added to the Cache!"),
                    onCacheGetError: (context, str, ex) => Utils.WriteError($"\tError encountered getting an item - {ex.Message}"),
                    onCachePutError: (context, str, ex) => Utils.WriteError($"\tError encountered putting an item - {ex.Message}")
                    );

            // call the endpoint repeatedly
            for (int i = 0; i < 10; i++)
            {
                var id = rand.Next(1, 10);

                Console.WriteLine($"Loading user #{id}");

                User user = await cachePolicy.ExecuteAsync(context => GetUser(httpClient, id), new Context($"user-{id}"));

                Console.WriteLine($"\tUser '{user.Username}' retrieved");
            }
        }

        static async Task<User> GetUser(HttpClient httpClient, int id)
        {
            var response = await httpClient.GetAsync($"/users/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<User>(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                Utils.WriteSuccess($"\tUser '{data.Username}' retrieved from remote endpoint");
                return data;
            }

            throw new Exception("Error loading user");
        }
    }
}
