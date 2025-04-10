﻿using System;
using UnityEngine;

public class Instantiate : StateMachineBehaviour
{

    public GameObject original;
    public Vector3 offset;
    [Range(0f, 1f)]
    public float normalizedTime;

    private bool restart = true;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        double x = stateInfo.normalizedTime - Math.Truncate(stateInfo.normalizedTime);
        if (x < normalizedTime)
        {
            restart = true;
        }

        if (restart && x > normalizedTime)
        {
            Instantiate(original, animator.transform.position +  offset.z*animator.transform.forward+ offset.y * animator.transform.up+ offset.x * animator.transform.right, Quaternion.identity);
            restart = false;
        }
    }

}
