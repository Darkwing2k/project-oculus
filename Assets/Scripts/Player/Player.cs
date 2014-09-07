﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
    public bool useOculus;
    public bool useGamepad;

    public Camera[] normalCameras;
    public OVRCameraController vrCamera;

    public Transform eyeCenter;

    public bool noClip;

	// Use this for initialization
	void Start () 
    {
        noClip = false;

        if (useOculus)
        {
            vrCamera.gameObject.SetActive(true);

            foreach (Camera c in normalCameras)
            {
                c.gameObject.SetActive(false);
            }
        }
        else
        {
            vrCamera.gameObject.SetActive(false);

            foreach (Camera c in normalCameras)
            {
                c.gameObject.SetActive(true);
            }
        }
	}

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Space))
        {
            noClip = !noClip;
            this.gameObject.collider.enabled = !noClip;
        }
	}
}