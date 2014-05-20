using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserMode : MonoBehaviour 
{
    private Player playerRef;
    private bool oculusConnected;

    public List<GameObject> cutable;
    public GameObject target;

    private float timer = 0;
    private float timerMax = 2;

    public enum LaserState {WAITING ,TARGETING, LASERING};
    public LaserState currentState = LaserState.WAITING;
    public LaserState desiredState = LaserState.WAITING;

    // === Targeting values
    public enum TargetingMethods { LIGHT, CROSSHAIR};
    public TargetingMethods targetingSystem;

    private Light lightTargeter;
    private LineRenderer laser;
    private bool laserCanHitTarget;
    public Material laserMaterial;
    public Color laserColor;
    public float laserWidth;
    public Vector3 laserPos;

    // === Debug Variables when Oculus is not connected ===
    private LineRenderer pointer;
    private Ray laserTargeter;
    private Ray lookingDirection;
    private float alpha = 0;
    private float beta = 0;

	// Use this for initialization
	void Start ()
    {
        playerRef = this.transform.parent.GetComponent<Player>();
        oculusConnected = playerRef.useOculus;

        if (oculusConnected)
        {
            switch (targetingSystem)
            {
                case TargetingMethods.LIGHT:
                    {
                        GameObject lightTargeterGO = new GameObject("lightTargeter");
                        lightTargeterGO.transform.position = playerRef.eyeCenter.position;
                        lightTargeterGO.transform.rotation = playerRef.eyeCenter.rotation;
                        lightTargeterGO.transform.parent = playerRef.eyeCenter;

                        lightTargeter = lightTargeterGO.AddComponent<Light>();
                        lightTargeter.type = LightType.Spot;
                        lightTargeter.color = Color.red;
                        lightTargeter.range = 1;
                        lightTargeter.spotAngle = 4;
                        lightTargeter.intensity = 1;
                        lightTargeter.enabled = false;

                        laser = lightTargeterGO.AddComponent<LineRenderer>();
                        laser.SetWidth(laserWidth, laserWidth);
                        laser.material = laserMaterial;
                        laser.SetColors(laserColor, laserColor);
                        laser.enabled = false;
                        

                        break;
                    }
                case TargetingMethods.CROSSHAIR:
                    {
                        break;
                    }
            }
        }
        else
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        // === Waiting State ===
        if (currentState == LaserState.WAITING)
        {
            if (Input.GetButtonDown("Use") && cutable.Count != 0)
            {
                print("checking visibility");
                foreach (GameObject g in cutable)
                {
                    RaycastHit hit;
                    Physics.Raycast(this.transform.position, g.transform.position - this.transform.position, out hit);

                    if (hit.collider.gameObject.GetInstanceID() == g.GetInstanceID())
                    {
                        desiredState = LaserState.TARGETING;
                    }
                }
            }
        }

        // === Targeting State ===
        if (currentState == LaserState.TARGETING)
        {
            if (Input.GetButtonDown("Use"))
            {
                desiredState = LaserState.WAITING;
            }

            RaycastHit hit;

            if (oculusConnected)
            {
                Ray r = new Ray();
                r.origin = playerRef.eyeCenter.position;
                r.direction = playerRef.eyeCenter.forward;

                Physics.Raycast(r, out hit, 10);                

            }
            else
            {
                float rStickH = Input.GetAxis("RStickH");
                float rStickV = Input.GetAxis("RStickV");

                if (Mathf.Abs(rStickH) > 0.3f || Mathf.Abs(rStickH) > 0.3f)
                {
                    alpha = alpha + (rStickH * 1f);
                }

                if (Mathf.Abs(rStickV) > 0.3f || Mathf.Abs(rStickV) > 0.3f)
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

            foreach (GameObject g in cutable)
            {
                if (hit.collider.gameObject.GetInstanceID() == g.GetInstanceID())
                {
                    target = g;
                    desiredState = LaserState.LASERING;
                    break;
                }
            }
        }

        // === Lasering State ===
        if (currentState == LaserState.LASERING)
        {
            if (Input.GetButtonDown("Use"))
            {
                desiredState = LaserState.WAITING;
            }

            if (oculusConnected)
            {
                switch (targetingSystem)
                {
                    case TargetingMethods.LIGHT:
                        {
                            lightTargeter.transform.LookAt(target.transform.position);
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

                if(laserCanHitTarget)
                    timer += Time.deltaTime;
            }
            else
            {
                laser.enabled = false;
            }

            if (timer > timerMax)
            {
                cutable.Remove(target);
                Destroy(target.GetComponent<Lock>());
                Destroy(target.transform.parent.GetComponentInChildren<FixedJoint>());
                target = null;

                desiredState = LaserState.WAITING;
            }
        }

        if (currentState != desiredState)
        {
            changeStateTo(desiredState);
        }
	}

    private void changeStateTo(LaserState targetState)
    {
        // === On-Exit Operations ======================================================================================
        switch (currentState)
        {
            case LaserState.WAITING:
                {
                    this.transform.parent.GetComponent<FlightControl>().ControlsActivated = false;
                   
                    break;
                }
            case LaserState.TARGETING:
                {

                    break;
                }
            case LaserState.LASERING:
                {
                    if (oculusConnected)
                    {
                        switch (targetingSystem)
                        {
                            case TargetingMethods.LIGHT:
                                {
                                    lightTargeter.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 1);

                                    break;
                                }
                            case TargetingMethods.CROSSHAIR:
                                {
                                    break;
                                }
                        }

                        laser.enabled = false;
                    }
                    else
                    {
                        
                    }

                    break;
                }
        }
        // =============================================================================================================

        // === On-Enter Operations ======================================================================================
        switch (targetState)
        {
            case LaserState.WAITING:
                {
                    playerRef.laserModeActive = false;

                    if (oculusConnected)
                    {
                        switch (targetingSystem)
                        {
                            case TargetingMethods.LIGHT:
                                {
                                    lightTargeter.enabled = false;
                                    break;
                                }
                            case TargetingMethods.CROSSHAIR:
                                {
                                    break;
                                }
                        }
                    }
                    else
                    {
                        Destroy(pointer);
                    }

                    this.transform.parent.GetComponent<FlightControl>().ControlsActivated = true;

                    break;
                }
            case LaserState.TARGETING:
                {
                    playerRef.laserModeActive = true;

                    if (oculusConnected)
                    {
                        switch (targetingSystem)
                        {
                            case TargetingMethods.LIGHT:
                                {
                                    lightTargeter.enabled = true;
                                    break;
                                }
                            case TargetingMethods.CROSSHAIR:
                                {
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
                    playerRef.laserModeActive = true;

                    Vector3 laserStartPos = playerRef.eyeCenter.transform.position + laserPos;

                    Ray r = new Ray(laserStartPos, target.transform.position - laserStartPos);
                    RaycastHit hit;

                    if (Physics.Raycast(r, out hit, 10))
                    {
                        if (hit.collider.gameObject.GetInstanceID() == target.GetInstanceID())
                            laserCanHitTarget = true;
                        else
                            laserCanHitTarget = false;
                    }

                    laser.SetPosition(0, laserStartPos);
                    laser.SetPosition(1, hit.point);

                    if (oculusConnected)
                    {
                        
                    }
                    else
                    {
                        pointer.SetPosition(1, target.transform.position);
                    }


                    timer = 0;
                    timerMax = target.GetComponent<Lock>().duration;

                    break;
                }
        }
        // =============================================================================================================

        currentState = targetState;
    }
}
