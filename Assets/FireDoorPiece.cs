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

    public FireDoor.DoorState state;

    public bool open;

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

        if (open)
        {
            this.transform.position = openPosition;
        }
        else
        {
            this.transform.position = closePosition;
        }

        //print(m.bounds.size.x * this.transform.localScale.x);
	}
	
	// Update is called once per frame
	void Update ()
    {
        

        if (state == FireDoor.DoorState.CLOSING)
        {
            this.transform.position = Vector3.Lerp(openPosition, closePosition, doorRef.steps);
        }
    }
}
