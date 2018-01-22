using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    public CardEntity[] AllCards { get { return cards.ToArray(); } }
    private List<CardEntity> cards;

    public Deck(CardEntity[] cards)
    {
        this.cards = new List<CardEntity>(cards);
    }
	
    public CardEntity[] GetCards(IEntityFilter<CardEntity>[] filters)
    {
        return EntityFiltering.GetEntities(cards, filters);
    }
}
