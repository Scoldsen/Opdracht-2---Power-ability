using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    UIManager manager;
    private RenderTexture rendTexture;
    private Camera cam;
    public int playerIndex;

    void Start()
    {
        cam = GetComponent<Camera>();
        manager = FindObjectOfType<UIManager>();
        rendTexture = new RenderTexture((int)manager.cellWidth, (int)manager.cellHeight, 16, RenderTextureFormat.ARGB32);
        rendTexture.Create();
        cam.targetTexture = rendTexture;
        manager.SetRenderTexture(playerIndex, rendTexture);
    }
}
