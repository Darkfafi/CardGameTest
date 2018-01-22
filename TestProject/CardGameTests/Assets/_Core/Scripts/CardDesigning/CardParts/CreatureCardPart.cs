using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCardPart : EntityPart
{
    public CreatureType CreatureType { get; private set; }
    public CreatureClass CreatureClass { get; private set; }

    public int Attack { get; private set; }
    public HealthPart Health { get; private set; }

    private int givenStartHealth;

    public CreatureCardPart(CreatureType cType, CreatureClass cClass, int attack, int health)
    {
        CreatureType = cType;
        CreatureClass = cClass;
        givenStartHealth = health;
    }

    protected override void OnAdd()
    {
        Health = Parent.AddEntityPart<HealthPart>(givenStartHealth);
    }

    protected override void OnRemove()
    {
        Parent.RemoveEntityPart(Health);
    }
}

public enum CreatureType
{
    Vampire,
    Human,
    Orc,
    Player
}

public enum CreatureClass
{
    None,
    Warrior,
    Clerc
}

public enum CreatureStatus
{
    None,
    Attacking,
    Defending
}
