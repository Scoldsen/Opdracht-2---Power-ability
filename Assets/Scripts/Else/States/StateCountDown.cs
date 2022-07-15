using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class StateCountDown : State
{
    private GameManager manager;
    private TextMeshProUGUI countdownObj;
    private int countdownSeconds = 10;

    public StateCountDown(GameManager _manager, TextMeshProUGUI _countdownObj, int _countdownSeconds)
    {
        manager = _manager;
        countdownObj = _countdownObj;
        countdownSeconds = _countdownSeconds;
    }

    public override async void Start()
    {
        float startTime = Time.realtimeSinceStartup;
        float timeRemaining = countdownSeconds;
        int timeRemainingSecs = countdownSeconds + 1;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            int secs = Mathf.FloorToInt(timeRemaining);
            if (secs != timeRemainingSecs)
            {
                timeRemainingSecs = secs;
                countdownObj.text = $"{timeRemainingSecs}";
            }
            float opacity = timeRemaining % 1;
            countdownObj.color = new Color(255, 255, 255, opacity);
            await Task.Yield();
        }
        if (GameManager.Instance.IsCancelled()) return;
        manager.SwitchState("StartGame");
    }

    public override void Update()
    {
        UIManager.Instance.RotateCamera();
    }
}
