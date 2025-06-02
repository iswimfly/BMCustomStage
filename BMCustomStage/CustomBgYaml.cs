using System;
using Flash2;
using YamlDotNet.Serialization;
using static Flash2.stcoli_smkb2;


namespace BMCustomStage
{
    internal class CustomBgYaml
    {

        [YamlMember(Alias = "author_name", Description = "\nThe name of the stage's author.")]
        public string AuthorName { get; set; }

        [YamlMember(Alias = "bg_name", Description = "\nThe name of the background. MUST BE UNIQUE.")]
        public string CustomBackgroundName { get; set; }

        [YamlMember(Alias = "custom_shaders", Description = "\nIf the custom background uses custom shaders, set this to true.")]
        public bool CustomShaders { get; set; }

        [YamlMember(Alias = "original_background_name", Description = "\nThe original background this custom one is replacing (used currently for skybox and music.")]
        public MainGameDef.eBg OriginalBackgroundName { get; set; }

        [YamlMember(Alias = "custom_shader_count", Description = "\nThe number of total custom shaders your background has. SHOULD BE AUTO GENERATED!!!")]
        public int CustomShaderCount { get; set; }

        [YamlMember(Alias = "custom_background_music", Description = "\nDoes this background have a custom BGM? If so, set to true. Otherwise the BGM will default to the base background.")]
        public bool CustomBGM { get; set; }

        [YamlIgnore]
        public string ShaderAssetBundleName
        {
            get
            {
                return ("shaders_" + CustomBackgroundName.ToLower());
            }
        }

        [YamlIgnore]
        public string AssetBundleName
        {
            get
            {
                return ("bg_custom_" + CustomBackgroundName.ToLower());
            }
        }

        [YamlIgnore]
        public string AssetBundleFullPath { get; set; }

        [YamlIgnore]
        public string ShaderAssetBundleFullPath { get; set; }

        [YamlIgnore]
        public string LightSceneAssetBundleFullPath { get; set; }

        [YamlIgnore]
        public string BackgroundPrefab
        {
            get
            {
                return string.Format("bg/custom/bg_{0}/background.prefab", CustomBackgroundName.ToLower());
            }
        }

        [YamlIgnore]
        public string LightPrefab
        {
            get
            {
                return string.Format("bg/custom/bg_{0}/light.prefab", CustomBackgroundName.ToLower());
            }
        }

        [YamlIgnore]
        public string ScreenEffectPrefab
        {
            get
            {
                return string.Format("bg/custom/bg_{0}/screeneffect.prefab", CustomBackgroundName.ToLower());
            }
        }

        [YamlIgnore]
        public string OriginalBackgroundAssetBundle
        {
            get
            {
                return "bg_" + OriginalBackgroundName.ToString().ToLower();
            }
        }

        [YamlIgnore]
        public string LightSceneAssetBundle
        {
            get
            {
                return "scene_lightingsettings_light_" + AuthorName.ToLower() + "_" + CustomBackgroundName.ToLower() + "_unity";
            }
        }

        [YamlIgnore]
        public int bgm_cuesheetid {  get; set; }
        [YamlIgnore]
        public int bgm_cueid { get; set; }
        [YamlIgnore]

        public int bgm_dx_cuesheetid { get; set; }
        [YamlIgnore]
        public int bgm_dx_cueid { get; set; }

        [YamlIgnore]
        public int lightSceneID { get; set; }

        [YamlIgnore]
        public string lightSceneStr
        {
            get
            {
                return "Light_" + AuthorName.ToLower() + "_" + CustomBackgroundName.ToLower();
            }
        }

    }
}

