using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    public TerrainGenerator terrainGenerator;

    private CancellationTokenSource cancel;
    public CancellationToken cancelToken;

    [SerializeField]
    private TextMeshProUGUI countdownObj;

    [SerializeField]
    private int nPlayers = 1;

    public PlayerInputManager playerInputManager;
    private State currentState;

    private Dictionary<string, State> states = new Dictionary<string, State>();
    public List<NewMovingSphere> allPlayers = new List<NewMovingSphere>();

    private bool anyPlayerFinished = false;
    public GameObject fireworks;
    
    [HideInInspector]
    public bool oneRoundFinished = false;
    private NewMovingSphere winner;

    [SerializeField]
    private RisingLiquid water;

    public void InitWater()
    {
        water.InitForLevel(terrainGenerator.currentLevel.GetTop());
    }

    public void StartWater()
    {
        water.StartRising();
    }

    public bool IsCancelled()
    {
        return cancelToken.IsCancellationRequested;
    }

    public void SwitchState(string stateName)
    {
        if (states.ContainsKey(stateName))
        {
            currentState?.End();
            currentState = states[stateName];
            currentState.Start();
        }
    }

    public void OnPlayerJoined()
    {
        var allPlayersInScene = FindObjectsOfType<NewMovingSphere>();
        for (int i = 0; i < allPlayersInScene.Length; i++)
        {
            var currentPlayer = allPlayersInScene[i];
            if (!allPlayers.Contains(currentPlayer))
            {
                SetupPlayerTrailColor(currentPlayer, allPlayers.Count);
                allPlayers.Add(currentPlayer);
                TPPlayerFarAway(currentPlayer);
            }
        }
    }

    public void TPPlayerFarAway(NewMovingSphere player)
    {
        var farAwayPos = new Vector3(10000, 0, 10000);
        player.startPosition = farAwayPos;
        player.transform.position = farAwayPos;
    }

    public void TPPlayersFarAway()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            TPPlayerFarAway(allPlayers[i]);
        }
    }

    public void DisableFireworks()
    {
        fireworks.SetActive(false);
    }

    public void ResetWinTrigger()
    {
        anyPlayerFinished = false;
        winner?.gameObject.GetComponent<PlayerUI>().HideWinText();
        var wintrigger = FindObjectOfType<WinTrigger>();
        if(wintrigger != default) wintrigger.transform.position = new Vector3(10000, 0, 10000);
    }

    public void OnPlayerFinish(NewMovingSphere player)
    {
        if (!anyPlayerFinished)
        {
            winner = player;
            anyPlayerFinished = true;
            winner.gameObject.GetComponent<PlayerUI>().ShowWinText();
            fireworks.SetActive(true);
            SwitchState("CountdownNext");
        }   
    }

    protected override void Awake()
    {
        base.Awake();   
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.enabled = false;
        AddStates();
    }

    private void AddStates()
    {
        states.Add("GenerateTerrain", new StateGenerateLevel(this));
        states.Add("Turntable", new StateTerrainTurnTable(this));
        states.Add("Countdown", new StateCountDown(this, countdownObj, 6));
        states.Add("StartGame", new StateStartGame(this));
        states.Add("CountdownNext", new StateCountDownForNext(this, 10));
    }

    private void Start()
    {
        cancel = new CancellationTokenSource();
        cancelToken = cancel.Token;
        fireworks.SetActive(false);
        SwitchState("GenerateTerrain");
    }

    private void Update()
    {
        ExtraFunctions.CheckQuitGame();
        if (currentState != null) currentState.Update();
    }

    public void InitPlayers()
    {
        Vector3 rayCasterTarget = terrainGenerator.currentLevel.GetCenterPosition();
        rayCasterTarget.y = 10;
        if (nPlayers < 2) nPlayers = 2;
        float anglePerPlayer = 2 * Mathf.PI / nPlayers;
        float distanceFromCenter = terrainGenerator.currentLevel.GetMaxXZLength() / 2;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            var player = allPlayers[i];

            float xPos = Mathf.Cos(i * anglePerPlayer);
            float zPos = Mathf.Sin(i * anglePerPlayer);

            Vector3 spawnPos = rayCasterTarget + new Vector3(xPos, 0, zPos) * distanceFromCenter;
            player.startPosition = spawnPos;
            player.transform.position = spawnPos;
            player.EnableSelf();
        }
    }

    public void SetupPlayerTrailColor(NewMovingSphere player, int playerIndex)
    {
        TrailRenderer trail = player.GetComponentInChildren<TrailRenderer>();
        if (!GameData.playerColors.ContainsKey(playerIndex)) playerIndex = 0;

        Color col = GameData.playerColors[playerIndex];
        trail.materials[0].color = col;
        player.climbingMaterial.color = col;
    }

    public void DisablePlayerJoining()
    {
        playerInputManager.DisableJoining();
    }

    private void OnDestroy()
    {
        cancel.Cancel();
    }

    public void StartCountDown()
    {
        if (currentState == states["Turntable"])
        {
            SwitchState("Countdown");
        }
    }
}
