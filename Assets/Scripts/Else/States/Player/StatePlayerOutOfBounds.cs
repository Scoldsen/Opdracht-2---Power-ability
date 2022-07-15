using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerOutOfBounds : StatePlayer
{
    private float duration = 3;
    private float startTime = 0;

    public StatePlayerOutOfBounds(PlayerStateMachine _stm, NewMovingSphere _player) : base(_stm, _player)
    {

    }

    public override void Start()
    {
        startTime = Time.time;
        player.DisableCamera();
    }

    public override void Update()
    {
        if (Time.time > startTime + duration)
        {
            stm.SwitchState("Reset");
        }
    }
}
