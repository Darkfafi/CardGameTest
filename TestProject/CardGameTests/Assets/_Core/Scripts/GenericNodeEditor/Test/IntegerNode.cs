using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[NodeViewForModel(typeof(IntegerNodeModel))]
public class IntegerNodeView : BaseNodeView
{
    private IntegerNodeModel modelDisplaying;

    public IntegerNodeView(IntegerNodeModel modelToRepresent, Vector2 position, IOriginScene scene, bool canBeRemoved) : base(modelToRepresent, position, scene, canBeRemoved)
    {
        modelDisplaying = modelToRepresent;
    }

    protected override void ContextMenuAddition(GenericMenu gm)
    {

    }

    protected override void CustomHandleEvents(Event e)
    {

    }

    protected override void OnDataOfInputUpdated(object dataReceived)
    {

    }

    protected override void OnDraw()
    {
        modelDisplaying.SetValue(EditorGUI.IntField(new Rect(ViewportRect.GetViewportPositionCenter()
            - ViewportRect.GetViewportSize() * 0.3f, ViewportRect.GetViewportSize() * 0.6f), modelDisplaying.Value));
    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {

    }
}

public class IntegerNodeModel : BaseNodeModel
{
    public int Value { get; private set; }
    private OutputSocketModel output;

    public IntegerNodeModel(ConnectionsController connectionsController) : base(connectionsController)
    {

    }

    public void SetValue(int value)
    {
        if (Value == value) { return; }
        Value = value;
        output.SendOutputData(connectionsController);
    }

    protected override void ConstructSockets()
    {
        output = AddOutputSocket(OnRequestData, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT, typeof(int));
    }

    private object OnRequestData()
    {
        return Value;
    }

    protected override void DataReceivedFromInputSocket(object data)
    {
        output.SendOutputData(connectionsController);
    }
}
