using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class EnemyColliderInfo : MonoBehaviour
{
    public SpiderControl control;

    private bool started = false;

    public void Start()
    {
    }

    public void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.tag.Equals("Player"))
            control.generalBehaviour.collisionWithObject = true;
    }

    void FixedUpdate()
    {
        control = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
        started = true;
    }
}

