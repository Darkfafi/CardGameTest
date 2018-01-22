using System;
using System.Collections.Generic;

[Serializable]
public abstract class BaseNodeModel : IModel
{
    public event Action<object> ModelInputReceivedDataEvent;

    public int NodeModelID { get; private set; }

    public InputSocketModel[] InputSockets { get { return inputSockets.ToArray(); } }
    public OutputSocketModel[] OutputSockets { get { return outputSockets.ToArray(); } }

    protected ConnectionsController connectionsController { get; private set; }

    private List<InputSocketModel> inputSockets = new List<InputSocketModel>();
    private List<OutputSocketModel> outputSockets = new List<OutputSocketModel>();

    public BaseNodeModel(ConnectionsController connectionsController)
    {
        this.connectionsController = connectionsController;
        BeforeConstruction();
        ConstructSockets();
    }

    public void GetAllInput()
    {
        for (int i = 0; i < inputSockets.Count; i++)
        {
            inputSockets[i].ProcessData(connectionsController);
        }
    }

    public void SendAllOutput()
    {
        for (int i = 0; i < outputSockets.Count; i++)
        {
            outputSockets[i].ProcessData(connectionsController);
        }
    }

    public void Clean()
    {
        connectionsController = null;
        ModelInputReceivedDataEvent = null;

        for(int i = 0; i < inputSockets.Count; i++)
        {
            inputSockets[i].Clean();
        }

        for (int i = 0; i < outputSockets.Count; i++)
        {
            outputSockets[i].Clean();
        }

        outputSockets = null;
        inputSockets = null;
    }

    protected InputSocketModel AddInputSocket(int maxConnectionsAmount, Type inputType)
    {
        InputSocketModel s = (InputSocketModel)InternalAddInputSocket(typeof(InputSocketModel), SocketDataType.DataConnection, maxConnectionsAmount, inputType);
        s.DataReceiveCallbackEvent += (data) => { OnDataReceiveCallbackEvent(data); };
        return s;
    }

    protected OutputSocketModel AddOutputSocket(Func<object> outputRequestMethod, int maxConnectionsAmount, Type outputType)
    {
        OutputSocketModel m = (OutputSocketModel)InternalAddOutputSocket(typeof(OutputSocketModel), SocketDataType.DataConnection, maxConnectionsAmount, outputType);
        m.SetRequestOutputMethod(outputRequestMethod);
        return m;
    }

    private BaseNodeSocketModel InternalAddInputSocket(Type t, SocketDataType socketDataType, int maxConnectionsAmount, Type t2)
    {
        InputSocketModel s = (InputSocketModel)Activator.CreateInstance(t, new object[] { t2, this, socketDataType, maxConnectionsAmount });
        inputSockets.Add(s);
        return s;
    }

    private BaseNodeSocketModel InternalAddOutputSocket(Type t, SocketDataType socketDataType, int maxConnectionsAmount, Type t2)
    {
        OutputSocketModel s = (OutputSocketModel)Activator.CreateInstance(t, new object[] { t2, this, socketDataType, maxConnectionsAmount });
        outputSockets.Add(s);
        return s;
    }

    private void OnDataReceiveCallbackEvent(object data)
    {
        DataReceivedFromInputSocket(data);

        if (ModelInputReceivedDataEvent != null)
            ModelInputReceivedDataEvent(data);
    }

    protected abstract void DataReceivedFromInputSocket(object data);
    protected virtual void BeforeConstruction() { }
    protected abstract void ConstructSockets();
}
