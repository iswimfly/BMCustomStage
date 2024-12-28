using Flash2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using Flash2.Selector.MainGame.ChallengeMode.SMB1;
using Framework;

namespace BMCustomStage.Patches
{
    internal class SelMgCourseSelectWindowPatch
    {

        public unsafe static void CreateDetour()
        {
            SelMgCourseSelectWindowPatch.OnSubmitInstance = OnSubmit;
            SelMgCourseSelectWindowPatch.OnSubmitOriginal = ClassInjector.Detour.Detour<SelMgCourseSelectWindowPatch.OnSubmitDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(SelMgCourseSelectWindow).GetMethod("onSubmit")).GetValue(null)))).MethodPointer, SelMgCourseSelectWindowPatch.OnSubmitInstance);
        }

        private static void OnSubmit(IntPtr thisPtr, int playerIndex, eLayer eLayer, IntPtr itemDataPtr)
        {
            SelMgCourseSelectWindowPatch.OnSubmitOriginal(thisPtr, playerIndex, eLayer, itemDataPtr);
            SelTextItemData itemData = new SelTextItemData(itemDataPtr);
            foreach (CustomCourse customCourse in DataManager.Courses)
            {
                foreach (Subcategory subcat in customCourse.Subcategories)
                {
                    if (itemData.textKey.Contains(subcat.Name))
                    {
                        GameParam.selectorParam.selectedCourse = (MainGameDef.eCourse)subcat.CourseEnum;
                        GameParam.selectorParam.selectedStageIndex = SingletonBase<MgCourseDataManager>.Instance.m_CourseDataDict[(MainGameDef.eCourse)subcat.CourseEnum].m_elementList[0].m_stageId;
                        GameParam.selectorParam.selectedCourseProgress = 0;
                    }
                }
            }
        }

        private static SelMgCourseSelectWindowPatch.OnSubmitDelegate OnSubmitInstance;


        private static SelMgCourseSelectWindowPatch.OnSubmitDelegate OnSubmitOriginal;

        private delegate void OnSubmitDelegate(IntPtr thisPtr, int playerIndex, eLayer eLayer, IntPtr selTextItemData);

    }
}
