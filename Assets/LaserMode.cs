using UnityEngine;
using System.Collections;

public class LaserMode : MonoBehaviour 
{

    private GameObject cutable;
    public bool oculusConnected;

    public Ray lookingDirection;
    public Ray laserTargeter;
    private LineRenderer lr;

    private float alpha = 0;
    private float beta = 0;

    private float timer = 0;
    private float timerMax = 2;

    private bool newInput;

    public enum State {WAITING ,TARGETING, LASERING};
    public State currentState = State.WAITING;

	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        newInput = Input.GetButtonDown("Use");

        if (currentState == State.WAITING)
        {
            if (newInput && cutable != null)
            {
                newInput = false;

                RaycastHit hit;
                Physics.Raycast(this.transform.position, cutable.transform.position - this.transform.position, out hit);

                if (hit.collider.gameObject.GetInstanceID() == cutable.GetInstanceID())
                {
                    changeStateTo(State.TARGETING);
                }
            }
        }

        if (currentState == State.TARGETING)
        {
            if (newInput)
            {
                newInput = false;

                changeStateTo(State.WAITING);
            }

            RaycastHit hit;

            if (oculusConnected)
            {
                Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
                Physics.Raycast(r, out hit, 10);

                Debug.DrawRay(r.origin, r.direction, Color.cyan);
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

                lr.SetPosition(1, hit.point);

                Debug.DrawRay(laserTargeter.origin, laserTargeter.direction, Color.cyan);
            }

            if(hit.collider.gameObject.GetInstanceID() == cutable.GetInstanceID())
            {
                changeStateTo(State.LASERING);
            }            
        }

        if (currentState == State.LASERING)
        {
            timer += Time.deltaTime;

            if (timer >= timerMax)
            {
                changeStateTo(State.WAITING);
            }
        }
	}

    private void changeStateTo(State targetState)
    {
        // === On-Exit Operations ======================================================================================
        switch (currentState)
        {
            case State.WAITING:
                {
                    this.transform.parent.GetComponent<FlightControl>().controlsActivated = false;
                   
                    break;
                }
            case State.TARGETING:
                {

                    break;
                }
            case State.LASERING:
                {
                    cutable.tag = "Untagged";
                    Destroy(cutable.transform.parent.GetComponent<FixedJoint>());

                    break;
                }
        }
        // =============================================================================================================

        // === On-Enter Operations ======================================================================================
        switch (targetState)
        {
            case State.WAITING:
                {
                    Destroy(lr);
                    this.transform.parent.GetComponent<FlightControl>().controlsActivated = true;

                    break;
                }
            case State.TARGETING:
                {
                    if (!oculusConnected)
                    {
                        lookingDirection = laserTargeter = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

                        alpha = Vector3.Angle(new Vector3(0, 0, 1), laserTargeter.direction);
                        beta = Vector3.Angle(new Vector3(0, 1, 0), laserTargeter.direction);

                        lookingDirection.direction.Normalize();
                        laserTargeter.direction.Normalize();

                        lr = this.gameObject.AddComponent<LineRenderer>();
                        lr.SetPosition(0, this.transform.position + new Vector3(0, -0.05f, 0));
                        lr.SetWidth(0.005f, 0.005f);
                    }

                    break;
                }
            case State.LASERING:
                {
                    lr.SetPosition(1, cutable.transform.position);
                    timer = 0;

                    break;
                }
        }
        // =============================================================================================================

        currentState = targetState;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Cutable")
        {
            print("cutable object in range");
            cutable = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Cutable")
        {
            print("cutable object left range");
            cutable = null;
        }
    }
}
