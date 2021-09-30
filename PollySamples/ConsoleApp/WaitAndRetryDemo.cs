using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

using ConsoleApp.Models;
using Polly.Retry;

namespace ConsoleApp
{
	public static class WaitAndRetryDemo
	{
		public static async Task Run(HttpClient httpClient)
		{
			Utils.OutputSectionHeader("Wait & Retry Policy - Standard");

			// define the policy
			var retryPolicy = Policy
					.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.WaitAndRetryAsync(
						new[] {
							TimeSpan.FromSeconds(1),
							TimeSpan.FromSeconds(2),
							TimeSpan.FromSeconds(3)
						}, 
						onRetry: (exception, retryCount) => { 
							Utils.WriteWarning($"\tCall failed; retry attempt {retryCount}"); 
					});

			// execute operation
			await RunDemo(httpClient, retryPolicy).ConfigureAwait(false);
		}

		public static async Task RunWithExponentialBackoff(HttpClient httpClient)
		{
			Utils.OutputSectionHeader("Wait & Retry Policy - Exponential Backoff");

			// define the policy
			var retryPolicy = Policy
					.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.WaitAndRetryAsync(
						3,
						retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
						onRetry: (exception, retryCount) =>
						{
							Utils.WriteWarning($"\tCall failed; retry attempt {retryCount}");
						}
					);

			// execute operation
			await RunDemo(httpClient, retryPolicy).ConfigureAwait(false);
		}

		private static async Task RunDemo(HttpClient httpClient, AsyncRetryPolicy<HttpResponseMessage> retryPolicy)
		{
			Random rand = new Random();
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine($"API Call #{i + 1}:");

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
