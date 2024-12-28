using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Flash2;
using Flash2.Selector;
using Flash2.Selector.MainGame;
using Flash2.Selector.MainGame.ChallengeMode.SMB1;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;

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
            SelMainMenuWindowBase worlds = UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>().m_WindowCollection[SelectorDef.MainMenuKind.MgChallengeCourseSelect_SMB1];
            SelMgCourseSelectWindow selMgCourseSelectWindow = new SelMgCourseSelectWindow(worlds.Pointer);
            if (modeItemData.mainGamemode == (SelectorDef.MainGameKind)8 || modeItemData.mainGamemode == (SelectorDef.MainGameKind)9 || modeItemData.mainGamemode == (SelectorDef.MainGameKind)10)
            {
                if (modeItemData.mainGamemode == (SelectorDef.MainGameKind)8)
                {
                    GameParam.selectorParam.selectedCourse = (MainGameDef.eCourse)44;
                    GameParam.selectorParam.practiceSelectedCourse = (MainGameDef.eCourse)44;
                }
                else if (modeItemData.mainGamemode == (SelectorDef.MainGameKind)9 || modeItemData.mainGamemode == (SelectorDef.MainGameKind)10)
                {
                    foreach (CustomCourse course in DataManager.Courses)
                    {
                        if (modeItemData.mainGamemode == (SelectorDef.MainGameKind)10)
                        {
                            {
                                if (GameParam.Instance.m_selectorParam.mainGameMode == (SelectorDef.MainGameKind)10)
                                {
                                    if (modeItemData.textKey.Contains(course.CourseName))
                                    {
                                        selMgCourseSelectWindow.m_MgCourseData = Main.categoryDict[course.CourseName];

                                    }
                                }
                            }
                        }
                        if (modeItemData.textKey.Contains(course.CourseName))
                        {
                            Main.selectedCourse = course.CourseName;
                        }
                    }
                }
                GameParam.selectorParam.selectedStageIndex = 0;
            }
            else
            {
                Main.selectedCourse = null;
                selMgCourseSelectWindow.m_MgCourseData = Main.categoryDict["original"];
            }
        }

        private static SelMgModeSelectWindowPatch.OnSubmitDelegate OnSubmitInstance;

        private static SelMgModeSelectWindowPatch.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

    }
}
