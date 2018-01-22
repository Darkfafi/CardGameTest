using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeExecution<Input, Output>
{
    private StartNode<Input> start;
    private EndNode<Output> end;

    private BaseNode[] allNodes;
    private NodeSocketConnection[] connections;

    public NodeExecution(StartNode<Input> startNode, EndNode<Output> endNode, BaseNode[] allNodes, NodeSocketConnection[] allConnections)
    {

    }

    public Output Run(Input input)
    {
        return DoNodeChain(input);
    }

    private Output DoNodeChain(Input input)
    {
        return default(Output);
    }
}
