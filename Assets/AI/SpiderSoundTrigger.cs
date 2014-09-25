using UnityEngine;
using System.Collections;

public class SpiderSoundTrigger : Triggerable {

    public AudioClip walkCycleAudioClip;

    public GameObject spiderShell;

    public AudioSource audioSource;
    private AudioSource walkSoundSource;

    private float timer;
    public float animDuration;
    private bool animEnabled;
    private bool initialized;

    void Start()
    {
        timer = 0.0f;
        animEnabled = false;
        initialized = false;
        walkSoundSource = spiderShell.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider pCollider)
    {
        if (pCollider.gameObject.tag.Equals("Player") && triggered)
        {
            animEnabled = true;
        }
    }

    void Update()
    {
        if (animEnabled)
        {
            if (!initialized)
            {
                initEvent();
                initialized = true;
            }

            animateSpider();

            timer += Time.deltaTime;
            if (timer >= animDuration)
            {
                endEvent();
            }
        }
    }

    private void animateSpider()
    {
        spiderShell.GetComponent<Animation>().Play();
        if (!walkSoundSource.isPlaying)
            walkSoundSource.Play();
    }

    private void initEvent()
    {
        spiderShell.SetActive(true);
        audioSource.gameObject.SetActive(true);

        audioSource.Play();

        Vector3 targetVel = spiderShell.transform.forward.normalized * 2.0f;
        spiderShell.rigidbody.velocity = targetVel;
    }

    private void endEvent()
    {
        animEnabled = false;
        spiderShell.SetActive(false);
        audioSource.gameObject.SetActive(false);
    }

    public override void trigger()
    {
        if (!triggered)
        {
            triggered = true;
        }
    }

    
}
