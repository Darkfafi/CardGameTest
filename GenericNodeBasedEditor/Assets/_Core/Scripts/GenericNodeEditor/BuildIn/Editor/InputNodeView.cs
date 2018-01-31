using UnityEditor;
using UnityEngine;

[NodePath(NodePathAttribute.DO_NOT_MENTION)]
public class InputNodeView : BaseNodeView
{
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
        GUI.Box(new Rect(ViewportRect.GetViewportPositionCenter() - ViewportRect.GetViewportSize() * 0.3f, ViewportRect.GetViewportSize() * 0.6f), "Entry Point", GUI.skin.box);

    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {

    }
}
