using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Polly;
using Polly.Contrib.Simmy;
using Polly.Contrib.Simmy.Outcomes;

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

			// define some choas
			var badResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
			var chaosPolicy = MonkeyPolicy.InjectResultAsync<HttpResponseMessage>(with =>
				with.Result(badResponse)
					.InjectionRate(0.5)
					.Enabled());

			// execute operation
			Random rand = new Random();
			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine($"API Call #{i+1}:");

				var response = await retryPolicy
						.WrapAsync(chaosPolicy)
						.ExecuteAsync(async () => await httpClient.GetAsync($"/users/{rand.Next(1, 10)}").ConfigureAwait(false));

				if (response.IsSuccessStatusCode)
				{
					var data = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
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
