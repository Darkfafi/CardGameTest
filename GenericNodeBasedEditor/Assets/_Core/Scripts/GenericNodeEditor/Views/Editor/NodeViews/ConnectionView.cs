using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConnectionView : INodeEditorDrawable
{
    public event Action<ConnectionView> ConnectionDestroyRequestEvent;
    public ConnectionModel ConnectionModel { get; private set; }
    public ViewportRect ViewportRect { get; private set; }

    private NodeSocketView viewInput, viewOutput;

    public ConnectionView(ConnectionModel model, IOriginScene scene, NodeSocketView viewInput, NodeSocketView viewOutput)
    {
        SetScene(scene);
        ConnectionModel = model;
        ConnectionModel.ConnectionDestroyedEvent += OnConnectionDestroyedEvent;
        this.viewInput = viewInput;
        this.viewOutput = viewOutput;

        model.InputSocket.ParentNode.NodeConnectedAs(SocketModelType.Input);
        model.OutputSocket.ParentNode.NodeConnectedAs(SocketModelType.Output);
    }

    public void SetScene(IOriginScene scene)
    {
        ViewportRect = new ViewportRect(new Rect(0, 0, 0, 0), scene);
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

    public void Draw()
    {
        Vector2 posA = viewInput.ViewportRect.GetViewportPositionCenter();
        Vector2 posB = viewOutput.ViewportRect.GetViewportPositionCenter();

        Vector2 tA = posA;
        tA.x -= 50;
        tA.y += 50;

        Vector2 tB = posB;
        tB.x += 50;
        tB.y -= 50;

        DrawConnectionLine(posA, posB, tA, tB, viewInput.ViewColor + (Color.white * 0.3f), 8 * ViewportRect.Scale);
    }

    public void HandleEvents(Event e)
    {

    }

    public void OnRemovedFromDraw()
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
