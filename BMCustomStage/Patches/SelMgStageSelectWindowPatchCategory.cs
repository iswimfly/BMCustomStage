using System;
using System.Runtime.CompilerServices;
using Flash2;
using Flash2.Selector.MainGame;
using Flash2.Selector.MainGame.StoryMode;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{

    internal static class SelMgStageSelectWindowPatchCategory
    {

        public unsafe static void CreateDetour()
        {
            SelMgStageSelectWindowPatchCategory.OnSubmitInstance = OnSubmit;
            SelMgStageSelectWindowPatchCategory.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgStageSelectWindowPatchCategory.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgStageSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgStageSelectWindowPatchCategory.OnSubmitInstance);
        }

        private static void OnSubmit(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr)
        {
            SelIconAndBestTimeItemData itemData = new SelIconAndBestTimeItemData(itemDataPtr);
            SelMgStageSelectWindowPatchCategory.OnSubmitOriginal(thisPtr, playerIndex, inputLayer, itemDataPtr);

        }

        private static SelMgStageSelectWindowPatchCategory.OnSubmitDelegate OnSubmitInstance;

        private static SelMgStageSelectWindowPatchCategory.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

    }
}

