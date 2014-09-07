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

    public bool shortEvent;
    public float timer;

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

        if (triggered && shortEvent)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                camerasReactToPlayer = false;
                triggered = false;
                shortEvent = false;

                foreach (SecurityCamera currentCam in secCams)
                {
                    currentCam.alarmed = false;
                    currentCam.playerInCollider = false;
                    currentCam.playerVisible = false;

                    currentCam.changeStateTo(SecurityCamera.ActionState.RETURN);
                    currentCam.camLight.color = currentCam.normalColor;
                }
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
