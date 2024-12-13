using System;

namespace BMCustomStage
{
	
	internal static class Log
	{
		
		private static void PrintMessage(string message, ConsoleColor foregroundColour)
		{
			Console.ResetColor();
			Console.Write('[');
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("CustomStageLoader");
			Console.ResetColor();
			Console.Write("] ");
			Console.ForegroundColor = foregroundColour;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		
		public static void Config(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Blue);
		}

		
		public static void Info(string message)
		{
			Log.PrintMessage(message, ConsoleColor.White);
		}

		
		public static void Success(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Green);
		}

		
		public static void Warning(string message)
		{
			Log.PrintMessage(message, ConsoleColor.DarkYellow);
		}

		
		public static void Error(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Red);
		}
	}
}
