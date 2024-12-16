using Flash2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{
    internal class CalcBgCuePatch
    {

        
        public unsafe static void CreateDetour()
        {
            calcInstance = CalcBGCue;
            calcOriginal = ClassInjector.Detour.Detour<CalcBgCuePatch.CalcDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(MainGame).GetMethod("calcBgCue")).GetValue(null)))).MethodPointer, CalcBgCuePatch.calcInstance);
        }

        private static sound_id.cue CalcBGCue(IntPtr thisPtr, Flash2.MainGameDef.eBg eBg)
        {
            int stageID = MainGame.Instance.GetStageID();
            if (GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)8 || GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)9)
            {
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
                                    if (bgYaml.CustomBGM)
                                    {
                                        if (OptionsDef.AudioDef.LoadBGMMode() == 0)
                                        {
                                            return (sound_id.cue)bgYaml.bgm_cueid;
                                        }
                                        else if (OptionsDef.AudioDef.LoadBGMMode() == 1 && bgYaml.bgm_dx_cueid != 0)
                                        {
                                            return (sound_id.cue)bgYaml.bgm_dx_cueid;
                                        }
                                    }
                                }
                            }
                            return Main.stageIndexToCue[stageID];

                        }
                        else
                        {
                            return Main.stageIndexToCue[stageID];
                        }
                    }
                }
                return Main.stageIndexToCue[stageID];
            }
            else
            {
                return Main.stageIndexToCue[GameParam.selectorParam.selectedStageIndex];
            }
        }

        private static CalcBgCuePatch.CalcDelegate calcInstance;

        private static CalcBgCuePatch.CalcDelegate calcOriginal;

        private delegate sound_id.cue CalcDelegate(IntPtr thisPtr, MainGameDef.eBg eBg);

    }
}
