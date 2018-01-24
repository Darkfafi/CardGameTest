using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOriginScene
{
    Vector2 SceneOrigin { get; }
    Vector2 ToViewportPosition(Vector2 positionToConvert);
    void RegisterDrawable(INodeEditorDrawable instance);
    void UnregisterDrawable(INodeEditorDrawable instance);
}
