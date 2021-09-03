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
			var fallbackPolicy = Policy
					.Handle<Exception>()
					.OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.FallbackAsync<HttpResponseMessage>(
						fallbackValue: new HttpResponseMessage(System.Net.HttpStatusCode.OK) {
								Content = new StringContent("FALLBACK RESULT!")
						},
						onFallbackAsync: async e => { Console.WriteLine("\tFallback triggered"); }
					);

			for (int i = 0; i < 10; i++)
			{
				// execute using the policy
				var result = await fallbackPolicy
						.ExecuteAsync(async () => await httpClient.GetAsync("fallback").ConfigureAwait(false))
						.ConfigureAwait(false);

				var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
				Console.WriteLine($"Attempt #{i}: '{content}'");
				Thread.Sleep(500);
			}

			Utils.WaitToProceed();
		}
	}
}
