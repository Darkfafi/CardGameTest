using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPart : EntityPart
{
    public int MaxHealth { get; private set; }
    public int Health { get; private set; }

    public HealthPart(int healthAmount)
    {
        SetOverallHealth(healthAmount);
    }

    public void SetMaxHealth(int amount)
    {
        MaxHealth = Mathf.Clamp(amount, 0, amount);
    }

    public void SetHealth(int amount)
    {
        Health = Mathf.Clamp(amount, 0, MaxHealth);
    }

    public void SetOverallHealth(int amount)
    {
        SetMaxHealth(amount);
        SetHealth(amount);
    }

    protected override void OnAdd()
    {
        Debug.Log("Hello: " + Health + "/" + MaxHealth);
    }

    protected override void OnRemove()
    {
        Debug.Log("Bye");
    }
}
