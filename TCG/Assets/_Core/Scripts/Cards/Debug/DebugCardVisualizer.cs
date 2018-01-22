using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DebugCardVisualizer : MonoBehaviour {

    [SerializeField]
    private CardView cardView;

    [SerializeField]
    private BaseCardAsset asset;

    [SerializeField]
    private float rotationDuration = 0;
    private float preRotDuration = 0;

    private bool spinning = false;

    protected void Awake()
    {
        BaseCard card = CreateCardFor(asset);
        cardView.DisplayCard(card);
        preRotDuration = rotationDuration;
        Rot();
    }

    protected void Update()
    {
        if(!spinning)
        {
            if(rotationDuration > 0 && preRotDuration <= 0)
            {
                Rot();
            }
        }

        preRotDuration = rotationDuration;
    }

    private void Rot()
    {
        if (rotationDuration <= 0) { return; }
        spinning = true;
        cardView.transform.DORotate(new Vector3(0, 180, 0), rotationDuration).SetEase(Ease.OutBack).OnComplete(() => {
            cardView.transform.DORotate(new Vector3(0, 180, 0), rotationDuration, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack).OnComplete(() => {
                spinning = false;
                Rot();
            });
        });
    }

    private BaseCard CreateCardFor(BaseCardAsset asset)
    {
        switch (asset.CardType)
        {
            case CardType.Creature:
                return new CreatureCard((CreatureCardAsset)asset, null);
            case CardType.Spell:
                return new SpellCard((SpellCardAsset)asset, null);
            default:
                return null;
        }

    }
}
