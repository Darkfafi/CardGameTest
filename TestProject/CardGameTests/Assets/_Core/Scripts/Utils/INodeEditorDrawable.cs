using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNodeEditorDrawable
{
    public ViewportRect ViewportRect { get; protected set; }
    public abstract void Draw();
    public abstract void HandleEvents(Event e);
    public abstract void OnRemovedFromDraw();
}