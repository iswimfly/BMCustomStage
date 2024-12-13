using System;
using System.Runtime.CompilerServices;

namespace BMCustomStage.Patches
{
	
	internal class TestPatch
	{

		public static void CreateDetour()
		{

		}

		private static void Test(IntPtr thisPtr)
		{
			TestPatch.TestOriginal(thisPtr);
		}

		private static TestPatch.TestDelegate TestInstance;


		private static TestPatch.TestDelegate TestOriginal;

		private delegate void TestDelegate(IntPtr thisPtr);

	}
}
