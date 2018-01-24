using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.ComponentModel;

public abstract class BaseNodeModel : IModel, ISaveable
{
    public event Action<object> ModelInputReceivedDataEvent;
    public event Action<SocketModelType> NodeConnectedAsEvent;

    public BaseNodeSocketModel[] InputSockets { get { return inputSockets.ToArray(); } }
    public BaseNodeSocketModel[] OutputSockets { get { return outputSockets.ToArray(); } }

    protected ConnectionsController connectionsController { get; private set; }

    private List<BaseNodeSocketModel> inputSockets = new List<BaseNodeSocketModel>();
    private List<BaseNodeSocketModel> outputSockets = new List<BaseNodeSocketModel>();

    private List<object> inputsLoaded = new List<object>();
    private List<object> outputsLoaded = new List<object>();

    public BaseNodeModel() { }

    public BaseNodeModel(ConnectionsController connectionsController)
    {
        this.connectionsController = connectionsController;
        BeforeConstruction();
        ConstructSockets();
    }

    public void NodeConnectedAs(SocketModelType nodeRoleInConnection)
    {
        if (NodeConnectedAsEvent != null)
            NodeConnectedAsEvent(nodeRoleInConnection);
    }

    public void GetAllInput()
    {
        if (connectionsController == null) { return; }
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
        inputsLoaded = null;
        outputsLoaded = null;
    }

    protected InputSocketModel<T> AddInputSocket<T>(int maxConnectionsAmount)
    {
        InputSocketModel<T> s = (InputSocketModel<T>)InternalAddInputSocket(typeof(InputSocketModel<T>), SocketDataType.DataConnection, maxConnectionsAmount);
        s.DataReceiveCallbackEvent += (data) => { OnDataReceiveCallbackEvent(data); };
        return s;
    }

    protected OutputSocketModel<T> AddOutputSocket<T>(Func<T> outputRequestMethod, int maxConnectionsAmount)
    {
        OutputSocketModel<T> m = (OutputSocketModel<T>)InternalAddOutputSocket(typeof(OutputSocketModel<T>), SocketDataType.DataConnection, maxConnectionsAmount);
        m.SetRequestOutputMethod(outputRequestMethod);
        return m;
    }

    protected virtual InputSocketModel<T> GetLoadedInputSocket<T>(int index)
    {
        InputSocketModel<T> s = (InputSocketModel<T>)(inputsLoaded[index]);
        s.DataReceiveCallbackEvent += (data) => { OnDataReceiveCallbackEvent(data); };
        return s;
    }

    protected virtual OutputSocketModel<T> GetLoadedOutputSocket<T>(int index, Func<T> outputRequestMethod)
    {
        OutputSocketModel<T> os = ((OutputSocketModel<T>)(outputsLoaded[index]));
        os.SetRequestOutputMethod(outputRequestMethod);
        return os;
    }

    private BaseNodeSocketModel InternalAddInputSocket(Type t, SocketDataType socketDataType, int maxConnectionsAmount)
    {
        BaseNodeSocketModel s = (BaseNodeSocketModel)Activator.CreateInstance(t, new object[] { this, socketDataType, maxConnectionsAmount, inputSockets.Count});
        inputSockets.Add(s);
        return s;
    }

    private BaseNodeSocketModel InternalAddOutputSocket(Type t, SocketDataType socketDataType, int maxConnectionsAmount)
    {
        BaseNodeSocketModel s = (BaseNodeSocketModel)Activator.CreateInstance(t, new object[] { this, socketDataType, maxConnectionsAmount, outputSockets.Count });
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


    // Saving and Loading 

    public void Save(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {
        XmlElement sockets = doc.CreateElement("Sockets");
        BaseNodeSocketModel socketModel;
        int l = inputSockets.Count + outputSockets.Count;
        for (int i = 0; i < l; i++)
        {
            socketModel = (i < inputSockets.Count) ? inputSockets[i] : outputSockets[i - inputSockets.Count];
            XmlElement socketElement = doc.CreateElement("Socket");
            socketElement.AppendChild(doc.CreateTextNode(references.Saving_GetRefCounterFor(socketModel).ToString()));
            sockets.AppendChild(socketElement);
        }

        saveableElement.AppendChild(doc.CreateElementWithData("ConnectionsControllerReference", references.Saving_GetRefCounterFor(connectionsController).ToString()));

        saveableElement.AppendChild(sockets);
        SpecificSave(doc, references, saveableElement);
    }

    public void Load(XmlElement savedData, XmlObjectReferences references)
    {
        SpecificLoad(savedData, references);
        XmlNodeList socketNodesList = savedData.GetElementsByTagName("Socket");
        references.Loading_GetReferenceFrom(uint.Parse(savedData.GetSingleDataFrom("ConnectionsControllerReference")), OnConnectionsControllerLoaded);

        for(int i = 0; i < socketNodesList.Count; i++)
        {
            XmlNode node = socketNodesList.Item(i);
            uint referenceId = uint.Parse(node.InnerText);
            references.Loading_GetReferenceFrom(referenceId, OnLoadingNodeReceived);
        }
    }

    private void OnConnectionsControllerLoaded(uint referenceId, object connectionsControllerInstance)
    {
        connectionsController = (ConnectionsController)connectionsControllerInstance;
    }

    private void OnLoadingNodeReceived(uint referenceId, object nodeInstance)
    {
        BaseNodeSocketModel socket = (BaseNodeSocketModel)nodeInstance;
        if(socket.SocketModelType == SocketModelType.Input)
        {
            inputSockets.Add(socket);
            inputsLoaded.Add(nodeInstance);
        }
        else
        {
            outputSockets.Add(socket);
            outputsLoaded.Add(nodeInstance);
        }
    }

    protected abstract void SpecificSave(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement);
    protected abstract void SpecificLoad(XmlElement savedData, XmlObjectReferences references);
    protected abstract void OnAllDataLoaded();

    public void AllDataLoaded()
    {
        OnAllDataLoaded();
        GetAllInput();
        SendAllOutput();
    }
}
