using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseCardEffect
{
    [SerializeField]
    private float test;

    protected virtual void OnEnterLocation(){}
    protected virtual void OnExitLocation() { } // Exits given location
    protected virtual void OnPhaseChanged() { } // pre phase, next phase
}

public class FireEffect : BaseCardEffect
{
    [SerializeField]
    private float dmg;

    protected override void OnEnterLocation()
    {
        throw new NotImplementedException();
    }

    protected override void OnExitLocation()
    {
        throw new NotImplementedException();
    }

    protected override void OnPhaseChanged()
    {
        throw new NotImplementedException();
    }
}