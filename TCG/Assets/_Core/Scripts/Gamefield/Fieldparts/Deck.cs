using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Deck
{
    public enum CardGrabType
    {
        Top,
        Bottom,
        Random
    }

    public BaseCard[] Cards { get { return cards.ToArray(); } }
    private List<BaseCard> cards;

    public Deck(BaseCard[] cardsForDeck)
    {
        cards = new List<BaseCard>(cardsForDeck);
    }

    public void Shuffel()
    {
        if (cards.Count <= 1) { return; }
        BaseCard[] randomPlacedCards = new BaseCard[] { };
        
        while(cards.Count > 0)
        {

        }
    }

    public BaseCard DrawCard(CardGrabType grabType = CardGrabType.Top)
    {
        int index = GetCardGrabIndex(grabType);
        return DrawCard(index);
    }
    
    public BaseCard DrawCard(int indexCard)
    {
        if (indexCard >= 0 && indexCard < cards.Count)
        {
            BaseCard card = cards[indexCard];
            RemoveCard(indexCard);
            return card;
        }

        return null;
    }

    public void RemoveCard(CardGrabType grabType = CardGrabType.Top)
    {
        int index = GetCardGrabIndex(grabType);
        RemoveCard(index);
    }

    public void RemoveCard(int indexCard)
    {
        if (indexCard >= 0 && indexCard < cards.Count)
        {
            cards.RemoveAt(indexCard);
        }
    }

    public int GetCardGrabIndex(CardGrabType grabType)
    {
        if (cards.Count == 0) { return -1; }
        switch (grabType)
        {
            case CardGrabType.Top:
                return cards.Count - 1;
            case CardGrabType.Bottom:
                return 0;
            case CardGrabType.Random:
                return UnityEngine.Random.Range(0, cards.Count);
            default:
                return -1;
        }
    }
}
