using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

// Input generates a node which requires the given input type, Output generates a node which returns the given output type
public abstract class BaseNodeEditor<Input, Output> : EditorWindow, IOriginScene 
{
    public Vector2 SceneOrigin { get { return new Vector2(position.width * 0.5f + viewport.x, position.height * 0.5f + viewport.y); } }

    private List<BaseNodeEditorDrawable> drawables = new List<BaseNodeEditorDrawable>();
    private List<NodeSocketConnection> connections = new List<NodeSocketConnection>();
    private List<BaseNode> nodes = new List<BaseNode>();

    private Vector2 viewport = new Vector2();
    private float scale = 1;

    private NodeSocketConnection linkingConnection;

    private StartNode<Input> startNode;
    private EndNode<Output> outputNode;

    private Color connectionColor = Color.white;

    // Start Editor
    public void OnEnable()
    {
        wantsMouseMove = true;

        ColorUtility.TryParseHtmlString("#E9FFB2", out connectionColor);

        if(startNode == null)
        {
            startNode = new StartNode<Input>();
            AddNode(startNode, new Vector2(-350, 0), false);
        }

        if(outputNode == null)
        {
            outputNode = new EndNode<Output>();
            AddNode(outputNode, new Vector2(350, 0), false);
        }
    }

    public Vector2 ToViewportPosition(Vector2 positionToConvert)
    {
        return SceneOrigin + (positionToConvert - SceneOrigin) * (1 / scale) - viewport;
    }

    public void RegisterDrawable(BaseNodeEditorDrawable instance)
    {
        drawables.Add(instance);

        SetViewportPosition(viewport);
        SetScale(scale);
    }

    public void UnregisterDrawable(BaseNodeEditorDrawable instance)
    {
        drawables.Remove(instance);
        instance.OnRemovedFromDraw();
    }

    public void OnGUI()
    {
        // Drawing
        DrawBackground();

        EditorGUI.DrawRect(new Rect(SceneOrigin, new Vector2(10 * scale, 10 * scale)), Color.green);

        DrawDrawables();

        // Object Interactions
        DrawablesInteractionHandling(Event.current);

        // Screen Interactions
        ScreenNavigationControlls(Event.current);
        ScreenInteraction(Event.current);

        // UI
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;
        EditorGUI.LabelField(new Rect(new Vector2(10, position.height - 20), new Vector2(250, 100)), "X: " + viewport.x + "Y: " + viewport.y + " | Zoom: " + Mathf.RoundToInt(scale * 100) + "%", guiStyle);

        if(linkingConnection != null)
        {
            linkingConnection.SetPreviewDrawPosition(Event.current.mousePosition);
            linkingConnection.Draw();
            GUI.changed = true;
        }

        if (GUI.changed) { Repaint(); }
    }

    private void DrawablesInteractionHandling(Event e)
    {
        for (int i = drawables.Count - 1; i >= 0; i--)
        {
            drawables[i].HandleEvents(e);
        }
    }

