using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserDisplayView : MonoBehaviour, IUserDisplay
{
    [SerializeField]
    private Text nameText;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Text attackText;

    [SerializeField]
    private Text dmgIndicatorText; // Hurt pop-up text

    public void UpdateUserInfo(UserInfo info)
    {
        new DisplayAction((completeStatus) =>
        {
            nameText.text = info.Name;
            healthText.text = "HP: " + info.Health.ToString();
            attackText.text = "Power: " + info.Power.ToString();
        });
    }

    public void DisplayDamageEffect(int amount)
    {
        if (amount == 0) { return; }

        new DisplayAction((completeStatus) =>
        {
            completeStatus.SetCompletePermission(DisplayComplete.CompleteSetType.SelfSet);
            dmgIndicatorText.gameObject.SetActive(true);
            dmgIndicatorText.color = Color.red;
            dmgIndicatorText.text = "-" + amount.ToString();
            dmgIndicatorText.transform.DOShakePosition(0.5f).OnComplete(() =>
            {
                dmgIndicatorText.gameObject.SetActive(false);
                completeStatus.SetCompleteStatus(true);
            });
        });
    }

    public void DisplayHealingEffect(int amount)
    {
        if (amount == 0) { return; }

        new DisplayAction((completeStatus) =>
        {
            completeStatus.SetCompletePermission(DisplayComplete.CompleteSetType.SelfSet);
            dmgIndicatorText.gameObject.SetActive(true);
            dmgIndicatorText.color = Color.green;
            dmgIndicatorText.text = "+" + amount.ToString();
            dmgIndicatorText.transform.DOShakePosition(0.5f).OnComplete(() =>
            {
                dmgIndicatorText.gameObject.SetActive(false);
                completeStatus.SetCompleteStatus(true);
            });
        });
    }

    public void DisplayAttackEffect(int attackAmount)
    {
        Debug.Log("Attack Effect!");
    }
}
