using System;
using System.Xml;

public class InputNodeModel<Output> : BaseFlowNodeModel
{
    public override FlowNodeType NodeFlowType
    {
        get
        {
            return FlowNodeType.OutFlow;
        }
    }

    private OutputSocketModel<Output> output;
    private Output data;

    public InputNodeModel() : base() { }

    public InputNodeModel(ConnectionsController connectionsController) : base(connectionsController)
    {

    }

    protected override void ConstructSockets()
    {
        if(typeof(Output) != typeof(EmptyData))
            output = AddOutputSocket(OnOutputRequestMethod, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT);
    }

    public void Run(Output data)
    {
        if (typeof(Output) != typeof(EmptyData))
        {
            this.data = data;

            if (DebugMode)
                UnityEngine.Debug.Log("Data Set: " + data.ToString() + " | Type: " + data.GetType().Name);

            SendAllOutput();
        }
        else if (DebugMode)
        {
            UnityEngine.Debug.Log("Data Set: { Empty }");
        }

        SafeNextNodeFlowRequest();
    }

    private Output OnOutputRequestMethod()
    {
        return data;
    }

    protected override void DataReceivedFromInputSocket(object data)
    {
        if (typeof(Output) != typeof(EmptyData))
            output.SendOutputData(connectionsController);
    }

    protected override void OnAllDataLoaded()
    {
        if (typeof(Output) != typeof(EmptyData))
        {
            output = GetLoadedOutputSocket(0, OnOutputRequestMethod);
            output.SendOutputData(connectionsController);
        }
    }

    protected override void SpecificSave(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {

    }

    protected override void SpecificLoad(XmlElement savedData, XmlObjectReferences references)
    {

    }

    protected override void OnSelectedAsCurrentFlowNode(BaseFlowNodeModel previousFlowNode)
    {

    }

    protected override void OnUnselectedAsCurrentFlowNode()
    {

    }
}
