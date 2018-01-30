using UnityEditor;
using UnityEngine;

[NodePath(NodePathAttribute.DO_NOT_MENTION)]
public class InputNodeView : BaseNodeView
{
    public InputNodeView(BaseNodeModel modelToRepresent, Vector2 position, IOriginScene scene, bool canBeRemoved) : base(modelToRepresent, position, scene, canBeRemoved)
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

    }

    protected override void OnNodeConnectedAs(SocketModelType nodeRoleInConnection)
    {

    }
}
