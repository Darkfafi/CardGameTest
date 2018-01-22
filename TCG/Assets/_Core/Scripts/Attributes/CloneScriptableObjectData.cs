using System;
using System.Collections.Generic;
using UnityEngine;

public class CloneScriptableObjectData : PropertyAttribute
{
    public Type BaseClassType { get; private set; }

	public CloneScriptableObjectData(Type baseClassType)
    {
        if(!typeof(ScriptableObject).IsAssignableFrom(baseClassType))
        {
            Debug.LogError("Type given to 'CloneScriptableObjectData' attribute MUST be a ScriptableObject, not a " + baseClassType.ToString());
            return;
        }

        BaseClassType = baseClassType;
    }
}
