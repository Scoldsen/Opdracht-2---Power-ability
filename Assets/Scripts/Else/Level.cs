using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Game/Level", order = 1)]
public class Level : ScriptableObject
{
    public Vector3Int nChunks = new Vector3Int(10, 10, 10);
    public Vector3Int chunkDimensions = new Vector3Int(5, 5, 5);

    public AnimationCurve mountainShape;
    public bool solidBounds = false;
    public int shellThickness = 2;
    public float noiseScale = 0.1F;

    public int nCoins = 50;
    public int nTrees = 50;

    public float GetTop()
    {
        return nChunks.y * chunkDimensions.y;
    }

    public Vector3 GetCenterPosition()
    {
        return ExtraFunctions.MultiplyVector(nChunks * chunkDimensions, 0.5f);
    }

    public Vector3 GetLevelSize()
    {
        var xLen = nChunks.x * chunkDimensions.x;
        var yLen = nChunks.y * chunkDimensions.y;
        var zLen = nChunks.z * chunkDimensions.z;
        return new Vector3 (xLen, yLen, zLen);
    }

    public float GetMaxXZLength()
    {
        return Mathf.Max(nChunks.x * chunkDimensions.x, nChunks.z * chunkDimensions.z);
    }
}
