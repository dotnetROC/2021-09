using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Polly;

using ConsoleApp.Models;

namespace ConsoleApp
{
	public static class RetryDemo
	{
		public static async Task Run(HttpClient httpClient)
		{
			Utils.OutputSectionHeader("Retry Policy");

			// define the policy
			var retryPolicy = Policy
					.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.RetryAsync(
						3, 
						onRetry: (exception, retryCount) => { 
							Utils.WriteWarning($"\tCall failed; retry attempt {retryCount}"); 
					});

			// execute operation
			Random rand = new Random();
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine($"API Call #{i+1}:");

				var response = await retryPolicy
						.WrapAsync(Utils.GetHttpChoas())
						.ExecuteAsync(async () => await httpClient.GetAsync($"/users/{rand.Next(1, 10)}").ConfigureAwait(false))
						.ConfigureAwait(false);

				if (response.IsSuccessStatusCode)
				{
					var data = JsonConvert
							.DeserializeObject<User>(await response.Content.ReadAsStringAsync()
							.ConfigureAwait(false));
					Utils.WriteSuccess($"\tData: {data}");
				}
				else
				{
					Utils.WriteError("Failed to get a successful response");
				}
			}

			Utils.WaitToProceed();
		}
	}
}
