using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class ChildrenPositionSorter : MonoBehaviour
{
    public Transform LeftChild { get { return (registeredChildrenXSorted.Length > 0) ? registeredChildrenXSorted[0] : null; } }
    public Transform RightChild { get { return (registeredChildrenXSorted.Length > 0) ? registeredChildrenXSorted[registeredChildrenXSorted.Length - 1] : null; } }

    public Transform TopChild { get { return (registeredChildrenYSorted.Length > 0) ? registeredChildrenYSorted[registeredChildrenYSorted.Length - 1] : null; } }
    public Transform BottomChild { get { return (registeredChildrenYSorted.Length > 0) ? registeredChildrenYSorted[0] : null; } }

    public float SpacingX { get; private set; }
    public float SpacingY { get; private set; }
    public float DistX { get; private set; }
    public float DistY { get; private set; }

    private int preLength = 0;

    [Header("Options")]
    public bool RunAsUpdate = false;
    public bool SortXAxis = true;
    public bool SortYAxis = true;

    [Header("Optimization")]
    [SerializeField]
    private bool dynamicChildren = true;

    [Header("Editor Tools")]
    [SerializeField]
    private bool runMethodButton = false;

    private Transform[] registeredChildren = null;
    private Transform[] registeredChildrenXSorted = null;
    private Transform[] registeredChildrenYSorted = null;

    protected void LateUpdate()
    {
        if(runMethodButton)
        {
            runMethodButton = false;
            SortChildren();
        }
        else if(RunAsUpdate)
        {
            SortChildren();
        }
    }

    private void SortChildren()
    {
        if(registeredChildren == null || dynamicChildren)
        {
            SetChildren();
        }

        if(SortXAxis)
        {
            if(registeredChildrenXSorted.Length > 2)
            {
                float nDist = Mathf.Abs(RightChild.transform.position.x - LeftChild.transform.position.x);
                if (preLength != registeredChildren.Length || nDist != DistX)
                {
                    DistX = nDist;
                    SpacingX = DistX / (registeredChildrenXSorted.Length - 1);

                    for (int i = 1; i <= registeredChildrenXSorted.Length - 2; i++)
                    {
                        Vector3 pos = registeredChildrenXSorted[i].transform.position;
                        pos.x = registeredChildrenXSorted[0].transform.position.x + SpacingX * i;
                        registeredChildrenXSorted[i].transform.position = pos;
                    }
                }
            }
        }

        if (SortYAxis)
        {
            if (registeredChildrenYSorted.Length > 2)
            {
                float nDist = Mathf.Abs(TopChild.transform.position.y - BottomChild.transform.position.y);
                if (preLength != registeredChildren.Length || nDist != DistY)
                {
                    DistY = Mathf.Abs(TopChild.transform.position.y - BottomChild.transform.position.y);
                    SpacingY = DistY / (registeredChildrenYSorted.Length - 1);

                    for (int i = 1; i <= registeredChildrenYSorted.Length - 2; i++)
                    {
                        Vector3 pos = registeredChildrenYSorted[i].transform.position;
                        pos.y = registeredChildrenYSorted[0].transform.position.y + SpacingY * i;
                        registeredChildrenYSorted[i].transform.position = pos;
                    }
                }
            }
        }

        preLength = registeredChildren.Length;
    }

    private void SetChildren()
    {
        Transform[] t = new Transform[gameObject.transform.childCount];

        for(int i = 0; i < t.Length; i++)
        {
            t[i] = gameObject.transform.GetChild(i);
        }

        registeredChildren = t;
        registeredChildrenXSorted = registeredChildren.OrderBy((child => child.transform.position.x)).ToArray();
        registeredChildrenYSorted = registeredChildrenXSorted.OrderBy((child => child.transform.position.y)).ToArray();
    }
}
