using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMap : MonoBehaviour
{
    [System.Serializable]
    public struct TransformMapKvP
    {
        public AttachPoint AttachPoint => attachPoint;
        [SerializeField] private AttachPoint attachPoint;
        
        public Transform Transform =>transform;
        [SerializeField] private Transform transform;
    }

    [SerializeField] private List<TransformMapKvP> attachPointList;
    
    [SerializeField] private Dictionary<AttachPoint, Transform> attachPointMap = new Dictionary<AttachPoint, Transform>();

    private void Awake()
    {
        attachPointMap.Clear();
        for (int i = 0, ilen = attachPointList.Count; i < ilen; i++)
        {
            attachPointMap.Add(attachPointList[i].AttachPoint, attachPointList[i].Transform);
        }
        attachPointList.Clear();
    }

    public Transform GetAttachTransform(AttachPoint attachPoint)
    {
        attachPointMap.TryGetValue(attachPoint, out var tfm);
        return tfm;
    }
    
    public void SetAttachTransform(AttachPoint attachPoint, Transform transform)
    {
        attachPointMap[attachPoint] = transform;
    }
}
