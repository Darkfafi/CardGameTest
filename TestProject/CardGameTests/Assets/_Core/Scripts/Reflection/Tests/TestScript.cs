using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript
{
    public float Test;

    [Inject]
    private BaseWeapon weapon;



    public void DebugWeapon()
    {
        Debug.Log(weapon);
    }
}

public abstract class BaseWeapon
{

}

public class Sword : BaseWeapon
{

}

