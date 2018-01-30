using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class GenericMenuBar
{
    private const int BUTTON_PREFERED_WIDTH = 50;
    private const int BUTTON_PREFERED_OFFSET = 2;

    private Dictionary<string, GUIMenuButton> buttonCallbacksMap = new Dictionary<string, GUIMenuButton>();

    private EditorWindow window;

    public GenericMenuBar(EditorWindow window)
    {
        this.window = window;
    }

    public void DrawMenuBarGUI(float height)
    {
        DrawBackground(height);

        foreach (KeyValuePair<string, GUIMenuButton> pair in buttonCallbacksMap)
        {
            pair.Value.Draw(GetButtonPosition(pair.Value.Index, height), GetButtonSize(height));
        }
    }

    private Vector2 GetButtonSize(float height)
    {
        return new Vector2(BUTTON_PREFERED_WIDTH, height);
    }

    private Vector2 GetButtonPosition(int index, float height)
    {
        Vector2 size = GetButtonSize(height);
        return new Vector2(index * size.x + index * BUTTON_PREFERED_OFFSET, 0);
    }

    private void DrawBackground(float height)
    {
        EditorGUI.DrawRect(new Rect(0, 0, window.position.width, height), Color.gray);
    }

    public void AddButton(string buttonText, Action<string> buttonClickCallback)
    {
        if(buttonCallbacksMap.ContainsKey(buttonText))
        {
            Debug.LogError("Menu already containing button with text '" + buttonText + "'");
            return;
        }

        GUIMenuButton b = new GUIMenuButton(buttonText, buttonClickCallback, buttonCallbacksMap.Count);

        buttonCallbacksMap.Add(buttonText, b);
    }

    private class GUIMenuButton
    {
        public Rect Rect { get; private set; }
        public int Index { get; private set; }
        private string title;
        private Action<string> callback;

        public GUIMenuButton(string title, Action<string> callback, int index)
        {
            Rect = new Rect();
            Index = index;
            this.title = title;
            this.callback = callback;
        }

        public void Draw(Vector2 position, Vector2 size)
        {
            Rect = new Rect(position, size);
            if(GUI.Button(Rect, new GUIContent(title)))
            {
                if (callback != null)
                    callback(title);
            }
        }
    }
}
