using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageEffect : BaseEffect
{
    public override void DoEffect(params BaseEntity[] targets)
    {

    }

    public override IEffectFilter[] EffectFilters()
    {
        throw new NotImplementedException();
    }

    private BaseEntity[] TargetFinder()
    {
        return new BaseEntity[] { };
    }
}
