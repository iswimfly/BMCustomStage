using System;
using System.Runtime.CompilerServices;
using Flash2;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
	// Token: 0x0200000A RID: 10
	internal static class GameParamPatch
	{
		// Token: 0x0600002B RID: 43 RVA: 0x000037CC File Offset: 0x000019CC
		public unsafe static void CreateDetour()
		{
			ResetParamInstance = ResetParam;
			ResetParamOriginal = ClassInjector.Detour.Detour<GameParamPatch.ResetParamDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(GameParam).GetMethod("ResetParam")).GetValue(null)))).MethodPointer, GameParamPatch.ResetParamInstance);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003844 File Offset: 0x00001A44
		private static void ResetParam(IntPtr _thisPtr, bool isOnBoot)
		{
			GameParamPatch.ResetParamOriginal(_thisPtr, isOnBoot);
			GameParam gameParam = new GameParam(_thisPtr);
			bool flag = gameParam.m_SkipParam.m_SkipMgHowToPlayArray.Length <= 8;
			if (flag)
			{
				bool[] skipHowToPlayArray = new bool[9];
				Array.Copy(gameParam.m_SkipParam.m_SkipMgHowToPlayArray, skipHowToPlayArray, 8);
				skipHowToPlayArray[8] = false;
				gameParam.m_SkipParam.m_SkipMgHowToPlayArray = skipHowToPlayArray;
			}
		}

		// Token: 0x04000015 RID: 21
		private static GameParamPatch.ResetParamDelegate ResetParamInstance;

		// Token: 0x04000016 RID: 22
		private static GameParamPatch.ResetParamDelegate ResetParamOriginal;

		// Token: 0x02000016 RID: 22
		// (Invoke) Token: 0x0600004B RID: 75
		private delegate void ResetParamDelegate(IntPtr _thisPtr, bool isOnBoot);

	}
}
