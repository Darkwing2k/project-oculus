using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireDoor : MonoBehaviour 
{
    public enum DoorState {OPEN, CLOSED, CLOSING, OPENING};
    public DoorState currentState;

    private FireDoorPiece[] doorPieces;

    public float steps;
    public float stepSize;

	// Use this for initialization
	void Start () 
    {
        currentState = DoorState.OPEN;
        doorPieces = this.GetComponentsInChildren<FireDoorPiece>();

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
            steps += stepSize;

            if (steps > 1)
            {
                currentState = DoorState.CLOSED;

                foreach (FireDoorPiece d in doorPieces)
                {
                    d.state = DoorState.CLOSED;
                }
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag.Equals("Player") && currentState == DoorState.OPEN)
        {
            currentState = DoorState.CLOSING;
            steps = 0;

            foreach(FireDoorPiece d in doorPieces)
            {
                d.state = DoorState.CLOSING;
            }
        }
    }
}
