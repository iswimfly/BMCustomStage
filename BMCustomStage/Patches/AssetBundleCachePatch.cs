using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Framework;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BMCustomStage.Patches
{

    internal static class AssetBundleCachePatch
    {

        public unsafe static void CreateDetour()
        {
            GetStreamingAssetFullpathInstance = GetStreamingAssetFullpath;
            GetStreamingAssetFullpathOriginal = ClassInjector.Detour.Detour<AssetBundleCachePatch.GetStreamingAssetFullpathDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(AssetBundleCache).GetMethod("_get_streaming_asset_fullpath")).GetValue(null)))).MethodPointer, AssetBundleCachePatch.GetStreamingAssetFullpathInstance);
        }


        private static IntPtr GetStreamingAssetFullpath(IntPtr fileNamePtr, bool isUrl)
        {
            string fileName = IL2CPP.Il2CppStringToManaged(fileNamePtr);
            string thumbnailAb;
            CustomStageYaml stage;
            CustomBgYaml bg;
            IntPtr result;
            if (DataManager.TryGetStageFromAssetBundleName(fileName, out stage))
            {
                result = IL2CPP.ManagedStringToIl2Cpp(stage.AssetBundleFullPath);
            }
            else if (DataManager.TryGetBgFromAssetBundleName(fileName, out bg))
            {
                result = IL2CPP.ManagedStringToIl2Cpp(bg.AssetBundleFullPath);
            }
            else if (DataManager.TryGetBgCustomShadersFromAssetBundleName(fileName) != null)
            {
                result = IL2CPP.ManagedStringToIl2Cpp(DataManager.TryGetBgCustomShadersFromAssetBundleName(fileName));
            }
            else if (DataManager.TryGetThumbnailAb(fileName) != null)
            {
                result = IL2CPP.ManagedStringToIl2Cpp(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "custom_stages", DataManager.TryGetThumbnailAb(fileName)));
            }
            else if (fileName.Contains("placeholder"))
            {
                result = IL2CPP.ManagedStringToIl2Cpp(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "custom_stages", "placeholder"));
            }
            else if (DataManager.lightingSettingsAbToPathDict[fileName] != null)
            {
                result = IL2CPP.ManagedStringToIl2Cpp(DataManager.lightingSettingsAbToPathDict[fileName]);
            }
            else
            {
                result = AssetBundleCachePatch.GetStreamingAssetFullpathOriginal(fileNamePtr, isUrl);
            }
            return result;
        }

        private static AssetBundleCachePatch.GetStreamingAssetFullpathDelegate GetStreamingAssetFullpathInstance;

        private static AssetBundleCachePatch.GetStreamingAssetFullpathDelegate GetStreamingAssetFullpathOriginal;

        private delegate IntPtr GetStreamingAssetFullpathDelegate(IntPtr fileName, bool isUrl);

    }
}
