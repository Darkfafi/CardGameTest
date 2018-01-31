using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[NodeViewForModel(typeof(AdditionNodeModel))]
public class AdditionNodeView : BaseNodeView
{
    private int updatedValue = 0;

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