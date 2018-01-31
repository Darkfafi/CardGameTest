using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseNodeDataRunner<Input> : MonoBehaviour, INodeDataRunner
{
    [SerializeField]
    private TextAsset gneFile;

    private NodeDataExecuter<Input> executer;

    public void Run(Input data)
    {
        executer.Run(data);
    }

    protected virtual void Awake()
    {
        if(!GenericNodesSaveData.IsValidFile(gneFile))
        {
            throw new InvalidOperationException("The File Must be a Generic Node Editor File!");
        }
        XmlObjectReferences refs = new XmlObjectReferences();

        GenericNodesSaveData data = new GenericNodesSaveData();
        refs.Loading_LoadContainerWithXml(data, gneFile.text);
        executer = new NodeDataExecuter<Input>(data);
    }

    protected void OnDestroy()
    {
        executer.Clean();
        executer = null;
    }
}

public interface INodeDataRunner
{

}

public struct EmptyData
{

}
