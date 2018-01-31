using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

public abstract class BaseNodeView : INodeEditorDrawable
{
    public const int SOCKET_HEIGHT = 35;
    public const int SOCKET_WIDTH = 30;
    public const int SOCKET_OFFSET = 7;

    public event Action<NodeSocketView> NodeSocketClickedEvent;
    public string Title { get; private set; }
    public BaseNodeModel NodeModel { get; private set; }
    public ViewportRect ViewportRect { get; private set; }
    public bool IsBeingDragged { get; private set; }
    public bool CanBeRemoved { get; private set; }
    public ViewData ViewData { get; private set; }

    private Action<BaseNodeView> removeNodeCallbacks;
    private Action<BaseNodeView> resetConnectionsCallbacks;

    private List<NodeSocketView> socketViews;

    private Color nodeColor;

    private Vector2 sceneStartPosition;

    private bool initialized = false;

    public void Initialize(BaseNodeModel modelToRepresent, Vector2 position, IOriginScene scene, bool canBeRemoved, string title)
    {
        if (initialized) { return; }
        CanBeRemoved = canBeRemoved;
        NodeModel = modelToRepresent;
        NodeModel.ModelInputReceivedDataEvent += OnModelInputReceivedDataEvent;
        NodeModel.NodeConnectedAsEvent += OnNodeConnectedAs;
        Initization();
        sceneStartPosition = position;
        Title = title;
        SetScene(scene);
        initialized = true;
    }

    public NodeSocketView GetSocketView(BaseNodeSocketModel model)
    {
        for(int i = 0; i < socketViews.Count; i++)
        {
            if(socketViews[i].SocketModel == model)
            {
                return socketViews[i];
            }
        }

        return null;
    }

    public void SetScene(IOriginScene scene)
    {
        int na = Mathf.Max(NodeModel.InputSockets.Length, NodeModel.OutputSockets.Length);
        float height = Mathf.Clamp((SOCKET_HEIGHT * na) + (SOCKET_OFFSET * na), 100, float.PositiveInfinity);
        ViewportRect = new ViewportRect(new Rect(sceneStartPosition.x, sceneStartPosition.y, 300, height), scene);
        ViewData = new ViewData(GetType(), NodeModel, ViewportRect, CanBeRemoved, Title);

        if (socketViews == null)
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

    public void Draw()
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

        Vector2 titlePos = new Vector2(ViewportRect.GetViewportPosition().x, ViewportRect.GetViewportPosition().y - 20);
        Vector2 titleSize = new Vector2(100, 20);
        GUI.color = new Color(0.55f, 0.95f, 0.55f);

        ViewData.Title = GUI.TextField(new Rect(titlePos, titleSize), ViewData.Title, GUI.skin.box);

        Vector2 dbPos = new Vector2(titlePos.x + ViewportRect.GetViewportSize().x - 20, ViewportRect.GetViewportPosition().y - titleSize.y);
        Vector2 dbSize = new Vector2(20, titleSize.y);

        GUI.color = NodeModel.DebugMode ? new Color(0.5f, 0.7f, 0.85f) : Color.white;
        NodeModel.SetDebugMode(GUI.Toggle(new Rect(dbPos, dbSize), NodeModel.DebugMode, new GUIContent("D", "Debug Mode: " + (NodeModel.DebugMode ? "On" : "Off")), GUI.skin.button));
        GUI.color = Color.white;
        OnDraw();
    }

    public void HandleEvents(Event e)
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

    public void OnRemovedFromDraw()
    {
        for(int i = socketViews.Count - 1; i >= 0; i--)
        {
            ViewportRect.Scene.UnregisterDrawable(socketViews[i]);
            socketViews[i].SocketViewClickedEvent -= OnSocketViewClickedEvent;
        }

        socketViews = null;
        NodeModel.ModelInputReceivedDataEvent -= OnModelInputReceivedDataEvent;
        NodeModel.NodeConnectedAsEvent -= OnNodeConnectedAs;
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
        BaseNodeSocketModel[] inputs = NodeModel.InputSockets;
        BaseNodeSocketModel[] outputs = NodeModel.OutputSockets;

        socketViews = new List<NodeSocketView>(inputs.Length + outputs.Length);

        for (int i = 0; i < inputs.Length; i++)
        {
            CreateSocketView(inputs[i], i);
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            CreateSocketView(outputs[i], i);
        }
    }

    private void CreateSocketView(BaseNodeSocketModel model, int index)
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

    private void Initization()
    {
        ColorUtility.TryParseHtmlString("#607D8B", out nodeColor);
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