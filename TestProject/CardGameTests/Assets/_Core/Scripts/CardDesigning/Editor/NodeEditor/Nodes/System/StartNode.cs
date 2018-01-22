using System;
using UnityEditor;
using UnityEngine;

[NodePath(NodePathAttribute.DO_NOT_MENTION)]
public class StartNode<Input> : BaseNode
{
    private Action<Input> outputMethod;
    private Input input;

    public void SetInput(Input input)
    {
        this.input = input;
        outputMethod(input);
    }

    protected override void ContextMenuAddition(GenericMenu genericMenu)
    {

    }

    protected override void CustomHandleEvents(Event e)
    {

    }

    protected override void OnCreation()
    {
        AllowTitleToBeSet = false;
        Title = "<Start Node>";
        outputMethod = AddOutputSocket<Input>(OnRequestStartNodeData);
    }

    private Input OnRequestStartNodeData()
    {
        return input;
    }

    protected override void OnDestruct()
    {

    }

    protected override void OnDraw()
    {

    }
}
