using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : BaseNodeEditorDrawable
{
    public const int SOCKET_HEIGHT = 35;
    public const int SOCKET_WIDTH = 30;
    public const int SOCKET_OFFSET = 7;

    public bool IsBeingDragged { get; private set; }
    public bool CanBeRemoved { get; private set; }

    public string Title { get; protected set; }

    protected bool AllowTitleToBeSet = true;

    private bool initialized = false;
    private GUIStyle style;

    private List<BaseNodeSocket> inputSockets = new List<BaseNodeSocket>();
    private List<BaseNodeSocket> outputSockets = new List<BaseNodeSocket>();

    private IOriginScene scene;

    private Action<BaseNodeSocket> socketClickedCallbacks;
    private Action<BaseNode> removeNodeCallbacks;
    private Action<BaseNode> resetConnectionsCallbacks;

    private Vector2 givenSize;

    private Color nodeColor;
    private Color inputColor;
    private Color outputColor;

    public void Initlialize(string title, Vector2 position, Vector2 size, GUIStyle style, IOriginScene scene, bool canBeRemoved)
    {
        if (initialized) { return; }

        ColorUtility.TryParseHtmlString("#607D8B", out nodeColor);
        ColorUtility.TryParseHtmlString("#F75940", out inputColor);
        ColorUtility.TryParseHtmlString("#3DC7BE", out outputColor);
        Title = title;
        this.scene = scene;
        givenSize = size;
        CanBeRemoved = canBeRemoved;
        ViewportRect = new ViewportRect(new Rect(position, size), scene);
        initialized = true;
        OnCreation();
    }

    public override void OnRemovedFromDraw()
    {
        for (int i = 0; i < inputSockets.Count; i++)
        {
            scene.UnregisterDrawable(inputSockets[i]);
        }

        for (int i = 0; i < outputSockets.Count; i++)
        {
            scene.UnregisterDrawable(outputSockets[i]);
        }
    }

    public void Destruct()
    {
        for (int i = 0; i < inputSockets.Count; i++)
        {
            inputSockets[i].UnregisterFromBaseInteractions(OnSocketInteraction);
        }

        for (int i = 0; i < outputSockets.Count; i++)
        {
            outputSockets[i].UnregisterFromBaseInteractions(OnSocketInteraction);
        }

        OnDestruct();
    }

    public void RegisterToTriggers(Action<BaseNode> removeNodeCallback, Action<BaseNodeSocket> socketClickedCallback, Action<BaseNode> resetConnectionCallback)
    {
        removeNodeCallbacks += removeNodeCallback;
        socketClickedCallbacks += socketClickedCallback;
        resetConnectionsCallbacks += resetConnectionCallback;
    }

    public void UnRegisterFromTriggers(Action<BaseNode> removeNodeCallback, Action<BaseNodeSocket> socketClickedCallback, Action<BaseNode> resetConnectionCallback)
    {
        removeNodeCallbacks -= removeNodeCallback;
        socketClickedCallbacks -= socketClickedCallback;
        resetConnectionsCallbacks -= resetConnectionCallback;
    }

    public override void Draw()
    {
        EditorGUI.DrawRect(new Rect(ViewportRect.GetViewportPosition(), ViewportRect.GetViewportSize()), nodeColor);
        string givenTitle = EditorGUI.TextField(new Rect(ViewportRect.GetViewportPosition() - new Vector2((-ViewportRect.GetViewportSize().x * 0.5f) + ViewportRect.GetViewportSize().x * 0.35f, (20)),
            new Vector2(ViewportRect.GetViewportSize().x * 0.7f, 20)), Title);

        if (AllowTitleToBeSet)
            Title = givenTitle;

        BaseNodeSocket s;
        Rect r;
        for(int i = 0; i < inputSockets.Count; i++)
        {
            s = inputSockets[i];
            r = s.ViewportRect.Rect;
            r.center = new Vector2(ViewportRect.Rect.center.x - s.ViewportRect.Rect.width, ViewportRect.Rect.center.y + (s.ViewportRect.Rect.height * i) + (SOCKET_OFFSET * i));
            s.ViewportRect.Rect = r;
        }
        for (int i = 0; i < outputSockets.Count; i++)
        {
            s = outputSockets[i];
            r = s.ViewportRect.Rect;
            r.center = new Vector2(ViewportRect.Rect.center.x + ViewportRect.Rect.width, ViewportRect.Rect.center.y + (s.ViewportRect.Rect.height * i) + (SOCKET_OFFSET * i));
            s.ViewportRect.Rect = r;
        }

        OnDraw();
    }

    public override void HandleEvents(Event e)
    {
        CustomHandleEvents(e);

        switch(e.type)
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
                if(e.button == 0)
                {
                    IsBeingDragged = false;
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 0 && IsBeingDragged)
                {
                    Drag(ViewportRect.GetRecalculatedDelta(e.delta));
                    e.Use();
                }
                break;
        }
    }

    protected void AddInputSocket<T>(Action<T> onInputData)
    {
        NodeSocket<T> s = CreateSocket<T>(SocketType.In, inputSockets.Count, inputColor);
        s.ListenToSocketReceiveData(onInputData);
        inputSockets.Add(s);
        scene.RegisterDrawable(s);
        DoHeightCheck();
    }

    protected Action<T> AddOutputSocket<T>(Func<T> requestSocketData)
    {
        NodeSocket<T> s = CreateSocket<T>(SocketType.Out, outputSockets.Count, outputColor);
        outputSockets.Add(s);
        s.SetRequestSocketDataMethod(requestSocketData);
        scene.RegisterDrawable(s);
        DoHeightCheck();
        return s.GetOutputDataMethod();
    }

    private void DoHeightCheck()
    {
        Rect r = ViewportRect.Rect;
        r.height = GetMinimumHeight();

        if (ViewportRect.Rect.height < r.height)
        {
            if(givenSize.y > r.height)
            {
                r.height = givenSize.y;
            }

            ViewportRect.Rect = r;
        }
    }

    protected abstract void OnCreation();
    protected abstract void OnDraw();
    protected abstract void OnDestruct();
    protected abstract void ContextMenuAddition(GenericMenu genericMenu);
    protected abstract void CustomHandleEvents(Event e);

    private NodeSocket<T> CreateSocket<T>(SocketType socketType, int amount, Color color)
    {
        NodeSocket<T> socket = new NodeSocket<T>(this, amount, socketType, new Rect(0, 0, SOCKET_WIDTH, SOCKET_HEIGHT), scene, color);
        socket.RegisterToBaseInteractions(OnSocketInteraction);
        return socket;
    }

    private void OnSocketInteraction(BaseNodeSocket socket)
    {
        if (socketClickedCallbacks != null)
            socketClickedCallbacks(socket);
    }

    private float GetMinimumHeight()
    {
        int sAmount = Mathf.Max(inputSockets.Count, outputSockets.Count);
        return sAmount * SOCKET_HEIGHT + (sAmount - 1) * SOCKET_OFFSET;
    }

    private void Drag(Vector2 delta)
    {
        ViewportRect.Rect.position += delta;
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

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class NodePathAttribute : Attribute
{
    public const string DO_NOT_MENTION = "-1337_DoNotMentionPath.exe";
    public string Path { get; private set; }

    public NodePathAttribute(string path)
    {
        Path = path;
    }
}
