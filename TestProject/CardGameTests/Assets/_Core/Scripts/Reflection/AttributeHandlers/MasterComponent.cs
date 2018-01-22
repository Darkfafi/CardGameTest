using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public abstract class MasterComponent : MonoBehaviour
{
    private List<object> instanceDataHoldersMap = new List<object>();

    protected void Inject<T>(T data)
    {
        Dictionary<object , FieldInfo[]> infosMap = new Dictionary<object, FieldInfo[]>();
        for(int i = 0; i < instanceDataHoldersMap.Count; i++)
        {
            FieldInfo[] infos = instanceDataHoldersMap[i].GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(t => t.FieldType.IsAssignableFrom(data.GetType())).ToArray();

            infosMap.Add(instanceDataHoldersMap[i], infos);
        }

        foreach(KeyValuePair<object, FieldInfo[]> pair in infosMap)
        {
            FieldInfo[] infos = pair.Value;
            for (int i = 0; i < infos.Length; i++)
            {
                if(infos[i].IsDefined(typeof(InjectAttribute), true))
                    infos[i].SetValue(pair.Key, data);
            }
        }
    }

    protected void Link<T>(T dataHolder) where T : class
    {
        if (instanceDataHoldersMap.Contains(dataHolder)) { return; }
        instanceDataHoldersMap.Add(dataHolder);
    }
}

public class InjectAttribute : Attribute { }