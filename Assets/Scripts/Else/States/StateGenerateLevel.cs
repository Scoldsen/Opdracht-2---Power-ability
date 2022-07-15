using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class StateGenerateLevel : State
{
    public GameManager manager;

    public StateGenerateLevel(GameManager _manager)
    {
        manager = _manager;
    }

    public override async void Start()
    {
        if (GameManager.Instance.IsCancelled()) return;
        var generator = manager.terrainGenerator;

        UIManager.Instance.ShowLoadScreen();
        Debug.Log("Terrain generation started");
        manager.DisableFireworks();
        manager.ResetWinTrigger();
        await generator.GenerateLevelAsync();
        manager.InitWater();
        await Task.Yield();
        manager.SwitchState("Turntable");
    }

    public override async void End()
    {
        if (GameManager.Instance.IsCancelled()) return;
        Debug.Log("Terrain generation finished");
        UIManager.Instance.HideLoadScreen();
        await Task.Yield();
    }
}
