﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public enum FlowNodeType
{
    InOutFlow,
    OutFlow,
    InFlow
}

public abstract class BaseFlowNodeModel : BaseNodeModel
{
    public abstract FlowNodeType NodeFlowType { get; }

    private const string inputMethodName = "InternalAddInputSocket";
    private const string outputMethodName = "InternalAddOutputSocket";

    public BaseFlowNodeModel() : base() { }

    public BaseFlowNodeModel(ConnectionsController connectionsController) : base(connectionsController)
    {

    }

    public void SelectAsCurrentFlowNode(BaseFlowNodeModel previousFlowNode)
    {
        OnSelectedAsCurrentFlowNode(previousFlowNode);
    }

    public void UnselectAsCurrentFlowNode()
    {
        OnUnselectedAsCurrentFlowNode();
    }

    protected override InputSocketModel<T> GetLoadedInputSocket<T>(int index)
    {
        index = (NodeFlowType == FlowNodeType.InFlow || NodeFlowType == FlowNodeType.InOutFlow) ? index + 1 : index;
        return base.GetLoadedInputSocket<T>(index);
    }

    protected override OutputSocketModel<T> GetLoadedOutputSocket<T>(int index, Func<T> outputRequestMethod)
    {
        index = (NodeFlowType == FlowNodeType.OutFlow || NodeFlowType == FlowNodeType.InOutFlow) ? index + 1 : index;
        return base.GetLoadedOutputSocket<T>(index, outputRequestMethod);
    }

    protected abstract void OnSelectedAsCurrentFlowNode(BaseFlowNodeModel previousFlowNode);
    protected abstract void OnUnselectedAsCurrentFlowNode();

    protected override void BeforeConstruction()
    {
        switch(NodeFlowType)
        {
            case FlowNodeType.InFlow:
                CreateFlowInput();
                break;
            case FlowNodeType.OutFlow:
                CreateFlowOutput();
                break;
            case FlowNodeType.InOutFlow:
                CreateFlowInput();
                CreateFlowOutput();
                break;
        }
    }

    private BaseNodeSocketModel InvokeMethod(string mn, params object[] parameters)
    {
        MethodInfo mi = typeof(BaseNodeModel).GetMethod(mn, BindingFlags.NonPublic | BindingFlags.Instance);
        return (BaseNodeSocketModel)mi.Invoke(this, parameters);
    }

    private void CreateFlowInput()
    {
        InvokeMethod(inputMethodName, typeof(InputSocketModel<BaseFlowNodeModel>), SocketDataType.FlowConnection, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT);

    }

    private void CreateFlowOutput()
    {
        InvokeMethod(outputMethodName, typeof(OutputSocketModel<BaseFlowNodeModel>), SocketDataType.FlowConnection, 1);
    }
}
