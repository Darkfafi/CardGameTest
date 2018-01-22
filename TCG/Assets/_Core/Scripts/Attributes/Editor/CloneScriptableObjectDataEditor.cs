using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CloneScriptableObjectData))]
public class CloneScriptableObjectDataEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CloneScriptableObjectData a = attribute as CloneScriptableObjectData;
        property.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, position.height), "Cloned " + a.BaseClassType.ToString(),  property.objectReferenceValue, a.BaseClassType, false);
    }
}
