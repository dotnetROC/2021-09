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

		public static void WriteError(string msg)
		{
			ColoredWriteLine(msg, ConsoleColor.Red);
		}

		public static void WriteWarning(string msg)
		{
			ColoredWriteLine(msg, ConsoleColor.Yellow);
		}

		public static void WriteSuccess(string msg)
		{
			ColoredWriteLine(msg, ConsoleColor.Green);
		}

		private static void ColoredWriteLine(string msg, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(msg);
			Console.ResetColor();
		}
	}
}
