using System;
using System.Runtime.CompilerServices;
using Flash2;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
	
	internal static class GameParamPatch
	{
		
		public unsafe static void CreateDetour()
		{
			ResetParamInstance = ResetParam;
			ResetParamOriginal = ClassInjector.Detour.Detour<GameParamPatch.ResetParamDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(GameParam).GetMethod("ResetParam")).GetValue(null)))).MethodPointer, GameParamPatch.ResetParamInstance);
		}

		private static void ResetParam(IntPtr _thisPtr, bool isOnBoot)
		{
            GameParamPatch.ResetParamOriginal(_thisPtr, isOnBoot);
            GameParam gameParam = new GameParam(_thisPtr);
            bool flag = gameParam.m_SkipParam.m_SkipMgHowToPlayArray.Length <= 8;
            if (flag)
            {
                bool[] skipHowToPlayArray = new bool[10];
                for (int i = 0; i < gameParam.m_SkipParam.m_SkipMgHowToPlayArray.Count; i++)
                {
                    skipHowToPlayArray[i] = gameParam.m_SkipParam.m_SkipMgHowToPlayArray[i];
                }
                skipHowToPlayArray[8] = false;
                skipHowToPlayArray[9] = false;
                gameParam.m_SkipParam.m_SkipMgHowToPlayArray = skipHowToPlayArray;
            }
        }

		private static GameParamPatch.ResetParamDelegate ResetParamInstance;
		
		private static GameParamPatch.ResetParamDelegate ResetParamOriginal;

		private delegate void ResetParamDelegate(IntPtr _thisPtr, bool isOnBoot);

	}
}
