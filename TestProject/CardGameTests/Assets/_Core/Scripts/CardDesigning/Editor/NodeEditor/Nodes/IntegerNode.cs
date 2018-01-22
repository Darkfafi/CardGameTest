using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[NodePath("Calculations")]
public class IntegerNode : BaseNode
{
    private int value = 0;
    private int preValue = 0;

    private Action<int> valueOutputChanged; 

    protected override void ContextMenuAddition(GenericMenu genericMenu)
    {

    }

    protected override void CustomHandleEvents(Event e)
    {

    }

    protected override void OnCreation()
    {
        ViewportRect.Rect.width = 200;
        ViewportRect.Rect.height = 100;
        valueOutputChanged = AddOutputSocket<int>(OnRequestIntegerData);
    }

    private int OnRequestIntegerData()
    {
        Debug.Log(value);
        return value;
    }

    protected override void OnDestruct()
    {

    }

    protected override void OnDraw()
    {
        Vector2 pos = ViewportRect.GetViewportPosition();

        pos += ViewportRect.GetViewportSize() * 0.5f;

        pos.x -= ViewportRect.GetViewportSize().x * 0.4f;
        pos.y -= ViewportRect.GetViewportSize().y * 0.2f;

        value = EditorGUI.IntField(new Rect(pos, new Vector2(ViewportRect.GetViewportSize().x * 0.8f, ViewportRect.GetViewportSize().y * 0.4f)), value);
        if(value != preValue)
        {
            preValue = value;
            valueOutputChanged(value);
        }
    }
}
