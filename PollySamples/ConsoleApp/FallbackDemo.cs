using ConsoleApp.Models;

using Newtonsoft.Json;

using Polly;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
	public static class FallbackDemo
	{
		public static async Task Run(HttpClient httpClient)
		{
			Utils.OutputSectionHeader("Fallback Policy");

			User fallbackUser = new User {
				Id = 0,
				Name = "Fallback User",
				Email = "user@fallback.org",
				Username = "fallbackUser"
			};

			// define the policy
			var fallbackPolicy = Policy
					.Handle<Exception>()
					.OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.FallbackAsync<HttpResponseMessage>(
						fallbackValue: new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
								Content = new StringContent(JsonConvert.SerializeObject(fallbackUser))
						},
						onFallbackAsync: async e => { Utils.WriteWarning("\tFallback triggered"); }
					);

			// execute operation
			Random rand = new Random();
			for (int i = 0; i < 10; i++)
			{
				// execute using the policy
				var result = await fallbackPolicy
						.WrapAsync(Utils.GetHttpChoas())
						.ExecuteAsync(async () => await httpClient.GetAsync($"/users/{rand.Next(1, 10)}").ConfigureAwait(false))
						.ConfigureAwait(false);

				string jsonContent = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
				User user = JsonConvert.DeserializeObject<User>(jsonContent);
				Utils.WriteSuccess($"API Call #{i+1}: '{user}'");
			}

			Utils.WaitToProceed();
		}
	}
}
