using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlastDoor : Triggerable
{
    public enum DoorState {OPEN, CLOSED, CLOSING, OPENING};
    public DoorState currentState;
    public DoorState desiredState;

    private BlastDoorPiece[] doorPieces;

    public float steps;
    public float stepSize;
    
	// Use this for initialization
	void Start () 
    {
        doorPieces = this.GetComponentsInChildren<BlastDoorPiece>();

        foreach (BlastDoorPiece d in doorPieces)
        {
            d.doorRef = this;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () 
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

    public override void trigger()
    {
        if (!(triggerOnce && triggered))
        {
            switch (currentState)
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
