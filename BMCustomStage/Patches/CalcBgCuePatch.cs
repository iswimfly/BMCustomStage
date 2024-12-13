﻿using Flash2;
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

        // Token: 0x06000034 RID: 52 RVA: 0x00004240 File Offset: 0x00002440
        public unsafe static void CreateDetour()
        {
            calcInstance = CalcBGCue;
            calcOriginal = ClassInjector.Detour.Detour<CalcBgCuePatch.CalcDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(MainGame).GetMethod("calcBgCue")).GetValue(null)))).MethodPointer, CalcBgCuePatch.calcInstance);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00004245 File Offset: 0x00002445
        private static sound_id.cue CalcBGCue(IntPtr thisPtr, Flash2.MainGameDef.eBg eBg)
        {
            if (GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)8)
            {
                foreach (CustomStageYaml stage in DataManager.Stages)
                {
                    if (stage.StageId == GameParam.selectorParam.selectedStageIndex)
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
                            return Main.stageIndexToCue[GameParam.selectorParam.selectedStageIndex];

                        }
                        else
                        {
                            return Main.stageIndexToCue[GameParam.selectorParam.selectedStageIndex];
                        }
                    }
                }
                return Main.stageIndexToCue[GameParam.selectorParam.selectedStageIndex];
            }
            else
            {
                return Main.stageIndexToCue[GameParam.selectorParam.selectedStageIndex];
            }
            CalcBgCuePatch.calcOriginal(thisPtr, eBg);
        }

        private static CalcBgCuePatch.CalcDelegate calcInstance;

        private static CalcBgCuePatch.CalcDelegate calcOriginal;

        private delegate sound_id.cue CalcDelegate(IntPtr thisPtr, MainGameDef.eBg eBg);

    }
}