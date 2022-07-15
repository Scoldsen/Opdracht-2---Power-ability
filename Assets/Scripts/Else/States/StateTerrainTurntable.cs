using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class StateTerrainTurnTable : State
{
    public GameManager manager;

    public StateTerrainTurnTable(GameManager _manager)
    {
        manager = _manager;
    }

    public override void Start()
    {
        manager.TPPlayersFarAway();
        UIManager.Instance.SetupRotatingCamera();
        manager.playerInputManager.enabled = true;
        AudioManager.Instance.PlayMusic("tim4");
        if (manager.oneRoundFinished)
        {
            manager.SwitchState("Countdown");
        }
    }

    public override void Update()
    {
        UIManager.Instance.RotateCamera();
    }
}
