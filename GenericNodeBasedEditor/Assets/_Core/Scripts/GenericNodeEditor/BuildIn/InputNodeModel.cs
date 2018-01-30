using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

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
        if(typeof(Output) != typeof(ConnectionModel))
            output = AddOutputSocket(OnOutputRequestMethod, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT);
    }

    public void Run(Output data)
    {
        // When run, pass flow to the next flow node
        this.data = data;
        SendAllOutput();
    }

    private Output OnOutputRequestMethod()
    {
        return data;
    }

    protected override void DataReceivedFromInputSocket(object data)
    {
        if (typeof(Output) != typeof(ConnectionModel))
            output.SendOutputData(connectionsController);
    }

    protected override void OnAllDataLoaded()
    {
        if (typeof(Output) != typeof(ConnectionModel))
        {
            output = GetLoadedOutputSocket(0, OnOutputRequestMethod);
            output.SendOutputData(connectionsController);
        }
    }

    protected override void OnSelectedAsCurrentFlowNode(BaseFlowNodeModel previousFlowNode)
    {

    }

    protected override void OnUnselectedAsCurrentFlowNode()
    {

    }

    protected override void SpecificLoad(XmlElement savedData, XmlObjectReferences references)
    {

    }

    protected override void SpecificSave(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {

    }
}
