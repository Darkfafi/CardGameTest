using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class AdditionNodeModel : BaseFlowNodeModel
{
    public AdditionNodeModel() : base() { } 

    public AdditionNodeModel(ConnectionsController connectionsController) : base(connectionsController)
    {

    }

    public override FlowNodeType NodeFlowType
    {
        get
        {
            return FlowNodeType.InOutFlow;
        }
    }

    private InputSocketModel<int> input1;
    private InputSocketModel<int> input2;
    private OutputSocketModel<int> output;

    public int DoOperation()
    {
        return input1.GetAnyConnedctedData(connectionsController) + input2.GetAnyConnedctedData(connectionsController);
    }

    protected override void ConstructSockets()
    {
        input1 = AddInputSocket<int>(1);
        input2 = AddInputSocket<int>(1);
        output = AddOutputSocket<int>(OnDataRequest, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT);
    }

    private int OnDataRequest()
    {
        return DoOperation();
    }

    protected override void OnSelectedAsCurrentFlowNode(BaseFlowNodeModel previousFlowNode)
    {

    }

    protected override void OnUnselectedAsCurrentFlowNode()
    {

    }

    protected override void DataReceivedFromInputSocket(object data)
    {
        output.SendOutputData(connectionsController);
    }

    protected override void SpecificSave(XmlDocument doc, XmlObjectReferences references, XmlElement saveableElement)
    {

    }

    protected override void SpecificLoad(XmlElement savedData, XmlObjectReferences references)
    {

    }

    protected override void OnAllDataLoaded()
    {
        input1 = GetLoadedInputSocket<int>(0);
        input2 = GetLoadedInputSocket<int>(1);
        output = GetLoadedOutputSocket<int>(0, OnDataRequest);
    }
}