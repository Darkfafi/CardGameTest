using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[NodeViewForModel(typeof(AdditionNodeModel))]
public class AdditionNodeView : BaseNodeView
{
    private AdditionNodeModel modelRepresenting;
    private int updatedValue = 0;
    
    public AdditionNodeView(AdditionNodeModel modelToRepresent, Vector2 pos, IOriginScene scene, bool canBeRemoved) : base(modelToRepresent, pos, scene, canBeRemoved)
    {
        modelRepresenting = modelToRepresent;
    }

    protected override void ContextMenuAddition(GenericMenu gm)
    {

    }

    protected override void CustomHandleEvents(Event e)
    {

    }

    protected override void OnDataOfInputUpdated(object dataReceived)
    {
        updatedValue = modelRepresenting.DoOperation();
    }

    protected override void OnDraw()
    {
        GUI.Box(new Rect(ViewportRect.GetViewportPositionCenter() - ViewportRect.GetViewportSize() * 0.3f, ViewportRect.GetViewportSize() * 0.6f), updatedValue.ToString(), GUI.skin.box);
    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {
        updatedValue = modelRepresenting.DoOperation();
    }
}

[Serializable]
public class AdditionNodeModel : BaseFlowNodeModel
{
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

    private InputSocketModel input1;
    private InputSocketModel input2;
    private OutputSocketModel output;

    public int DoOperation()
    {
        return ((int)input1.GetAnyConnedctedData(connectionsController)) + ((int)input2.GetAnyConnedctedData(connectionsController));
    }

    protected override void ConstructSockets()
    {
        input1 = AddInputSocket(1, typeof(int));
        input2 = AddInputSocket(1, typeof(int));
        output = AddOutputSocket(OnDataRequest, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT, typeof(int));
    }

    private object OnDataRequest()
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
}