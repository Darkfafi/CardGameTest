using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardNodeEditor : BaseNodeEditor<CardEntity, CardEntity>
{
    [MenuItem("Assets/Create/CardEffect")]
    public static void OpenWindow()
    {
        CardNodeEditor window = GetWindow<CardNodeEditor>("Card Editor", true);
        window.ShowPopup();
    }
}
