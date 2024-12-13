using System;
using System.Runtime.CompilerServices;
using Flash2;
using Flash2.Selector.MainGame;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
	// Token: 0x0200000C RID: 12
	internal static class SelMgModeSelectWindowPatch
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00004168 File Offset: 0x00002368
		public unsafe static void CreateDetour()
		{
			SelMgModeSelectWindowPatch.OnSubmitInstance = OnSubmit;
			SelMgModeSelectWindowPatch.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgModeSelectWindowPatch.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgModeSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgModeSelectWindowPatch.OnSubmitInstance);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000041E0 File Offset: 0x000023E0
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
