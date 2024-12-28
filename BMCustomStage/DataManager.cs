using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Flash2;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace BMCustomStage
{
	internal static class DataManager
	{
		public static IReadOnlyCollection<CustomStageYaml> Stages
		{
			get
			{
				return DataManager.uniqueIdToStage.Values;
			}
		}

		public static IReadOnlyCollection<CustomBgYaml> Backgrounds
		{
			get
			{
				return DataManager.uniqueBgToData.Values;
			}
		}

		public static IReadOnlyCollection<CustomCourse> Courses
		{
			get
			{
				return DataManager.nameToCourse.Values;
			}
		}

		public static void Initialise()
		{
            string stageFolderPath;
            if (!Main.melonMode)
            {
                stageFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "custom_stages");
            }
            else
            {
                stageFolderPath = Path.Combine("H:\\SteamLibrary\\steamapps\\common\\smbbm\\mods\\", "custom_stages");
            }
            if (Directory.Exists(stageFolderPath))
            {
                Dictionary<int, ValueTuple<CustomStageYaml, DateTime>> stages = new Dictionary<int, ValueTuple<CustomStageYaml, DateTime>>();
                IDeserializer deserializer = new DeserializerBuilder().Build();
                IEnumerable<string> source = Directory.EnumerateDirectories(stageFolderPath);
                foreach (string packPath in source)
                {
                    string packName = packPath.Split('\\').Last();
                    bool thumbnails = File.Exists(Path.Combine(packPath + $"\\{packName}_thumbnails"));
                    if (thumbnails)
                    {
                        thumbnailAbNameToAbPath.Add($"{packName}_thumbnails", Path.Combine(packPath + $"\\{ packName}_thumbnails"));
                    }
                    string stagesPath = Path.Combine(packPath, "stages");
                    bool flag2 = !Directory.Exists(stagesPath);
                    if (!flag2)
                    {
                        IEnumerable<string> source2 = Directory.EnumerateDirectories(stagesPath);
                        foreach (string stagePath in source2)
                        {
                            string stageConfigPath = Path.Combine(stagePath, "custom_stage.yaml");
                            if (!File.Exists(stageConfigPath))
                            {
                                Log.Warning("Failed to load stage at " + stagePath + ":\n    The custom_stage.yaml configuration file does not exist.");
                            }
                            else
                            {
                                CustomStageYaml customStageYaml;
                                try
                                {
                                    customStageYaml = deserializer.Deserialize<CustomStageYaml>(File.ReadAllText(stageConfigPath));
                                }
                                catch
                                {
                                    Log.Warning("Failed to load stage at " + stagePath + ":\n    An unknown error occured while parsing the custom_stage.yaml file.");
                                    continue;
                                }
                                if (customStageYaml.AuthorId == 0 || customStageYaml.AuthorId > 32767)
                                {
                                    Log.Warning("Failed to load stage at " + stagePath + ":\n    The author ID is invalid.");
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(customStageYaml.AuthorName))
                                    {
                                        Log.Warning("Failed to load stage at " + stagePath + ":\n    The author name is empty.");
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(customStageYaml.StageName))
                                        {
                                            Log.Warning("Failed to load stage at " + stagePath + ":\n    The stage name is empty.");
                                        }
                                        else
                                        {
                                            if (customStageYaml.Background == null)
                                            {
                                                Log.Warning("Failed to load stage at " + stagePath + ":\n    The background is invalid.");
                                            }
                                            else
                                            {
                                                customStageYaml.AssetBundleFullPath = Path.Combine(stagePath, customStageYaml.AssetBundleName);

                                                if (!File.Exists(customStageYaml.AssetBundleFullPath))
                                                {
                                                    Log.Warning("Failed to load stage at " + stagePath + ":\n    The asset bundle file does not exist.");
                                                }
                                                else
                                                {
                                                    DateTime lastModifiedTime = File.GetLastWriteTimeUtc(customStageYaml.AssetBundleFullPath);
                                                    ValueTuple<CustomStageYaml, DateTime> existingStage;
                                                    if (stages.TryGetValue(customStageYaml.StageId, out existingStage))
                                                    {
                                                        bool flag10 = lastModifiedTime < existingStage.Item2;
                                                        if (flag10)
                                                        {
                                                            Log.Info(string.Concat(new string[]
                                                            {
                                                                "Skipping stage at ",
                                                                stagePath,
                                                                " because ",
                                                                Path.GetDirectoryName(existingStage.Item1.AssetBundleFullPath),
                                                                " is newer"
                                                            }));
                                                            continue;
                                                        }
                                                        Log.Info(string.Concat(new string[]
                                                        {
                                                            "Skipping stage at ",
                                                            Path.GetDirectoryName(existingStage.Item1.AssetBundleFullPath),
                                                            " because ",
                                                            stagePath,
                                                            " is newer"
                                                        }));
                                                    }
                                                    customStageYaml.PackPath = packPath.Split('\\').Last();
                                                    stages[customStageYaml.StageId] = new ValueTuple<CustomStageYaml, DateTime>(customStageYaml, lastModifiedTime);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string[] packsArray = Directory.GetFiles(packPath);
                    Array.Sort(packsArray);
                    foreach (string file in packsArray)
                    {
                        Log.Info(file);
                        if (file.Contains(".json"))
                        {
                            CustomCourse course = JsonConvert.DeserializeObject<CustomCourse>(File.ReadAllText(file));
                            if (thumbnails)
                            {
                                course.ThumbnailAssetBundle = packName + "_thumbnails";
                                course.ThumbnailPath = $"thumbnails/custom/{course.ThumbnailName}";
                            }
                            else
                            {
                                course.ThumbnailPath = null;
                            }
                            foreach (Subcategory subcat in course.Subcategories)
                            {
                                subcat.CourseEnum = ((int)course.AuthorId << 16) + (int)subcat.CourseID;
                                if (string.IsNullOrEmpty(subcat.ThumbnailName))
                                {
                                    subcat.ThumbnailPath = null;
                                }
                                else
                                {
                                    if (thumbnails)
                                    {
                                        subcat.ThumbnailPath = $"thumbnails/custom/{subcat.ThumbnailName}";
                                    }
                                    else
                                    {
                                        subcat.ThumbnailPath = null;
                                    }
                                }
                            }
                            if (course.CategoryType == "Story" || course.CategoryType == "Challenge")
                            {
                                nameToCourse.Add(course.CourseName, course);
                            }
                            else
                            {
                                Log.Error($"Invalid Course: {course.CourseName}. Please specify \"Story\" or \"Challenge\" as the category type.");
                            }
                        }
                    }
                }

                foreach (CustomStageYaml customStageYaml2 in from value in stages.Values
				orderby value.Item1.StageId
				select value.Item1)
				{
					DataManager.uniqueIdToStage[customStageYaml2.StageId] = customStageYaml2;
					DataManager.abNameToStageId[customStageYaml2.AssetBundleName] = customStageYaml2.StageId;
					Log.Info("Added custom stage at " + Path.GetDirectoryName(customStageYaml2.AssetBundleFullPath));
				}

                // Leaving this here in case I change where things are placed. Probably won't though.
                string bgFolderPath;
                if (Main.melonMode)
                {
                    bgFolderPath = Path.Combine("H:\\SteamLibrary\\steamapps\\common\\smbbm\\mods\\", "custom_stages");
                }
                else
                {
                    bgFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "custom_stages");
                }
                if (Directory.Exists(bgFolderPath))
                {
                    Dictionary<string, ValueTuple<CustomBgYaml, DateTime>> backgrounds = new Dictionary<string, ValueTuple<CustomBgYaml, DateTime>>();
                    IDeserializer bgDeserializer = new DeserializerBuilder().Build();
                    IEnumerable<string> pathDirectories = Directory.EnumerateDirectories(bgFolderPath);

                    foreach (string packPath in pathDirectories)
					{
                        string backgroundPath = Path.Combine(packPath, "backgrounds");
                        CustomBgYaml customBgYaml = new CustomBgYaml();
						if (Directory.Exists(backgroundPath))
						{
							IEnumerable<string> bgDirectories = Directory.EnumerateDirectories(backgroundPath);
							foreach(string bgDirectory in bgDirectories)
							{
                                string bgConfigPath = Path.Combine(bgDirectory, "custom_bg.yaml");
                                try
                                {
                                    customBgYaml = bgDeserializer.Deserialize<CustomBgYaml>(File.ReadAllText(bgConfigPath));
                                }
                                catch
                                {
                                    Log.Warning("Failed to load background at " + bgFolderPath + ":\n    An unknown error occured while parsing the custom_bg.yaml file.");
                                    continue;
                                }

                                if (string.IsNullOrWhiteSpace(customBgYaml.AuthorName))
                                {
                                    Log.Warning("Failed to load background at " + bgFolderPath + ":\n    The author name was empty.");
                                }
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(customBgYaml.CustomBackgroundName))
                                    {
                                        Log.Warning("Failed to load background at " + bgFolderPath + ":\n    The background does not have a name.");
                                    }
									else
									{
										if (string.IsNullOrWhiteSpace(customBgYaml.OriginalBackgroundName.ToString()))
										{
											Log.Warning("Failed to load background at " + bgFolderPath + ":\n    No original background found.");
										}
										else
										{
                                            customBgYaml.AssetBundleFullPath = Path.Combine(bgDirectory, customBgYaml.AssetBundleName);
                                            if (!File.Exists(customBgYaml.AssetBundleFullPath))
                                            {
                                                Log.Warning("Failed to load background at " + bgFolderPath + $":\n    Asset Bundle not found at: \"{customBgYaml.AssetBundleFullPath}\"");
                                            }
                                            else
                                            {
                                                if (customBgYaml.CustomShaders)
                                                {
                                                    customBgYaml.ShaderAssetBundleFullPath = Path.Combine(bgDirectory, customBgYaml.ShaderAssetBundleName);
                                                    if (!File.Exists(customBgYaml.AssetBundleFullPath))
                                                    {
                                                        Log.Warning("Failed to load background at " + bgFolderPath + $":\n    Asset Bundle not found at: \"{customBgYaml.AssetBundleFullPath}\"");
                                                    }
                                                    else
                                                    {
                                                        DateTime lastModifiedTime = File.GetLastWriteTimeUtc(customBgYaml.AssetBundleFullPath);
                                                        ValueTuple<CustomBgYaml, DateTime> existingBg;
                                                        if (backgrounds.TryGetValue(customBgYaml.CustomBackgroundName, out existingBg))
                                                        {
                                                            bool flag10 = lastModifiedTime < existingBg.Item2;
                                                            if (flag10)
                                                            {
                                                                Log.Info(string.Concat(new string[]
                                                                {
                                                                "Skipping background at ",
                                                                bgDirectory,
                                                                " because ",
                                                                Path.GetDirectoryName(existingBg.Item1.AssetBundleFullPath),
                                                                " is newer"
                                                                }));
                                                                continue;
                                                            }
                                                            Log.Info(string.Concat(new string[]
                                                            {
                                                            "Skipping background at ",
                                                            Path.GetDirectoryName(existingBg.Item1.AssetBundleFullPath),
                                                            " because ",
                                                            bgDirectory,
                                                            " is newer"
                                                            }));
                                                        }
                                                        backgrounds[customBgYaml.CustomBackgroundName] = new ValueTuple<CustomBgYaml, DateTime>(customBgYaml, lastModifiedTime);
                                                    }
                                                }
                                                else
                                                {
                                                    DateTime lastModifiedTime = File.GetLastWriteTimeUtc(customBgYaml.AssetBundleFullPath);
                                                    ValueTuple<CustomBgYaml, DateTime> existingBg;
                                                    if (backgrounds.TryGetValue(customBgYaml.CustomBackgroundName, out existingBg))
                                                    {
                                                        bool flag10 = lastModifiedTime < existingBg.Item2;
                                                        if (flag10)
                                                        {
                                                            Log.Info(string.Concat(new string[]
                                                            {
                                                                "Skipping background at ",
                                                                bgDirectory,
                                                                " because ",
                                                                Path.GetDirectoryName(existingBg.Item1.AssetBundleFullPath),
                                                                " is newer"
                                                            }));
                                                            continue;
                                                        }
                                                        Log.Info(string.Concat(new string[]
                                                        {
                                                            "Skipping background at ",
                                                            Path.GetDirectoryName(existingBg.Item1.AssetBundleFullPath),
                                                            " because ",
                                                            bgDirectory,
                                                            " is newer"
                                                        }));
                                                    }
                                                    backgrounds[customBgYaml.CustomBackgroundName] = new ValueTuple<CustomBgYaml, DateTime>(customBgYaml, lastModifiedTime);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

						}

                    }
					foreach (CustomBgYaml customBgYaml in from value in backgrounds.Values
					select value.Item1)
					{
						DataManager.uniqueBgToData[customBgYaml.CustomBackgroundName] = customBgYaml;
						DataManager.abNameToBgName[customBgYaml.AssetBundleName] = customBgYaml.CustomBackgroundName;
						Log.Info("Added custom background at " + Path.GetDirectoryName(customBgYaml.AssetBundleFullPath));
						if (customBgYaml.CustomShaders)
						{
							Log.Info("Added custom shaders for " + customBgYaml.CustomBackgroundName);
						}
					}
                }


            }
		}

		public static bool TryGetStageFromAssetBundleName(string assetBundleName, out CustomStageYaml stage)
		{
			int stageId;
			bool result;
			if (DataManager.abNameToStageId.TryGetValue(assetBundleName, out stageId))
			{
				result = DataManager.uniqueIdToStage.TryGetValue(stageId, out stage);
			}
			else
			{
				stage = null;
				result = false;
			}
			return result;
		}

		public static bool TryGetBgFromAssetBundleName(string assetBundleName, out CustomBgYaml bg)
		{
			string bgName;
			bool result = true;
            if (DataManager.abNameToBgName.TryGetValue(assetBundleName, out bgName))
            {
                result = DataManager.uniqueBgToData.TryGetValue(bgName, out bg);
            }
            else
            {
                bg = null;
                result = false;
            }
            return result;
		}

        public static string TryGetBgCustomShadersFromAssetBundleName(string assetBundleName)
        {
            foreach (CustomBgYaml bg in Backgrounds)
			{
				if (bg.CustomShaders)
				{
					if (assetBundleName == bg.ShaderAssetBundleName)
					{
						return bg.ShaderAssetBundleFullPath;
					}
				}
			}
			return null;
        }

		public static CustomBgYaml TryGetBgYamlFromStageId(int stageId)
		{
			CustomBgYaml result = null;
			CustomStageYaml stage = null;
			if (DataManager.uniqueIdToStage.TryGetValue(stageId, out stage))
			{
				DataManager.uniqueBgToData.TryGetValue(stage.CustomBackgroundName, out result);
			}
			return result;
		}

        public static string TryGetThumbnailAb(string assetBundleName)
        {
            string result = null;
            thumbnailAbNameToAbPath.TryGetValue(assetBundleName, out result);
            return result;
        }
        private static readonly Dictionary<int, CustomStageYaml> uniqueIdToStage = new Dictionary<int, CustomStageYaml>();

		private static readonly Dictionary<string, int> abNameToStageId = new Dictionary<string, int>();

		private static readonly Dictionary<string, CustomBgYaml> uniqueBgToData = new Dictionary<string, CustomBgYaml>();

		private static readonly Dictionary<string, string> abNameToBgName = new Dictionary<string, string>();

		private static readonly Dictionary<string, CustomCourse> nameToCourse = new Dictionary<string, CustomCourse>();

        private static readonly Dictionary<string, string> thumbnailAbNameToAbPath = new Dictionary<string, string>();

	}
}
