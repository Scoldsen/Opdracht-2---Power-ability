using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePlayerDizzy : StatePlayer
{
    private float dizzyDuration = 2;
    private float startTime = Time.time;

    private float currentTime;

    private float emitInterval = .2f;
    private float lastEmitTime;

    public StatePlayerDizzy(PlayerStateMachine _stm, NewMovingSphere _player) : base(_stm, _player)
    {

    }

    public override void Start()
    {
        player.collisionParticles.SetActive(true);
        player.collisionParticles.transform.position = player.hitPosition;

        startTime = Time.realtimeSinceStartup;
        currentTime = 0;
        emitInterval = .2f;
        lastEmitTime = 0;
    }
    public override void Update()
    {
        if (player.terrainGenerator.PostionIsOutOfBounds(player.transform.position))
        {
            stm.SwitchState("OutOfBounds");
        }

        if (currentTime < dizzyDuration)
        {
            currentTime = Time.realtimeSinceStartup - startTime;

            if (currentTime - lastEmitTime > emitInterval)
            {
                player.starParticles.Emit(1);
                lastEmitTime = currentTime;
            }
        }

        else
        {
            stm.SwitchState("Normal");
        }
    }

    public override void End()
    {
        player.collisionParticles.SetActive(false);
    }
}
