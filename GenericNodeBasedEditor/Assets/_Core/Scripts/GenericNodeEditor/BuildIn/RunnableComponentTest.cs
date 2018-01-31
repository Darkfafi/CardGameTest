using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnableComponentTest : BaseNodeDataRunner<int>
{
    [SerializeField]
    private int value = 100;

    protected override void Awake()
    {
        base.Awake();
        Run(value);
    }
}
