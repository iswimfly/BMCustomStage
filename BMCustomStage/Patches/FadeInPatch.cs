using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Flash2;
using Framework;
using UnhollowerBaseLib.Runtime;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using System.Reflection;

namespace BMCustomStage.Patches
{
    internal class FadeInPatch
    {

        public static unsafe void CreateDetour()
        {
            FadeInInstance = FadeIn;
            MethodBase method = typeof(MainGame).GetMethods().Where(x => x.Name == "FadeIn").First();
            FadeInOriginal = ClassInjector.Detour.Detour<FadeInPatch.FadeInDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null)))).MethodPointer, FadeInPatch.FadeInInstance);
        }

        private static void FadeIn(IntPtr thisPtr)
        {
            GameObject bgObject = MainGame.Instance.bgObj;
            foreach (Renderer renderer in bgObject.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    Shader newShader;
                    if (material.shader.name == "Custom/WormholeSeeThrough")
                    {
                        newShader = FadeInPatch.WormholeShader;
                    }
                    else
                    {
                        string abPath;
                        if (FadeInPatch.shaderNameToAbPath.TryGetValue(material.shader.name, out abPath))
                        {
                            newShader = AssetBundleCache.LoadAsset<Shader>(abPath);
                            int oldRenderQueue = material.renderQueue;
                            material.shader = newShader;
                            material.renderQueue = oldRenderQueue;
                        }
                    }
                }
            }

            FadeInPatch.FadeInOriginal(thisPtr);
        }

        private static FadeInPatch.FadeInDelegate FadeInInstance;


        private static FadeInPatch.FadeInDelegate FadeInOriginal;

        private delegate void FadeInDelegate(IntPtr thisPtr);

        private static readonly Dictionary<string, string> shaderNameToAbPath = new Dictionary<string, string>
        {
            {
                "Custom/BilliardsGuideLine",
                "shader/billiardsguideline.shader"
            },
            {
                "Custom/BilliardsTargetGuideLine",
                "shader/billiardstargetguideline.shader"
            },
            {
                "Custom/BoatWave",
                "shader/boatwave.shader"
            },
            {
                "Custom/BoxBlur",
                "shader/boxblur.shader"
            },
            {
                "Custom/DepthMask",
                "shader/depthmask.shader"
            },
            {
                "Custom/DiffuseColorDebug",
                "shader/diffusecolordebug.shader"
            },
            {
                "Custom/DiffuseColorOverlayDebug",
                "shader/diffusecoloroverlaydebug.shader"
            },
            {
                "Custom/DiffuseVc",
                "shader/diffusevc.shader"
            },
            {
                "SEGA/Effect/Effect Uber",
                "shader/effectuber.shader"
            },
            {
                "Shader Forge/FlowmapBlend",
                "shader/flowmapblend.shader"
            },
            {
                "Shader Forge/FlowmapLightning",
                "shader/flowmaplightning.shader"
            },
            {
                "Shader Forge/FlowmapNonParticleScroll",
                "shader/flowmapnonparticlescroll.shader"
            },
            {
                "Shader Forge/FlowmapNonParticleScrollMask",
                "shader/flowmapnonparticlescrollmask.shader"
            },
            {
                "Shader Forge/FlowmapOpaqueScroll",
                "shader/flowmapopaquescroll.shader"
            },
            {
                "Shader Forge/FlowmapPtnAnm",
                "shader/flowmapptnanm.shader"
            },
            {
                "Shader Forge/FlowmapScrollAdd",
                "shader/flowmapscrolladd.shader"
            },
            {
                "Shader Forge/FlowmapScrollAddMask",
                "shader/flowmapscrolladdmask.shader"
            },
            {
                "Shader Forge/FlowmapScrollAllBlend",
                "shader/flowmapscrollallblend.shader"
            },
            {
                "Shader Forge/FlowmapScrollBlend",
                "shader/flowmapscrollblend.shader"
            },
            {
                "Shader Forge/FlowmapScrollBlendTilt",
                "shader/flowmapscrollblendtilt.shader"
            },
            {
                "Shader Forge/FlowmapScrollMultiply",
                "shader/flowmapscrollmultiply.shader"
            },
            {
                "Shader Forge/Fresnel",
                "shader/fresnel.shader"
            },
            {
                "Custom/GaussianBlur",
                "shader/gaussianblur.shader"
            },
            {
                "Custom/GaussianBlurDepth",
                "shader/gaussianblurdepth.shader"
            },
            {
                "Shader Forge/Genzan",
                "shader/genzan.shader"
            },
            {
                "Custom/ImageEffectBrightness",
                "shader/imageeffectbrightness.shader"
            },
            {
                "Custom/Normal",
                "shader/normal.shader"
            },
            {
                "Shader Forge/P_AddBlend",
                "shader/p_addblend.shader"
            },
            {
                "Shader Forge/P_AddBlendCutOff",
                "shader/p_addblendcutoff.shader"
            },
            {
                "Shader Forge/P_AddBlendCutOffIgnoreProjector ",
                "shader/p_addblendcutoffignoreprojector.shader"
            },
            {
                "Shader Forge/P_AddBlendTilt",
                "shader/p_addblendtilt.shader"
            },
            {
                "Shader Forge/P_Additive",
                "shader/p_additive.shader"
            },
            {
                "Shader Forge/P_Blend",
                "shader/p_blend.shader"
            },
            {
                "Shader Forge/P_ForceDraw",
                "shader/p_forcedraw.shader"
            },
            {
                "Shader Forge/P_UVScroll",
                "shader/p_uvscroll.shader"
            },
            {
                "Shader Forge/P_UVScrollAddA",
                "shader/p_uvscrolladda.shader"
            },
            {
                "Shader Forge/P_UVScrollAnimBlend",
                "shader/p_uvscrollanimblend.shader"
            },
            {
                "Shader Forge/P_UVScrollMultiAdd",
                "shader/p_uvscrollmultiadd.shader"
            },
            {
                "Shader Forge/P_UVScrollMultiRand",
                "shader/p_uvscrollmultirand.shader"
            },
            {
                "Shader Forge/P_UVScrollMultiRandAdd",
                "shader/p_uvscrollmultirandadd.shader"
            },
            {
                "Shader Forge/P_UVScrollMultiRandBlend",
                "shader/p_uvscrollmultirandblend.shader"
            },
            {
                "Custom/Particle/ParticleEffectMaker",
                "shader/particleeffectmaker.shader"
            },
            {
                "Custom/Particle/ParticleEffectMakerAdditive",
                "shader/particleeffectmakeradditive.shader"
            },
            {
                "Custom/Particle/ParticleEffectMakerMulti",
                "shader/particleeffectmakermulti.shader"
            },
            {
                "Custom/Particle/ParticleEffectMakerMultiAdditive",
                "shader/particleeffectmakermultiadditive.shader"
            },
            {
                "Shader Forge/ScreenDistortion",
                "shader/screendistortion.shader"
            },
            {
                "Custom/Shimmer",
                "shader/shimmer.shader"
            },
            {
                "Custom/Specular",
                "shader/specular.shader"
            },
            {
                "Custom/SpriteCrossFadeShader",
                "shader/spritecrossfadeshader.shader"
            },
            {
                "Custom/SurfaceGrTrVc",
                "shader/surfacegrtrvc.shader"
            },
            {
                "Custom/SurfaceLpVc",
                "shader/surfacelpvc.shader"
            },
            {
                "Custom/SurfaceOfsShader",
                "shader/surfaceofs.shader"
            },
            {
                "Custom/SurfacePtVc",
                "shader/surfaceptvc.shader"
            },
            {
                "Custom/SurfaceSemiGrTrVc",
                "shader/surfacesemigrtrvc.shader"
            },
            {
                "Custom/SurfaceSemiTrVc",
                "shader/surfacesemitrvc.shader"
            },
            {
                "Custom/SurfaceTrVc",
                "shader/surfacetrvc.shader"
            },
            {
                "Custom/SurfaceVc",
                "shader/surfacevc.shader"
            },
            {
                "Custom/SurfaceVcScr",
                "shader/surfacevcscr.shader"
            },
            {
                "Custom/TestShader",
                "shader/testshader.shader"
            },
            {
                "Custom/TextureBbVc",
                "shader/texturebbvc.shader"
            },
            {
                "Custom/TextureClampArrowBbVc",
                "shader/textureclamparrowbbvc.shader"
            },
            {
                "Custom/TextureClampBbVc",
                "shader/textureclampbbvc.shader"
            },
            {
                "Custom/TextureLpAd",
                "shader/texturelpad.shader"
            },
            {
                "Custom/TextureMkVc",
                "shader/texturemkvc.shader"
            },
            {
                "Custom/TextureVa",
                "shader/textureva.shader"
            },
            {
                "Custom/TextureVc",
                "shader/texturevc.shader"
            },
            {
                "Custom/TextureVcCutoutPattern",
                "shader/texturevccutoutpattern.shader"
            },
            {
                "Custom/TextureVcOpaque",
                "shader/texturevcopaque.shader"
            },
            {
                "Custom/TextureVcOpaquePattern",
                "shader/texturevcopaquepattern.shader"
            },
            {
                "Custom/ToonRim",
                "shader/toonrim.shader"
            },
            {
                "Custom/ToonRimPt",
                "shader/toonrimpt.shader"
            },
            {
                "Custom/ToonRimTr",
                "shader/toonrimtr.shader"
            },
            {
                "Custom/UIAddShader",
                "shader/ui/uiaddshader.shader"
            },
            {
                "Hidden/UI/AlphaMask",
                "shader/ui/uialphamask.shader"
            },
            {
                "Hidden/Default (UIDissolve)",
                "shader/ui/uidissolve.shader"
            },
            {
                "Hidden/Default (UIEffect)",
                "shader/ui/uieffect.shader"
            },
            {
                "Hidden/Default (UIHsvModifier)",
                "shader/ui/uihsvmodifier.shader"
            },
            {
                "UI/IgnoreAlpha",
                "shader/ui/uiingorealpha.shader"
            },
            {
                "Hidden/Default (UIShiny)",
                "shader/ui/uishiny.shader"
            },
            {
                "Hidden/Default (UITransition)",
                "shader/ui/uitransition.shader"
            },
            {
                "Custom/Reflective/SpecularOpaque",
                "shader/unityreflectivespecular.shader"
            },
            {
                "Custom/Reflective/SpecularTransparent",
                "shader/unityreflectivespecularalpha.shader"
            },
            {
                "Shader Forge/UnlitAddVC",
                "shader/unlitaddvc.shader"
            },
            {
                "Shader Forge/UnlitBlendVC",
                "shader/unlitblendvc.shader"
            },
            {
                "Custom/UnlitColorDebug",
                "shader/unlitcolordebug.shader"
            },
            {
                "Shader Forge/UnlitOpaqueVC",
                "shader/unlitopaquevc.shader"
            },
            {
                "sidefx/VAT_softBodyFlowMap",
                "shader/vat_softbodyflowmap.shader"
            },
            {
                "sidefx/VAT_softBodyFlowMapAdd",
                "shader/vat_softbodyflowmapadd.shader"
            },
            {
                "Shader Forge/VelvetAdd",
                "shader/velvetadd.shader"
            },
            {
                "sidefx/vertex_rigid_body_shader",
                "shader/vertex_rigid_body.shader"
            }
        };

        private static Shader WormholeShader
        {
            get
            {
                bool flag = FadeInPatch._wormholeShader == null;
                if (flag)
                {
                    GameObject tiersGo = AssetBundleCache.GetAsset<GameObject>("stage/smb2/pot/st2042/st2042.prefab");
                    bool flag2 = tiersGo == null;
                    if (flag2)
                    {
                        tiersGo = AssetBundleCache.LoadAsset<GameObject>("stage/smb2/pot/st2042/st2042.prefab");
                        Log.Info("Loaded Tiers");
                    }
                    FadeInPatch._wormholeShader = tiersGo.GetComponent<MainGameStage>().m_WormholeList[0].transform.Find("Behavior").Find("obj_wormhole").Find("worm_surface").GetComponent<MeshRenderer>().material.shader;
                    Log.Info("Fished WormholeSeeThrough shader out of Tiers");
                }
                return FadeInPatch._wormholeShader;
            }
        }


        private static Shader _wormholeShader = null;
    }
}
