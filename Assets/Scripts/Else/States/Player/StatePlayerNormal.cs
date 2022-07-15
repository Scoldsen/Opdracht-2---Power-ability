using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerNormal : StatePlayer
{

    public StatePlayerNormal(PlayerStateMachine _stm, NewMovingSphere _player) : base(_stm, _player)
    {

    }

    public override void Update()
    {
        if (player.terrainGenerator.PostionIsOutOfBounds(player.transform.position))
        {
            stm.SwitchState("OutOfBounds");
        }
    }
}
