using System;
using UnityEditor;
using UnityEngine;

public class NodeSocketView : INodeEditorDrawable
{
    public event Action<NodeSocketView> SocketViewClickedEvent;
    public BaseNodeSocketModel SocketModel { get; private set; }
    public ViewportRect ViewportRect { get; private set; }
    public BaseNodeView ParentNodeView { get; private set; }
    public int Index { get; private set; }
    public Color ViewColor { get { return viewColor; } }
    private Color viewColor;


    private GUIStyle style;

    public NodeSocketView(BaseNodeSocketModel model, BaseNodeView parentView, int index, Vector2 size, IOriginScene scene)
    {
        SetScene(scene);
        ViewportRect.Rect = new Rect(Vector2.zero, size);
        SocketModel = model;
        this.ParentNodeView = parentView;
        this.Index = index;
    }

    public void SetScene(IOriginScene scene)
    {
        ViewportRect = new ViewportRect(new Rect(Vector2.zero, Vector2.zero), scene);
    }

    public void Draw()
    {
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.button);
            string cHex = GetColorForSocket();
            ColorUtility.TryParseHtmlString(cHex, out viewColor);
            style.normal.background = GenerateTexture(128, 128, ViewColor);
            style.normal.textColor = new Color(1, 0.98f, 230f / 255f);
        }

        string tt = ((SocketModel.SocketModelType == SocketModelType.Input) ? "Input: " : "Output: ");
        GUI.Box(new Rect(ViewportRect.GetViewportPosition(), ViewportRect.GetViewportSize()), new GUIContent(">", "<" + Index + ">" + tt + SocketModel.SocketStreamingType.Name), style);
    }

    public void HandleEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)
                {
                    if(ViewportRect.RectContains(e.mousePosition))
                    {
                        if (SocketViewClickedEvent != null)
                            SocketViewClickedEvent(this);
                    }
                }
                break;
        }
    }

    public void OnRemovedFromDraw()
    {
        SocketViewClickedEvent = null;
        SocketModel.Clean();
        SocketModel = null;
        ViewportRect = null;
        style = null;
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

    private string GetColorForSocket()
    {
        string c = "#000000";
        switch (SocketModel.SocketDataType)
        {
            case SocketDataType.DataConnection:
                c = ((SocketModel.SocketModelType == SocketModelType.Input) ? "#E64829" : "#F75940");
                break;
            case SocketDataType.FlowConnection:
                c = ((SocketModel.SocketModelType == SocketModelType.Input) ? "#2CB6AD" : "#3DC7BE");
                break;
        }
        return c;
    }
}
