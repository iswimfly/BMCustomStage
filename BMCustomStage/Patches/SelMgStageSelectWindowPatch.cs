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
    
    internal static class SelMgStageSelectWindowPatch
    {
        
        public unsafe static void CreateDetour()
        {
            SelMgStageSelectWindowPatch.OnSubmitInstance = OnSubmit;
            SelMgStageSelectWindowPatch.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgStageSelectWindowPatch.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgStageSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgStageSelectWindowPatch.OnSubmitInstance);
        }

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
