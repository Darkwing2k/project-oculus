using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buzzer : MonoBehaviour 
{
    public float targetHeight;
    public float force;
    public float maxDist;

    public List<Triggerable> objectsToTrigger;

    public bool pressed;

	// Use this for initialization
	void Start () 
    {
        targetHeight = this.transform.position.y;
	}

    void FixedUpdate()
    {
        if (transform.position.y < targetHeight)
        {
            this.rigidbody.AddForce(0, force, 0, ForceMode.Force);

            if (targetHeight - transform.position.y > maxDist && this.rigidbody.velocity.y < 0)
            {
                changePressed(true);
            }
            else
            {
                changePressed(false);
            }
        }
        else
        {
            this.rigidbody.velocity = Vector3.zero;
        }
    }

    private void changePressed(bool newValue)
    {
        if (pressed != newValue)
        {
            pressed = newValue;

            foreach (Triggerable current in objectsToTrigger)
            {
                current.trigger();
            }
        }
    }

	// Update is called once per frame
	void Update () 
    {
	
	}
}
