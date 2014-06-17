using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour 
{
    public GameObject playerRef;

    public enum AlertState {NORMAL, STARTLED, ALARMED}
    public AlertState currentState;

    public float fovLength;
    public float fovAngle;

    public float timerMax;
    public float timer;

    public Light camLight;
    public Color normal;
    public Color startled;
    public Color alarmed;

    private Vector3 startRot;

	// Use this for initialization
	void Start () 
    {
        this.GetComponent<SphereCollider>().radius = fovLength;
        startRot = this.transform.parent.eulerAngles;

        currentState = AlertState.NORMAL;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (playerRef != null)
        {
            float angle = Vector3.Angle(this.transform.forward, playerRef.transform.position - this.transform.position);

            print(angle);

            if (angle < fovAngle)
            {
                currentState = AlertState.STARTLED;

                camLight.color = startled;
                this.transform.parent.LookAt(playerRef.transform.position);

                timer += Time.deltaTime;

                if (timer > timerMax)
                {
                    camLight.color = alarmed;
                    currentState = AlertState.ALARMED;
                }
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            print("player is close to seccam");
            playerRef = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef = null;

            camLight.color = normal;
            timer = 0;
            this.transform.parent.eulerAngles = startRot;
        }
    }
}
