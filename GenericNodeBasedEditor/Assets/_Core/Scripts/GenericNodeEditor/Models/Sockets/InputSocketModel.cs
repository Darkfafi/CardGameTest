using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSocketModel<T> : BaseNodeSocketModel<T>
{
    public event Action<T> DataReceiveCallbackEvent;

    public InputSocketModel() : base() { }

    public InputSocketModel(BaseNodeModel parentNode, SocketDataType dataType, int maxConnectionsAmount, int index) : base(parentNode, SocketModelType.Input, dataType, maxConnectionsAmount, index)
    {

    }
    
    public void ReceiveData(T data)
    {
        if (DataReceiveCallbackEvent != null)
            DataReceiveCallbackEvent(data);
    }

    public T[] GetConnectedData(ConnectionsController connections)
    {
        BaseNodeSocketModel[] outputs = connections.GetOutputsConnectedToInput(this);
        T[] returnValues = new T[outputs.Length];
        if(outputs.Length == 0)
        {
            return new T[] { default(T) };
        }
        else
        {
            for(int i = 0; i < returnValues.Length; i++)
            {
                returnValues[i] = ((OutputSocketModel<T>)outputs[i]).GetOutputData();
            }
        }
        return returnValues;
    }

    public T GetAnyConnedctedData(ConnectionsController connections)
    {
        T[] data = GetConnectedData(connections);
        return data[UnityEngine.Random.Range(0, data.Length)];
    }

    public override void Clean()
    {
        base.Clean();
        DataReceiveCallbackEvent = null;
    }

    public override void ProcessData(ConnectionsController connections)
    {
        T[] data = GetConnectedData(connections);
        for(int i = 0; i < data.Length; i++)
        {
            ReceiveData(data[i]);
        }
    }
}
