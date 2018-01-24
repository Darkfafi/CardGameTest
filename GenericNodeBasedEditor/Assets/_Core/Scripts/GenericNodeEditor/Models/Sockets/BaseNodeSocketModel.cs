using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;

public enum SocketModelType
{
    Input,
    Output
}

public enum SocketDataType
{
    DataConnection,
    FlowConnection
}

public abstract class BaseNodeSocketModel<T> : BaseNodeSocketModel
{
    public BaseNodeSocketModel() { }

    public BaseNodeSocketModel(BaseNodeModel parentNode, SocketModelType socketModelType, SocketDataType socketDataType, int maxConnectionsAmount, int index)
    {
        ParentNode = parentNode;
        SocketModelType = socketModelType;
        SocketDataType = socketDataType;
        SocketStreamingType = typeof(T);
        MaxConnectionsAmount = maxConnectionsAmount;
        Index = index;
    }

    public override void Clean()
    {
        ParentNode = null;
        SocketStreamingType = null;
    }

    public override void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        saveableElement.AppendElements(
            doc.CreateElementWithData("SocketIndex", Index.ToString()),
            doc.CreateElementWithData("ParentNodeReference", references.Saving_GetRefCounterFor(ParentNode).ToString()),
            doc.CreateElementWithData("SocketModelType", ((int)SocketModelType).ToString()),
            doc.CreateElementWithData("SocketDataType", ((int)SocketDataType).ToString()),
            doc.CreateElementWithData("SocketStreamingType", SocketStreamingType.FullName),
            doc.CreateElementWithData("MaxConnectionsAmount", MaxConnectionsAmount.ToString())
        );
    }

    public override void Load(XmlElement savedData, XmlObjectReferences references)
    {
        Index = int.Parse(savedData.GetSingleDataFrom("SocketIndex"));
        references.Loading_GetReferenceFrom(uint.Parse(savedData.GetSingleDataFrom("ParentNodeReference")), OnParentNodeLoaded);
        SocketModelType = (SocketModelType)int.Parse(savedData.GetSingleDataFrom("SocketModelType"));
        SocketDataType = (SocketDataType)int.Parse(savedData.GetSingleDataFrom("SocketDataType"));
        SocketStreamingType = Type.GetType(savedData.GetSingleDataFrom("SocketStreamingType"));
        MaxConnectionsAmount = int.Parse(savedData.GetSingleDataFrom("MaxConnectionsAmount"));
    }

    public override void AllDataLoaded()
    {

    }

    private void OnParentNodeLoaded(uint referenceId, object parentNodeInstance)
    {
        ParentNode = (BaseNodeModel)parentNodeInstance;
    }
}

public abstract class BaseNodeSocketModel : IModel, ISaveable
{
    public int Index { get; protected set; }
    public BaseNodeModel ParentNode { get; protected set; }
    public Type SocketStreamingType { get; protected set; }
    public SocketModelType SocketModelType { get; protected set; }
    public SocketDataType SocketDataType { get; protected set; }
    public int MaxConnectionsAmount { get; protected set; }
    public abstract void ProcessData(ConnectionsController connections);
    public abstract void Clean();

    public abstract void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement);
    public abstract void Load(XmlElement savedData, XmlObjectReferences references);

    public abstract void AllDataLoaded();
}