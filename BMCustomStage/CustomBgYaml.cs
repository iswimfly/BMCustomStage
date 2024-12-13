using System;
using Flash2;
using YamlDotNet.Serialization;


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

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000014 RID: 20 RVA: 0x00002103 File Offset: 0x00000303
        [YamlIgnore]
        public string AssetBundleName
        {
            get
            {
                return ("bg_custom_" + CustomBackgroundName.ToLower());
            }
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x06000015 RID: 21 RVA: 0x00002125 File Offset: 0x00000325
        // (set) Token: 0x06000016 RID: 22 RVA: 0x0000212D File Offset: 0x0000032D
        [YamlIgnore]
        public string AssetBundleFullPath { get; set; }

        [YamlIgnore]
        public string ShaderAssetBundleFullPath { get; set; }

        [YamlIgnore]
        public string BackgroundPrefab
        {
            get
            {
                return string.Format("bg/custom/bg_{0}/background.prefab", CustomBackgroundName.ToLower());
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
        public int bgm_cuesheetid {  get; set; }
        [YamlIgnore]
        public int bgm_cueid { get; set; }
        [YamlIgnore]

        public int bgm_dx_cuesheetid { get; set; }
        [YamlIgnore]
        public int bgm_dx_cueid { get; set; }

    }
}

