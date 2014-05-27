using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireDoor : Triggerable
{
    public enum DoorState {OPEN, CLOSED, CLOSING, OPENING};
    public DoorState currentState;
    public DoorState desiredState;

    public enum Trigger { COLLIDER, BUTTON };
    public Trigger currentTrigger;

    private FireDoorPiece[] doorPieces;

    public float steps;
    public float stepSize;
    
	// Use this for initialization
	void Start () 
    {
        doorPieces = this.GetComponentsInChildren<FireDoorPiece>();

        if (currentTrigger != Trigger.COLLIDER)
            this.gameObject.collider.enabled = false;

        foreach (FireDoorPiece d in doorPieces)
        {
            d.doorRef = this;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (currentState == DoorState.CLOSING)
        {
            steps -= stepSize;

            if (steps < 0)
            {
                desiredState = DoorState.CLOSED;
            }
        }

        if (currentState == DoorState.OPENING)
        {
            steps += stepSize;

            if (steps > 1)
            {
                desiredState = DoorState.OPEN;
            }
        }

        if (desiredState != currentState)
        {
            switch (desiredState)
            {
                case DoorState.CLOSED:
                    {
                        steps = 0;
                        break;
                    }
                case DoorState.CLOSING:
                    {

                        break;
                    }
                case DoorState.OPEN:
                    {
                        steps = 1;
                        break;
                    }
                case DoorState.OPENING:
                    {
                       
                        break;
                    }
            }

            currentState = desiredState;
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag.Equals("Player") && currentState == DoorState.OPEN)
        {
            desiredState = DoorState.CLOSING;
        }
    }

    public override void trigger()
    {
        if (currentTrigger == Trigger.BUTTON)
        {
            if (!(triggerOnce && triggered))
            {
                switch (desiredState)
                {
                    case DoorState.CLOSED:
                        {
                            desiredState = DoorState.OPENING;
                            break;
                        }
                    case DoorState.CLOSING:
                        {
                            desiredState = DoorState.OPENING;
                            break;
                        }
                    case DoorState.OPEN:
                        {
                            desiredState = DoorState.CLOSING;
                            break;
                        }
                    case DoorState.OPENING:
                        {
                            desiredState = DoorState.CLOSING;
                            break;
                        }
                }

                triggered = true;
            }
        }
    }
}
