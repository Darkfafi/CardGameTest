using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseNodeDataRunner : MonoBehaviour
{
    [SerializeField]
    private TextAsset gneFile;

    private NodeDataExecuter executer;

    protected void Awake()
    {
        if(!GenericNodesSaveData.IsValidFile(gneFile))
        {
            throw new System.InvalidOperationException("The File Must be a Generic Node Editor File!");
        }
        XmlObjectReferences refs = new XmlObjectReferences();

        GenericNodesSaveData data = new GenericNodesSaveData();
        refs.Loading_LoadContainerWithXml(data, gneFile.text);
        executer = new NodeDataExecuter(data);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class NodeInputDataAttribute : Attribute
{
    public Type InputDataType { get; private set; }

    public NodeInputDataAttribute(Type inputDataType)
    {
        InputDataType = inputDataType;
    }
}
