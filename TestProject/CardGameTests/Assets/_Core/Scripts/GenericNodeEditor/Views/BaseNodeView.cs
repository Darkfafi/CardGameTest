using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;

public abstract class BaseNodeView : BaseNodeEditorDrawable
{
    public const int SOCKET_HEIGHT = 35;
    public const int SOCKET_WIDTH = 30;
    public const int SOCKET_OFFSET = 7;

    public event Action<NodeSocketView> NodeSocketClickedEvent;
    public BaseNodeModel NodeModel { get; private set; }
    public bool IsBeingDragged { get; private set; }
    public bool CanBeRemoved { get; private set; }

    private Action<BaseNodeView> removeNodeCallbacks;
    private Action<BaseNodeView> resetConnectionsCallbacks;

    private List<NodeSocketView> socketViews;

    private Color nodeColor;

    public BaseNodeView(BaseNodeModel modelToRepresent, Vector2 position, IOriginScene scene, bool canBeRemoved)
    {
        ColorUtility.TryParseHtmlString("#607D8B", out nodeColor);
        CanBeRemoved = canBeRemoved;
        NodeModel = modelToRepresent;
        NodeModel.ModelInputReceivedDataEvent += OnModelInputReceivedDataEvent;
        int na = Mathf.Max(NodeModel.InputSockets.Length, NodeModel.OutputSockets.Length);
        float height = Mathf.Clamp((SOCKET_HEIGHT * na) + (SOCKET_OFFSET * na), 100, float.PositiveInfinity);
        ViewportRect = new ViewportRect(new Rect(position.x, position.y, 300, height), scene);
        CreateSocketViews();
    }

    public void RegisterToInteractionEvents(Action<BaseNodeView> removeNodeCallback, Action<BaseNodeView> resetConnectionsCallback)
    {
        removeNodeCallbacks += removeNodeCallback;
        resetConnectionsCallbacks += resetConnectionsCallback;
    }

    public void UnregisterToInteractionEvents(Action<BaseNodeView> removeNodeCallback, Action<BaseNodeView> resetConnectionsCallback)
    {
        removeNodeCallbacks -= removeNodeCallback;
        resetConnectionsCallbacks -= resetConnectionsCallback;
    }

    public override void Draw()
    {
        EditorGUI.DrawRect(new Rect(ViewportRect.GetViewportPosition(), ViewportRect.GetViewportSize()), nodeColor);

        NodeSocketView sv;
        Rect r;
        for(int i = 0; i < socketViews.Count; i++)
        {
            sv = socketViews[i];
            r = sv.ViewportRect.Rect;
            float modX = sv.SocketModel.SocketModelType == SocketModelType.Input ? -sv.ViewportRect.Rect.width : ViewportRect.Rect.width;
            float x = ViewportRect.Rect.center.x + modX;
            float y = ViewportRect.Rect.center.y + (sv.ViewportRect.Rect.height * sv.Index) + (SOCKET_OFFSET * sv.Index);
            r.center = new Vector2(x, y);
            sv.ViewportRect.Rect = r;
        }

        OnDraw();
    }

    public void NodeConnectedAs(SocketModelType nodeRoleInConnection)
    {
        OnNodeConnectedAs(nodeRoleInConnection);
    }

    public override void HandleEvents(Event e)
    {
        CustomHandleEvents(e);
        switch (e.type)
        {
            case EventType.MouseDown:
                if (ViewportRect.RectContains(e.mousePosition))
                {
                    if (e.button == 0)
                    {
                        IsBeingDragged = true;
                    }
                    else if (e.button == 1)
                    {
                        ContextMenu(CanBeRemoved);
                        e.Use();
                    }
                }
                break;
            case EventType.MouseUp:
                if (e.button == 0)
                {
                    IsBeingDragged = false;
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && IsBeingDragged)
                {
                    Drag(ViewportRect.GetRecalculatedDelta(e.delta));
                    e.Use();
                }
                break;
        }
    }

    public override void OnRemovedFromDraw()
    {
        for(int i = socketViews.Count - 1; i >= 0; i--)
        {
            ViewportRect.Scene.UnregisterDrawable(socketViews[i]);
            socketViews[i].SocketViewClickedEvent -= OnSocketViewClickedEvent;
        }

        socketViews = null;
        NodeModel.ModelInputReceivedDataEvent -= OnModelInputReceivedDataEvent;
        NodeModel.Clean();
        NodeModel = null;
        NodeSocketClickedEvent = null;
    }

    protected abstract void OnDraw();
    protected abstract void CustomHandleEvents(Event e);
    protected abstract void ContextMenuAddition(GenericMenu gm);
    protected abstract void OnNodeConnectedAs(SocketModelType nodeRoleInConnection);
    protected abstract void OnDataOfInputUpdated(object dataReceived);

    private void OnModelInputReceivedDataEvent(object dataReceived)
    {
        OnDataOfInputUpdated(dataReceived);
    }

    private void Drag(Vector2 delta)
    {
        ViewportRect.Rect.position += delta;
    }

    private void CreateSocketViews()
    {
        InputSocketModel[] inputs = NodeModel.InputSockets;
        OutputSocketModel[] outputs = NodeModel.OutputSockets;

        socketViews = new List<NodeSocketView>(inputs.Length + outputs.Length);

        for (int i = 0; i < inputs.Length; i++)
        {
            CreateInputSocketView(inputs[i], i);
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            CreateOutputSocketView(outputs[i], i);
        }
    }

    private void CreateInputSocketView(InputSocketModel model, int index)
    {
        NodeSocketView nsv = new NodeSocketView(model, this, index, new Vector3(SOCKET_WIDTH, SOCKET_HEIGHT), ViewportRect.Scene);
        socketViews.Add(nsv);
        ViewportRect.Scene.RegisterDrawable(nsv);
        nsv.SocketViewClickedEvent += OnSocketViewClickedEvent;
    }

    private void CreateOutputSocketView(OutputSocketModel model, int index)
    {
        NodeSocketView nsv = new NodeSocketView(model, this, index, new Vector3(SOCKET_WIDTH, SOCKET_HEIGHT), ViewportRect.Scene);
        socketViews.Add(nsv);
        ViewportRect.Scene.RegisterDrawable(nsv);
        nsv.SocketViewClickedEvent += OnSocketViewClickedEvent;
    }

    private void OnSocketViewClickedEvent(NodeSocketView nodeClicked)
    {
        if (NodeSocketClickedEvent != null)
            NodeSocketClickedEvent(nodeClicked);
    }

    private void ContextMenu(bool canBeRemoved)
    {
        GenericMenu gm = new GenericMenu();
        if (canBeRemoved)
            gm.AddItem(new GUIContent("Remove Item"), false, () => { if (removeNodeCallbacks != null) { removeNodeCallbacks(this); } });
        else
            gm.AddDisabledItem(new GUIContent("Remove Item"));

        gm.AddItem(new GUIContent("Reset Connections"), false, () => { if (resetConnectionsCallbacks != null) { resetConnectionsCallbacks(this); } });

        gm.AddSeparator("");
        ContextMenuAddition(gm);
        gm.ShowAsContext();
    }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class NodeViewForModelAttribute : Attribute
{
    public Type NodeModelType { get; private set; }

    public NodeViewForModelAttribute(Type nodeModelType)
    {
        NodeModelType = nodeModelType;
    }
}


