using UnityEngine;
using System.Collections;

public class TriggerSpider : Triggerable {

    private bool started = false;

    SpiderControl spiderControl;
    AudioSource soundSource;
    public AudioClip jumpSound;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!started)
        {
            spiderControl = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
            soundSource = this.gameObject.GetComponent<AudioSource>();
            started = true;
        }
	}

    public override void trigger()
    {
        if (!triggered)
        {
            triggered = true;

            soundSource.clip = jumpSound;
            soundSource.loop = false;
            soundSource.Play();

            spiderControl.WaitingOnCheckpoint = false;
        }
    }
}
