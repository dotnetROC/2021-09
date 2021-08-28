using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp
{
	static class Program
	{
		static HttpClient _http;

		static void Main(string[] args)
		{
			RunDemo().GetAwaiter().GetResult();
		}

		static async Task RunDemo()
		{
			_http = new HttpClient
			{
				BaseAddress = new Uri("http://localhost:6038/"),
				Timeout = TimeSpan.FromSeconds(2)
			};

			Console.WriteLine($"{Utils.DoubleDivider}\nTesting Polly.NET Policies!\n{Utils.DoubleDivider}");
			Utils.WaitToProceed();

			await FallbackDemo.Run(_http);
		}
	}
}
