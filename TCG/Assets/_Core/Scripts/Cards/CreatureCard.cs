using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCard : BaseCard
{
    public int AttackAmount { get; private set; }
    public int HealthAmount { get; private set; }
    public CardKeyword[] Keywords { get { return keywords.ToArray(); } }

    private List<CardKeyword> keywords;

    public CreatureCard(CreatureCardAsset asset, BaseGamePlayer owner) : base(asset, owner)
    {
        AttackAmount = asset.AttackAmount;
        HealthAmount = asset.HealthAmount;
        keywords = new List<CardKeyword>(asset.Keywords);
    }
}
