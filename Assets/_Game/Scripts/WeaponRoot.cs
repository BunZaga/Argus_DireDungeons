using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRoot : MonoBehaviour
{
    [SerializeField] private Transform trailTip;
    [SerializeField] private Transform trailBase;
    [SerializeField] private Transform trail;
    [SerializeField] private int trailFrameCount;
    [SerializeField] private float distanceThreshold;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;
    private int curFrame;

    private Vector3 lastTipPos;
    private Vector3 lastBasePos;
    private Vector3 tipPosition;
    private Vector3 basePosition;

    private int maxVertices;
    
    private const int NUM_VERTICES = 6;

    private void Awake()
    {
        mesh = new Mesh();
        trail.GameObject().GetComponent<MeshFilter>().mesh = mesh;
        trail.SetParent(null);
        trail.position = Vector3.zero;
        trail.rotation = Quaternion.identity;
        trail.localScale = Vector3.one;
        maxVertices = trailFrameCount * NUM_VERTICES;
        vertices = new Vector3[maxVertices];
        uvs = new Vector2[maxVertices];
        
        triangles = new int[maxVertices];
        trail.gameObject.SetActive(false);
        
    }

    public void SetTrailActive(bool value)
    {
        if (value)
        {
            ForceUpdate();
        }
        trail.gameObject.SetActive(value);
    }

    public void SetMaterial(Material mat)
    {
        trail.gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    private void CalculateUVs()
    {
        var ticX = 1.0f / trailFrameCount;
        var head = curFrame + NUM_VERTICES;

        for (int i = 0, uvtic = 1; i < maxVertices; i+= NUM_VERTICES, uvtic++)
        {
            var previousTic = ticX * (uvtic - 1);
            var nextTic = previousTic + ticX;
            var readHead = (head + i) % maxVertices;
            /*uvs[readHead] = new Vector2(nextTic, 1);
            uvs[readHead + 1] = new Vector2(nextTic, 0);
            uvs[readHead + 2] = new Vector2(previousTic, 0);*/
            uvs[readHead] = new Vector2(nextTic, 0);
            uvs[readHead + 1] = new Vector2(nextTic, 1);
            uvs[readHead + 2] = new Vector2(previousTic, 1);
            
            /*uvs[readHead + 3] = new Vector2(nextTic, 1);
            uvs[readHead + 4] = new Vector2(previousTic, 0);
            uvs[readHead + 5] = new Vector2(previousTic, 1);*/
            uvs[readHead + 3] = new Vector2(nextTic, 0);
            uvs[readHead + 4] = new Vector2(previousTic, 1);
            uvs[readHead + 5] = new Vector2(previousTic, 0);
        }
        
        mesh.uv = uvs;
    }
    
    private void ForceUpdate()
    {
        tipPosition = trailTip.position;
        basePosition = trailBase.position;
        
        vertices[curFrame] = basePosition;
        vertices[curFrame + 1] = tipPosition;
        vertices[curFrame + 2] = lastTipPos;
        
        vertices[curFrame + 3] = basePosition;
        vertices[curFrame + 4] = lastTipPos;
        vertices[curFrame + 5] = lastBasePos;
        
        /*vertices[curFrame + 6] = basePosition;
        vertices[curFrame + 7] = lastTipPos;
        vertices[curFrame + 8] = tipPosition;
        
        vertices[curFrame + 9] = lastTipPos;
        vertices[curFrame + 10] = basePosition;
        vertices[curFrame + 11] = lastBasePos;*/
        
        triangles[curFrame] = curFrame;
        triangles[curFrame + 1] = curFrame + 1;
        triangles[curFrame + 2] = curFrame + 2;
        triangles[curFrame + 3] = curFrame + 3;
        triangles[curFrame + 4] = curFrame + 4;
        triangles[curFrame + 5] = curFrame + 5;
        /*triangles[curFrame + 6] = curFrame + 6;
        triangles[curFrame + 7] = curFrame + 7;
        triangles[curFrame + 8] = curFrame + 8;
        triangles[curFrame + 9] = curFrame + 9;
        triangles[curFrame + 10] = curFrame + 10;
        triangles[curFrame + 11] = curFrame + 11;*/

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        lastTipPos = tipPosition;
        lastBasePos = basePosition;
        CalculateUVs();
        curFrame += NUM_VERTICES;
    }
    
    private void LateUpdate()
    {
        tipPosition = trailTip.position;

        if (Vector3.Distance(tipPosition, lastTipPos) < distanceThreshold)
            return;
        
        if (curFrame == maxVertices)
        {
            curFrame = 0;
        }
        
        basePosition = trailBase.position;
        
        vertices[curFrame] = basePosition;
        vertices[curFrame + 1] = tipPosition;
        vertices[curFrame + 2] = lastTipPos;
        
        vertices[curFrame + 3] = basePosition;
        vertices[curFrame + 4] = lastTipPos;
        vertices[curFrame + 5] = lastBasePos;
        
        /*vertices[curFrame + 6] = basePosition;
        vertices[curFrame + 7] = lastTipPos;
        vertices[curFrame + 8] = tipPosition;
        
        vertices[curFrame + 9] = lastTipPos;
        vertices[curFrame + 10] = basePosition;
        vertices[curFrame + 11] = lastBasePos;*/
        
        triangles[curFrame] = curFrame;
        triangles[curFrame + 1] = curFrame + 1;
        triangles[curFrame + 2] = curFrame + 2;
        triangles[curFrame + 3] = curFrame + 3;
        triangles[curFrame + 4] = curFrame + 4;
        triangles[curFrame + 5] = curFrame + 5;
        /*triangles[curFrame + 6] = curFrame + 6;
        triangles[curFrame + 7] = curFrame + 7;
        triangles[curFrame + 8] = curFrame + 8;
        triangles[curFrame + 9] = curFrame + 9;
        triangles[curFrame + 10] = curFrame + 10;
        triangles[curFrame + 11] = curFrame + 11;*/
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        lastTipPos = tipPosition;
        lastBasePos = basePosition;
        
        CalculateUVs();
        curFrame += NUM_VERTICES;
    }
}
