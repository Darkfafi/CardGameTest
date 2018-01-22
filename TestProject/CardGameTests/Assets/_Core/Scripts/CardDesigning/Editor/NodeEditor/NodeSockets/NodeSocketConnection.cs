using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeSocketConnection : BaseNodeEditorDrawable
{
    public BaseNodeSocket InputSocket { get { return ((origin.SocketType == SocketType.In) ? origin : connectionSocket); } }
    public BaseNodeSocket OutputSocket { get { return ((origin.SocketType == SocketType.Out) ? origin : connectionSocket); } }

    private BaseNodeSocket origin;
    private BaseNodeSocket connectionSocket;

    private Vector2 positionToDrawTo;
    private Color color;

    private int r1 = 25 + UnityEngine.Random.Range(25, 75);
    private int r2 = 25 + UnityEngine.Random.Range(25, 75);

    private int r3 = 15 + UnityEngine.Random.Range(15, 30);
    private int r4 = 15 + UnityEngine.Random.Range(15, 30);

    public bool IsConnectionForNode(BaseNode node)
    {
        if ((origin != null && origin.ParentNode == node) || (connectionSocket != null && connectionSocket.ParentNode == node)) { return true; }
        return false;
    }

    public NodeSocketConnection(BaseNodeSocket origin, IOriginScene scene, Color color)
    {
        this.origin = origin;
        this.color = color;
        ViewportRect = new ViewportRect(new Rect(), scene);
    }

    public bool ConnectTo(BaseNodeSocket socket)
    {
        if (!CanConnectToSocket(socket)) { return false; }
        connectionSocket = socket; 
        InputSocket.ListenTo(OutputSocket);
        InputSocket.RequestSocketDataUpdate(OutputSocket);

        return true;
    }

    public void DestroyConnection()
    {
        InputSocket.UnlistenFrom(OutputSocket);
        InputSocket.ResetToDefaultValue();
        origin = null;
        connectionSocket = null;
    }

    public void SetPreviewDrawPosition(Vector2 pos)
    {
        positionToDrawTo = pos;
    }

    public override void Draw()
    {
        if (origin == null) { return; }
        if(connectionSocket != null)
        {
            positionToDrawTo = connectionSocket.ViewportRect.GetViewportPosition();
            positionToDrawTo.x += connectionSocket.ViewportRect.GetViewportSize().x * 0.5f;
            positionToDrawTo.y += connectionSocket.ViewportRect.GetViewportSize().y * 0.5f;
        }

        Vector2 posOrigin = origin.ViewportRect.GetViewportPosition();
        posOrigin.x += origin.ViewportRect.GetViewportSize().x * 0.5f;
        posOrigin.y += origin.ViewportRect.GetViewportSize().y * 0.5f;


        Vector2 t1 = posOrigin;
        Vector2 t2 = positionToDrawTo;

        t1.x += r1;
        t2.x -= r2;

        t1.y -= r3;
        t2.y += r4;

        Handles.DrawBezier(
            posOrigin,
            positionToDrawTo,
            t1,
            t2,
            color,
            null,
            8 * ViewportRect.Scale
        );
    }

    public override void HandleEvents(Event e)
    {

    }

    private bool CanConnectToSocket(BaseNodeSocket otherSocket)
    {
        if (otherSocket.ParentNode == origin.ParentNode) { return false; }
        if (otherSocket.SocketType == origin.SocketType) { return false; }
        if (!otherSocket.ConnectionTypeRequired.IsAssignableFrom(origin.ConnectionTypeRequired)) { return false; }

        return true;
    }

    public override void OnRemovedFromDraw()
    {

    }
}
