using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[NodePath(NodePathAttribute.DO_NOT_MENTION)]
public class EndNode<Output> : BaseNode
{
    public bool HasReceivedOutput { get; private set; }
    private Output output;

    public void ResetOutput()
    {
        HasReceivedOutput = false;
        output = default(Output);
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
        Title = "<End Node>";
        AddInputSocket<Output>(OnOutputReceived);
    }

    private void OnOutputReceived(Output value)
    {
        HasReceivedOutput = true;
        output = value;
        Debug.Log(output);
    }

    protected override void OnDestruct()
    {

    }

    protected override void OnDraw()
    {

    }
}
