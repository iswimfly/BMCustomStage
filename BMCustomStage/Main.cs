using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BMCustomStage.Patches;
using Flash2;
using Flash2.Selector;
using Flash2.Selector.MainGame.ChallengeMode.SMB1;
using Flash2.Selector.MainGame.StoryMode;
using Framework;
using Framework.Text;
using Framework.Window;
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
        public static bool melonMode = false;
        private static bool isInitialised;
        private static SelMainMenuSequence sequence = null;
        private static bool debugMode;
        private static bool isReloading;
		private static bool menuModded = false;
        private static SelectorDef.MainGameKind storedGameKind;
        public static string selectedCourse;
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
                Main.VibeCheck();
            }
            if (Main.debugMode)
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    if (Input.GetKeyDown(KeyCode.R))
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
            SelMgWorldSelectWindowPatch.CreateDetour();
            SelMgCourseSelectWindowPatch.CreateDetour();
            SelPauseWindowPatch.CreateDetour();
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
            mgkValToName.Add((SelectorDef.MainGameKind)10, "Challenge_Custom");
            MethodInfo mgkNameToValGetter = erhMaingamekind.GetProperty("nameToValueCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<string, SelectorDef.MainGameKind> mgkNameToVal = mgkNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, SelectorDef.MainGameKind>>();
            mgkNameToVal.Add("Practice_Custom", (SelectorDef.MainGameKind)8);
            mgkNameToVal.Add("Story_Custom", (SelectorDef.MainGameKind)9);
            mgkNameToVal.Add("Challenge_Custom", (SelectorDef.MainGameKind)10);
            MethodInfo ecValToNameGetter = erhECourse.GetProperty("valueToNameCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eCourse, string> ecValToName = ecValToNameGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eCourse, string>>();
            ecValToName.Add((MainGameDef.eCourse)44, "Custom_Stages");
            Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse> eCourseArray = new Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse>();
            Il2CppSystem.Collections.Generic.List<string> courseStringArray = new Il2CppSystem.Collections.Generic.List<string>();
            foreach (string courseName in SelMgCmCourseItemData.s_CourseNames)
            {
                courseStringArray.Add(courseName);
            }
            foreach (MainGameDef.eCourse eCourse in SelMgCmCourseItemData.s_EnumCourses)
            {
                eCourseArray.Add(eCourse);
            }
            foreach (CustomCourse course in DataManager.Courses)
            {
                foreach (Subcategory subcat in course.Subcategories)
                {
                    ecValToName.Add((MainGameDef.eCourse)subcat.CourseEnum, $"Custom_Stages_{subcat.Name}");
                    courseStringArray.Add(subcat.Name);
                    eCourseArray.Add((MainGameDef.eCourse)subcat.CourseEnum);
                }
            }
            SelMgCmCourseItemData.s_EnumCourses = new Il2CppStructArray<MainGameDef.eCourse>(eCourseArray.Count);
            SelMgCmCourseItemData.s_CourseNames = new Il2CppStringArray(courseStringArray.Count);
            for (int j = 0; j < eCourseArray.Count; j++)
            {
                SelMgCmCourseItemData.s_EnumCourses[j] = eCourseArray[j];
                SelMgCmCourseItemData.s_CourseNames[j] = courseStringArray[j];
            }
            MethodInfo ecNameToValGetter = erhECourse.GetProperty("nameToValueCollection").GetGetMethod();
            Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eCourse> ecNameToVal = ecNameToValGetter.Invoke(null, new Il2CppReferenceArray<Il2CppSystem.Object>(0L)).Cast<Il2CppSystem.Collections.Generic.Dictionary<string, MainGameDef.eCourse>>();
            ecNameToVal.Add("Custom_Stages", (MainGameDef.eCourse)44);
            foreach (CustomCourse course in DataManager.Courses)
            {
                foreach (Subcategory subcat in course.Subcategories)
                {
                    ecNameToVal.Add($"Custom_Stages_{subcat.Name}", (MainGameDef.eCourse)subcat.CourseEnum);
                }
            }
            SelPauseWindow.s_MgMenuKindCollection.Add((SelectorDef.MainGameKind)8, (SelectorDef.MainMenuKind)9);
            MainGameStage.s_StageSelectCollection.Add((SelectorDef.MainGameKind)8, (SelectorDef.MainMenuKind)9);
            SelPauseWindow.s_MgMenuKindCollection.Add((SelectorDef.MainGameKind)9, (SelectorDef.MainMenuKind)5);
            MainGameStage.s_StageSelectCollection.Add((SelectorDef.MainGameKind)9, (SelectorDef.MainMenuKind)5);
            SelPauseWindow.s_MgMenuKindCollection.Add((SelectorDef.MainGameKind)10, (SelectorDef.MainMenuKind)6);
            MainGameStage.s_StageSelectCollection.Add((SelectorDef.MainGameKind)10, (SelectorDef.MainMenuKind)6);

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
                        bgYaml.bgm_cuesheetid = bgSoundId;
                        bgYaml.bgm_cueid = bgCueId;
                        bgSoundId++;
                        bgCueId++;
                        loaded.Dispose();
                        // DX is original
                        if (File.Exists(bgLocation + "bgm_worldCustom_dx.acb"))
                        {
                            CriAtomExAcb loaded_dx = CriAtomExAcb.LoadAcbFile(null, (bgLocation + "bgm_worldCustom_dx.acb"), (bgLocation + "bgm_worldCustom_dx.awb"));
                            Sound.Instance.m_cueSheetParamDict[(sound_id.cuesheet)bgSoundId] = new Sound.cuesheet_param_t(bgSoundId.ToString(), (bgLocation + "bgm_worldCustom_dx.acb"), (bgLocation + "bgm_worldCustom_dx.awb"));
                            Sound.Instance.m_cueParamDict[(sound_id.cue)bgCueId] = new Sound.cue_param_t((sound_id.cuesheet)bgSoundId, "st1");
                            bgYaml.bgm_dx_cuesheetid = bgSoundId;
                            bgYaml.bgm_dx_cueid = bgCueId;
                            bgSoundId++;
                            bgCueId++;
                            loaded_dx.Dispose();
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

            foreach (CustomCourse course in DataManager.Courses)
            {
                AssetBundleCache.element_t element = new AssetBundleCache.element_t();
                if (!SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.ContainsKey(course.ThumbnailAssetBundle))
                {
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add(course.ThumbnailAssetBundle, element);
                    Il2CppSystem.Collections.Generic.List<string> dependencyList = new Il2CppSystem.Collections.Generic.List<string>();
                    dependencyList.Add("shader");
                    SingletonBase<AssetBundleCache>.Instance.m_assetBundleDependencyDict.Add(course.ThumbnailAssetBundle, dependencyList);
                }
                if (!SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.ContainsKey(course.ThumbnailPath))
                {
                    SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(course.ThumbnailPath, course.ThumbnailAssetBundle);
                }
                foreach (Subcategory subcategory in course.Subcategories)
                {
                    if (!SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.ContainsKey(subcategory.ThumbnailPath))
                    {
                        SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add(subcategory.ThumbnailPath, course.ThumbnailAssetBundle);
                    }
                }
                element.Load(course.ThumbnailAssetBundle);
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

            // Placeholder thumbnails if they're missing from the pack's assetbundle
            AssetBundleCache.element_t elementPlaceholder = new AssetBundleCache.element_t();
            SingletonBase<AssetBundleCache>.Instance.m_assetBundleNameToEntityDict.Add("placeholder", elementPlaceholder);
            SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add("thumbnails/custom/thumb_custchallenge.png", "placeholder");
            SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add("thumbnails/custom/thumb_custpractice.png", "placeholder");
            SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add("thumbnails/custom/thumb_custworld.png", "placeholder");
            SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add("thumbnails/custom/thumb_custstage.png", "placeholder");
            SingletonBase<AssetBundleCache>.Instance.m_pathToAssetBundleNameDict.Add("thumbnails/custom/thumb_custmode.png", "placeholder");
            elementPlaceholder.Load("placeholder");

            TextData textData = new TextData();

            textData.textDictionary.Add("maingame_practicecustom", new TextData.Context
            {
                text = "Custom Stages - Practice Mode"
            });
            foreach (CustomCourse course in DataManager.Courses)
            {
                textData.textDictionary.Add($"maingame_custom_{course.CourseName}", new TextData.Context
                {
                    text = course.CourseName
                });
                foreach (Subcategory subcat in course.Subcategories)
                {
                    textData.textDictionary.Add($"maingame_custom_{subcat.Name}", new TextData.Context
                    {
                        text = subcat.Name
                    });
                }
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
                m_ThumbnailSpritePath = "thumbnails/custom/thumb_custpractice.png",
                m_StartMovieIDStr = "Invalid",
                m_EndMovieIDStr = "Invalid",
                m_NextCourseStr = "Invalid",
                m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
            };
            newCourses.Add(44, customPracticeDatum);
            Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse> originalMainGame = new Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse>();
            foreach (var item in MainGameDef.storyCourses.ToList())
            {
                originalMainGame.Add(item);
            }
            storyDict.Add("original", originalMainGame);
            foreach (CustomCourse course in DataManager.Courses)
            {
                MgCourseDatum customCourseDatum = new MgCourseDatum();
                Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse> storyCourse = new Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse>();
                foreach (Subcategory subcat in course.Subcategories)
                {
                    Subcategory nextCourse = null;
                    if (course.Subcategories.IndexOf(subcat) + 1 < course.Subcategories.Count)
                    {
                        nextCourse = course.Subcategories[(course.Subcategories.IndexOf(subcat) + 1)];
                    }

                    switch (course.CategoryType)
                    {
                        case "Story":
                            if (nextCourse != null)
                            {
                                if (subcat.ThumbnailPath == null)
                                {
                                    subcat.ThumbnailPath = "thumbnails/custom/thumb_custworld.png";
                                }
                                else
                                {
                                    if (!IsValidThumbnail(subcat.ThumbnailPath))
                                    {
                                        subcat.ThumbnailPath = "thumbnails/custom/thumb_custworld.png";
                                    }
                                }
                                customCourseDatum = new MgCourseDatum
                                {
                                    m_GameKindStr = "Story",
                                    m_CourseStr = $"Custom_Stages_{subcat.Name}",
                                    m_courseNameTextReference = new TextReference
                                    {
                                        m_Key = $"maingame_custom_{subcat.Name}"
                                    },
                                    m_ThumbnailSpritePath = subcat.ThumbnailPath,
                                    m_NextCourse = (MainGameDef.eCourse)nextCourse.CourseEnum,
                                    m_NextCourseStr = $"Custom_Stages_{nextCourse.Name}",
                                    m_StartMovieIDStr = "Invalid",
                                    m_EndMovieIDStr = "Invalid",
                                    m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
                                };
                            }
                            else
                            {
                                if (subcat.ThumbnailPath == null)
                                {
                                    subcat.ThumbnailPath = "thumbnails/custom/thumb_custworld.png";
                                }
                                else
                                {
                                    if (!IsValidThumbnail(subcat.ThumbnailPath))
                                    {
                                        subcat.ThumbnailPath = "thumbnails/custom/thumb_custworld.png";
                                    }
                                }
                                customCourseDatum = new MgCourseDatum
                                {
                                    m_GameKindStr = "Story",
                                    m_CourseStr = $"Custom_Stages_{course.CourseName}{subcat.Name}",
                                    m_courseNameTextReference = new TextReference
                                    {
                                        m_Key = $"maingame_custom_{course.CourseName}{subcat.Name}"
                                    },
                                    m_ThumbnailSpritePath = subcat.ThumbnailPath,
                                    m_StartMovieIDStr = "Invalid",
                                    m_EndMovieIDStr = "Invalid",
                                    m_NextCourseStr = "Invalid",
                                    m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
                                };
                            }
                            storyCourse.Add((MainGameDef.eCourse)subcat.CourseEnum);
                            break;

                        case "Challenge":
                            if (subcat.ThumbnailPath == null)
                            {
                                subcat.ThumbnailPath = "thumbnails/custom/thumb_custchallenge.png";
                            }
                            else
                            {
                                if (!IsValidThumbnail(subcat.ThumbnailPath))
                                {
                                    subcat.ThumbnailPath = "thumbnails/custom/thumb_custchallenge.png";
                                }
                            }
                            customCourseDatum = new MgCourseDatum
                            {
                                m_GameKindStr = "Challenge",
                                m_CourseStr = $"Custom_Stages_{course.CourseName}{subcat.Name}",
                                m_courseNameTextReference = new TextReference
                                {
                                    m_Key = $"maingame_custom_{course.CourseName}{subcat.Name}"
                                },
                                m_ThumbnailSpritePath = subcat.ThumbnailPath,
                                m_StartMovieIDStr = "Invalid",
                                m_EndMovieIDStr = "Invalid",
                                m_NextCourseStr = "Invalid",
                                m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
                            };
                            break;

                        case "Special":
                            if (subcat.ThumbnailPath == null)
                            {
                                subcat.ThumbnailPath = "thumbnails/custom/thumb_custchallenge.png";
                            }
                            else
                            {
                                if (!IsValidThumbnail(subcat.ThumbnailPath))
                                {
                                    subcat.ThumbnailPath = "thumbnails/custom/thumb_custchallenge.png";
                                }
                            }
                            MainGameDef.eGameKind gameKind = MainGameDef.eGameKind.Invalid;
                            switch(subcat.SpecialMode)
                            {
                                case "Golden":
                                    break;

                                case "Rotten":
                                    break;

                                default:
                                    if (course.CategoryType == "Challenge")
                                    {
                                        gameKind = MainGameDef.eGameKind.Challenge;
                                    }
                                    else
                                    {
                                        gameKind = MainGameDef.eGameKind.Story;
                                    }
                                    break;
                            }
                            customCourseDatum = new MgCourseDatum
                            {
                                m_GameKind = gameKind,
                                m_GameKindStr = subcat.SpecialMode,
                                m_CourseStr = $"Custom_Stages_{course.CourseName}{subcat.Name}",
                                m_courseNameTextReference = new TextReference
                                {
                                    m_Key = $"maingame_custom_{course.CourseName}{subcat.Name}"
                                },
                                m_ThumbnailSpritePath = subcat.ThumbnailPath,
                                m_StartMovieIDStr = "Invalid",
                                m_EndMovieIDStr = "Invalid",
                                m_NextCourseStr = "Invalid",
                                m_elementList = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.element_t>()
                            };
                            break;
                    }
                    newCourses.Add(subcat.CourseEnum, customCourseDatum);
                        
                }
                if (course.CategoryType == "Story")
                {
                    storyDict.Add(course.CourseName, storyCourse);
                }
            }
            Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t>> goalList = new Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t>>();
            foreach (CustomStageYaml stage in DataManager.Stages)
            {
                Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t> goals = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t>();
                goals.Add(new MgCourseDatum.goal_t
                {
                    m_goalKindStr = "Blue",
                    m_nextStep = 1
                });
                if (stage.GreenGoalValue != 0)
                {
                    goals.Add(new MgCourseDatum.goal_t
                    {
                        m_goalKindStr = "Green",
                        m_nextStep = stage.GreenGoalValue + 1
                    });
                }
                if (stage.RedGoalValue != 0)
                {
                    goals.Add(new MgCourseDatum.goal_t
                    {
                        m_goalKindStr = "Red",
                        m_nextStep = stage.RedGoalValue + 1
                    });
                }
                goals[0].Initialize();
                customPracticeDatum.m_elementList.Add(new MgCourseDatum.element_t
                {
                    m_stageId = stage.StageId,
                    m_CalibratedStageId = 0,
                    m_IsHalfTime = false,
                    m_IsCheckPoint = false,
                    m_goalList = goals
                });
                goalList.Add(stage.StageId, goals);
            }

            foreach (CustomCourse course in DataManager.Courses)
            {
                foreach (Subcategory subcat in course.Subcategories)
                {
                    foreach (int stage in subcat.Stages)
                    {
                        if (goalList.ContainsKey((course.AuthorId << 16) + stage))
                        {
                            if (course.CategoryType == "Story")
                            {
                                Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t> goals = new Il2CppSystem.Collections.Generic.List<MgCourseDatum.goal_t>();
                                goals.Add(new MgCourseDatum.goal_t
                                {
                                    m_goalKindStr = "Blue",
                                    m_nextStep = 1
                                });
                                newCourses[subcat.CourseEnum].m_elementList.Add(new MgCourseDatum.element_t
                                {
                                    m_stageId = ((course.AuthorId << 16) + stage),
                                    m_CalibratedStageId = 0,
                                    m_IsHalfTime = false,
                                    m_IsCheckPoint = false,
                                    m_goalList = goals
                                });
                            }
                            else
                            {
                                newCourses[subcat.CourseEnum].m_elementList.Add(new MgCourseDatum.element_t
                                {
                                    m_stageId = ((course.AuthorId << 16) + stage),
                                    m_CalibratedStageId = 0,
                                    m_IsHalfTime = false,
                                    m_IsCheckPoint = false,
                                    m_goalList = goalList[((course.AuthorId << 16) + stage)]
                                });
                            }
                        }
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

            foreach (CustomStageYaml stage in DataManager.Stages)
            {
                Il2CppSystem.Collections.Generic.List<int> limitTimes = new Il2CppSystem.Collections.Generic.List<int>();
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitNormal);
                limitTimes.Add(stage.TimeLimitDarkBanana);
                limitTimes.Add(stage.TimeLimitGoldenBanana);
                string backgroundName = string.Empty;
                if (stage.CustomBackground)
                {
                    backgroundName = DataManager.TryGetBgYamlFromStageId(stage.StageId).OriginalBackgroundName.ToString();
                }
                else
                {
                    backgroundName = stage.Background;
                }
                string stageKind = string.Empty;
                if (stage.IsBonus)
                {
                    stageKind = "Bonus";
                }
                else
                {
                    stageKind = "Normal";
                }
                MgStageDatum customStageDatum = new MgStageDatum
                {
                    m_stageId = stage.StageId,
                    m_stageObjPath = string.Format("stage/custom/st{0}/stage.prefab", stage.StageId),
                    m_bgStr = backgroundName,
                    m_BgRootObjPath = string.Format("stage/custom/st{0}/BgRoot.prefab", stage.StageId),
                    m_stageKindStr = stageKind,
                    m_LimitTimeList = limitTimes,
                    m_specialLightObjPath = "",
                    m_specialBgmCueStr = "invalid",
                    m_thumbnailSpritePath = string.Format("stage/custom/st{0}/thumbnail.png", stage.StageId),
                    m_GuideAssetPath = string.Format("stage/custom/st{0}/Guide.asset", stage.StageId),
                    m_GuideRottenAssetPath = string.Format("stage/custom/st{0}/Guide_Golden.asset", stage.StageId),
                    m_GuideGoldenAssetPath = string.Format("stage/custom/st{0}/Guide_Rotten.asset", stage.StageId),
                    m_StageAnimationStartKindStr = "OnInitialize",
                    m_ShiftAnimationStartFrame = 0
                };
                customStageDatum.Initialize();
                SingletonBase<MgStageDataManager>.Instance.m_stageDataDict[stage.StageId] = customStageDatum;
                MgBgDataManager mgBgDataManager = UnityEngine.Object.FindObjectOfType<MgBgDataManager>();
                stageIndexToCue.Add(stage.StageId, mgBgDataManager.m_bgDataDict[customStageDatum.bg].bgmCue);
                stageIndexToBg.Add(stage.StageId, customStageDatum.bg);
            }

            if (SaveData.userParam.mainGameCourseParam.m_CourseParamDict.m_Dict.ContainsKey(44.ToString()))
            {
                Log.Info("Custom practice save data already found, deleting...");
                SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Remove(44.ToString());

            }
            foreach (CustomCourse course in DataManager.Courses)
            {
                foreach (Subcategory subcat in course.Subcategories)
                {
                    if (SaveData.userParam.mainGameCourseParam.m_CourseParamDict.m_Dict.ContainsKey(subcat.CourseEnum.ToString()))
                    {
                        Log.Info($"Custom save data found for {subcat.Name}, deleting...");
                        SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Remove(subcat.CourseEnum.ToString());
                    }
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
                foreach (CustomCourse course in DataManager.Courses)
                {
                    foreach (Subcategory subcat in course.Subcategories)
                    {
                        if (subcat.Stages.Contains(stage.AuthorStageId))
                        {
                            if (!courseProgressDict.ContainsKey(subcat.CourseEnum))
                            {
                                courseProgressDict.Add(subcat.CourseEnum, new SaveData.MainGameCourseParam.ProgressParamDict());
                            }
                            int count = courseProgressDict[subcat.CourseEnum].entryList.Count;
                            courseProgressDict[subcat.CourseEnum].Add(count, new SaveData.MainGameCourseParam.ProgressParam
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
                foreach (Subcategory subcat in course.Subcategories)
                {
                    SaveData.userParam.mainGameCourseParam.m_CourseParamDict.Add(subcat.CourseEnum.ToString(), new SaveData.MainGameCourseParam.CourseParam
                    {
                        m_ProgressParamDict = courseProgressDict[subcat.CourseEnum],
                        m_IsCleared = false,
                        m_IsClearedWithoutAssist = false,
                        m_IsPassedCheckPoint = false,
                        m_ChallengeModeBestClearTimeInCentiSec = 0
                    });
                }

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
                    m_Identifier = "thumbnails/custom/thumb_custpractice.png:thumb_custpractice"
                };
                dataList.m_ItemDataList.Add(itemData);
                SelMainMenuWindowBase worlds = Main.sequence.m_WindowCollection[SelectorDef.MainMenuKind.MgChallengeCourseSelect_SMB1];
                SelMgCourseSelectWindow selMgCourseSelectWindow = new SelMgCourseSelectWindow(worlds.Pointer);
                categoryDict.Add("original", selMgCourseSelectWindow.m_MgCourseData);
                foreach (CustomCourse course in DataManager.Courses)
                {
                    SelMgModeItemData courseItemData = new SelMgModeItemData();
                    switch (course.CategoryType)
                    {
                        case "Story":
                            courseItemData.transitionMenuKind = (SelectorDef.MainMenuKind)4;
                            courseItemData.mainGamemode = (SelectorDef.MainGameKind)9;
                            courseItemData.mainGameKind = MainGameDef.eGameKind.Story;
                            break;

                        case "Challenge":
                            courseItemData.transitionMenuKind = (SelectorDef.MainMenuKind)6;
                            courseItemData.mainGamemode = (SelectorDef.MainGameKind)10;
                            courseItemData.mainGameKind = MainGameDef.eGameKind.Challenge;
                            SelMgCmCourseItemDataListObject subcategories = new SelMgCmCourseItemDataListObject();
                            subcategories.m_ModeNameText = new TextReference
                            {
                                m_Key = $"maingame_custom_{course.CourseName}"
                            };
                            foreach (Subcategory subcat in course.Subcategories)
                            {
                                subcategories.m_ItemDataList.Add(new SelMgCmCourseItemData
                                {
                                    m_Course = $"{subcat.Name}",
                                    m_Difficulty = 1,
                                    m_Text = new TextReference
                                    {
                                        m_Key = $"maingame_custom_{subcat.Name}"
                                    }
                                });
                            }
                            categoryDict.Add(course.CourseName, subcategories);
                            break;

                        case "Special":
                            courseItemData.transitionMenuKind = (SelectorDef.MainMenuKind)4;
                            courseItemData.mainGamemode = (SelectorDef.MainGameKind)9;
                            courseItemData.mainGameKind = MainGameDef.eGameKind.Story;
                            foreach (Subcategory subcat in course.Subcategories)
                            {
                                
                            }
                            break;

                    }

                    courseItemData.textKey = $"maingame_custom_{course.CourseName}";
                    courseItemData.descriptionTextKey = "";
                    courseItemData.isHideText = true;
                    courseItemData.supplementaryTextKey = "";
                    string m_Identifier = string.Empty;
                    if (course.ThumbnailPath != null)
                    {
                        m_Identifier = course.ThumbnailPath + ":" + course.ThumbnailName.Substring(0, course.ThumbnailName.Length - 4);
                        if (!IsValidThumbnail(course.ThumbnailPath))
                        {
                            m_Identifier = "thumbnails/custom/thumb_custmode.png:thumb_custmode";
                        }
                    }
                    courseItemData.m_ThumbnailSpritePath = new SubAssetSpritePath
                    {
                        m_Identifier = m_Identifier
                    };
                    dataList.m_ItemDataList.Add(courseItemData);
                }

                Log.Info("Created Custom Courses.");

            }

            SelHowToPlayData howToPlayData = Main.sequence.GetData<SelHowToPlayData>((SelMainMenuSequence.Data)25);
            if (howToPlayData.m_PCData.m_MainGameDataArray.Length <= 8)
            {
                SelHowToPlayItemDataListObject[] newHowToPlayArray = new SelHowToPlayItemDataListObject[11];
                int i = 0;
                foreach (SelHowToPlayItemDataListObject item in howToPlayData.m_PCData.m_MainGameDataArray)
                {
                    newHowToPlayArray[i] = item;
                    i++;
                }
                newHowToPlayArray[8] = newHowToPlayArray[4];
                newHowToPlayArray[9] = newHowToPlayArray[1];
                newHowToPlayArray[10] = newHowToPlayArray[4];
                howToPlayData.m_PCData.m_MainGameDataArray = newHowToPlayArray;

                newHowToPlayArray = new SelHowToPlayItemDataListObject[11];
                i = 0;
                foreach (SelHowToPlayItemDataListObject item in howToPlayData.m_PCData.m_MainGameDataArray)
                {
                    newHowToPlayArray[i] = item;
                    i++;
                }
                newHowToPlayArray[8] = newHowToPlayArray[4];
                newHowToPlayArray[9] = newHowToPlayArray[1];
                newHowToPlayArray[10] = newHowToPlayArray[2];
                howToPlayData.m_ConsoleData.m_MainGameDataArray = newHowToPlayArray;
            }
        }

        private static bool IsValidThumbnail(string thumbnail)
        {
            bool result = false;
            Sprite sprite;
            try
            {
                sprite = AssetBundleCache.LoadAsset<Sprite>(thumbnail);
            }
            catch
            {
                sprite = null;
            }
            return sprite != null;
        }

        public static void VibeCheck()
        {
            try
            {
                newStoryModeDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict["original"].Count);
                for (int j = 0; j < Main.storyDict["original"].Count; j++)
                {
                    newStoryModeDef[j] = Main.storyDict["original"][j];
                }
                MainGameDef._storyCourses_k__BackingField = newStoryModeDef;
                if (newStoryModeDef != null)
                {
                    SelMainMenuWindowBase worlds = UnityEngine.Object.FindObjectOfType<SelMainMenuSequence>().m_WindowCollection[SelectorDef.MainMenuKind.MgStoryWorldSelect];
                    SelMgWorldSelectWindow window = new SelMgWorldSelectWindow(worlds.Pointer);
                    foreach (CustomCourse course in DataManager.Courses)
                    {
                        if (Main.selectedCourse == course.CourseName)
                        {
                            Il2CppSystem.Collections.Generic.List<SelIconItemData> newList = new Il2CppSystem.Collections.Generic.List<SelIconItemData>();
                            int i = 0;
                            if (window.m_ItemDataList.Count == 0)
                            {
                                foreach (Subcategory subcat in course.Subcategories)
                                {
                                    SelMgCourseItemData courseItemData = new SelMgCourseItemData();
                                    courseItemData.course = (MainGameDef.eCourse)subcat.CourseEnum;
                                    courseItemData.textKey = $"maingame_custom_{subcat.Name}";
                                    courseItemData.thumbnailSprite = AssetBundleCache.LoadAsset<Sprite>(subcat.ThumbnailPath);
                                    newList.Add(courseItemData);
                                    i++;
                                }
                            }
                            else
                            {
                                foreach (Subcategory subcat in course.Subcategories)
                                {
                                    SelMgCourseItemData courseItemData = new SelMgCourseItemData(window.m_ItemDataList[i].Pointer);
                                    courseItemData.course = (MainGameDef.eCourse)subcat.CourseEnum;
                                    courseItemData.textKey = $"maingame_custom_{subcat.Name}";
                                    courseItemData.thumbnailSprite = AssetBundleCache.LoadAsset<Sprite>(subcat.ThumbnailPath);
                                    newList.Add(courseItemData);
                                    i++;
                                }
                            }
                            window.m_ItemDataList = newList;
                            newStoryModeDef = new Il2CppStructArray<MainGameDef.eCourse>(Main.storyDict[course.CourseName].Count);
                            for (int j = 0; j < Main.storyDict[course.CourseName].Count; j++)
                            {
                                newStoryModeDef[j] = Main.storyDict[course.CourseName][j];
                            }
                            MainGameDef._storyCourses_k__BackingField = newStoryModeDef;
                        }
                    }
                }
            }
            catch
            {
                // This throws an error and I could not care less
            }

        }


        public static readonly Il2CppSystem.Collections.Generic.Dictionary<int, sound_id.cue> stageIndexToCue = new Il2CppSystem.Collections.Generic.Dictionary<int, sound_id.cue>();
        public static readonly Il2CppSystem.Collections.Generic.Dictionary<int, MainGameDef.eBg> stageIndexToBg = new Il2CppSystem.Collections.Generic.Dictionary<int, MainGameDef.eBg>();
        public static readonly Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, MgBgDatum> originalBgDatumDict = new Il2CppSystem.Collections.Generic.Dictionary<MainGameDef.eBg, MgBgDatum>();
        public static readonly Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse>> storyDict = new Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<MainGameDef.eCourse>>();
        public static readonly Il2CppSystem.Collections.Generic.Dictionary<string, SelMgCmCourseItemDataListObject> categoryDict = new Il2CppSystem.Collections.Generic.Dictionary<string, SelMgCmCourseItemDataListObject>();
        public static Il2CppStructArray<MainGameDef.eCourse> newStoryModeDef = null;

    }
}
