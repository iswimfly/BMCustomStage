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
            foreach (CustomCourse customCourse in DataManager.Courses)
            {
                foreach (Subcategory subcategory in customCourse.Subcategories)
                {
                    if ((int)GameParam.selectorParam.selectedCourse == subcategory.CourseEnum)
                    {
                        Il2CppStructArray<MainGameDef.eCourse> newStoryModeDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict["original"].Count);
                        switch (subcategory.SpecialMode)
                        {
                            case "Golden":
                                GameParam.selectorParam.selectedGameKind = MainGameDef.eGameKind.Golden;
                                break;

                            case "Rotten":
                                GameParam.selectorParam.selectedGameKind = MainGameDef.eGameKind.Rotten;
                                break;

                            default:
                                if (customCourse.CategoryType == "Challenge")
                                {
                                    GameParam.selectorParam.selectedGameKind = MainGameDef.eGameKind.Challenge;
                                }
                                else
                                {
                                    GameParam.selectorParam.selectedGameKind = MainGameDef.eGameKind.Story;
                                }
                                break;
                        }
                        for (int j = 0; j < Main.storyDict["original"].Count; j++)
                        {
                            newStoryModeDef[j] = Main.storyDict["original"][j];
                        }
                        MainGameDef._storyCourses_k__BackingField = newStoryModeDef;
                        Main.selectedCourse = customCourse.CourseName;
                        Main.VibeCheck();
                    }
                }
            }
            SelMgStageSelectWindowPatchCategory.OnSubmitOriginal(thisPtr, playerIndex, inputLayer, itemDataPtr);
        }

        private static SelMgStageSelectWindowPatchCategory.OnSubmitDelegate OnSubmitInstance;

        private static SelMgStageSelectWindowPatchCategory.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, AppInput.eLayer inputLayer, IntPtr itemDataPtr);

    }
}

