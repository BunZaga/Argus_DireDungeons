using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public TransformMap TransformMap => transformMap;
    [SerializeField] private TransformMap transformMap;
}
