using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StateCountDownForNext : StateCount
{
    public GameManager manager;

    public StateCountDownForNext(GameManager _manager, int nseconds) : base(_manager, nseconds)
    {
        manager = _manager;
    }

    public override void OnCountDownFinished()
    {
        Debug.Log("Oncountdownfinished");
        UIManager.Instance.HidePlayerScreens();
        manager.oneRoundFinished = true;
        manager.SwitchState("GenerateTerrain");
    }
}
