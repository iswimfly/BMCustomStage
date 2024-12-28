using Flash2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
    internal class SelPauseWindowPatch
    {
        public static unsafe void CreateDetour()
        {
            SelPauseWindowPatch.TestInstance = changeToMgStageSelect;
            SelPauseWindowPatch.TestOriginal = ClassInjector.Detour.Detour<SelPauseWindowPatch.PauseDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelPauseWindow).GetMethod("changeToMgStageSelect")).GetValue(null)))).MethodPointer, SelPauseWindowPatch.TestInstance);
        }

        private static void changeToMgStageSelect(IntPtr thisPtr, IntPtr selPauseSelectItemDataPtr)
        {
            Il2CppStructArray<MainGameDef.eCourse> storyDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict["original"].Count);

            for (int j = 0; j < Main.storyDict["original"].Count; j++)
            {
                storyDef[j] = Main.storyDict["original"][j];
            }
            MainGameDef._storyCourses_k__BackingField = storyDef;
            SelPauseWindowPatch.TestOriginal(thisPtr, selPauseSelectItemDataPtr);
        }

        private static SelPauseWindowPatch.PauseDelegate TestInstance;


        private static SelPauseWindowPatch.PauseDelegate TestOriginal;

        private delegate void PauseDelegate(IntPtr thisPtr, IntPtr selPauseSelectItemDataPtr);
    }
}
