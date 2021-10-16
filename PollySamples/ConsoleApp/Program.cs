using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp
{
	static class Program
	{

		static async Task Main(string[] args)
		{
			await RunDemo().ConfigureAwait(false);
		}

		static async Task RunDemo()
		{
			HttpClient http = new HttpClient
			{
				BaseAddress = new Uri("https://jsonplaceholder.typicode.com/"),
				Timeout = TimeSpan.FromSeconds(2)
			};

			Console.WriteLine($"{Utils.DoubleDivider}\nTesting Polly.NET Policies!\n{Utils.DoubleDivider}");
			Utils.WaitToProceed();

			// demos
			//await RetryDemo.Run(http);

			//await FallbackDemo.Run(http);
			
			//await WaitAndRetryDemo.Run(http);
			//await WaitAndRetryDemo.RunWithExponentialBackoff(http);

			//await CircuitBreakerDemo.Run(http);

			await CachePolicyDemo.Run(http);

			Utils.WaitToProceed();
		}
	}
}
