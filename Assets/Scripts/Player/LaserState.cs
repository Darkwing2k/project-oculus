using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserState : PlayerState 
{
    public Cutable currentTarget;
    public List<Cutable> possibleTargets;

    private AudioSource laserSoundSrc;
    public AudioClip laserSound;

    private float timer = 0;
    private float timerMax = 2;

    public enum LaserStateInternal {IDLE, TARGETING, LASERING};
    public LaserStateInternal currentState;
    public LaserStateInternal desiredState;
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

    public override void enterState(Usable usableRef, List<Usable> all)
    {
        psm.FlightControlEnabled = false;

        possibleTargets = new List<Cutable>();

        foreach (Usable current in all)
        {
            Cutable c;

            if ((c = current.gameObject.GetComponent<Cutable>()) != null)
            {
                possibleTargets.Add(c);
            }
        }

        desiredState = LaserStateInternal.TARGETING;
    }

    public override void handleInput(PlayerStateMachine.InputType inputT, List<Usable> usables)
    {
        if (inputT == PlayerStateMachine.InputType.USE)
        {
            psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
        }
    }

    public override void exitState()
    {
        psm.FlightControlEnabled = true;

        desiredState = LaserStateInternal.IDLE;
    }

	// Update is called once per frame
	void Update () 
    {
        // === Targeting State ===
        if (currentState == LaserStateInternal.TARGETING)
        {
            // Raycast in player´s look direction to define a target
            RaycastHit hit;

            if (playerScript.useOculus)
            {
                Ray playerLook = new Ray();
                playerLook.origin = playerScript.eyeCenter.position;
                playerLook.direction = playerScript.eyeCenter.forward;

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

                // check if the raycast hit any of the possible targets around
                foreach (Cutable target in possibleTargets)
                {
                    if (hit.collider.gameObject.GetInstanceID() == target.transform.parent.gameObject.GetInstanceID())
                    {
                        currentTarget = target;
                        desiredState = LaserStateInternal.LASERING;
                        break;

                    }
                }
            }
        }

        // === Lasering State ===
        if (currentState == LaserStateInternal.LASERING)
        {
            if (playerScript.useOculus)
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
                currentTarget.GetComponent<Cutable>().Cut();

                possibleTargets.Remove(currentTarget);

                if (possibleTargets.Count > 0)
                {
                    desiredState = LaserStateInternal.TARGETING;
                }
                else
                {
                    desiredState = LaserStateInternal.IDLE;
                    psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
                }

                currentTarget = null;
            }
        }

        changeStateTo(desiredState);
	}

    public void changeStateTo(LaserStateInternal targetState)
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
                case LaserStateInternal.IDLE:
                    {

                        break;
                    }
                case LaserStateInternal.TARGETING:
                    {

                        break;
                    }
                case LaserStateInternal.LASERING:
                    {
                        //when leaving lasering state, destroy laser and crosshair

                        Destroy(laser.gameObject);

                        if (playerScript.useOculus)
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

                        break;
                    }
            }
            // =============================================================================================================

            // === On-Enter Operations ======================================================================================
            switch (targetState)
            {
                case LaserStateInternal.IDLE:
                    {
                        //when entering idle state, destroy crosshair

                        if (playerScript.useOculus)
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

                        break;
                    }
                case LaserStateInternal.TARGETING:
                    {
                        //when entering targeting state, create crosshair

                        if (playerScript.useOculus)
                        {
                            switch (targetingSystem)
                            {
                                case TargetingMethods.LIGHT:
                                    {
                                        lightTargeter = GameObject.Instantiate(Resources.Load("LightTargeter")) as GameObject;
                                        lightTargeter.transform.position = playerScript.eyeCenter.position;
                                        lightTargeter.transform.rotation = playerScript.eyeCenter.rotation;
                                        lightTargeter.transform.parent = playerScript.eyeCenter;

                                        break;
                                    }
                                case TargetingMethods.CROSSHAIR:
                                    {
                                        crosshairGUIObject = GameObject.Instantiate(Resources.Load("CrosshairGUIObject")) as GameObject;
                                        crosshairGUIObject.GetComponent<CrosshairBehaviour>().playerRef = playerScript;

                                        break;
                                    }
                            }
                        }

                        break;
                    }
                case LaserStateInternal.LASERING:
                    {
                        //when entering lasering state, create laser

                        GameObject laserGO = GameObject.Instantiate(Resources.Load("Laser")) as GameObject;
                        laser = laserGO.GetComponent<LineRenderer>();
                        laserGO.transform.parent = playerScript.eyeCenter;

                        laserSoundSrc =  this.gameObject.AddComponent<AudioSource>();

                        laserSoundSrc.loop = false;
                        laserSoundSrc.clip = laserSound;

                        Vector3 laserStartPos = playerScript.eyeCenter.transform.position + laserPos;

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

                        if (playerScript.useOculus)
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

                        timer = 0;
                        timerMax = currentTarget.GetComponent<Cutable>().duration;

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
