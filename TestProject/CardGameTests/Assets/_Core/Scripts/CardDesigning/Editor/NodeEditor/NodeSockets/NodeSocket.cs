using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SocketType
{
    In,
    Out
}

public class NodeSocket<T> : BaseNodeSocket
{
    private int index;

    private Action<BaseNodeSocket> socketClickedCallbacks;
    private Action<T> receiveOutputCallbacks;
    private Func<T> requestSocketData;

    private Color color;
    private GUIStyle style;

    public NodeSocket(BaseNode parent, int index, SocketType socketType, Rect rect, IOriginScene scene, Color color)
    {
        ParentNode = parent;
        ViewportRect = new ViewportRect(rect, scene);
        ConnectionTypeRequired = typeof(T);
        this.index = index;
        SocketType = socketType;
        this.color = color;
    }

    public Action<T> GetOutputDataMethod()
    {
        return OutputUpdateReceived;
    }

    public T OutputData()
    {
        if (requestSocketData != null)
            return requestSocketData();

        return default(T);
    }

    public override void RequestSocketDataUpdate(BaseNodeSocket outputSocket)
    {
        NodeSocket<T> op = outputSocket as NodeSocket<T>;
        OutputUpdateReceived(op.OutputData());
    }

    public override void ResetToDefaultValue()
    {
        OutputUpdateReceived(default (T));
    }

    public void SetRequestSocketDataMethod(Func<T> requestSocketDataMethod)
    {
        requestSocketData = requestSocketDataMethod;
    }

    public void ListenToSocketReceiveData(Action<T> receiveOutputCallback)
    {
        receiveOutputCallbacks += receiveOutputCallback;
    }

    public void UnlistenToSocketReceiveData(Action<T> receiveOutputCallback)
    {
        receiveOutputCallbacks -= receiveOutputCallback;
    }

    private void OutputUpdateReceived(T outputData)
    {
        if (receiveOutputCallbacks != null)
            receiveOutputCallbacks(outputData);
    }

    public override void RegisterToBaseInteractions(Action<BaseNodeSocket> socketClickedCallback)
    {
        socketClickedCallbacks += socketClickedCallback;
    }

    public override void UnregisterFromBaseInteractions(Action<BaseNodeSocket> socketClickedCallback)
    {
        socketClickedCallbacks -= socketClickedCallback;
    }

    public override void ListenTo(BaseNodeSocket outputNode)
    {
        NodeSocket<T> op = outputNode as NodeSocket<T>;
        op.ListenToSocketReceiveData(OutputUpdateReceived);
    }

    public override void UnlistenFrom(BaseNodeSocket outputNode)
    {
        NodeSocket<T> op = outputNode as NodeSocket<T>;
        op.UnlistenToSocketReceiveData(OutputUpdateReceived);
    }

    public override void Draw()
    {
        if(style == null)
        {
            style = new GUIStyle(GUI.skin.button);
            style.normal.background = GenerateTexture(128, 128, color);
            style.normal.textColor = new Color(1, 0.98f, 230f / 255f);
        }

        string tt = ((SocketType == SocketType.In) ? "Input: " : "Output: ");
        GUI.Box(new Rect(ViewportRect.GetViewportPosition(), ViewportRect.GetViewportSize()), new GUIContent(">", "<"+ index +">" + tt + ConnectionTypeRequired.Name), style);
    }

    public override void HandleEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (ViewportRect.RectContains(e.mousePosition))
                {
                    if(e.button == 0)
                    {
                        if (socketClickedCallbacks != null)
                            socketClickedCallbacks(this);
                    }
                }
                break;
        }
    }

    public override void OnRemovedFromDraw()
    {

    }

    private Texture2D GenerateTexture(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

public abstract class BaseNodeSocket : BaseNodeEditorDrawable
{
    public BaseNode ParentNode { get; protected set; }
    public Type ConnectionTypeRequired { get; protected set; }
    public SocketType SocketType { get; protected set; }
    public abstract void RegisterToBaseInteractions(Action<BaseNodeSocket> socketClickedCallback);
    public abstract void UnregisterFromBaseInteractions(Action<BaseNodeSocket> socketClickedCallback);
    public abstract void ListenTo(BaseNodeSocket outputNode);
    public abstract void UnlistenFrom(BaseNodeSocket outputNode);
    public abstract void RequestSocketDataUpdate(BaseNodeSocket outputSocket);
    public abstract void ResetToDefaultValue();
}

