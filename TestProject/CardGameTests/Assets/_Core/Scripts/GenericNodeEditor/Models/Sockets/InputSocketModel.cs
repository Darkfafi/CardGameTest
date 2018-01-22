using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("NodeInputSocketModel")]
public class InputSocketModel : BaseNodeSocketModel
{
    public event Action<object> DataReceiveCallbackEvent;

    public InputSocketModel() { }

    public InputSocketModel(Type t, BaseNodeModel parentNode, SocketDataType socketDataType, int maxConnectionsAmount) : base(t, parentNode, SocketModelType.Input, socketDataType, maxConnectionsAmount)
    {
    }

    public void ReceiveData(object data)
    {
        if (DataReceiveCallbackEvent != null)
            DataReceiveCallbackEvent(data);
    }

    public object[] GetConnectedData(ConnectionsController connections)
    {
        BaseNodeSocketModel[] outputs = connections.GetOutputsConnectedToInput(this);
        object[] returnValues = new object[outputs.Length];
        if(outputs.Length == 0)
        {
            return new object[] { GetDefault(SocketStreamingType) };
        }
        else
        {
            for(int i = 0; i < returnValues.Length; i++)
            {
                returnValues[i] = ((OutputSocketModel)outputs[i]).GetOutputData();
            }
        }
        return returnValues;
    }

    public object GetAnyConnedctedData(ConnectionsController connections)
    {
        object[] data = GetConnectedData(connections);
        return data[UnityEngine.Random.Range(0, data.Length)];
    }

    public override void Clean()
    {
        base.Clean();
        DataReceiveCallbackEvent = null;
    }

    public override void ProcessData(ConnectionsController connections)
    {
        object[] data = GetConnectedData(connections);
        for(int i = 0; i < data.Length; i++)
        {
            ReceiveData(data[i]);
        }
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
