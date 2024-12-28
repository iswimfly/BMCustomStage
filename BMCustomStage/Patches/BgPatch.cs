using Flash2;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
    internal class BgPatch
    {

        public static unsafe void CreateDetour()
        {
            BgInstance = LoadAsset;
            BgOriginal = ClassInjector.Detour.Detour<BgPatch.BgDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(MgBgDatum).GetMethod("LoadAsset")).GetValue(null)))).MethodPointer, BgPatch.BgInstance);
        }

        private static void LoadAsset(IntPtr thisPtr)
        {
            if (GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)8 || GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)9 || GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)10)
            {
                int stageID = MainGame.Instance.GetStageID();
                foreach (CustomStageYaml stage in DataManager.Stages)
                {
                    if (stage.StageId == stageID)
                    {
                        if (stage.CustomBackground)
                        {
                            foreach (CustomBgYaml bgYaml in DataManager.Backgrounds)
                            {
                                if (bgYaml.CustomBackgroundName == stage.CustomBackgroundName)
                                {
                                    if (Main.storedBgDatum.m_bg != MainGameDef.eBg.Invalid)
                                    {
                                        Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_bgObjPath = Main.storedBgDatum.m_bgObjPath;
                                    }
                                    Main.storedBgDatum.m_bg = bgYaml.OriginalBackgroundName;
                                    Main.storedBgDatum.m_bgObjPath = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_bgObjPath;
                                    Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_bgObjPath = bgYaml.BackgroundPrefab;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Hello");
                // Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_bgObjPath = Main.storedBgDatum.m_bgObjPath;
                // Main.storedBgDatum.m_bg = MainGameDef.eBg.Invalid;
            }
            BgPatch.BgOriginal(thisPtr);
        }

        private static BgPatch.BgDelegate BgInstance;


        private static BgPatch.BgDelegate BgOriginal;

        private delegate void BgDelegate(IntPtr thisPtr);
    }
}
