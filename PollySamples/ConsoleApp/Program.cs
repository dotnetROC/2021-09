using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp
{
	static class Program
	{
		static HttpClient _http;

		static async Task Main(string[] args)
		{
			await RunDemo().ConfigureAwait(false);
		}

		static async Task RunDemo()
		{
			_http = new HttpClient
			{
				BaseAddress = new Uri("https://jsonplaceholder.typicode.com/"),
				Timeout = TimeSpan.FromSeconds(2)
			};

			Console.WriteLine($"{Utils.DoubleDivider}\nTesting Polly.NET Policies!\n{Utils.DoubleDivider}");
			Utils.WaitToProceed();

			// demos
			await RetryDemo.Run(_http);

			await FallbackDemo.Run(_http);
			
			await WaitAndRetryDemo.Run(_http);
			await WaitAndRetryDemo.RunWithExponentialBackoff(_http);


			Utils.WaitToProceed();
		}
	}
}
