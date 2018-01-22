using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardAsset : ScriptableObject
{
    public CardType CardType { get { return cardType; } }
    public Sprite CardArt { get { return cardArt; } }
    public string CardDescription { get { return description; } }
    public int CardManaCost { get { return manaCost; } }
    public int CardAmountInDeck { get { return amountAbleInDeck; } }

    protected abstract CardType cardType { get; }

    [Header("Base Feedback")]
    [SerializeField]
    private Sprite cardArt;

    [SerializeField, TextArea(2, 4)]
    private string description;

    [Header("Base Stats")]
    [SerializeField, Range(0, 99)]
    private int manaCost;

    [Header("Base Deck building Rules")]
    [SerializeField, Range(1, 4)]
    private int amountAbleInDeck = 4;

    [Header("Base Effects")]
    [SerializeField, CloneScriptableObjectData(typeof(CardEffectAsset))]
    private CardEffectAsset baseCardEffect;
}
