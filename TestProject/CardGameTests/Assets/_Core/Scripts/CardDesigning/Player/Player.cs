using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseEntity
{
    public List<CardEntity> cardsInHand;
    public List<CardEntity> cardsInDeck;

    public Player()
    {
        AddEntityPart<HealthPart>(20);
    }
}
