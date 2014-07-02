using UnityEngine;
using System.Collections;

public class BlastDoorPiece : MonoBehaviour 
{
    public BlastDoor doorRef;

    public Vector3 closePosition;
    public Vector3 openPosition;

    // RIGHT means relative to the object (the red arrow)
    public enum Direction {UP, DOWN, LEFT, RIGHT };
    public Direction openDirection;

    public BlastDoor.DoorState masterState;

    public bool autoDetectSize;

    public enum Dimension { X, Y, Z};
    public Dimension dimension;

	// Use this for initialization
	void Start () 
    {
        //open = false;

        closePosition = this.transform.position;

        Mesh m = this.GetComponent<MeshFilter>().mesh;

        if (autoDetectSize)
        {
            float height = m.bounds.size.y * this.transform.localScale.y;
            float width = m.bounds.size.x * this.transform.localScale.x;
            float angle = this.transform.eulerAngles.y * Mathf.Deg2Rad;

            switch (openDirection)
            {
                case Direction.UP:
                    openPosition = closePosition + new Vector3(0, 1, 0) * height;
                    break;
                case Direction.DOWN:
                    openPosition = closePosition - new Vector3(0, 1, 0) * height;
                    break;
                case Direction.LEFT:
                    openPosition = closePosition - new Vector3(Mathf.Cos(angle), 0, -1 * Mathf.Sin(angle)) * width;
                    break;
                case Direction.RIGHT:
                    openPosition = closePosition + new Vector3(Mathf.Cos(angle), 0, -1 * Mathf.Sin(angle)) * width;
                    break;
            }
        }
        else
        {
            float value = 0;

            switch (dimension)
            {
                case Dimension.Y:
                    {
                        value = m.bounds.size.y * this.transform.localScale.y;
                        break;
                    }
                case Dimension.X:
                    {
                        value = m.bounds.size.x * this.transform.localScale.x;
                        break;
                    }
                case Dimension.Z:
                    {
                        value = m.bounds.size.z * this.transform.localScale.z;
                        break;
                    }
            }

            float angle = this.transform.eulerAngles.y * Mathf.Deg2Rad;

            switch (openDirection)
            {
                case Direction.UP:
                    openPosition = closePosition + new Vector3(0, 1, 0) * value;
                    break;
                case Direction.DOWN:
                    openPosition = closePosition - new Vector3(0, 1, 0) * value;
                    break;
                case Direction.LEFT:
                    openPosition = closePosition - new Vector3(Mathf.Cos(angle), 0, -1 * Mathf.Sin(angle)) * value;
                    break;
                case Direction.RIGHT:
                    openPosition = closePosition + new Vector3(Mathf.Cos(angle), 0, -1 * Mathf.Sin(angle)) * value;
                    break;
            }

        }

        if (masterState == BlastDoor.DoorState.OPEN)
        {
            this.transform.position = openPosition;
        }
        else if(masterState == BlastDoor.DoorState.CLOSED)
        {
            this.transform.position = closePosition;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        masterState = doorRef.currentState;

        switch (masterState)
        {
            case BlastDoor.DoorState.OPEN:
                {
                    this.transform.position = openPosition;
                    break;
                }
            case BlastDoor.DoorState.OPENING:
                {
                    this.transform.position = Vector3.Lerp(closePosition, openPosition, doorRef.steps);
                    break;
                }
            case BlastDoor.DoorState.CLOSED:
                {
                    this.transform.position = closePosition;
                    break;
                }
            case BlastDoor.DoorState.CLOSING:
                {
                    this.transform.position = Vector3.Lerp(closePosition, openPosition, doorRef.steps);
                    break;
                }
        }
    }
}
