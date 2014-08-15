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

    private AudioSource sound;

    public AudioClip moving;
    public float movingPitch;
    private bool moveSoundActive;
    public AudioClip shut;
    public float shutPitch;
    
    
	// Use this for initialization
	void Start () 
    {
        doorPieces = this.GetComponentsInChildren<BlastDoorPiece>();

        sound = this.gameObject.GetComponent<AudioSource>();

        foreach (BlastDoorPiece d in doorPieces)
        {
            d.doorRef = this;
        }

        moveSoundActive = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        if (currentState == DoorState.CLOSING)
        {
            //moveDoor(-1, DoorState.CLOSED);

            steps -= stepSize;

            if (steps < 1 && !moveSoundActive)
            {
                playSound(moving, movingPitch);
                moveSoundActive = true;
            }

            if (steps < 0)
            {
                desiredState = DoorState.CLOSED;

                playSound(shut, shutPitch);
            }
        }

        if (currentState == DoorState.OPENING)
        {
            //moveDoor(1, DoorState.OPEN);

            steps += stepSize;

            if (steps > 0 && !moveSoundActive)
            {
                playSound(moving, movingPitch);
                moveSoundActive = true;
            }

            if (steps > 1)
            {
                desiredState = DoorState.OPEN;

                playSound(shut, shutPitch);
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
            moveSoundActive = false;

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

    private void playSound(AudioClip clip, float pitch)
    {
        if (sound != null)
        {
            sound.Stop();

            sound.clip = clip;

            sound.pitch = pitch;

            sound.Play();
        }
    }
}
