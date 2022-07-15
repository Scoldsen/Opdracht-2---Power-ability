using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStartGame : State
{
    public GameManager manager;

    public StateStartGame(GameManager _manager)
    {
        manager = _manager;
    }

    public override void Start()
    {
        UIManager.Instance.SetupPlayerScreens();
        manager.DisablePlayerJoining();
        manager.InitPlayers();
        manager.terrainGenerator.GenerateTopFallingBlocks();
        //manager.InitWater();
        manager.StartWater();
    }
}
