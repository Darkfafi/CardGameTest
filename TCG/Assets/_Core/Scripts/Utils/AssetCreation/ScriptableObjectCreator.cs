using UnityEngine;
using UnityEditor;

public static class ScriptableObjectCreator
{
	public static T CreateAsset<T>() where T : ScriptableObject
    {
        T so = (T)ScriptableObject.CreateInstance<T>();
        ProjectWindowUtil.CreateAsset(so, "New " + typeof(T).Name + ".asset");
        return so;
    }
}
