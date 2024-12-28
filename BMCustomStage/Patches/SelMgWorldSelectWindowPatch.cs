using Flash2.Selector;
using Flash2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using Flash2.Selector.MainGame.StoryMode;
using Framework;
using UnityEngine;
using System.Reflection;

namespace BMCustomStage.Patches
{
    internal class SelMgWorldSelectWindowPatch
    {

        public unsafe static void CreateDetour()
        {
            SelMgWorldSelectWindowPatch.updateInstance = updateItemDataListFromCourses;
            SelMgWorldSelectWindowPatch.updateOriginal = ClassInjector.Detour.Detour<SelMgWorldSelectWindowPatch.UpdateDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgWorldSelectWindow).GetMethod("updateItemDataListFromCourses")).GetValue(null)))).MethodPointer, SelMgWorldSelectWindowPatch.updateInstance);
        }


        private static void updateItemDataListFromCourses(IntPtr thisPtr, IntPtr selIconItemData)
        {
            Il2CppStructArray<MainGameDef.eCourse> newStoryModeDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict["original"].Count);
            SelMgWorldSelectWindowPatch.updateOriginal(thisPtr, selIconItemData);
            SelMainMenuWindowBase worlds = UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>().m_WindowCollection[SelectorDef.MainMenuKind.MgStoryWorldSelect];
            SelMgWorldSelectWindow window = new SelMgWorldSelectWindow(worlds.Pointer);
            if (GameParam.Instance.m_selectorParam.mainGameMode == (SelectorDef.MainGameKind)9)
            {
                try
                {
                    foreach (CustomCourse course in DataManager.Courses)
                    {
                        if (Main.selectedCourse == course.CourseName)
                        {
                            Il2CppSystem.Collections.Generic.List<SelIconItemData> newList = new Il2CppSystem.Collections.Generic.List<SelIconItemData>();
                            int i = 0;
                            foreach (Subcategory subcat in course.Subcategories)
                            {
                                SelMgCourseItemData courseItemData = new SelMgCourseItemData(window.m_ItemDataList[i].Pointer);
                                courseItemData.course = (MainGameDef.eCourse)subcat.CourseEnum;
                                courseItemData.textKey = $"maingame_custom_{subcat.Name}";
                                // courseItemData.thumbnailSprite = sprite;
                                courseItemData.thumbnailSprite = AssetBundleCache.LoadAsset<Sprite>(subcat.ThumbnailPath);
                                newList.Add(courseItemData);
                                i++;
                            }
                            window.m_ItemDataList = newList;
                            newStoryModeDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict[course.CourseName].Count);
                            for (int j = 0; j < Main.storyDict[course.CourseName].Count; j++)
                            {
                                newStoryModeDef[j] = Main.storyDict[course.CourseName][j];
                            }
                            MainGameDef._storyCourses_k__BackingField = newStoryModeDef;
                            Main.newStoryModeDef = newStoryModeDef;

                        }

                    }
                }
                catch
                {
                    Console.WriteLine("Nope");
                }
            }
            else
            {
                int i = 0;
                foreach (MainGameDef.eCourse course in Main.storyDict["original"])
                {
                    SelMgCourseItemData courseItemData = new SelMgCourseItemData(window.m_ItemDataList[i].Pointer);
                    courseItemData.thumbnailSprite = AssetBundleCache.LoadAsset<Sprite>(UnityEngine.Object.FindObjectOfType<MgCourseDataManager>().m_CourseDataDict[course].thumbnailSpritePath);
                    i++;
                }
            }
        }

        private static SelMgWorldSelectWindowPatch.UpdateDelegate updateInstance;

        private static SelMgWorldSelectWindowPatch.UpdateDelegate updateOriginal;

        private delegate void UpdateDelegate(IntPtr thisPtr, IntPtr selIconItemData);



    }
}
