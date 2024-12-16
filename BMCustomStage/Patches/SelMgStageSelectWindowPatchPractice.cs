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
    
    internal static class SelMgStageSelectWindowPatchPractice
    {
        
        public unsafe static void CreateDetour()
        {
            SelMgStageSelectWindowPatchPractice.OnSubmitInstance = OnSubmit;
            SelMgStageSelectWindowPatchPractice.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgStageSelectWindowPatchPractice.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgStageSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgStageSelectWindowPatchPractice.OnSubmitInstance);
        }

        private static void OnSubmit(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr)
        {
            SelBestTimeItemData itemData = new SelBestTimeItemData(itemDataPtr);
            SelMgStageSelectWindowPatchPractice.OnSubmitOriginal(thisPtr, playerIndex, inputLayer, itemDataPtr);

        }

        private static SelMgStageSelectWindowPatchPractice.OnSubmitDelegate OnSubmitInstance;

        private static SelMgStageSelectWindowPatchPractice.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

    }
}

