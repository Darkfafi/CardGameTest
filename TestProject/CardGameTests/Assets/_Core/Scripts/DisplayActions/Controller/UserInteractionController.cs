using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInteractionController
{
    private IUser user;
    private IUser targetUser;
    private IUserInteractionDisplay display;

    public UserInteractionController(IUser user, IUser targetUser, IUserInteractionDisplay display)
    {
        this.user = user;
        this.targetUser = targetUser;
        this.display = display;
        display.AttackClickedEvent += OnAttackClickedEvent;
        display.HealClickedEvent += OnHealClickedEvent;
    }

    public void CleanController()
    {
        display.AttackClickedEvent -= OnAttackClickedEvent;
        display.HealClickedEvent -= OnHealClickedEvent;
    }

    private void OnHealClickedEvent()
    {
        user.HealUser(targetUser);
    }

    private void OnAttackClickedEvent()
    {
        user.AttackUser(targetUser);
    }
}
