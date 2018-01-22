using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UserDesignEditor : EditorWindow
{
    [MenuItem("Assets/Create/Custom/User")]
    public static void CreateDesignUserAsset()
    {
        UserDesign ud = (UserDesign)ScriptableObject.CreateInstance(typeof(UserDesign));
        ProjectWindowUtil.CreateAsset(ud, "New " + ud.Name + ".asset");
    }
}
