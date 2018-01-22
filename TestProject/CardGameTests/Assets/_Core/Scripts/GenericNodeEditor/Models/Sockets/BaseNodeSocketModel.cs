using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

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

public abstract class BaseNodeSocketModel : IModel, ISerializable
{
    [XmlIgnore]
    public BaseNodeModel ParentNode { get; private set; }

    public int ParentNodeID { get; private set; }

    public string SocketStreamingTypeName { get; private set; }

    public Type SocketStreamingType { get
        {
            if(socketStreamingType == null)
            {
                socketStreamingType = Type.GetType(SocketStreamingTypeName);
            }

            return socketStreamingType;
        }
    }

    public SocketModelType SocketModelType { get; private set; }
    public SocketDataType SocketDataType { get; private set; }
    public int MaxConnectionsAmount { get; private set; }

    private Type socketStreamingType = null;

    public BaseNodeSocketModel() { }

    public BaseNodeSocketModel(Type t, BaseNodeModel parentNode, SocketModelType socketModelType, SocketDataType socketDataType, int maxConnectionsAmount)
    {
        ParentNode = parentNode;
        ParentNodeID = ParentNode.NodeModelID;
        SocketModelType = socketModelType;
        SocketDataType = socketDataType;
        SocketStreamingTypeName = t.FullName;
        MaxConnectionsAmount = maxConnectionsAmount;
    }

    public virtual void Clean()
    {
        ParentNode = null;
    }

    public abstract void ProcessData(ConnectionsController connections);

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("ParentNode", ParentNode);
    }
}