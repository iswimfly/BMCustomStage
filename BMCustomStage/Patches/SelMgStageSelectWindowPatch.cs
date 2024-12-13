using System;
using System.Runtime.CompilerServices;
using Flash2;
using Flash2.Selector.MainGame;
using Flash2.Selector.MainGame.PracticeMode.SMB1;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
    // Token: 0x0200000C RID: 12
    internal static class SelMgStageSelectWindowPatch
    {
        // Token: 0x06000032 RID: 50 RVA: 0x00004168 File Offset: 0x00002368
        public unsafe static void CreateDetour()
        {
            SelMgStageSelectWindowPatch.OnSubmitInstance = OnSubmit;
            SelMgStageSelectWindowPatch.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgStageSelectWindowPatch.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgStageSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgStageSelectWindowPatch.OnSubmitInstance);
        }

        // Token: 0x06000033 RID: 51 RVA: 0x000041E0 File Offset: 0x000023E0
        private static void OnSubmit(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr)
        {
            SelBestTimeItemData itemData = new SelBestTimeItemData(itemDataPtr);
            SelMgStageSelectWindowPatch.OnSubmitOriginal(thisPtr, playerIndex, inputLayer, itemDataPtr);
            
        }

        private static SelMgStageSelectWindowPatch.OnSubmitDelegate OnSubmitInstance;

        private static SelMgStageSelectWindowPatch.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

    }
}
