using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
    public bool useOculus;
    public bool useGamepad;

    public CrosshairBehaviour buttonInfo;

    [System.Obsolete("Only for display in Editor, use LiftModeEnabled instead")]
    public bool liftModeActive; 
    public bool LiftModeEnabled
    {
        set { liftModeActive = value; }
        get { return liftModeActive; }
    }
    public List<LiftModeTarget> liftables;
    public LiftMode liftModeScript;

    [System.Obsolete("Read Only. Use LaserModeEnabled instead")]
    public bool laserModeActiv;
    public bool LaserModeEnabled
    {
        set
        { 
            laserModeActiv = value;
            FlightControlEnabled = !value;
            laserModeScript.changeStateTo(value ? LaserMode.LaserState.TARGETING : LaserMode.LaserState.IDLE);
        }
        get { return laserModeActiv; }
    }
    public List<LaserModeTarget> cutables;
    public LaserMode laserModeScript;

    [System.Obsolete("Read Only. Use FlightControlEnabled instead")]
    public bool flightControlActive;
    public bool FlightControlEnabled
    {
        set { flightControlActive = value; }
        get { return flightControlActive; }
    }
    public FlightControl2 flightControlScript;

    public Camera[] normalCameras;
    public OVRCameraController vrCamera;

    public Transform eyeCenter;

    public bool noClip;

	// Use this for initialization
	void Start () 
    {
        FlightControlEnabled = true;
        LaserModeEnabled = false;
        LiftModeEnabled = false;

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
        if (Input.GetButtonDown("Use"))
        {
            bool inputHandled = false;

            if (!inputHandled && cutables.Count != 0 && !LiftModeEnabled && !LaserModeEnabled)
            {
                foreach (LaserModeTarget t in cutables)
                {
                    RaycastHit hit;
                    Physics.Raycast(this.transform.position, t.transform.position - this.transform.position, out hit);

                    //print("checking visibility");

                    if (hit.collider.gameObject.GetInstanceID() == t.transform.parent.gameObject.GetInstanceID())
                    {
                        //print("starting lasermode");
                        this.LaserModeEnabled = true;
                    }
                }

                inputHandled = true;
            }

            if (!inputHandled && LaserModeEnabled)
            {
                this.LaserModeEnabled = false;
                inputHandled = true;
            }

            if (!inputHandled && liftables.Count != 0 && !LiftModeEnabled && !LaserModeEnabled)
            {
                LiftModeTarget liftable = liftModeScript.shortestDistanceTo(liftables);

                if (liftable != null)
                {
                    liftModeScript.attachObject(liftable.transform.parent.gameObject);
                    LiftModeEnabled = true;
                    inputHandled = true;
                }
            }

            if (!inputHandled && LiftModeEnabled)
            {
                liftModeScript.releaseObject();
                LiftModeEnabled = false;
                inputHandled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button4) || Input.GetKeyDown(KeyCode.Space))
        {
            noClip = !noClip;
            this.gameObject.collider.enabled = !noClip;
        }
	}

    public void registerLiftable(LiftModeTarget u)
    {
        print("added liftable");

        liftables.Add(u);
    }

    public void removeLiftable(LiftModeTarget u)
    {
        print("removed liftable");

        liftables.Remove(u);
    }

    public void registerCutable(LaserModeTarget u)
    {
        print("added cutable");

        cutables.Add(u);
    }

    public void removeCutable(LaserModeTarget u)
    {
        print("removed cutable");

        cutables.Remove(u);
    }
}
