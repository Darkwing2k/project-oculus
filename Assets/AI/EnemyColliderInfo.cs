using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.AI
{
    public class EnemyColliderInfo : MonoBehaviour
    {
        public SpiderControl control;

        public void Start()
        {
            control = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
        }

        public void OnCollisionEnter(Collision c)
        {
            if (!c.gameObject.tag.Equals("Player"))
                control.generalBehaviour.collisionWithObject = true;
        }
    }
}
