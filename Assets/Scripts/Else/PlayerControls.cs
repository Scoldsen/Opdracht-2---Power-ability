using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyAxis
{
    public KeyCode keyNeg;
    public KeyCode keyPos;
    public float currentValue = 0;
    public float accel = 3;
    public float deAccel = 3;

    public KeyAxis(KeyCode _keyNeg, KeyCode _keyPos)
    {
        keyNeg = _keyNeg;
        keyPos = _keyPos;
    }

    public void Update()
    {
        if (!Input.GetKey(keyNeg) && !Input.GetKey(keyPos))
        {
            float sign = Mathf.Sign(currentValue);
            currentValue = sign * Mathf.Max((Mathf.Abs(currentValue) - deAccel * Time.deltaTime), 0);
        }
        else
        {
            if (Input.GetKey(keyNeg)) currentValue -= accel * Time.deltaTime;
            if (Input.GetKey(keyPos)) currentValue += accel * Time.deltaTime;
        }
        currentValue = Mathf.Clamp(currentValue, -1, 1);
    }
}

public class SingleButton
{
    KeyCode button;
    public bool keyPressedStart, keyPressed, keyPressedStop;

    public SingleButton(KeyCode _button)
    {
        button = _button;
    }

    public void Update()
    {
        keyPressedStart = Input.GetKeyDown(button);
        keyPressed = Input.GetKey(button);
        keyPressedStop = Input.GetKeyUp(button);
    }
}

public class PlayerControls
{
    public KeyAxis axisHor;
    public KeyAxis axisVer;

    public SingleButton jumpButton;
    public SingleButton climbButton;

    public PlayerControls(KeyCode keyHorNeg, KeyCode keyHorPos, KeyCode keyVerNeg, KeyCode keyVerPos, KeyCode keyJump, KeyCode keyClimb)
    {
        axisHor = new KeyAxis(keyHorNeg, keyHorPos);
        axisVer = new KeyAxis(keyVerNeg, keyVerPos);
        jumpButton = new SingleButton(keyJump);
        climbButton = new SingleButton(keyClimb);
    }

    public void UpdateVals()
    {
        axisHor.Update();
        axisVer.Update();
        jumpButton.Update();
        climbButton.Update();
    }
}
