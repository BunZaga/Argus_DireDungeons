using System.Collections.Generic;
using UnityEngine;

public class AttachMesh : MonoBehaviour
{
    public struct BoneMap
    {
        public string[] Bones;
        public int RootBone;
    }

    [SerializeField] private AttachPoint rootMesh;
    [SerializeField] private AttachPoint rootBiped;
    [SerializeField] private TransformMap transformMap;
    public TransformMap TransformMap => transformMap;

    private Dictionary<int, BoneMap> boneMaps = new Dictionary<int, BoneMap>();

    public void SetTransformMap(TransformMap transformMap)
    {
        this.transformMap = transformMap;
    }

    private BoneMap boneMap;
    
    public void CreateAndAttachMeshPiece(SkinnedMeshRenderer meshReference)
    {
        var boneMap = GenerateBoneMap(meshReference);
      
        var newObj = new GameObject(meshReference.gameObject.name);
        newObj.transform.parent = transformMap.GetAttachTransform(rootMesh);
        
        var newRenderer = newObj.AddComponent<SkinnedMeshRenderer>();

        List<Transform> tempTransformList = new List<Transform>();

        var biped = transformMap.GetAttachTransform(rootBiped);
        Transform t;
        for (int i = 0; i < boneMap.Bones.Length; ++i)
        {
            t = FindChildByName(boneMap.Bones[i], biped);
            if (t != null)
            {
                tempTransformList.Add(t);
            }
        }

        newRenderer.bones = tempTransformList.ToArray();
        newRenderer.sharedMesh = meshReference.sharedMesh;
        newRenderer.sharedMaterials = meshReference.sharedMaterials;
        newRenderer.rootBone = newRenderer.bones[boneMap.RootBone];
    }
    
    private BoneMap GenerateBoneMap(SkinnedMeshRenderer smr)
    {
        if (!boneMaps.ContainsKey(smr.bones.Length))
        {
            var newBoneMap = new BoneMap();
            newBoneMap.Bones = new string[smr.bones.Length];
            boneMaps.Add(smr.bones.Length, newBoneMap);
        }

        var boneMap = boneMaps[smr.bones.Length];
        
        for (int i = 0, ilen = smr.bones.Length; i < ilen; ++i)
        {
            boneMap.Bones[i] = smr.bones[i].name;
            if(smr.rootBone.name == smr.bones[i].name)
            {
                boneMap.RootBone = i;
            }
        }
        
        return boneMap;
    }
    
    Transform ReturnObj = null;
    private Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        ReturnObj = null;
        if (ThisGObj.name == ThisName)
        {
            return ThisGObj.transform;
        }
        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);
            if (ReturnObj)
            {
                return ReturnObj;
            }
        }
        return null;
    }
}
