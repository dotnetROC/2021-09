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

			// define the policy
			var fallbackPolicy = Policy<string>
					.HandleResult(r => r == "failure")
					.FallbackAsync<string>(
						fallbackValue: "Fallback value",
						onFallbackAsync: async x => Console.WriteLine("\tFallback triggered"));

			for (int i = 0; i < 10; i++)
			{
				// execute using the policy
				var result = await fallbackPolicy
						.ExecuteAsync(async () => await InvokeFallbackEndpoint(httpClient).ConfigureAwait(false))
						.ConfigureAwait(false);

				Console.WriteLine($"Attempt #{i}: '{result}'");
				Thread.Sleep(500);
			}

			Utils.WaitToProceed();
		}

		static async Task<string> InvokeFallbackEndpoint(HttpClient http)
		{
			// invoke endpoint that randomly fails
			var result = await http.GetAsync("fallback");

			return result.IsSuccessStatusCode
					? await result.Content.ReadAsStringAsync().ConfigureAwait(false)
					: "failure";
		}
	}
}
