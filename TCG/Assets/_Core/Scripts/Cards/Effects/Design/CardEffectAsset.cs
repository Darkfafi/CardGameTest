using System;
using UnityEngine;

public class CardEffectAsset : ScriptableObject
{
    public BaseCardEffect CardEffect { get { return cardEffect; } }
    public Type CardEffectType { get { return cardEffectType; } }

    private Type cardEffectType;
    private BaseCardEffect cardEffect;

    public void Initialize(BaseCardEffect effect, Type type)
    {
        cardEffectType = type;
        cardEffect = effect;
    }
}