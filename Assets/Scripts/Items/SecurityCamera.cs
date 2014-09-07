using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour 
{
    public GameObject playerRef;
    public SecurityCameraMaster secCamMaster;
    public bool alarmed;

    public enum ActionState { LOOK, FOLLOW, SEARCH, RETURN }
    public ActionState currentState;
    private ActionState desiredState;

    public float fovMagnitude;
    public float fovAngle;

    public float rotateSpeed;
    private float lookRotateStep;
    private short lookRotateDir;
    private float randomRotateStep;
    private float returnRotateStep;
    public float startledSpeedBuff;

    public float timerTillLosCheck;
    private float losTimer;

    public float timerTillAlert;
    private float alertTimer;
    public float timerTillLook;
    private float lookTimer;
    private float searchTimer;

    public bool playerInCollider;
    public bool playerVisible;

    public Light camLight;
    public Color normalColor;
    public Color startledColor;
    public Color alarmedColor;

    public Vector3 rotStart;
    public Vector3 rotEnd;
    private float distance;
    private Vector3 randomRotStart;
    private Vector3 randomRotEnd;
    private bool reachedRandomRot;
    private Vector3 returnRotStart;
    private Vector3 returnRotEnd;
    private bool reachedReturnRot;

	// Use this for initialization
	void Start () 
    {
        this.transform.parent.eulerAngles = rotStart;

        if(camLight == null)
            camLight = this.transform.parent.GetComponent<Light>();

        currentState = ActionState.LOOK;

        lookRotateDir = 1;

        playerRef = FindObjectOfType<Player>().gameObject;

        distance = Vector3.Distance(rotStart, rotEnd);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (playerInCollider && secCamMaster.camerasReactToPlayer)
        {
            losTimer += Time.deltaTime;

            if (losTimer > timerTillLosCheck)
            {
                checkPlayerVisibility();

                losTimer = 0;
            }
        }

        switch (currentState)
        {
            case ActionState.LOOK:
                {
                    this.transform.parent.eulerAngles = Vector3.Lerp(rotStart, rotEnd, lookRotateStep);

                    lookRotateStep += (rotateSpeed * Time.deltaTime / distance) * lookRotateDir; //rotateStepSize * lookRotateDir;

                    if (lookRotateStep > 1 || lookRotateStep < 0)
                    {
                        lookRotateDir *= -1;
                    }

                    if (playerVisible)
                        desiredState = ActionState.FOLLOW;

                    break;
                }
            case ActionState.FOLLOW:
                {
                    this.transform.parent.LookAt(playerRef.transform.position);

                    if (!alarmed)
                    {
                        alertTimer += Time.deltaTime;

                        if (alertTimer > timerTillAlert)
                        {
                            alarmed = true;
                            camLight.color = alarmedColor;
                        }
                    }
                    else
                    {
                        secCamMaster.secCamOnAlert(playerRef.transform.position);
                    }

                    if (!playerVisible)
                        desiredState = ActionState.SEARCH;

                    break;
                }
            case ActionState.SEARCH:
                {
                    if (reachedRandomRot)
                    {
                        searchTimer += Time.deltaTime;

                        if (searchTimer > 1)
                        {
                            searchTimer = 0;

                            randomRotStart = this.transform.parent.eulerAngles;

                            randomRotEnd = Vector3.Lerp(rotStart, rotEnd, Random.value);

                            distance = Vector3.Distance(randomRotStart, randomRotEnd);

                            if (distance > 180)
                            {
                                randomRotStart = this.transform.parent.eulerAngles - new Vector3(360, 0, 0);
                                distance = distance = Vector3.Distance(randomRotStart, randomRotEnd);
                            }

                            reachedRandomRot = false;
                        }
                    }

                    if (!reachedRandomRot)
                    {
                        this.transform.parent.eulerAngles = Vector3.Lerp(randomRotStart, randomRotEnd, randomRotateStep);

                        randomRotateStep += (rotateSpeed + startledSpeedBuff) * Time.deltaTime / distance;

                        if (randomRotateStep > 1)
                        {
                            reachedRandomRot = true;
                            randomRotateStep = 0;
                        }
                    }

                    lookTimer += Time.deltaTime;

                    if (lookTimer > timerTillLook)
                        desiredState = ActionState.RETURN;

                    if (playerVisible)
                        desiredState = ActionState.FOLLOW;

                    break;
                }
            case ActionState.RETURN:
                {
                    this.transform.parent.eulerAngles = Vector3.Lerp(returnRotStart, returnRotEnd, returnRotateStep);

                    returnRotateStep += (rotateSpeed + startledSpeedBuff) * Time.deltaTime / distance;

                    if (returnRotateStep > 1)
                    {
                        reachedReturnRot = true;
                        desiredState = ActionState.LOOK;
                    }

                    break;
                }
        }

        changeStateTo(desiredState);
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            playerInCollider = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            playerInCollider = false;
            playerVisible = false;
        }
    }

    public void changeStateTo(ActionState targetState)
    {
        if (currentState != targetState)
        {
            // ================ On-Exit Operations =================================
            switch (currentState)
            {
                case ActionState.LOOK:
                    {
                        break;
                    }
                case ActionState.FOLLOW:
                    {
                        break;
                    }
                case ActionState.SEARCH:
                    {
                        lookTimer = 0;
                        break;
                    }
                case ActionState.RETURN:
                    {
                        break;
                    }
            }
            // =====================================================================

            // ================ On-Enter Operations ================================
            switch (targetState)
            {
                case ActionState.LOOK:
                    {
                        alarmed = false;
                        camLight.color = normalColor;
                        alertTimer = 0;

                        distance = Vector3.Distance(rotStart, rotEnd);

                        break;
                    }
                case ActionState.FOLLOW:
                    {
                        if (alarmed)
                            camLight.color = alarmedColor;
                        else
                            camLight.color = startledColor;

                        break;
                    }
                case ActionState.SEARCH:
                    {
                        camLight.color = startledColor;

                        reachedRandomRot = true;

                        randomRotateStep = 0;
                        

                        break;
                    }
                case ActionState.RETURN:
                    {
                        returnRotStart = this.transform.parent.eulerAngles;
                        returnRotEnd = Vector3.Lerp(rotStart, rotEnd, lookRotateStep);

                        distance = Vector3.Distance(returnRotStart, returnRotEnd);

                        reachedReturnRot = false;
                        returnRotateStep = 0;

                        break;
                    }
            }
            // =====================================================================

            //now switch
            currentState = targetState;
            desiredState = targetState;
        }
    }

    public void checkPlayerVisibility()
    {
        float fovSqrMagnitude = fovMagnitude * fovMagnitude;

        Vector3 thisToPlayer = playerRef.transform.position - this.transform.position;

        if (thisToPlayer.sqrMagnitude < fovSqrMagnitude)
        {
            float angle = Vector3.Angle(this.transform.forward, thisToPlayer);

            if (angle < fovAngle)
            {
                Ray r = new Ray(this.transform.position, thisToPlayer);
                RaycastHit hit;

                Physics.Raycast(r, out hit);

                Debug.DrawRay(r.origin, r.direction * fovMagnitude, Color.red, 3);

                if (hit.collider.gameObject.GetInstanceID() == playerRef.GetInstanceID())
                {

                    playerVisible = true;
                    return;
                }
            }
        }
        playerVisible = false;
    }

    public void lookAtPlayer()
    {
        if (playerInCollider)
        {
            this.transform.parent.LookAt(playerRef.transform.position);
        }
    }
}