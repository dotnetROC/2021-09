using System;

namespace ConsoleApp
{
	public static class Utils
	{
		public const string DoubleDivider = "===========================";
		public const string Divider = "---------------------------";

		public static void OutputSectionHeader(string header)
		{
			Console.WriteLine($"{Utils.Divider}\n{header}\n{Utils.Divider}\n");
		}

		public static void WaitToProceed()
		{
			Console.WriteLine("\nPress any key to continue...\n");
			Console.ReadKey();
		}
	}
}
