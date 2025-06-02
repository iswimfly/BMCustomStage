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
using System.IO;

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
                                        Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_lightObjPath = Main.storedBgDatum.m_lightObjPath;
                                        Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_screenEffectObjPath = Main.storedBgDatum.m_screenEffectObjPath;
                                        Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_lightSceneId = Main.storedBgDatum.m_lightSceneId;
                                        Main.originalBgDatumDict[Main.storedBgDatum.m_bg].m_lightSceneIdStr = Main.storedBgDatum.m_lightSceneIdStr;
                                    }
                                    Main.storedBgDatum.m_bg = bgYaml.OriginalBackgroundName;
                                    Main.storedBgDatum.m_bgObjPath = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_bgObjPath;
                                    Main.storedBgDatum.m_lightObjPath = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightObjPath;
                                    Main.storedBgDatum.m_screenEffectObjPath = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_screenEffectObjPath;
                                    Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_bgObjPath = bgYaml.BackgroundPrefab;
                                    Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightObjPath = bgYaml.LightPrefab;
                                    Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_screenEffectObjPath = bgYaml.ScreenEffectPrefab;
                                    if (File.Exists(bgYaml.LightSceneAssetBundleFullPath))
                                    {
                                        Main.storedBgDatum.m_lightSceneId = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightSceneId;
                                        Main.storedBgDatum.m_lightSceneIdStr = Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightSceneIdStr;
                                        Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightSceneId = (AppScene.eID)bgYaml.lightSceneID;
                                        Main.originalBgDatumDict[bgYaml.OriginalBackgroundName].m_lightSceneIdStr = bgYaml.lightSceneStr;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Not a custom stage. Ignoring BgPatch.");
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
