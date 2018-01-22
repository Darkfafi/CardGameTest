using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EffectFilterEditor : EditorWindow
{
    // Tracking
    private List<Node> nodes;
    private List<Connection> connections = new List<Connection>();

    // Styles
    private GUIStyle nodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    private Vector2 currentViewPosition;

    private Color backgroundColor;
    private Color gridColor;


    [MenuItem("Assets/Custom/CreateEffectFilter")]
    private static void OpenWindow()
    {
        EffectFilterEditor window = GetWindow<EffectFilterEditor>();
        window.titleContent = new GUIContent("Effect Filter Editor");
    }

    private void OnEnable()
    {

        // Colors
        ColorUtility.TryParseHtmlString("#363636", out backgroundColor);
        ColorUtility.TryParseHtmlString("#239D60", out gridColor);

        // End Colors

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);

        wantsMouseMove = true;
        currentViewPosition = new Vector2();
    }

    private Texture2D CreateTextureOfColor(Color color)
    {
        Texture2D texture = new Texture2D(32, 32);
        for (int i = 0; i < texture.width; i++)
        {
            for(int j = 0; j < texture.height; j++)
            {
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
        return texture;
    }

    private void OnGUI()
    {
        // Background
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), backgroundColor);
        Color gc = gridColor;
        gc.a = 0.55f;
        DrawGrid(20, gc);
        gc.a = 0.8f;
        DrawGrid(100, gc);

        DrawNodes();
        DrawConnections();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        DrawPreviewNodeConnection(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawPreviewNodeConnection(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint != null) { return; }
        if (selectedInPoint == null && selectedOutPoint == null) { return; }
        ConnectionPoint p = (selectedInPoint == null) ? selectedOutPoint : selectedInPoint;

        Handles.DrawBezier(
            p.Rect.center + currentViewPosition,
            e.mousePosition,
            p.Rect.center + currentViewPosition + Vector2.left * 50f,
            e.mousePosition - Vector2.left * 50f,
            Color.magenta,
            null,
            2f
        );

        GUI.changed = true;
    }

    private void DrawConnections()
    {
        if (connections == null) { return; }

        for(int i = 0; i < connections.Count; i++)
        {
            connections[i].Draw(currentViewPosition);
        }
    }

    public void DrawGrid(float gridSpacing, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = gridColor;

        Vector3 offset = new Vector3(currentViewPosition.x % gridSpacing, currentViewPosition.y % gridSpacing);

        for(int i = 0; i < widthDivs; i++)
        {
            Vector3 s = new Vector3(gridSpacing * i + offset.x, -gridSpacing, 0);
            Vector3 e = new Vector3(s.x, s.y + position.height, s.z);
            Handles.DrawLine(s, e);
        }

        for (int i = 0; i < heightDivs; i++)
        {
            Vector3 s = new Vector3(-gridSpacing, gridSpacing * i + offset.y, 0);
            Vector3 e = new Vector3(s.x + position.width + gridSpacing, s.y, s.z);
            Handles.DrawLine(s, e);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes == null) { return; }
        
        for(int i = 0; i < nodes.Count; i++)
        {
            nodes[i].Draw(currentViewPosition);
        }
    }

    private void ProcessEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if(e.button == 2)
                {
                    Drag(e.delta);
                }
                break;
        }
    }

    private void Drag(Vector2 delta)
    {
        currentViewPosition += delta;
        GUI.changed = true;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Node"), false, () => { AddNode(mousePosition); });
        genericMenu.ShowAsContext();
    }

    private void AddNode(Vector2 mousePosition)
    {
        if(nodes == null)
        {
            nodes = new List<Node>();
        }

        nodes.Add(new Node(mousePosition - currentViewPosition, 200, 70, nodeStyle, inPointStyle, outPointStyle, SelectInPoint, SelectOutPoint, RemoveNode));
    }

    private void RemoveNode(Node nodeToRemove)
    {
        nodes.Remove(nodeToRemove);
        for(int i = connections.Count - 1; i >= 0; i--)
        {
            if(connections[i].InPoint.Node == nodeToRemove || connections[i].OutPoint.Node == nodeToRemove)
            {
                connections.RemoveAt(i);
            }
        }
    }

    private void ProcessNodeEvents(Event current)
    {
        if (nodes == null) { return; }
        for(int i = nodes.Count - 1; i >= 0; i--)
        {
            GUI.changed = nodes[i].ProcessEvents(current, currentViewPosition);
        }
    }

    private void SelectInPoint(ConnectionPoint inPoint)
    {
        if (inPoint.Type != ConnectionPointType.In) { return; }

        selectedInPoint = inPoint;
        if(selectedOutPoint != null)
        {
            CreateConnection();
            ClearConnectionSelection();
        }
    }

    private void SelectOutPoint(ConnectionPoint outPoint)
    {
        if (outPoint.Type != ConnectionPointType.Out) { return; }
        selectedOutPoint = outPoint;
        if (selectedInPoint != null)
        {
            CreateConnection();
            ClearConnectionSelection();
        }
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void CreateConnection()
    {
        if(connections == null)
        {
            connections = new List<Connection>();
        }

        connections.Add(new Connection(selectedInPoint, selectedOutPoint, RemoveConnection));
    }

    private void RemoveConnection(Connection connection)
    {
        connections.Remove(connection);
    }


}

