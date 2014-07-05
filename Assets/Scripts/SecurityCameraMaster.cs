using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCameraMaster : Triggerable 
{
    public GameObject playerRef;

    public List<SecurityCamera> secCams;

    public Vector3 playerSeenAt;
    public float infoAge;
    public float forgetPosIn;

    public bool playerInCollider;

    public bool camerasReactToPlayer;

	// Use this for initialization
	void Start () 
    {
        playerRef = FindObjectOfType<Player>().gameObject;

        foreach (SecurityCamera secCam in secCams)
        {
            secCam.secCamMaster = this;
        }

        playerSeenAt = Vector3.zero;

        triggerOnce = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (playerSeenAt != Vector3.zero)
        {
            infoAge += Time.deltaTime;

            if (infoAge > forgetPosIn)
            {
                playerSeenAt = Vector3.zero;
                infoAge = 0;
            }
        }
	}

    public void secCamOnAlert(Vector3 playerPos)
    {
        playerSeenAt = playerPos;

        infoAge = 0;
    }

    public override void trigger()
    {
        if (!triggered)
        {
            triggered = true;
            camerasReactToPlayer = true;

            foreach (SecurityCamera sc in secCams)
            {
                sc.lookAtPlayer();
                sc.checkPlayerVisibility();
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            foreach(SecurityCamera current in secCams)
            {
                current.playerInCollider = true;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            foreach (SecurityCamera current in secCams)
            {
                current.playerInCollider = false;
                current.playerVisible = false;
            }
        }
    }
}
