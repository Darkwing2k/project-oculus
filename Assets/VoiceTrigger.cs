using UnityEngine;
using System.Collections;

public class VoiceTrigger : Triggerable
{
    public bool onEnter;
    public AudioClip clipToPlay;
    public Player playerRef;

    public bool started;
    public bool played;

    public float delay;

	// Use this for initialization
	void Start () 
    {
        triggerOnce = true;
        playerRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (started && !played)
        {
            delay -= Time.deltaTime;

            if (delay <= 0)
            {
                playerRef.voiceSource.clip = clipToPlay;
                playerRef.voiceSource.Play();
                played = true;
            }
        }
	}

    public override void trigger()
    {
        if (!triggered)
        {
            if (!onEnter)
            {
                started = true;
                triggered = true;
            
            }
            else
            {
                this.gameObject.collider.enabled = true;

                triggered = true;
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (onEnter && !played)
        {
            if (playerRef.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                started = true;
            }
        }
    }
}
