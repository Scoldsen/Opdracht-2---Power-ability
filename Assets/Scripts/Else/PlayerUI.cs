using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public GameObject UIPanel;
    public Camera cam;
    public TextMeshProUGUI winText;

    public void PassUIToPlayerPanel(Transform playerPanel)
    {
        UIPanel.transform.SetParent(playerPanel);
        UIPanel.gameObject.SetActive(true);
    }

    public void SetupRenderTexture(int cellWidth, int cellHeight)
    {
        RenderTexture rendTexture = new RenderTexture(cellWidth, cellHeight, 16, RenderTextureFormat.ARGB32);
        rendTexture.Create();
        cam.targetTexture = rendTexture;
        UIPanel.GetComponent<RawImage>().texture = rendTexture;
    }

    public void HideWinText()
    {
        winText.gameObject.SetActive(false);
    }

    public void ShowWinText()
    {
        winText.gameObject.SetActive(true);
    }
}
