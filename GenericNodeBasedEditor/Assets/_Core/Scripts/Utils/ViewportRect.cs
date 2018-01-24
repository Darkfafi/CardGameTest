using UnityEngine;
using UnityEditor;
using System;
using System.Xml;

public class ViewportRect
{
    public Rect Rect;
    public Vector2 Offset { get { return offset; } }
    public float Scale { get { return scale; } }
    public IOriginScene Scene { get; private set; }

    private Vector2 offset = new Vector2();
    private float scale = 1;

    public ViewportRect(Rect rect, IOriginScene scene)
    {
        Scene = scene;
        Rect = rect;
    }

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }

    public void AddOffsetWithDelta(Vector2 delta)
    {
        SetOffset(offset + delta);
    }

    public void SetScale(float scale)
    {
        this.scale = scale;
    }

    public void AddScaleWithDelta(float delta)
    {
        SetScale(scale + delta);
    }

    public Vector2 GetViewportPosition()
    {
        Vector2 mid = Scene.SceneOrigin;
        Vector2 p = mid + (Rect.center + offset - mid) * scale;
        return p;
    }

    public Vector2 GetViewportPositionCenter()
    {
        Vector2 p = GetViewportPosition();
        p += GetViewportSize() * 0.5f;
        return p;
    }

    public Vector2 GetViewportSize()
    {
        return new Vector2(Rect.width * scale, Rect.height * scale);
    }

    public bool RectContains(Vector2 pos)
    {
        Rect r = new Rect(GetViewportPosition(), GetViewportSize());
        return r.Contains(pos);
    }

    public Vector2 GetRecalculatedPosition(Vector2 pos)
    {
        return pos + (offset * (1 / scale));
    }

    public Vector2 GetRecalculatedDelta(Vector2 delta)
    {
        return delta * (1 / scale);
    }
}
