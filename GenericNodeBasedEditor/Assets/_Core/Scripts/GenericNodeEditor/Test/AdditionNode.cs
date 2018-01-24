using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

[NodeViewForModel(typeof(AdditionNodeModel))]
public class AdditionNodeView : BaseNodeView
{
    private int updatedValue = 0;

    public AdditionNodeView()
    {

    }

    public AdditionNodeView(AdditionNodeModel modelToRepresent, Vector2 pos, IOriginScene scene, bool canBeRemoved) : base(modelToRepresent, pos, scene, canBeRemoved)
    {

    }

    protected override void ContextMenuAddition(GenericMenu gm)
    {

    }

    protected override void CustomHandleEvents(Event e)
    {

    }

    protected override void OnDataOfInputUpdated(object dataReceived)
    {
        updatedValue = ((AdditionNodeModel)NodeModel).DoOperation();
    }

    protected override void OnDraw()
    {
        GUI.Box(new Rect(ViewportRect.GetViewportPositionCenter() - ViewportRect.GetViewportSize() * 0.3f, ViewportRect.GetViewportSize() * 0.6f), updatedValue.ToString(), GUI.skin.box);
    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {
        updatedValue = ((AdditionNodeModel)NodeModel).DoOperation();
    }
}

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