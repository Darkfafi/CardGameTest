using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("NodeOutputSocketModel")]
public class OutputSocketModel : BaseNodeSocketModel
{
    private Func<object> outputRequest;

    public OutputSocketModel() { }

    public OutputSocketModel(Type t, BaseNodeModel parentNode, SocketDataType socketDataType, int maxConnectionsAmount) : base(t, parentNode, SocketModelType.Output, socketDataType, maxConnectionsAmount)
    {
    }

    public void SetRequestOutputMethod(Func<object> outputRequest)
    {
        this.outputRequest = outputRequest;
    }

    public object GetOutputData()
    {
        if (outputRequest != null)
            return outputRequest();

        return GetDefault(SocketStreamingType);
    }
    
    public void SendOutputData(ConnectionsController connections)
    {
        BaseNodeSocketModel[] inputSockets = connections.GetInputsConnectedToOutput(this);
        for(int i = 0; i < inputSockets.Length; i++)
        {
            ((InputSocketModel)inputSockets[i]).ReceiveData(GetOutputData());
        }
    }

    public override void Clean()
    {
        base.Clean();
        outputRequest = null;
    }

    public override void ProcessData(ConnectionsController connections)
    {
        SendOutputData(connections);
    }

    public object GetDefault(Type t)
    {
        return this.GetType().GetMethod("GetDefaultGeneric").MakeGenericMethod(t).Invoke(this, null);
    }

    public T GetDefaultGeneric<T>()
    {
        return default(T);
    }
}
