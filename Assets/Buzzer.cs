using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buzzer : MonoBehaviour 
{
    public float targetHeight;
    public float force;
    public float pressedHeight;

    public List<Triggerable> objectsToTrigger;

    public bool pressed;

	// Use this for initialization
	void Start () 
    {
        targetHeight = this.transform.localPosition.y;
	}

    void FixedUpdate()
    {
        if (this.transform.localPosition.y < targetHeight)
        {
            this.rigidbody.AddForce(0, force, 0, ForceMode.Force);

            if (this.transform.localPosition.y < pressedHeight)
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
