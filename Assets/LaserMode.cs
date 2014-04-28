using UnityEngine;
using System.Collections;

public class LaserMode : MonoBehaviour 
{

    private GameObject cutable;
    public bool cutMode = false;
    private Ray pointer;

	// Use this for initialization
	void Start ()
    {
        pointer.direction = new Vector3(0, 0, 1);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Use"))
        {
            if (!cutMode && cutable != null)
            {
                cutMode = true;
                this.GetComponent<FlightControl>().controlsActivated = false;
            }
            else
            {
                cutMode = false;
                this.GetComponent<FlightControl>().controlsActivated = true;
            }
        }

        Ray r = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(r, out hit, Mathf.Infinity))
        {
            //print(hit.collider.gameObject.layer);
        }

        Debug.DrawRay(r.origin, r.direction, Color.cyan);
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
