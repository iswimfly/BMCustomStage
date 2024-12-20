﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BMCustomStage.Patches;
using Flash2;
using Framework;
using Framework.Text;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Reflection;
using Il2CppSystem.Xml;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BMCustomStage
{
	public static class Main
	{

        private static bool isInitialised;
        private static SelMainMenuSequence sequence = null;
        private static bool debugMode;
        private static bool isReloading;
		private static bool menuModded = false;
		public static MgBgDatum storedBgDatum = new MgBgDatum();

        public static void OnModLoad(System.Collections.Generic.Dictionary<string, object> settingsDict)
		{
			object debugModeObj;
			bool debugMode;
			if (settingsDict.TryGetValue("DebugMode", out debugModeObj))
			{
				if (debugModeObj is bool)
				{
					debugMode = (bool)debugModeObj;
				}
				else
				{
					debugMode = false;
				}
			}
			else
			{
				debugMode = false;
			}
			Main.debugMode = debugMode;
		}

		public static void OnModUpdate()
		{
            if (!Main.isInitialised)
			{
				Log.Info("Initialising Custom Stage Loader");
				Main.isInitialised = true;
				Main.Initialise();
				Log.Info("Initialised Custom Stage Loader");
			}
            if (UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>() != null && Main.sequence == null && !menuModded)
            {
                Main.sequence = UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>();
            }
			else if (UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>() == null)
			{
				menuModded = false;
			}
            if (Main.sequence != null && Main.sequence.m_IsCreateWindows && !menuModded)
			{
				Main.AddMainMenuItems();
				Main.sequence = null;
				menuModded = true;
			}
			if (Main.debugMode && Input.GetKeyDown(UnityEngine.KeyCode.R))
			{
				if (!Main.isReloading && MainGame.mainGameStage == null)
				{
					Log.Info("Unloading stage and background assetbundles...");
					Main.isReloading = true;
					foreach (CustomStageYaml stage in DataManager.Stages)
					{
						AssetBundleCache.UnloadAssetBundle(stage.AssetBundleName);
					}
					foreach (CustomBgYaml background in DataManager.Backgrounds)
					{
						AssetBundleCache.UnloadAssetBundle(background.AssetBundleName);
					}
					Log.Info("Unloaded stage and background assetbundles");
				}
			}
            if (Main.isReloading && !SingletonBase<AssetBundleCache>.Instance.is_unloading)
            {
                Log.Info("Reloading stage and background assetbundles...");
                foreach (CustomStageYaml stage2 in DataManager.Stages)
                {
                    AssetBundleCache.element_t element = new AssetBundleCache.element_t();
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(stage2.AssetBundleName, element);
                    element.Load(stage2.AssetBundleName);
                }
                foreach (CustomBgYaml background2 in DataManager.Backgrounds)
                {
                    AssetBundleCache.element_t element = new AssetBundleCache.element_t();
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(background2.AssetBundleName, element);
                    element.Load(background2.AssetBundleName);
                }
                Log.Info("Loaded stage and background assetbundles");
                Main.isReloading = false;
            }
        }

        private static void Initialise()
        {
            AssetBundleCachePatch.CreateDetour();
            CollisionStagePartPatch.CreateDetour();
            GameParamPatch.CreateDetour();
            MainGameStagePatch.CreateDetour();
            SelMgModeSelectWindowPatch.CreateDetour();
            SelMgStageSelectWindowPatchPractice.CreateDetour();
            SelMgStageSelectWindowPatchCategory.CreateDetour();
            CalcBgCuePatch.CreateDetour();
            BgPatch.CreateDetour();
            FadeInPatch.CreateDetour();

            // MainGamePatch.CreateDetour();
            DataManager.Initialise();
            DataManager.Stages.OrderBy(entry => entry.StageId).ToList<CustomStageYaml>();
            System.IntPtr mainGameKindPtr = IL2CPP.GetIl2CppNestedType(IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Flash2", "SelectorDef"), "MainGameKind");
            Il2CppSystem.Type mainGameKindType = Il2CppType.TypeFromPointer(mainGameKindPtr, "<unknown type>");
            System.IntPtr eCoursePtr = IL2CPP.GetIl2CppNestedType(IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Flash2", "MainGameDef"), "eCourse");
            Il2CppSystem.Type eCourseType = Il2CppType.TypeFromPointer(eCoursePtr, "<unknown type>");
            Assembly assembly = Il2CppSystem.AppDomain.CurrentDomain.GetAssemblies().Single((Assembly a) => a.GetName().Name == "Assembly-CSharp");
            Il2CppSystem.Type enumRuntimeHelper = assembly.GetType("Framework.EnumRuntimeHelper`1");
            Il2CppSystem.Type erhMaingamekind = enumRuntimeHelper.MakeGenericType(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[]
            {
                mainGameKindType
            }));
            Il2CppSystem.Type erhECourse = enumRuntimeHelper.MakeGenericType(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[]
            {
                eCourseType
            }));
            MethodInfo mgkValToNameGetter = erhMaingamekind.GetProperty("valueToNameCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<SelectorDef.MainGameKind, string> mgkValToName = mgkValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<SelectorDef.MainGameKind, string>>();
            mgkValToName.Add((SelectorDef.MainGameKind)8, "Practice_Custom");
            mgkValToName.Add((SelectorDef.MainGameKind)9, "Story_Custom");
            MethodInfo mgkNameToValGetter = erhMaingamekind.GetProperty("nameToValueCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<string, SelectorDef.MainGameKind> mgkNameToVal = mgkNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, SelectorDef.MainGameKind>>();
            mgkNameToVal.Add("Practice_Custom", (SelectorDef.MainGameKind)8);
            mgkNameToVal.Add("Story_Custom", (SelectorDef.MainGameKind)9);
            MethodInfo ecValToNameGetter = erhECourse.GetProperty("valueToNameCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eCourse, string> ecValToName = ecValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eCourse, string>>();
            ecValToName.Add((MainGameDef.eCourse)44, "Custom_Stages");
            foreach (CustomCourse course in DataManager.Courses)
            {
                ecValToName.Add((MainGameDef.eCourse)course.courseEnum, $"Custom_Stages_Story_{course.courseName}");
            }
            MethodInfo ecNameToValGetter = erhECourse.GetProperty("nameToValueCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eCourse> ecNameToVal = ecNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eCourse>>();
            ecNameToVal.Add("Custom_Stages", (MainGameDef.eCourse)44);
            foreach (CustomCourse course in DataManager.Courses)
            {
                ecNameToVal.Add($"Custom_Stages_Story_{course.courseName}", (MainGameDef.eCourse)course.courseEnum);
            }
            SelPauseWindow.s_MgMenuKindCollection.Add((SelectorDef.MainGameKind)8, (SelectorDef.MainMenuKind)9);
            MainGameStage.s_StageSelectCollection.Add((SelectorDef.MainGameKind)8, (SelectorDef.MainMenuKind)9);
            SelPauseWindow.s_MgMenuKindCollection.Add((SelectorDef.MainGameKind)9, (SelectorDef.MainMenuKind)5);
            MainGameStage.s_StageSelectCollection.Add((SelectorDef.MainGameKind)9, (SelectorDef.MainMenuKind)5);

            {
                System.IntPtr cueIdPtr = IL2CPP.GetIl2CppNestedType(IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Flash2", "sound_id"), "cue");
                Il2CppSystem.Type cueIdType = UnhollowerRuntimeLib.Il2CppType.TypeFromPointer(cueIdPtr);

                Il2CppSystem.Reflection.Assembly assembly2 = Il2CppSystem.AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "Assembly-CSharp");
                Il2CppSystem.Type enumRuntimeHelper2 = assembly2.GetType("Framework.EnumRuntimeHelper`1");
                Il2CppSystem.Type erhCue = enumRuntimeHelper2.MakeGenericType(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[] { cueIdType }));

                Il2CppSystem.Reflection.MethodInfo cueValToNameGetter = erhCue.GetProperty("valueToNameCollection").GetGetMethod();
                Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, string> cueValToName = cueValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0)).Cast<Il2CppSystem.Collections.Generic.Dictionary<sound_id.cue, string>>();
                cueValToName.Add((sound_id.cue)20000, "bgm_Custom");

                Il2CppSystem.Reflection.MethodInfo cueNameToValGetter = erhCue.GetProperty("nameToValueCollection").GetGetMethod();
                Il2CppSystem.Collections.Generic.Dictionary<string, sound_id.cue> cueNameToVal = cueNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, sound_id.cue>>();
                cueNameToVal.Add("bgm_Custom", (sound_id.cue)20000);
            }

            if (UnityEngine.Object.FindObjectOfType<MgBgDataManager>() != null)
            {
                // Thankey Piggey
                System.IntPtr mainGameDefBgPtr = IL2CPP.GetIl2CppNestedType(IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Flash2", "MainGameDef"), "eBg");
                Il2CppSystem.Type mainGameDefeBg = Il2CppType.TypeFromPointer(mainGameDefBgPtr, "<unknown type>");
                Il2CppSystem.Type mainGameDefeBgType = enumRuntimeHelper.MakeGenericType(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[]
                {
                    mainGameDefeBg
                }));
                MethodInfo mgdValToNameGetter = mainGameDefeBgType.GetProperty("valueToNameCollection").GetGetMethod();
                Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, string> mgdValToName = mgdValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, string>>();
                mgdValToName.Add((MainGameDef.eBg)50, "custom_bg");
                MethodInfo mgdNameToValGetter = mainGameDefeBgType.GetProperty("nameToValueCollection").GetGetMethod();
                Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eBg> mgdNameToVal = mgdNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eBg>>();
                mgdNameToVal.Add("custom_bg", (MainGameDef.eBg)50);
                // Store everything in the public static dictionary so it can be used anywhere anytime.
                MgBgDataManager mgBgDataManager = UnityEngine.Object.FindObjectOfType<MgBgDataManager>();
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<MainGameDef.eBg, MgBgDatum> kvp in mgBgDataManager.m_bgDataDict)
                {
                    originalBgDatumDict.Add(kvp.Key, kvp.Value);
                }
                int bgSoundId = 7000;
                int bgCueId = 20000;
                foreach (CustomBgYaml bgYaml in DataManager.Backgrounds)
                {
                    if (bgYaml.CustomBGM)
                    {
                        string bgLocation = bgYaml.AssetBundleFullPath.Substring(0, bgYaml.AssetBundleFullPath.Length - bgYaml.AssetBundleName.Length);
                        CriAtomExAcb loaded = CriAtomExAcb.LoadAcbFile(null, (bgLocation + "bgm_worldCustom.acb"), (bgLocation + "bgm_worldCustom.awb"));
                        Sound.Instance.m_cueSheetParamDict[(sound_id.cuesheet)bgSoundId] = new Sound.cuesheet_param_t(bgSoundId.ToString(), (bgLocation + "bgm_worldCustom.acb"), (bgLocation + "bgm_worldCustom.awb"));
                        Sound.Instance.m_cueParamDict[(sound_id.cue)bgCueId] = new Sound.cue_param_t((sound_id.cuesheet)bgSoundId, "st1");
                        Sound.Instance.LoadCueSheetASync((sound_id.cuesheet)bgSoundId);
                        bgYaml.bgm_cuesheetid = bgSoundId;
                        bgYaml.bgm_cueid = bgCueId;
                        bgSoundId++;
                        bgCueId++;

                        // DX is original
                        if (File.Exists(bgLocation + "bgm_worldCustom_dx.acb"))
                        {
                            Sound.Instance.m_cueSheetParamDict[(sound_id.cuesheet)bgSoundId] = new Sound.cuesheet_param_t(bgSoundId.ToString(), (bgLocation + "bgm_worldCustom_dx.acb"), (bgLocation + "bgm_worldCustom_dx.awb"));
                            Sound.Instance.m_cueParamDict[(sound_id.cue)bgCueId] = new Sound.cue_param_t((sound_id.cuesheet)bgSoundId, "st1");
                            Sound.Instance.LoadCueSheetASync((sound_id.cuesheet)bgSoundId);
                            bgYaml.bgm_dx_cuesheetid = bgSoundId;
                            bgYaml.bgm_dx_cueid = bgCueId;
                            bgSoundId++;
                            bgCueId++;
                        }
                        else
                        {
                            bgYaml.bgm_dx_cuesheetid = 0;
                            bgYaml.bgm_dx_cueid = 0;
                        }

                    }
                }
                MgStageDataManager mgStageDataManager = UnityEngine.Object.FindObjectOfType<MgStageDataManager>();
                foreach (MgStageDatum stage in mgStageDataManager.m_stageDataDict.Values)
                {
                    stageIndexToCue.Add(stage.stageId, mgBgDataManager.m_bgDataDict[stage.bg].bgmCue);
                    stageIndexToBg.Add(stage.stageId, mgBgDataManager.m_bgDataDict[stage.bg].bg);
                }

            }

            foreach (CustomStageYaml stage in DataManager.Stages)
            {
                AssetBundleCache.element_t element = new AssetBundleCache.element_t();
                SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(stage.AssetBundleName, element);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/stage.prefab", stage.StageId), stage.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/BgRoot.prefab", stage.StageId), stage.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/Guide.asset", stage.StageId), stage.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/Guide_Golden.asset", stage.StageId), stage.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/Guide_Rotten.asset", stage.StageId), stage.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("stage/custom/st{0}/thumbnail.png", stage.StageId), stage.AssetBundleName);
                Il2CppSystem.Collections.Generic.List<string> dependencyList = new Il2CppSystem.Collections.Generic.List<string>();
                dependencyList.Add("shader");
                SingletonBase<AssetBundleCache>.Instance.m_assetBundleDependencyDict.Add(stage.AssetBundleName, dependencyList);
                element.Load(stage.AssetBundleName);
            }

            Il2CppSystem.Collections.Generic.List<string> shaderDependencyList = new Il2CppSystem.Collections.Generic.List<string>();

            foreach (CustomBgYaml bg in DataManager.Backgrounds)
            {
                AssetBundleCache.element_t element = new AssetBundleCache.element_t();
                AssetBundleCache.element_t element2 = new AssetBundleCache.element_t();
                SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(bg.AssetBundleName, element);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(bg.BackgroundPrefab, bg.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("bg/custom/bg_{0}/light.prefab", bg.CustomBackgroundName.ToLower()), bg.AssetBundleName);
                SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(string.Format("bg/custom/bg_{0}/screeneffect.prefab", bg.CustomBackgroundName.ToLower()), bg.AssetBundleName);
                if (bg.CustomShaders)
                {
                    shaderDependencyList.Add("shader");
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(bg.ShaderAssetBundleName, element2);
                    for (int j = 0; j < bg.CustomShaderCount; j++)
                    {
                        SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add($"shader/custom_shader{j}.shader", bg.ShaderAssetBundleName);
                    }
                    shaderDependencyList.Add(bg.ShaderAssetBundleName);
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleDependencyDict[bg.OriginalBackgroundAssetBundle] = shaderDependencyList;
                    element2.Load(bg.ShaderAssetBundleName);
                }
                element.Load(bg.AssetBundleName);
            }

            TextData textData = new TextData();

            textData.textDictionary.Add("maingame_practicecustom", new TextData.Context
            {
                text = "Custom Stages - Practice Mode"
            });
            foreach (CustomCourse course in DataManager.Courses)
            {
                textData.textDictionary.Add($"maingame_storycustom_{course.courseName}", new TextData.Context
                {
                    text = course.courseName
                });
            }


            foreach (CustomStageYaml stage2 in DataManager.Stages)
            {
                textData.textDictionary.Add(string.Format("stagename_st{0}", stage2.StageId), new TextData.Context
                {
                    text = stage2.StageName
                });
            }
            TextManager.AddData(GameParam.language, textData);
            System.Collections.Generic.Dictionary<int, MgCourseDatum> newCourses = new System.Collections.Generic.Dictionary<int, MgCourseDatum>();
            MgCourseDatum customPracticeDatum = new MgCourseDatum
            {
                m_GameKindStr = "Practice",
                m_CourseStr = "Custom_Stages",
                m_courseNameTextReference = new TextReference
                {
                    m_Key = "maingame_practicecustom"
                },
                m_ThumbnailSpritePath = "UI/thumbnail/Common/t_tmb_mode_challenge_01_01.tga",
                m_StartMovieIDStr = "Invalid",
                m_EndMovieIDStr = "Invalid",
                m_NextCourseStr = "Invalid",
                m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
            };
            newCourses.Add(44, customPracticeDatum);
            foreach (CustomCourse course in DataManager.Courses)
            {
                MgCourseDatum customCourseDatum = new MgCourseDatum
                {
                    m_GameKindStr = "Challenge",
                    m_CourseStr = $"Custom_Stages_Story_{course.courseName}",
                    m_courseNameTextReference = new TextReference
                    {
                        m_Key = $"maingame_storycustom_{course.courseName}"
                    },
                    m_ThumbnailSpritePath = "UI/thumbnail/Common/t_tmb_mode_challenge_01_01.tga",
                    m_StartMovieIDStr = "Invalid",
                    m_EndMovieIDStr = "Invalid",
                    m_NextCourseStr = "Invalid",
                    m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
                };
                newCourses.Add(course.courseEnum, customCourseDatum);
            }
            foreach (CustomStageYaml stage3 in DataManager.Stages)
            {
                Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t> goals = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t>();
                goals.Add(new MgCourseDatum.goal_t
                {
                    m_goalKindStr = "Blue",
                    m_nextStep = 1
                });
                goals[0].Initialize();
                customPracticeDatum.m_elementList.Add(new MgCourseDatum.element_t
                {
                    m_stageId = stage3.StageId,
                    m_CalibratedStageId = 0,
                    m_IsHalfTime = false,
                    m_IsCheckPoint = false,
                    m_goalList = goals
                });
                foreach (MgCourseDatum courseDatum in newCourses.Values)
                {
                    if (courseDatum.m_CourseStr == $"Custom_Stages_Story_{stage3.PackPath}")
                    {
                        courseDatum.m_elementList.Add(new MgCourseDatum.element_t
                        {
                            m_stageId = stage3.StageId,
                            m_CalibratedStageId = 0,
                            m_IsHalfTime = false,
                            m_IsCheckPoint = false,
                            m_goalList = goals
                        });
                    }
                }
            }
            customPracticeDatum.Initialize();
            foreach (MgCourseDatum customCourseDatum in newCourses.Values)
            {
                customCourseDatum.Initialize();
            }
            foreach (System.Collections.Generic.KeyValuePair<int, MgCourseDatum> kvp in newCourses)
            {
                SingletonBase<MgCourseDataManager>.Instance.m_CourseDataDict[(MainGameDef.eCourse)kvp.Key] = kvp.Value;
            }

            foreach (CustomStageYaml stage4 in DataManager.Stages)
            {
                Il2CppSystem.Collections.Generic.List<int> limitTimes = new Il2CppSystem.Collections.Generic.List<int>();
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitNormal);
                limitTimes.Add(stage4.TimeLimitDarkBanana);
                limitTimes.Add(stage4.TimeLimitGoldenBanana);
                string backgroundName = string.Empty;
                if (stage4.CustomBackground)
                {
                    backgroundName = DataManager.TryGetBgYamlFromStageId(stage4.StageId).OriginalBackgroundName.ToString();
                }
                else
                {
                    backgroundName = stage4.Background;
                }
                MgStageDatum customStageDatum = new MgStageDatum
                {
                    m_stageId = stage4.StageId,
                    m_stageObjPath = string.Format("stage/custom/st{0}/stage.prefab", stage4.StageId),
                    m_bgStr = backgroundName,
                    m_BgRootObjPath = string.Format("stage/custom/st{0}/BgRoot.prefab", stage4.StageId),
                    m_stageKindStr = "Normal",
                    m_LimitTimeList = limitTimes,
                    m_specialLightObjPath = "",
                    m_specialBgmCueStr = "invalid",
                    m_thumbnailSpritePath = string.Format("stage/custom/st{0}/thumbnail.png", stage4.StageId),
                    m_GuideAssetPath = string.Format("stage/custom/st{0}/Guide.asset", stage4.StageId),
                    m_GuideRottenAssetPath = string.Format("stage/custom/st{0}/Guide_Golden.asset", stage4.StageId),
                    m_GuideGoldenAssetPath = string.Format("stage/custom/st{0}/Guide_Rotten.asset", stage4.StageId),
                    m_StageAnimationStartKindStr = "OnInitialize",
                    m_ShiftAnimationStartFrame = 0
                };
                customStageDatum.Initialize();
                SingletonBase<MgStageDataManager>.Instance.m_stageDataDict[stage4.StageId] = customStageDatum;
            }

            for (int j = 44; j < newCourses.Count + 44; j++)
            {
                if (SaveData.userParam.mainGameCourseParam.m_CourseParamDict.m_Dict.ContainsKey(j.ToString()))
                {
                    Log.Info("Custom course save data already found, deleting...");
                    SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Remove(j.ToString());
                }
            }

            SaveData.MainGameCourseParam.ProgressParamDict practiceProgressDict = new SaveData.MainGameCourseParam.ProgressParamDict();
            System.Collections.Generic.Dictionary<int, SaveData.MainGameCourseParam.ProgressParamDict> courseProgressDict = new System.Collections.Generic.Dictionary<int, SaveData.MainGameCourseParam.ProgressParamDict>();
            int i = 0;
            foreach (CustomStageYaml stage in DataManager.Stages)
            {
                practiceProgressDict.Add(i, new SaveData.MainGameCourseParam.ProgressParam
                {
                    m_IsUnlocked = true,
                    m_IsCleared = false,
                    m_IsClearedWithoutAssist = false,
                    m_IsClearedWithoutJump = false,
                    m_IsClearedWithoutPauseMenu = false,
                    m_IsCompleted = false,
                    m_IsCompletedWithoutJump = false,
                    m_CharaKind = 0,
                    m_ClearTimeInCentiSec = 0,
                    m_PlayCount = 0,
                    m_DeathCount = 0,
                    m_RetryCount = 0
                });

                foreach (System.Collections.Generic.KeyValuePair<int, MgCourseDatum> kvp in newCourses)
                {
                    if (kvp.Value.m_CourseStr == $"Custom_Stages_Story_{stage.PackPath}")
                    {
                        if (!courseProgressDict.ContainsKey(kvp.Key))
                        {
                            courseProgressDict[kvp.Key] = new SaveData.MainGameCourseParam.ProgressParamDict();
                        }
                        int count = courseProgressDict[kvp.Key].entryList.Count;
                        courseProgressDict[kvp.Key].Add(count, new SaveData.MainGameCourseParam.ProgressParam
                        {
                            m_IsUnlocked = true,
                            m_IsCleared = false,
                            m_IsClearedWithoutAssist = false,
                            m_IsClearedWithoutJump = false,
                            m_IsClearedWithoutPauseMenu = false,
                            m_IsCompleted = false,
                            m_IsCompletedWithoutJump = false,
                            m_CharaKind = 0,
                            m_ClearTimeInCentiSec = 0,
                            m_PlayCount = 0,
                            m_DeathCount = 0,
                            m_RetryCount = 0
                        });
                    }
                }
                i++;
            }



            SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Add("44", new SaveData.MainGameCourseParam.CourseParam
            {
                m_ProgressParamDict = practiceProgressDict,
                m_IsCleared = false,
                m_IsClearedWithoutAssist = false,
                m_IsPassedCheckPoint = false,
                m_ChallengeModeBestClearTimeInCentiSec = 0
            });

            foreach (CustomCourse course in DataManager.Courses)
            {
                SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Add(course.courseEnum.ToString(), new SaveData.MainGameCourseParam.CourseParam
                {
                    m_ProgressParamDict = courseProgressDict[course.courseEnum],
                    m_IsCleared = false,
                    m_IsClearedWithoutAssist = false,
                    m_IsPassedCheckPoint = false,
                    m_ChallengeModeBestClearTimeInCentiSec = 0
                });
            }
            Log.Info("Created custom course save data");
        }

        private static void AddMainMenuItems()
		{
			SelMgModeItemDataListObject dataList = Main.sequence.GetData<SelMgModeItemDataListObject>((SelMainMenuSequence.Data)2);
			if (dataList.m_ItemDataList.Count <= 6)
			{
				SelMgModeItemData itemData = new SelMgModeItemData();
				itemData.transitionMenuKind = (SelectorDef.MainMenuKind)9;
				itemData.mainGamemode = (SelectorDef.MainGameKind)8;
				itemData.mainGameKind = (MainGameDef.eGameKind)3;
				itemData.textKey = "maingame_practicecustom";
				itemData.descriptionTextKey = "";
				itemData.isHideText = true;
				itemData.supplementaryTextKey = "";
				itemData.m_ThumbnailSpritePath = new SubAssetSpritePath
				{
					m_Identifier = "UI/thumbnail/Common/t_tmb_mode_practice_01.tga:t_tmb_mode_practice_01"
				};
				dataList.m_ItemDataList.Add(itemData);
                
				foreach(CustomCourse course in DataManager.Courses)
				{
                    SelMgModeItemData courseItemData = new SelMgModeItemData();
                    courseItemData.transitionMenuKind = (SelectorDef.MainMenuKind)5;
                    courseItemData.mainGamemode = (SelectorDef.MainGameKind)9;
                    courseItemData.mainGameKind = (MainGameDef.eGameKind)1;
					courseItemData.textKey = $"maingame_storycustom_{course.courseName}";
                    courseItemData.descriptionTextKey = "";
                    courseItemData.isHideText = true;
                    courseItemData.supplementaryTextKey = "";
                    courseItemData.m_ThumbnailSpritePath = new SubAssetSpritePath
                    {
                        m_Identifier = "UI/thumbnail/Common/t_tmb_mode_practice_01.tga:t_tmb_mode_practice_01"
                    };
                    dataList.m_ItemDataList.Add(courseItemData);
                }

                Log.Info("Created Custom Courses.");
				
			}

			SelHowToPlayData howToPlayData = Main.sequence.GetData<SelHowToPlayData>((SelMainMenuSequence.Data)25);
			if (howToPlayData.m_PCData.m_MainGameDataArray.Length <= 8)
			{
				SelHowToPlayItemDataListObject[] newHowToPlayArray = new SelHowToPlayItemDataListObject[10];
				int i = 0;
				foreach (SelHowToPlayItemDataListObject item in howToPlayData.m_PCData.m_MainGameDataArray)
				{
					newHowToPlayArray[i] = item;
					i++;
				}
				newHowToPlayArray[8] = newHowToPlayArray[4];
				newHowToPlayArray[9] = newHowToPlayArray[1];
				howToPlayData.m_PCData.m_MainGameDataArray = newHowToPlayArray;

				newHowToPlayArray = new SelHowToPlayItemDataListObject[10];
				i = 0;
                foreach (SelHowToPlayItemDataListObject item in howToPlayData.m_PCData.m_MainGameDataArray)
                {
                    newHowToPlayArray[i] = item;
                    i++;
                }
                newHowToPlayArray[8] = newHowToPlayArray[4];
                newHowToPlayArray[9] = newHowToPlayArray[1];
                howToPlayData.m_ConsoleData.m_MainGameDataArray = newHowToPlayArray;
			}
		}

		public static readonly Il2CppSystem.Collections.Generic.Dictionary<int, sound_id.cue> stageIndexToCue = new Il2CppSystem.Collections.Generic.Dictionary<int, sound_id.cue>();
		public static readonly Il2CppSystem.Collections.Generic.Dictionary<int, MainGameDef.eBg> stageIndexToBg = new Il2CppSystem.Collections.Generic.Dictionary<int, MainGameDef.eBg>();
        public static readonly Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, MgBgDatum> originalBgDatumDict = new Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, MgBgDatum>();
    }
}
