using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    public GameObject loadScreenObject;

    [SerializeField]
    private TextMeshProUGUI loadTextObj;
    [SerializeField]
    private Slider loadingBar;
    [SerializeField]
    private Image loadingBarColorObj;

    [SerializeField]
    private float camRotationSpeed = 10;
    private Transform camFocusObject = default;

    private Camera mainCam;

    [SerializeField]
    private Texture2D image;
    public GridLayoutGroup grid;

    public float cellWidth;
    public float cellHeight;

    private int nPlayers;
    private int nScreens;

    protected override void Awake()
    {
        base.Awake();
        mainCam = Camera.main;
    }

    public void ShowLoadScreen()
    {
        loadScreenObject.SetActive(true);
    }

    public void HideLoadScreen()
    {
        loadScreenObject.SetActive(false);
    }

    public void UpdateLoadBar(string loadText, int currentIndex, int totalIndex)
    {
        loadingBar.value = (float)currentIndex / (float)totalIndex;
        loadingBarColorObj.color = Color.Lerp(Color.red, Color.green, loadingBar.value);
        loadTextObj.text = $"{loadText}";
    }

    public void SetupRotatingCamera()
    {
        var manager = GameManager.Instance;
        Vector3 camFocusPosition = ExtraFunctions.MultiplyVector(manager.terrainGenerator.currentLevel.nChunks * manager.terrainGenerator.currentLevel.chunkDimensions, 0.5f);

        camFocusObject = new GameObject("MainCameraFocusPoint").transform;
        camFocusObject.position = camFocusPosition;

        float yPos = 2 * camFocusPosition.y;
        float zPos = -2 * camFocusPosition.z;

        mainCam.transform.position = new Vector3(camFocusPosition.x, yPos, zPos);
        mainCam.transform.LookAt(camFocusPosition);
        mainCam.transform.SetParent(camFocusObject);
    }

    public void RotateCamera()
    {
        camFocusObject?.Rotate(Vector3.up, camRotationSpeed * Time.deltaTime);
    }

    public void DisableMainCamera()
    {
        mainCam.enabled = false;
    }

    private void RemoveAllScreens()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void SetRenderTexture(int playerIndex, RenderTexture texture)
    {
        var imgObj = transform.GetChild(playerIndex).GetComponent<RawImage>();
        imgObj.texture = texture;
    }

    public void CalcNewScreenValues(int players)
    {
        nScreens = players;

        //oneven aantal spelers en niet een speler
        if (nScreens % 2 == 1 && nScreens != 1) nScreens++;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        int bestFitWidth = nScreens;
        int bestFitHeight = 1;

        float desiredRatio = 1;
        float bestFoundRatioDiv = float.MaxValue;

        for (int i = nScreens; i > 0; i--)
        {
            float nVer = nScreens / i;
            if (!ExtraFunctions.IsInt(nVer)) continue;
            float nHor = nScreens / nVer;
            if (!ExtraFunctions.IsInt(nHor)) continue;

            float widthPerCel = screenWidth / nHor;
            float heightPerCel = screenHeight / nVer;

            float ratio = (heightPerCel / widthPerCel);
            float ratioDiv = Mathf.Abs(desiredRatio - ratio);

            // Bepaal beste combinatie
            if (ratioDiv <= bestFoundRatioDiv)
            {
                bestFoundRatioDiv = ratioDiv;
                bestFitWidth = (int)nHor;
                bestFitHeight = (int)nVer;

                cellWidth = screenWidth / bestFitWidth;
                cellHeight = screenHeight / bestFitHeight;
                grid.cellSize = new Vector2(cellWidth, cellHeight);
            }

            // Als het verschil weer groter wordt is hij voorbij de beste combinatie
            else return;
        }
    }

    public void HidePlayerScreens()
    {
        grid.gameObject.SetActive(false);
    }

    public void ShowPlayerScreens()
    {
        grid.gameObject.SetActive(true);
    }

    public void SetupPlayerScreens()
    {
        if (!grid.gameObject.activeInHierarchy)
        {
            ShowPlayerScreens();
            return;
        }

        nPlayers = GameManager.Instance.allPlayers.Count;
        CalcNewScreenValues(nPlayers);

        for (int i = 0; i < nPlayers; i++)
        {
            var p = GameManager.Instance.allPlayers[i];
            var playerUI = p.GetComponent<PlayerUI>();
            playerUI.SetupRenderTexture((int)cellWidth, (int)cellHeight);
            playerUI.PassUIToPlayerPanel(grid.transform);
        }

        //Dummy scherm voor oneven aantallen
        if (nScreens > nPlayers)
        {
            GameObject obj = new GameObject();
            var img = obj.AddComponent<RawImage>();
            img.texture = image;
            obj.transform.SetParent(grid.transform);
        }
    }
}
