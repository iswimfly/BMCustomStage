using System;
using System.Runtime.CompilerServices;
using Flash2;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace BMCustomStage.Patches
{
	
	internal class CollisionStagePartPatch
	{

        public unsafe static void CreateDetour()
        {
            AwakeInstance = Awake;
            AwakeOriginal = ClassInjector.Detour.Detour<CollisionStagePartPatch.AwakeDelegate>(UnityVersionHandler.Wrap((Il2CppMethodInfo*)((void*)((IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(CollisionStagePart).GetMethod("Awake")).GetValue(null)))).MethodPointer, CollisionStagePartPatch.AwakeInstance);
        }

        private static void Awake(IntPtr thisPtr)
        {
            if (GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)8 || GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)9 || GameParam.selectorParam.mainGameMode == (SelectorDef.MainGameKind)10)
            {
                CollisionStagePart collisionStagePart = new CollisionStagePart(thisPtr);
                MainGameStage mainGameStage = collisionStagePart.GetComponentInParent<MainGameStage>();
                bool flag2 = mainGameStage == null || mainGameStage.m_collisionTransform == null || !collisionStagePart.transform.IsChildOf(mainGameStage.m_collisionTransform);
                if (flag2)
                {
                    CollisionStagePartPatch.AwakeOriginal(thisPtr);
                    return;
                }
                bool flag3 = collisionStagePart.m_CollisionStage == null;
                if (flag3)
                {
                    Log.Warning(collisionStagePart.gameObject.name + " does not have collision data set!");
                    CollisionStagePartPatch.AwakeOriginal(thisPtr);
                    return;
                }
                collisionStagePart.transform.localScale = Vector3.Scale(collisionStagePart.transform.localScale, new Vector3(1f / collisionStagePart.transform.lossyScale.x, 1f / collisionStagePart.transform.lossyScale.y, 1f / collisionStagePart.transform.lossyScale.z));
                Log.Info("Converting collision data for " + collisionStagePart.gameObject.name + "...");
                stcoli_smkb2.PART_DATA inputData = collisionStagePart.m_CollisionStage.part;
                Mesh mesh = new Mesh();
                Il2CppStructArray<Vector3> vertices = new Il2CppStructArray<Vector3>((long)inputData.triangle.Length);
                for (int i = 0; i < inputData.triangle.Length; i++)
                {
                    vertices[i] = inputData.triangle[i].pos;
                }
                mesh.SetVertices(vertices);
                Il2CppStructArray<int> triangles = new Il2CppStructArray<int>((long)inputData.gridTriangleList.X[0].Y[0].Z[0].indexList.Length);
                for (int j = 0; j < inputData.gridTriangleList.X[0].Y[0].Z[0].indexList.Length; j++)
                {
                    triangles[j] = inputData.gridTriangleList.X[0].Y[0].Z[0].indexList[j];
                }
                mesh.SetTriangles(triangles, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                List<stcoli_smkb2.CULLING> cullingList = new List<stcoli_smkb2.CULLING>();
                List<stcoli_smkb2.CULLING> nonedgeList = new List<stcoli_smkb2.CULLING>();
                stcoli_smkb2.Settings settings = new stcoli_smkb2.Settings();
                List<stcoli_smkb2.PART_DATA> partData = stcoli_smkb2.PART_DATA.create(mesh, cullingList, nonedgeList, settings, null);
                collisionStagePart.m_CollisionStage.part = partData[0];
                Log.Info("Converted collision data for " + collisionStagePart.gameObject.name);
            }
            CollisionStagePartPatch.AwakeOriginal(thisPtr);
        }

        private static CollisionStagePartPatch.AwakeDelegate AwakeInstance;

        private static CollisionStagePartPatch.AwakeDelegate AwakeOriginal;

        private delegate void AwakeDelegate(IntPtr thisPtr);

    }
}
