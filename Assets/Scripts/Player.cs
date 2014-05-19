using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public bool useOculus;
    public bool useGamepad;

    public bool liftModeActive;
    public bool laserModeActive;
    public bool flightControlActive;

    public Camera[] normalCameras;
    public OVRCameraController vrCamera;

    public Transform eyeCenter;

	// Use this for initialization
	void Start () 
    {
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
	
	}
}
