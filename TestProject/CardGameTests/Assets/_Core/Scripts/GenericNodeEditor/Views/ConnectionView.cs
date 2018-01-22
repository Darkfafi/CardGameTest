using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConnectionView : BaseNodeEditorDrawable
{
    public event Action<ConnectionView> ConnectionDestroyRequestEvent;
    public ConnectionModel ConnectionModel { get; private set; }

    private NodeSocketView viewInput, viewOutput;

    public ConnectionView(ConnectionModel model, IOriginScene scene, NodeSocketView viewInput, NodeSocketView viewOutput)
    {
        ViewportRect = new ViewportRect(new Rect(0, 0, 0, 0), scene);
        ConnectionModel = model;
        ConnectionModel.ConnectionDestroyedEvent += OnConnectionDestroyedEvent;
        this.viewInput = viewInput;
        this.viewOutput = viewOutput;

        viewInput.ParentNodeView.NodeConnectedAs(SocketModelType.Input);
        viewOutput.ParentNodeView.NodeConnectedAs(SocketModelType.Output);
    }

    public static void DrawConnectionLine(Vector2 a, Vector2 b, Color c, float size)
    {
        Vector2 tA = a;
        Vector2 tB = b;

        tA.x -= 50;
        tA.y += 50;

        tB.x += 50;
        tB.y -= 50;

        DrawConnectionLine(a, b, tA, tB, c, size);
    }

    public static void DrawConnectionLine(Vector2 a, Vector2 b, Vector2 ta, Vector2 tb, Color c, float size)
    {
        Handles.BeginGUI();
        Handles.DrawBezier(a, b, ta, tb, c, null, size);
        Handles.EndGUI();
    }

    public override void Draw()
    {
        Vector2 posA = viewInput.ViewportRect.GetViewportPositionCenter();
        Vector2 posB = viewOutput.ViewportRect.GetViewportPositionCenter();

        Vector2 tA = posA;
        tA.x -= 50;
        tA.y += 50;

        Vector2 tB = posB;
        tB.x += 50;
        tB.y -= 50;

        DrawConnectionLine(posA, posB, tA, tB, Color.white, 8 * ViewportRect.Scale);
    }

    public override void HandleEvents(Event e)
    {

    }

    public override void OnRemovedFromDraw()
    {
        ConnectionModel.ConnectionDestroyedEvent -= OnConnectionDestroyedEvent;
        ConnectionDestroyRequestEvent = null;
        ConnectionModel.Clean();
        ConnectionModel = null;
        ViewportRect = null;
        viewInput = null;
        viewOutput = null;
    }

    private void OnConnectionDestroyedEvent(ConnectionModel connection)
    {
        if (ConnectionDestroyRequestEvent != null)
            ConnectionDestroyRequestEvent(this);
    }
}
