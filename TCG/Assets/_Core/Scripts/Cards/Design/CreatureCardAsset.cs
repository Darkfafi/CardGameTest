using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCardAsset : BaseCardAsset
{
    public int AttackAmount { get { return attack; } }
    public int HealthAmount { get { return health; } }
    public CardKeyword[] Keywords { get { return keywords; } }

    protected override CardType cardType
    {
        get
        {
            return CardType.Creature;
        }
    }

    [Header("Creature Specific Stats")]
    [SerializeField, Range(0, 99)]
    private int attack;

    [SerializeField, Range(0, 99)]
    private int health;

    [SerializeField]
    private CardKeyword[] keywords;
}