using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private BoxCollider boxCollider;
    private TerrainGenerator generator;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        generator = FindObjectOfType<TerrainGenerator>();
    }

    public void ReshapeWinCollider()
    {
        var boxPos = GameManager.Instance.terrainGenerator.currentLevel.GetCenterPosition();
        boxPos.y = GameManager.Instance.terrainGenerator.currentLevel.GetTop() + 1;
        boxCollider.size = new Vector3(generator.currentLevel.nChunks.x * generator.currentLevel.chunkDimensions.x, 1, generator.currentLevel.nChunks.z * generator.currentLevel.chunkDimensions.z);
        boxCollider.transform.position = boxPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<NewMovingSphere>();
        if (player != null)
        {
            GameManager.Instance.OnPlayerFinish(player);
        }
    }
}
