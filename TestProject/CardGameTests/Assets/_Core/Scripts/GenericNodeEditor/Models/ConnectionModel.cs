using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConnectionModel : IModel
{
    public event Action<ConnectionModel> ConnectionDestroyedEvent;
    public OutputSocketModel OutputSocket { get { return outputSocket; } set { outputSocket = value; } }
    public InputSocketModel InputSocket { get { return inputSocket; } set { inputSocket = value; } }

    [SerializeField]
    private InputSocketModel inputSocket;

    [SerializeField]
    private OutputSocketModel outputSocket;

    public ConnectionModel() { } // For Serialization

    public ConnectionModel(InputSocketModel input, OutputSocketModel output)
    {
        InputSocket = input;
        OutputSocket = output;
    }

    public void DestroyConnection()
    {
        InputSocket.ParentNode.GetAllInput();

        if (ConnectionDestroyedEvent != null)
            ConnectionDestroyedEvent(this);
    }

    public void Clean()
    {
        ConnectionDestroyedEvent = null;
        OutputSocket = null;
        InputSocket = null;
    }
}
