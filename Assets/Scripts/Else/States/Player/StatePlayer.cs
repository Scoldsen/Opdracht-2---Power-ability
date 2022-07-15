using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayer : State
{
    public PlayerStateMachine stm;
    public NewMovingSphere player;

    public StatePlayer(PlayerStateMachine _stm, NewMovingSphere _player)
    {
        stm = _stm;
        player = _player;
    }
}
