using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEntity : BaseEntity
{
    public CardEntity()
    {
        AddEntityPart<GenericCardPart>(CardType.Creature);
    }
}
