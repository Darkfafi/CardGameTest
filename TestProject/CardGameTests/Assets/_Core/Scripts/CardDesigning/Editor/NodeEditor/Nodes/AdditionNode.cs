using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[NodePath("Calculations")]
public class AdditionNode : BaseNode
{
    private Action<int> outputAddition;

    private int firstNumber = 0;
    private int secondNumber = 0;
    private int result = 0;

    private float w = 100;
    private float h = 50;

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

        AddInputSocket<int>(OnFirstNumberReceived);
        AddInputSocket<int>(OnSecondNumberReceived);

        outputAddition = AddOutputSocket<int>(OnRequestAdditionData);
    }

    private int OnRequestAdditionData()
    {
        return DoAddition();
    }

    private void OnFirstNumberReceived(int value)
    {
        firstNumber = value;
        DoAddition();
        SendAdditionOutput();
    }

    private void OnSecondNumberReceived(int value)
    {
        secondNumber = value;
        DoAddition();
        SendAdditionOutput();
    }

    private int DoAddition()
    {
        result = firstNumber + secondNumber;
        return result;
    }

    private void SendAdditionOutput()
    {
        outputAddition(result);
    }

    protected override void OnDestruct()
    {

    }

    protected override void OnDraw()
    {
        Vector2 pos = ViewportRect.GetViewportPosition();
        pos.x += ViewportRect.GetViewportSize().x * 0.5f - (w * ViewportRect.Scale) * 0.5f;
        pos.y += ViewportRect.GetViewportSize().y * 0.5f - (h * ViewportRect.Scale) * 0.5f;

        EditorGUI.LabelField(new Rect(pos, new Vector2(w * ViewportRect.Scale, h * ViewportRect.Scale)), result.ToString(), GUI.skin.button);
    }
}
