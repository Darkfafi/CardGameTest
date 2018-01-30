using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[NodeViewForModel(typeof(IntegerNodeModel))]
public class IntegerNodeView : BaseNodeView
{

    public IntegerNodeView(IntegerNodeModel modelToRepresent, Vector2 position, IOriginScene scene, bool canBeRemoved) : base(modelToRepresent, position, scene, canBeRemoved)
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

    }

    protected override void OnDraw()
    {
        ((IntegerNodeModel)NodeModel).SetValue(EditorGUI.IntField(new Rect(ViewportRect.GetViewportPositionCenter()
            - ViewportRect.GetViewportSize() * 0.3f, ViewportRect.GetViewportSize() * 0.6f), ((IntegerNodeModel)NodeModel).Value));
    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {

    }
}
