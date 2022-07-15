using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerReset : StatePlayer
{

    public StatePlayerReset(PlayerStateMachine _stm, NewMovingSphere _player) : base(_stm, _player)
    {

    }

    public override void Start()
    {
        ResetPosition();
        player.EnableCamera();
        stm.SwitchState("Normal");
    }

    public void ResetPosition()
    {
        player.transform.position = player.startPosition;
        player.body.velocity = Vector3.zero;
    }
}