    private void ScreenInteraction(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)
                {
                    EditorGUI.FocusTextInControl("");
                }
                if(e.button == 1)
                {
                    OpenScreenOptionsMenu(e);
                    e.Use();
                }
                break;
        }
    }

    private void OpenScreenOptionsMenu(Event e)
    {
        GenericMenu gm = new GenericMenu();
        Type[] nodeTypes = Assembly.GetCallingAssembly().GetTypes().Where((n => typeof(BaseNode).IsAssignableFrom(n) && !n.IsAbstract)).ToArray();

        for(int i = 0; i < nodeTypes.Length; i++)
        {
            int index = i;
            string path = GetPathForNodeType(nodeTypes[i]);

            if(path != NodePathAttribute.DO_NOT_MENTION)
                gm.AddItem(new GUIContent("Add Node/" + path + nodeTypes[i].Name), false, () => { AddNode(nodeTypes[index], e.mousePosition); });
        }
        gm.ShowAsContext();
    }

    private string GetPathForNodeType(Type nodeType)
    {
        object[] attributes = nodeType.GetCustomAttributes(typeof(NodePathAttribute), true);
        if (attributes.Length == 0) { return ""; }
        NodePathAttribute attr = attributes[0] as NodePathAttribute;
        if (attr.Path == NodePathAttribute.DO_NOT_MENTION) { return NodePathAttribute.DO_NOT_MENTION; }
        return attr.Path + "/";
    }

    private void AddNode(Type nodeType, Vector2 position)
    {
        BaseNode node = (BaseNode)Activator.CreateInstance(nodeType);
        AddNode(node, position, true);
    }

    private void AddNode(BaseNode node, Vector2 position, bool canBeRemoved)
    {
        node.Initlialize(node.GetType().Name, ToViewportPosition(position), new Vector2(325, 100), new GUIStyle(), this, canBeRemoved);
        node.RegisterToTriggers(RemoveNode, SocketSelect, DestroyAllConnectionsForNode);
        nodes.Add(node);
        RegisterDrawable(node);
    }

    private void RemoveNode(BaseNode nodeToRemove)
    {
        UnregisterDrawable(nodeToRemove);
        nodeToRemove.UnRegisterFromTriggers(RemoveNode, SocketSelect, DestroyAllConnectionsForNode);
        nodes.Remove(nodeToRemove);
        DestroyAllConnectionsForNode(nodeToRemove);
        nodeToRemove.Destruct();
    }

    private void SocketSelect(BaseNodeSocket socketToSelect)
    {
        if(linkingConnection == null)
        {
            if(socketToSelect.SocketType == SocketType.Out)
                linkingConnection = new NodeSocketConnection(socketToSelect, this, connectionColor);
        }
        else
        {
            if(linkingConnection.ConnectTo(socketToSelect))
            {
                AddConnection(linkingConnection);
            }

            linkingConnection = null;
        }
    }

    private void DestroyAllConnectionsForNode(BaseNode node)
    {
        NodeSocketConnection[] c = GetAllConnectionsForNode(node);

        for(int i = 0; i < c.Length; i++)
        {
            RemoveConnection(c[i]);
        }
    }

    private NodeSocketConnection[] GetAllConnectionsForNode(BaseNode node)
    {
        List<NodeSocketConnection> connectionsReturnValue = new List<NodeSocketConnection>();

        for(int i = 0; i < connections.Count; i++)
        {
            if(connections[i].IsConnectionForNode(node))
            {
                connectionsReturnValue.Add(connections[i]);
            }
        }

        return connectionsReturnValue.ToArray();
    }

    private void AddConnection(NodeSocketConnection connection)
    {
        connections.Add(connection);
        RegisterDrawable(connection);
    }

    public void RemoveConnection(NodeSocketConnection connection)
    {
        connections.Remove(connection);
        connection.DestroyConnection();
        UnregisterDrawable(connection);
    }

    private void ScreenNavigationControlls(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDrag:
                if(e.button == 2)
                {
                    SetViewportPosition(viewport + e.delta);
                }
            break;
            case EventType.ScrollWheel:
                float inputScale = scale + -e.delta.y * (0.05f * scale);
                float oldScale = scale;
                SetScale(inputScale);
                SetViewportPosition(viewport * (scale / oldScale));
                break;

        }
    }

    private void DrawDrawables()
    {
        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].Draw();
        }
    }

    private void SetScale(float scale)
    {
        this.scale = Mathf.Clamp(scale, 0.25f, 1.25f);

        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].ViewportRect.SetScale(scale);
        }

        GUI.changed = true;
    }

    private void SetViewportPosition(Vector2 viewportPos)
    {
        viewport = viewportPos;

        for (int i = 0; i < drawables.Count; i++)
        {
            drawables[i].ViewportRect.SetOffset(viewport);
        }

        GUI.changed = true;
    }

    private void DrawBackground()
    {
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.2f, 0.2f, 0.2f));

        DrawGrid(20 * scale, 0.2f, Color.green);
        DrawGrid(100 * scale, 0.8f, Color.green);
    }

    private void DrawGrid(float gridSpacing, float alphaMiddle, Color gridColor)
    {
        int devW = Mathf.CeilToInt(position.width / gridSpacing);
        int devH = Mathf.CeilToInt(position.height / gridSpacing);

        Vector2 gridVisualOffset = new Vector2(viewport.x % gridSpacing, viewport.y % gridSpacing);

        Vector3 s;
        Vector3 e;

        Handles.BeginGUI();
        Color c1 = gridColor;
        c1.a = alphaMiddle;

        Handles.color = c1;

        for (int i = 0; i < devW; i++)
        {
            s = new Vector3(gridSpacing * i + gridVisualOffset.x, -gridSpacing);
            e = s;
            e.y += position.height + gridSpacing;

            Handles.DrawLine(s, e);
        }

        for (int i = 0; i < devH; i++)
        {
            s = new Vector3(-gridSpacing, gridSpacing * i + gridVisualOffset.y);
            e = s;
            e.x += position.width + gridSpacing;

            Handles.DrawLine(s, e);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}