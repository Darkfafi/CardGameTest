using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaster : MasterComponent
{

	protected void Awake()
    {
        TestScript ts = new TestScript();
        Link(ts);
        Inject(new Sword());
        ts.DebugWeapon();
    }
}
