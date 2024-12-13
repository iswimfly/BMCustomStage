using System;

namespace BMCustomStage
{
	// Token: 0x02000006 RID: 6
	internal static class Log
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002668 File Offset: 0x00000868
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

		// Token: 0x0600001D RID: 29 RVA: 0x000026BC File Offset: 0x000008BC
		public static void Config(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Blue);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000026C7 File Offset: 0x000008C7
		public static void Info(string message)
		{
			Log.PrintMessage(message, ConsoleColor.White);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000026D2 File Offset: 0x000008D2
		public static void Success(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Green);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026DD File Offset: 0x000008DD
		public static void Warning(string message)
		{
			Log.PrintMessage(message, ConsoleColor.DarkYellow);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026E7 File Offset: 0x000008E7
		public static void Error(string message)
		{
			Log.PrintMessage(message, ConsoleColor.Red);
		}
	}
}
