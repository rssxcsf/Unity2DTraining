using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StateType
{
    Idle, Move, Hit, Shock, Attack, Pick, Death,
    Chat,
    Sliding,
    Skill
}
public interface IState
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
}
public class Blackboard
{
    //存储共享数据
}
public class FSM
{
    public IState curState;
    public Dictionary<StateType, IState> states;
    public Blackboard blackboard;
    public FSM(Blackboard blackboard)
    {
        this.states = new Dictionary<StateType, IState>();
        this.blackboard = blackboard;
    }
    public void AddState(StateType stateType, IState state)
    {
        if (states.ContainsKey(stateType))
        {
            Debug.Log("[AddState] >>>>>>>已有该状态" + stateType);
            return;
        }
        states.Add(stateType, state);
    }
    public void SwitchState(StateType stateType)
    {
        if(!states.ContainsKey(stateType))
        {
            Debug.Log("[AddState] >>>>>>>没有该状态" + stateType);
            return;
        }
        if (curState == states[stateType])
            return;
        if(curState !=null)
        {
            curState.OnExit();
        }
        curState = states[stateType];
        curState.OnEnter();
    }
    public bool IsCurrentState(StateType type)
    {
        if (states.ContainsKey(type))

        {
            return curState == states[type];
        }
        return false;
    }
    public void OnUpdate()
    {
        curState.OnUpdate();
    }
}