using UnityEngine;
using System.Collections;

public class TriggerSpider : Triggerable {

    SpiderControl spiderControl;
    AudioSource soundSource;
    public AudioClip jumpSound;

	// Use this for initialization
	void Start () {
        spiderControl = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
        soundSource = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
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
