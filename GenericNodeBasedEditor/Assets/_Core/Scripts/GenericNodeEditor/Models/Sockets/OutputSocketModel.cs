using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputSocketModel<T> : BaseNodeSocketModel<T>
{
    private Func<T> outputRequest;

    public OutputSocketModel() : base() { }

    public OutputSocketModel(BaseNodeModel parentNode, SocketDataType dataType, int maxConnectionsAmount, int index) : base(parentNode, SocketModelType.Output, dataType, maxConnectionsAmount, index)
    {

    }

    public void SetRequestOutputMethod(Func<T> outputRequest)
    {
        this.outputRequest = outputRequest;
    }

    public T GetOutputData()
    {
        if (outputRequest != null)
            return outputRequest();

        return default(T);
    }
    
    public void SendOutputData(ConnectionsController connections)
    {
        if (connections == null) { return; }
        BaseNodeSocketModel[] inputSockets = connections.GetInputsConnectedToOutput(this);
        for(int i = 0; i < inputSockets.Length; i++)
        {
            ((InputSocketModel<T>)inputSockets[i]).ReceiveData(GetOutputData());
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
}
