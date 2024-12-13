using System;
using System.Runtime.CompilerServices;
using Flash2;
using Flash2.Selector.MainGame;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
	
	internal static class SelMgModeSelectWindowPatch
	{
		
		public unsafe static void CreateDetour()
		{
			SelMgModeSelectWindowPatch.OnSubmitInstance = OnSubmit;
			SelMgModeSelectWindowPatch.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgModeSelectWindowPatch.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgModeSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgModeSelectWindowPatch.OnSubmitInstance);
		}

		
		private static void OnSubmit(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr)
		{
			SelTextItemData itemData = new SelTextItemData(itemDataPtr);
			SelMgModeItemData modeItemData = itemData.Cast<SelMgModeItemData>();
			SelMgModeSelectWindowPatch.OnSubmitOriginal(thisPtr, playerIndex, inputLayer, itemDataPtr);
			if (modeItemData.mainGamemode == (SelectorDef.MainGameKind)8)
			{
				GameParam.selectorParam.practiceSelectedCourse = (MainGameDef.eCourse)44;
				GameParam.selectorParam.selectedCourse = (MainGameDef.eCourse)44;
				GameParam.selectorParam.selectedStageIndex = 0;
			}
		}

		private static SelMgModeSelectWindowPatch.OnSubmitDelegate OnSubmitInstance;

		private static SelMgModeSelectWindowPatch.OnSubmitDelegate OnSubmitOriginal;

		private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

	}
}
