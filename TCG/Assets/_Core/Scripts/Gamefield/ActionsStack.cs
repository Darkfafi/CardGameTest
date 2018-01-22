using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// One for the visuals (one by one)
// One for logics (Scan and execute all in 1 frame)
public class ActionsStack : MonoBehaviour
{
    public enum ExecuteLocation
    {
        Bottom,
        Top
    }

    public int StackLength { get { return actions.Count; } }

    private List<StackAction> actions = new List<StackAction>();
	
    public void RegisterAction(StackAction action, bool reregisterIfAlreadyIn)
    {
        if(reregisterIfAlreadyIn)
        {
            UnregisterAction(action);
        }

        actions.Add(action);
    }

    public void UnregisterAction(StackAction action)
    {
        if (actions.Contains(action))
        {
            actions.Remove(action);
        }
    }

    public Coroutine Execute(ExecuteLocation location)
    {
        int index = location == ExecuteLocation.Bottom ? 0 : actions.Count - 1;
        return Execute(index);
    }

    private Coroutine Execute(int index)
    {
        StackAction a = actions[index];
        actions.Remove(a);

        return a.Execute(this);
    }
}

public abstract class StackAction
{
    protected MonoBehaviour host { get; private set; }
    private Coroutine coroutineStarted;

    public virtual Coroutine Execute(MonoBehaviour host)
    {
        this.host = host;
        return host.StartCoroutine(ExecutionAction());
    }

    protected abstract IEnumerator ExecutionAction();
}