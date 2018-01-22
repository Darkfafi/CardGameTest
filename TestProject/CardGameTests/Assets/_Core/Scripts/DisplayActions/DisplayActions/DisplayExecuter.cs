using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplayExecuter : MonoBehaviour
{
    public const int DEFAULT_CHANNEL = -1337;

    public static DisplayExecuter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("<" + typeof(DisplayExecuter).Name + ">").AddComponent<DisplayExecuter>();
                DontDestroyOnLoad(_instance);
                _instance.gameObject.isStatic = true;
            }

            return _instance;
        }
    }

    private static DisplayExecuter _instance = null;

    private Dictionary<int, Queue<DisplayAction>> actionsMap = new Dictionary<int, Queue<DisplayAction>>();
    private List<int> removeList = new List<int>();

    public void SubmitActionDisplay(DisplayAction newAction)
    {
        if(!actionsMap.ContainsKey(newAction.Channel))
        {
            actionsMap.Add(newAction.Channel, new Queue<DisplayAction>());
        }

        actionsMap[newAction.Channel].Enqueue(newAction);

        if(actionsMap[newAction.Channel].Count == 1)
        {
            StartNextAction(newAction.Channel);
        }
    }

    protected void Update()
    {
        if (actionsMap.Count == 0) { return; }
        foreach (KeyValuePair<int, Queue<DisplayAction>> pair in actionsMap)
        {
            if (pair.Value.Count == 0) { continue; }
            if (pair.Value.Peek().HasCompleted)
            {
                pair.Value.Dequeue();
                if (pair.Value.Count > 0)
                {
                    StartNextAction(pair.Key);
                }
                else
                {
                    removeList.Add(pair.Key);
                }
            }
        }

        if(removeList.Count > 0)
        {
            for(int i = 0; i < removeList.Count; i++)
            {
                actionsMap.Remove(removeList[i]);
            }

            removeList.Clear();
        }
    }

    private void StartNextAction(int channel)
    {
        if (actionsMap[channel] == null || actionsMap[channel].Count == 0) { return; }
        actionsMap[channel].Peek().InvokeDisplayAction();
    }
}
