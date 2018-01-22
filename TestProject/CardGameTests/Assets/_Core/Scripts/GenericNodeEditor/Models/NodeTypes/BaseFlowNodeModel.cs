using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[Serializable]
public enum FlowNodeType
{
    InOutFlow,
    OutFlow,
    InFlow
}

[Serializable]
public abstract class BaseFlowNodeModel : BaseNodeModel
{
    public abstract FlowNodeType NodeFlowType { get; }

    private const string inputMethodName = "InternalAddInputSocket";
    private const string outputMethodName = "InternalAddOutputSocket";

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
        InvokeMethod(inputMethodName, typeof(InputSocketModel), SocketDataType.FlowConnection, ConnectionsController.INFINITE_CONNECTIONS_AMOUNT, typeof(BaseFlowNodeModel));

    }

    private void CreateFlowOutput()
    {
        InvokeMethod(outputMethodName, typeof(OutputSocketModel), SocketDataType.FlowConnection, 1, typeof(BaseFlowNodeModel));
    }
}
