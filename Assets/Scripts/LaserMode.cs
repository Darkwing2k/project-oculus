using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserMode : MonoBehaviour 
{
    public Player playerRef;

    public LaserModeTarget currentTarget;

    private AudioSource laserSoundSrc;
    public AudioClip laserSound;

    private float timer = 0;
    private float timerMax = 2;

    public enum LaserState {IDLE, TARGETING, LASERING};
    public LaserState currentState;
    public LaserState desiredState;
    private LineRenderer laser;

    // === Targeting values
    public enum TargetingMethods { LIGHT, CROSSHAIR};
    public TargetingMethods targetingSystem;

    private GameObject lightTargeter;
    private bool laserCanHitTarget;
    public Vector3 laserPos;

    private GameObject crosshairGUIObject;

    // === Debug Variables when Oculus is not connected ===
    private LineRenderer pointer;
    private Ray laserTargeter;
    private Ray lookingDirection;
    private float alpha = 0;
    private float beta = 0;

	// Use this for initialization
	void Start ()
    {
        playerRef = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        // === Targeting State ===
        if (currentState == LaserState.TARGETING)
        {
            // Raycast in player´s look direction to define a target
            RaycastHit hit;

            if (playerRef.useOculus)
            {
                Ray playerLook = new Ray();
                playerLook.origin = playerRef.eyeCenter.position;
                playerLook.direction = playerRef.eyeCenter.forward;

                Physics.Raycast(playerLook, out hit, 20);

                switch (targetingSystem)
                {
                    case TargetingMethods.LIGHT:
                        {
                            break;
                        }
                    case TargetingMethods.CROSSHAIR:
                        {
                            // Raycast is also needed to correctly position the crosshair
                            crosshairGUIObject.GetComponent<CrosshairBehaviour>().crosshairAdaptDistance(hit);

                            break;
                        }
                }

            }
            else
            {
                // ======== For non-Oculus mode ===========================================
                float rStickH = Input.GetAxis("RStickH");
                float rStickV = Input.GetAxis("RStickV");

                if (Mathf.Abs(rStickH) > 0.3f)
                {
                    alpha = alpha + (rStickH * 1f);
                }

                if (Mathf.Abs(rStickV) > 0.3f)
                {
                    beta = beta + (rStickV * 1f);

                    if (beta < 0)
                        beta = 0;

                    if (beta > 180)
                        beta = 180;
                }

                float radAlpha = alpha * Mathf.Deg2Rad;
                float radBeta = beta * Mathf.Deg2Rad;

                laserTargeter.direction = new Vector3(Mathf.Sin(radAlpha) * Mathf.Sin(radBeta), Mathf.Cos(radBeta), Mathf.Cos(radAlpha) * Mathf.Sin(radBeta));

                Physics.Raycast(laserTargeter, out hit, 10);

                pointer.SetPosition(1, hit.point);

                Debug.DrawRay(laserTargeter.origin, laserTargeter.direction, Color.cyan);
                // =========================================================================
            }

            // check if the raycast hit any of the possible targets around
            foreach (LaserModeTarget target in playerRef.cutables)
            {
                if (hit.collider.gameObject.GetInstanceID() == target.transform.parent.gameObject.GetInstanceID())
                {
                    currentTarget = target;
                    desiredState = LaserState.LASERING;
                    break;
                    
                }
            }
        }

        // === Lasering State ===
        if (currentState == LaserState.LASERING)
        {
            if (playerRef.useOculus)
            {
                switch (targetingSystem)
                {
                    case TargetingMethods.LIGHT:
                        {
                            // lightTargeter shall stay on the current target
                            lightTargeter.transform.LookAt(currentTarget.transform.position);
                            break;
                        }
                    case TargetingMethods.CROSSHAIR:
                        {
                            break;
                        }
                }
            }

            // when button is pressed, the laser is shown and the target loses duration
            if (Input.GetButton("Action"))
            {
                laser.enabled = true;

                if (laserCanHitTarget)
                    timer += Time.deltaTime;
            }
            else
            {
                laser.enabled = false;
            }

            // if the duration max is reached, the target is cut and the state will be changed to TARGETING
            if (timer > timerMax)
            {
                currentTarget.GetComponent<LaserModeTarget>().Cut();
                playerRef.removeCutable(currentTarget);

                desiredState = LaserState.TARGETING;
                
                currentTarget = null;
            }
        }

        // if no cutable is around, lasermode will be disabled
        // this happens in 2 cases: 1. all targets were cut 2. player flew through a cutable trigger and left it while in lasermode due to inertia
        if (playerRef.cutables.Count == 0)
        {
            playerRef.LaserModeEnabled = false;
        }

        changeStateTo(desiredState);
	}

    public void changeStateTo(LaserState targetState)
    {
        //some statemachine rules:

        // CURRENT -> TARGET     |  what happened             |  perform action
        //----------------------------------------------------------------------------------------------
        // IDLE -> TARGETING     |  player uses cutable       |  create crosshair
        // IDLE -> LASERING      |  no transition!            |  -
        //                       |                            |
        // TARGETING -> IDLE     |  player aborted            |  destroy crosshair
        // TARGETING -> LASERING |  player acquired a target  |  create laser
        //                       |                            |
        // LASERING -> IDLE      |  player aborted or all cut |  destroy crosshair and laser
        // LASERING -> TARGETING |  player lasered one of few |  destroy crosshair and laser (crosshair needs reset, so destroy here and create new on enter TARGETING)

        if (currentState != targetState)
        {

            // === On-Exit Operations ======================================================================================
            switch (currentState)
            {
                case LaserState.IDLE:
                    {

                        break;
                    }
                case LaserState.TARGETING:
                    {

                        break;
                    }
                case LaserState.LASERING:
                    {
                        //when leaving lasering state, destroy laser and crosshair

                        Destroy(laser.gameObject);

                        if (playerRef.useOculus)
                        {
                            switch (targetingSystem)
                            {
                                case TargetingMethods.LIGHT:
                                    {
                                        Destroy(lightTargeter);

                                        break;
                                    }
                                case TargetingMethods.CROSSHAIR:
                                    {
                                        Destroy(crosshairGUIObject);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            Destroy(pointer);
                        }

                        break;
                    }
            }
            // =============================================================================================================

            // === On-Enter Operations ======================================================================================
            switch (targetState)
            {
                case LaserState.IDLE:
                    {
                        //when entering idle state, destroy crosshair

                        if (playerRef.useOculus)
                        {
                            switch (targetingSystem)
                            {
                                case TargetingMethods.LIGHT:
                                    {
                                        Destroy(lightTargeter);

                                        break;
                                    }
                                case TargetingMethods.CROSSHAIR:
                                    {
                                        Destroy(crosshairGUIObject);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            Destroy(pointer);
                        }

                        break;
                    }
                case LaserState.TARGETING:
                    {
                        //when entering targeting state, create crosshair

                        if (playerRef.useOculus)
                        {
                            switch (targetingSystem)
                            {
                                case TargetingMethods.LIGHT:
                                    {
                                        lightTargeter = GameObject.Instantiate(Resources.Load("LightTargeter")) as GameObject;
                                        lightTargeter.transform.position = playerRef.eyeCenter.position;
                                        lightTargeter.transform.rotation = playerRef.eyeCenter.rotation;
                                        lightTargeter.transform.parent = playerRef.eyeCenter;

                                        break;
                                    }
                                case TargetingMethods.CROSSHAIR:
                                    {
                                        crosshairGUIObject = GameObject.Instantiate(Resources.Load("CrosshairGUIObject")) as GameObject;
                                        crosshairGUIObject.GetComponent<CrosshairBehaviour>().playerRef = playerRef;

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            lookingDirection = laserTargeter = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                            alpha = Vector3.Angle(new Vector3(0, 0, 1), laserTargeter.direction);
                            beta = Vector3.Angle(new Vector3(0, 1, 0), laserTargeter.direction);

                            lookingDirection.direction.Normalize();
                            laserTargeter.direction.Normalize();

                            pointer = this.gameObject.AddComponent<LineRenderer>();
                            pointer.SetPosition(0, playerRef.eyeCenter.position + new Vector3(0, -0.05f, 0));
                            pointer.SetWidth(0.005f, 0.005f);
                        }

                        break;
                    }
                case LaserState.LASERING:
                    {
                        //when entering lasering state, create laser

                        GameObject laserGO = GameObject.Instantiate(Resources.Load("Laser")) as GameObject;
                        laser = laserGO.GetComponent<LineRenderer>();
                        laserGO.transform.parent = playerRef.eyeCenter;

                        laserSoundSrc =  this.gameObject.AddComponent<AudioSource>();

                        laserSoundSrc.loop = false;
                        laserSoundSrc.clip = laserSound;

                        Vector3 laserStartPos = playerRef.eyeCenter.transform.position + laserPos;

                        Ray r = new Ray(laserStartPos, currentTarget.transform.position - laserStartPos);
                        RaycastHit hit;

                        if (Physics.Raycast(r, out hit, 10))
                        {
                            if (hit.collider.gameObject.GetInstanceID() == currentTarget.transform.parent.gameObject.GetInstanceID())
                                laserCanHitTarget = true;
                            else
                                laserCanHitTarget = false;
                        }

                        laser.SetPosition(0, laserStartPos);
                        laser.SetPosition(1, hit.point);

                        if (playerRef.useOculus)
                        {
                            switch (targetingSystem)
                            {
                                case TargetingMethods.LIGHT:
                                    {

                                        break;
                                    }
                                case TargetingMethods.CROSSHAIR:
                                    {
                                        crosshairGUIObject.GetComponent<CrosshairBehaviour>().snapToTarget(currentTarget.transform.position);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            pointer.SetPosition(1, currentTarget.transform.position);
                        }


                        timer = 0;
                        timerMax = currentTarget.GetComponent<LaserModeTarget>().duration;

                        break;
                    }
            }
            // =============================================================================================================

            // now switch states
            print("LaserMode state changed from " + currentState + " to " + targetState);
            desiredState = targetState;
            currentState = targetState;
        }
    }
}
