using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;

public static class CardCreator
{
    private const string UTIL_PATH = "Assets/Create/Card/";

    [MenuItem(UTIL_PATH + "CreatureCard")]
    public static void CreateCreatureCardObject()
    {
        ScriptableObjectCreator.CreateAsset<CreatureCardAsset>();
    }

    [MenuItem(UTIL_PATH + "SpellCard")]
    public static void CreateSpellCardObject()
    {
        ScriptableObjectCreator.CreateAsset<SpellCardAsset>();
    }
}


public static class EffectCreator
{
    private const string UTIL_PATH = "Assets/Create/CardEffect";
    
    [MenuItem(UTIL_PATH)]
    public static void CreateCardEffect()
    {
        Assembly assembly = GetAssembly();

        Type[] scriptableObjectTypes = (from type in assembly.GetTypes() where type.IsSubclassOf(typeof(CardEffectAsset)) select type).ToArray();


    }

    private static Assembly GetAssembly()
    {
        return Assembly.Load(new AssemblyName("Assembly-CSharp"));
    }
}