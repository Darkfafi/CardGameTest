using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCard
{
    public string CardName { get { return asset.name; } }
    public string CardDescription { get { return asset.CardDescription; } }
    public CardType CardType { get { return asset.CardType; } }
    public Sprite CardArt { get { return asset.CardArt; } }
    public BaseGamePlayer OwnerOfCard { get; private set; }

    public int CardCost { get; private set; }

    private BaseCardAsset asset;

    public BaseCard(BaseCardAsset asset, BaseGamePlayer cardOwnder)
    {
        this.asset = asset;
        CardCost = asset.CardManaCost;
        SetCardOwnder(cardOwnder);
    }

    public void SetCardOwnder(BaseGamePlayer newCardOwnder)
    {

    }
}
