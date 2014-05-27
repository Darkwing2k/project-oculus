using UnityEngine;
using System.Collections;

public class FireDoorPiece : MonoBehaviour 
{
    public FireDoor doorRef;

    public Vector3 closePosition;
    public Vector3 openPosition;

    // RIGHT means relative to the object (the red arrow)
    public enum Direction {UP, DOWN, LEFT, RIGHT };
    public Direction openDirection;

    public FireDoor.DoorState masterState;

	// Use this for initialization
	void Start () 
    {
        //open = false;

        closePosition = this.transform.position;

        Mesh m = this.GetComponent<MeshFilter>().mesh;

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

        if (masterState == FireDoor.DoorState.OPEN)
        {
            this.transform.position = openPosition;
        }
        else if(masterState == FireDoor.DoorState.CLOSED)
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
            case FireDoor.DoorState.OPEN:
                {
                    this.transform.position = openPosition;
                    break;
                }
            case FireDoor.DoorState.OPENING:
                {
                    this.transform.position = Vector3.Lerp(closePosition, openPosition, doorRef.steps);
                    break;
                }
            case FireDoor.DoorState.CLOSED:
                {
                    this.transform.position = closePosition;
                    break;
                }
            case FireDoor.DoorState.CLOSING:
                {
                    this.transform.position = Vector3.Lerp(closePosition, openPosition, doorRef.steps);
                    break;
                }
        }
    }
}
