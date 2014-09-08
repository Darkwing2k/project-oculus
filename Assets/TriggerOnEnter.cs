using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerOnEnter : MonoBehaviour 
{
    public List<Triggerable> toTrigger;
    public bool triggerOnce;
    public bool triggered;

    public AudioSource audio;

    void Start()
    {
        audio = this.gameObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter()
    {
        if (!triggerOnce || !triggered)
        {

            foreach (Triggerable current in toTrigger)
            {
                current.trigger();

                if (audio != null)
                {
                    audio.Play();
                }
            }
            triggered = true;
        }
    }
}
