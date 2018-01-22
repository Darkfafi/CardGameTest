using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController
{
    private IUserDisplay display;
    private IUser user;

    public UserController(IUser user, IUserDisplay display)
    {
        this.display = display;
        SetUser(user);
    }

    public void CleanController()
    {
        UnregisterFromEvents(this.user);
    }

    private void SetUser(IUser user)
    {
        if(this.user != null)
        {
            UnregisterFromEvents(this.user);
        }

        this.user = user;

        RegisterToEvents(this.user);
        display.UpdateUserInfo(this.user.UserInfo);
    }

    private void RegisterToEvents(IUser user)
    {
        user.RegisterToInfoChangeEvent(OnUpdatedInfo);
        user.RegisterToDamageEvent(OnUserDamaged);
        user.RegisterToHealingEvent(OnUserHealed);
    }

    private void UnregisterFromEvents(IUser user)
    {
        user.UnRegisterToInfoChangeEvent(OnUpdatedInfo);
        user.UnRegisterToDamageEvent(OnUserDamaged);
        user.UnRegisterToHealingEvent(OnUserHealed);
    }

    private void OnUserHealed(int amount)
    {
        display.DisplayHealingEffect(amount);
    }

    private void OnUpdatedInfo(UserInfo info)
    {
        display.UpdateUserInfo(info);
    }

    private void OnUserDamaged(int dmg)
    {
        display.DisplayDamageEffect(dmg);
    }
}

public interface IUserDisplay
{
    void UpdateUserInfo(UserInfo info);
    void DisplayDamageEffect(int dmgAmount);
    void DisplayHealingEffect(int attackAmount);
}

