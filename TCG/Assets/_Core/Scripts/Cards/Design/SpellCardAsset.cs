using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCardAsset : BaseCardAsset
{
    protected override CardType cardType
    {
        get
        {
            return CardType.Spell;
        }
    }
}