public class Node
{
    public Rect Rect;
    public string Title;
    public bool IsBeingDragged;

    public ConnectionPoint InPoint;
    public ConnectionPoint OutPoint;

    public GUIStyle Style;

    private Action<Node> removeNodeMethod;

    public Node(Vector2 position, float width, float height, 
        GUIStyle style, GUIStyle inPointStyle, GUIStyle outPointStyle, 
        Action<ConnectionPoint> inPointClicked, Action<ConnectionPoint> outPointClicked, Action<Node> removeNode)
    {
        Rect = new Rect(position, new Vector2(width, height));
        Style = style;
        InPoint = new ConnectionPoint(this, 0.5f, ConnectionPointType.In, inPointStyle, inPointClicked);
        OutPoint = new ConnectionPoint(this, 0.5f, ConnectionPointType.Out, outPointStyle, outPointClicked);
        removeNodeMethod = removeNode;
    }

    public void Drag(Vector2 delta)
    {
        Rect.position += delta;
    }

    public void Draw(Vector2 viewPosition)
    {
        InPoint.Draw(viewPosition);
        OutPoint.Draw(viewPosition);

        Rect r = Rect;

        r.x += viewPosition.x;
        r.y += viewPosition.y;

        GUI.Box(r, Title, Style);
    }

    public bool ProcessEvents(Event e, Vector2 viewPort)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)
                {
                    if(Rect.Contains(e.mousePosition - viewPort))
                    {
                        IsBeingDragged = true;
                    }
                    return true;
                }
                if(e.button == 1 && Rect.Contains(e.mousePosition - viewPort))
                {
                    GenericMenu gm = new GenericMenu();
                    gm.AddItem(new GUIContent("Remove Node"), false, ()=> { if (removeNodeMethod != null) removeNodeMethod(this); });
                    gm.ShowAsContext();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                IsBeingDragged = false;
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && IsBeingDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }
}

public enum ConnectionPointType { In, Out }

public class ConnectionPoint
{
    public Rect Rect;
    public ConnectionPointType Type;
    public Node Node;
    public GUIStyle Style;
    public Action<ConnectionPoint> OnClickConnectionPoint;

    private float percentageY;

    public ConnectionPoint(Node node, float percentageY, ConnectionPointType type, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
    {
        Node = node;
        Type = type;
        Style = style;
        OnClickConnectionPoint = onClickConnectionPoint;
        Rect = new Rect(0, 0, 20f, 30f);
        this.percentageY = percentageY;
    }

    public void Draw(Vector2 viewPosition)
    {
        Rect.y = Node.Rect.y + (Node.Rect.height * percentageY) - Rect.height * 0.5f;

        switch(Type)
        {
            case ConnectionPointType.In:
                Rect.x = Node.Rect.center.x - Node.Rect.width * 0.5f - Rect.width * 0.5f;
                break;
            case ConnectionPointType.Out:
                Rect.x = Node.Rect.center.x + Node.Rect.width * 0.5f - Rect.width * 0.5f;
                break;
        }

        Rect r = Rect;

        r.x += viewPosition.x;
        r.y += viewPosition.y;

        if (GUI.Button(r, "", Style))
        {
            if (OnClickConnectionPoint != null)
                OnClickConnectionPoint(this);
        }
    }
}

public class Connection
{
    public ConnectionPoint InPoint;
    public ConnectionPoint OutPoint;
    public Action<Connection> OnClickRemoveConnection;

    private float drawOffset1 = UnityEngine.Random.value * 100f + 25f;
    private float drawOffset2 = UnityEngine.Random.value * 100f + 25f;

    Vector2 buttonLocation;

    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> onClickRemoveConnection)
    {
        InPoint = inPoint;
        OutPoint = outPoint;
        OnClickRemoveConnection = onClickRemoveConnection;
    }

    public void Draw(Vector2 viewPort)
    {
        Handles.DrawBezier(
            InPoint.Rect.center + viewPort,
            OutPoint.Rect.center + viewPort,
            InPoint.Rect.center + viewPort + Vector2.left * drawOffset1,
            OutPoint.Rect.center + viewPort - Vector2.left * drawOffset2,
            Color.white,
            null,
            2f
        );

        Vector3[] v = Handles.MakeBezierPoints(
             InPoint.Rect.center + viewPort,
             OutPoint.Rect.center + viewPort,
             InPoint.Rect.center + viewPort + Vector2.left * drawOffset1,
             OutPoint.Rect.center + viewPort - Vector2.left * drawOffset2,
             20
         );

        buttonLocation = v[9];

        if (Handles.Button(buttonLocation, Quaternion.identity, 4, 8, Handles.RectangleCap))
        {
            if (OnClickRemoveConnection != null)
                OnClickRemoveConnection(this);
        }
    }
}