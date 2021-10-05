using ConsoleApp.Models;

using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
	public static class CircuitBreakerDemo
	{
		public static async Task Run(HttpClient httpClient)
		{
			Utils.OutputSectionHeader("Circuit Breaker");

			// define the policy
			var circuitBreakerPolicy = Policy
					.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
					.CircuitBreakerAsync(
						2,
						TimeSpan.FromSeconds(10),
						onBreak: (exception, timespan) =>
						{
							Utils.WriteWarning($"\t\tCircuit Breaker triggered open until {DateTimeOffset.Now.Add(timespan).TimeOfDay}");
						},
						onReset: () =>
						{
							Utils.WriteWarning("\tCircuit Breaker reset (closed)");
						}
					);

			// execute operation
			Random rand = new Random();
			bool successful = false;

			for (int i = 0; i < 3; i++)
			{
				Console.WriteLine($"Demo run #{i + 1}");

				int count = 0;

				do
				{
					Console.WriteLine($"\t({DateTimeOffset.Now.TimeOfDay}) API Call Attempt #{count + 1}:");

					try
					{
						Console.WriteLine($"\t\tCurrent circuit breaker status: {circuitBreakerPolicy.CircuitState}");

						var response = await circuitBreakerPolicy
								.WrapAsync(Utils.GetHttpChoas(0.8))
								.ExecuteAsync(async () => await httpClient.GetAsync($"/users/{rand.Next(1, 10)}").ConfigureAwait(false))
								.ConfigureAwait(false);

						if (response.IsSuccessStatusCode)
						{
							var data = JsonConvert
									.DeserializeObject<User>(await response.Content.ReadAsStringAsync()
									.ConfigureAwait(false));

							Utils.WriteSuccess($"\t\tData: {data}");
							break;
						}
						else
						{
							Utils.WriteError("\t\tFailed to get a successful response");
						}
					}
					catch (BrokenCircuitException bce)
					{
						// handle the Circuit Breaker Exception
						Utils.WriteWarning($"\t\tFailed fast because circuit breaker is open. Message = {bce.Message}");
					}

					Thread.Sleep(TimeSpan.FromSeconds(1.5));

					count++;
				} while (!successful);
			}
		}
	}
}
