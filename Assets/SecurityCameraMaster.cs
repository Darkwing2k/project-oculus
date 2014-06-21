using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCameraMaster : MonoBehaviour 
{
    public List<SecurityCamera> secCams;

    public Vector3 playerSeenAt;
    public float infoAge;
    public float forgetPosIn;

    private bool registered;

	// Use this for initialization
	void Start () 
    {
        secCams = new List<SecurityCamera>(FindObjectsOfType<SecurityCamera>());

        foreach (SecurityCamera secCam in secCams)
        {
            secCam.secCamMaster = this;
        }

        playerSeenAt = Vector3.zero;
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
}
