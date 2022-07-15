using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StateCount : State
{
    private GameManager manager;
    private int countdownDuration = 10;

    public StateCount(GameManager _manager, int _countdownDuration)
    {
        manager = _manager;
        countdownDuration = _countdownDuration;
    }

    public virtual void OnCountDownFinished()
    {

    }

    public override async void Start()
    {
        float startTime = Time.realtimeSinceStartup;
        float timeRemaining = countdownDuration;
        int timeRemainingSecs = countdownDuration + 1;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int secs = Mathf.FloorToInt(timeRemaining);
            if (secs != timeRemainingSecs)
            {
                timeRemainingSecs = secs;
            }
            float opacity = timeRemaining % 1;
            await Task.Yield();
        }
        OnCountDownFinished();
    }
}
