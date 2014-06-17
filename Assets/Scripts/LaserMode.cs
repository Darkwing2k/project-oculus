using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserMode : MonoBehaviour 
{
    public Player playerRef;

    public LaserModeTarget currentTarget;

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

	}
	
	// Update is called once per frame
	void Update () 
    {
        // === Targeting State ===
        if (currentState == LaserState.TARGETING)
        {
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
                            crosshairGUIObject.GetComponent<CrosshairBehaviour>().crosshairAdaptDistance(hit);

                            break;
                        }
                }

            }
            else
            {
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
            }

            foreach (LaserModeTarget g in playerRef.cutables)
            {
                if (hit.collider.gameObject.GetInstanceID() == g.transform.parent.gameObject.GetInstanceID())
                {
                    currentTarget = g;
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
                            lightTargeter.transform.LookAt(currentTarget.transform.position);
                            break;
                        }
                    case TargetingMethods.CROSSHAIR:
                        {
                            break;
                        }
                }
            }

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

            if (timer > timerMax)
            {
                currentTarget.GetComponent<LaserModeTarget>().Cut();
                playerRef.removeCutable(currentTarget);

                desiredState = LaserState.TARGETING;
                
                currentTarget = null;
            }
        }

        if (playerRef.cutables.Count == 0)
        {
            playerRef.LaserModeEnabled = false;
        }

        changeStateTo(desiredState);
	}

    public void changeStateTo(LaserState targetState)
    {
        //some statemachine rules:

        //IDLE -> TARGETING,       player uses cutable,        create crosshair
        //IDLE -> LASERING,        no transition!,             -

        //TARGETING -> IDLE,       player aborted,             destroy crosshair
        //TARGETING -> LASERING,   player acquired a target,   create laser

        //LASERING -> IDLE,        player aborted,             destroy crosshair and laser
        //LASERING -> TARGETING    player lasered one of few,  destroy crosshair and laser (crosshair needs reset, so destroy here and create new on enter TARGETING)

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

            print("LaserMode state changed from " + currentState + " to " + targetState);
            desiredState = targetState;
            currentState = targetState;
        }
    }
}
