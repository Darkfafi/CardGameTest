using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserModel : IUser
{
    private Action<int> callbacksDamage;
    private Action<int> callbacksHealing;
    private Action<UserInfo> callbacksInfoChange;

    public UserInfo UserInfo
    {
        get; private set;
    }

    public UserModel(string name, int health, int attack)
    {
        Initialize(name, health, attack);
    }

    public UserModel(UserDesign design)
    {
        Initialize(design.Name, design.Health, design.Attack);
    }

    public void GetHealed(int amount)
    {
        amount = Mathf.Clamp(amount, 0, UserInfo.MaxHealth - UserInfo.Health);
        SetHealth(UserInfo.Health + amount);

        if (callbacksHealing != null)
            callbacksHealing(amount);
    }

    public void GetDamage(int amount)
    {
        amount = Mathf.Clamp(amount, 0, UserInfo.Health);
        SetHealth(UserInfo.Health - amount);

        if (callbacksDamage != null)
            callbacksDamage(amount);
    }

    public void HealUser(IUser target)
    {
        target.GetHealed(UserInfo.Power);
    }

    public void HealSelf()
    {
        GetHealed(UserInfo.Power);
    }

    public void AttackUser(IUser target)
    {
        target.GetDamage(UserInfo.Power);
    }

    private void Initialize(string name, int health, int attack)
    {
        UserInfo = new UserInfo(name, health, attack);
    }

    private void SetHealth(int value)
    {
        UserInfo ui = UserInfo;
        ui.Health = value;
        UserInfo = ui;

        if (callbacksInfoChange != null)
            callbacksInfoChange(UserInfo);
    }

    private void SetName(string value)
    {
        UserInfo ui = UserInfo;
        ui.Name = value;
        UserInfo = ui;

        if (callbacksInfoChange != null)
            callbacksInfoChange(UserInfo);
    }

    private void SetAttack(int value)
    {
        UserInfo ui = UserInfo;
        ui.Power = value;
        UserInfo = ui;

        if (callbacksInfoChange != null)
            callbacksInfoChange(UserInfo);
    }

    // Callbacks

    public void RegisterToDamageEvent(Action<int> callback)
    {
        callbacksDamage += callback;
    }

    public void RegisterToHealingEvent(Action<int> callback)
    {
        callbacksHealing += callback;
    }

    public void RegisterToInfoChangeEvent(Action<UserInfo> callback)
    {
        callbacksInfoChange += callback;
    }


    public void UnRegisterToDamageEvent(Action<int> callback)
    {
        callbacksDamage -= callback;
    }

    public void UnRegisterToHealingEvent(Action<int> callback)
    {
        callbacksHealing -= callback;
    }

    public void UnRegisterToInfoChangeEvent(Action<UserInfo> callback)
    {
        callbacksInfoChange -= callback;
    }
}

public interface IUser
{
    UserInfo UserInfo { get; }

    void GetDamage(int amount);
    void GetHealed(int amount);
    void AttackUser(IUser target);
    void HealUser(IUser target);
    void HealSelf();

    void RegisterToDamageEvent(Action<int> callback);
    void RegisterToHealingEvent(Action<int> callback);
    void RegisterToInfoChangeEvent(Action<UserInfo> callback);

    void UnRegisterToDamageEvent(Action<int> callback);
    void UnRegisterToHealingEvent(Action<int> callback);
    void UnRegisterToInfoChangeEvent(Action<UserInfo> callback);
}

public struct UserInfo
{
    public string Name;
    public int MaxHealth;
    public int Health;
    public int Power;

    public UserInfo(string name, int health, int attack)
    {
        Name = name;
        Power = attack;
        MaxHealth = health;
        Health = health;
    }
}

