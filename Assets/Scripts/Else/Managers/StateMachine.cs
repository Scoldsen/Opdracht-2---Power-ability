using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private Dictionary<string, State> states = new Dictionary<string, State>();
    private State currentState;

    public virtual void Init()
    {

    }

    public void AddState(string stateName, State state)
    {
        states.Add(stateName, state);
    }

    public bool IsCurrentState(string stateName)
    {
        if (currentState != null)
        {
            State state = default;
            states.TryGetValue(stateName, out state);
            return currentState == state;
        }
        return false;
    }

    public void SwitchState(string stateName)
    {
        if (states.ContainsKey(stateName))
        {
            Debug.Log($"switching state to {stateName}");
            currentState?.End();
            currentState = states[stateName];
            currentState.Start();
        }
    }

    public void Tick()
    {
        currentState?.Update();
    }
}
