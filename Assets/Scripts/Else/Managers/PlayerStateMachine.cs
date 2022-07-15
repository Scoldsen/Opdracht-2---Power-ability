using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    private NewMovingSphere player;
    public PlayerStateMachine(NewMovingSphere _player)
    {
        player = _player;
    }

    public override void Init()
    {
        base.Init();
        AddState("Disabled", new StatePlayerDisabled(this, player));
        AddState("Normal", new StatePlayerNormal(this, player));
        AddState("OutOfBounds", new StatePlayerOutOfBounds(this, player));
        AddState("Reset", new StatePlayerReset(this, player));
        AddState("Dizzy", new StatePlayerDizzy(this, player));
    }
}
