using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class VisualCardRotation : MonoBehaviour {

    [SerializeField]
    private GameObject frontSide;

    [SerializeField]
    private GameObject backSide;

    protected void Update()
    {
        Vector3 a = (Camera.main.transform.position - transform.position);
        Vector3 b = -transform.forward;
        float rotAngle = (Vector3.Dot(a, b) / (a.magnitude * b.magnitude)) * Mathf.Rad2Deg;
        bool back = rotAngle < 0;
        frontSide.SetActive(!back);
        backSide.SetActive(back);
    }
}
