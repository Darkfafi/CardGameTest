using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInteractionView : MonoBehaviour, IUserInteractionDisplay
{
    public event Action AttackClickedEvent;
    public event Action HealClickedEvent;

    [SerializeField]
    private Button attackButton;

    [SerializeField]
    private Button healButton;

    protected void Awake()
    {
        attackButton.onClick.AddListener(OnClickedAttack);
        healButton.onClick.AddListener(OnClickedHealing);
    }

    protected void OnDestroy()
    {
        attackButton.onClick.RemoveListener(OnClickedAttack);
        healButton.onClick.RemoveListener(OnClickedHealing);
    }

    private void OnClickedHealing()
    {
        if (HealClickedEvent != null)
            HealClickedEvent();
    }

    private void OnClickedAttack()
    {
        if (AttackClickedEvent != null)
            AttackClickedEvent();
    }
}


public interface IUserInteractionDisplay
{
    event Action AttackClickedEvent;
    event Action HealClickedEvent;
}