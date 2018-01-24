using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeEditorDrawable
{
    ViewportRect ViewportRect { get; }
    void Draw();
    void HandleEvents(Event e);
    void OnRemovedFromDraw();
    void SetScene(IOriginScene scene);
}