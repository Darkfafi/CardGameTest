using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;


public class ScriptableObjectWindow : EditorWindow
{
    public Type[] Types
    {
        get { return types; }
        set
        {
            types = value;
            names = types.Select((type => type.FullName)).ToArray();
        }
    }

    private Type[] types;

    private int selectedIndex;
    private string[] names;
    
    public void OnGUI()
    {
        GUILayout.Label("Card Effect Class");
        selectedIndex = EditorGUILayout.Popup(selectedIndex, names);

        if(GUILayout.Button("Create Blueprint"))
        {
            ScriptableObjectCreator.CreateAsset<CardEffectAsset>().Initialize((BaseCardEffect)Activator.CreateInstance(Types[selectedIndex]), Types[selectedIndex]);
        }
    }
}


[CustomEditor(typeof(CardEffectAsset))]
public class CardEffectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CardEffectAsset asset = (CardEffectAsset)target;
        var obj = Convert.ChangeType(asset.CardEffect, asset.CardEffectType);
        FieldInfo[] infos = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        SerializedObject so = new UnityEditor.SerializedObject(infos[0].GetRawConstantValue());
    }
}