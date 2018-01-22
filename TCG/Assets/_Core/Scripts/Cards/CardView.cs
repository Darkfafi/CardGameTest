using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text costText;

    [SerializeField]
    private Text descriptionText;

    [SerializeField]
    private Text typeText;

    [SerializeField]
    private Image artImage;

    [SerializeField]
    private GameObject attackSectionWrapper;

    [SerializeField]
    private GameObject healthSectionWrapper;

    [SerializeField]
    private Text attackText;

    [SerializeField]
    private Text healthText;

    public void DisplayCard(CreatureCard card)
    {
        InternalDisplayCard(card);
        attackSectionWrapper.SetActive(true);
        healthSectionWrapper.SetActive(true);
        SetAttackDisplay(card.AttackAmount);
        SetHealthDisplay(card.HealthAmount);
    }

    public void DisplayCard(SpellCard card)
    {
        InternalDisplayCard(card);
        attackSectionWrapper.SetActive(false);
        healthSectionWrapper.SetActive(false);
    }

    public void DisplayCard(BaseCard card)
    {
        switch(card.CardType)
        {
            case CardType.Creature:
                DisplayCard((CreatureCard)card);
                break;
            case CardType.Spell:
                DisplayCard((SpellCard)card);
                break;
            default:
                Debug.LogError("No Card Display Set For Type: " + card.CardType);
                break;
        }
    }

    public void SetCostDisplay(int value)
    {
        costText.text = value.ToString();
    }

    public void SetAttackDisplay(int value)
    {
        attackText.text = value.ToString();
    }

    public void SetHealthDisplay(int value)
    {
        healthText.text = value.ToString();
    }

    protected void InternalDisplayCard(BaseCard baseCard)
    {
        nameText.text = baseCard.CardName;
        SetCostDisplay(baseCard.CardCost);
        descriptionText.text = baseCard.CardDescription;
        artImage.sprite = baseCard.CardArt;
        typeText.text = baseCard.CardType.ToString();
    }
}
