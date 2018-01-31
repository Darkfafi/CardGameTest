using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class NodeDataExecuter<Input>
{
    private InputNodeModel<Input> rootPoint;

    private ConnectionsController connections;

    private BaseFlowNodeModel currentFlowNode;
    private BaseFlowNodeModel preFlowNode = null;

    public NodeDataExecuter(GenericNodesSaveData data)
    {
        rootPoint = data.NodeModels.Where(nm => typeof(InputNodeModel<Input>).IsAssignableFrom(nm.GetType())).Cast<InputNodeModel<Input>>().ToArray()[0];
        //allOtherFlowNodes = data.NodeModels.Where(nm => typeof(BaseFlowNodeModel).IsAssignableFrom(nm.GetType()) && nm != rootPoint).Cast<BaseFlowNodeModel>().ToArray();
        connections = data.ConnectionController;
    }

    public void Run(Input inputData)
    {
        SetAsCurrentFlowNode(rootPoint);
        rootPoint.Run(inputData);
    }

    public void Clean()
    {
        SetAsCurrentFlowNode(null);
        currentFlowNode = null;
        preFlowNode = null;
        connections = null;
        rootPoint = null;
    }

    private void SetAsCurrentFlowNode(BaseFlowNodeModel newFlowNode)
    {
        if (currentFlowNode != null)
        {
            preFlowNode = currentFlowNode;
            preFlowNode.RequestNextNodeFlowEvent -= OnRequestNextNodeFlowEvent;
            preFlowNode.RequestPreviousNodeFlowEvent -= OnRequestPreviousNodeFlowEvent;
            preFlowNode.UnselectAsCurrentFlowNode();
        }

        currentFlowNode = newFlowNode;

        if (newFlowNode == null) { return; }

        currentFlowNode.RequestNextNodeFlowEvent += OnRequestNextNodeFlowEvent;
        currentFlowNode.RequestPreviousNodeFlowEvent += OnRequestPreviousNodeFlowEvent;

        newFlowNode.SelectAsCurrentFlowNode(preFlowNode);
    }

    private void OnRequestNextNodeFlowEvent()
    {
        BaseFlowNodeModel nextFlowNode = currentFlowNode.NextFlowNode(connections);
        if (nextFlowNode != null)
            SetAsCurrentFlowNode(nextFlowNode);
    }

    private void OnRequestPreviousNodeFlowEvent()
    {
        if (preFlowNode != null)
            SetAsCurrentFlowNode(preFlowNode);
    }
}
