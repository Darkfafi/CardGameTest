using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericCardPart : EntityPart
{
    public CardType CardType { get; private set; }

    public string Name;
    public string Description;
    public int Cost;

    public CardLocation CurrentLocation;

    public GenericCardPart(CardType type)
    {
        CardType = type;
    }

    protected override void OnAdd()
    {

    }

    protected override void OnRemove()
    {

    }
}

public enum CardLocation
{
    Deck,
    Hand,
    Field,
    Grave
}

public enum CardType
{
    Creature,
    Action,
    Support
}
