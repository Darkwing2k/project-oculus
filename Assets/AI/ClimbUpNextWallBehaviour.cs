using UnityEngine;
using System.Collections;

public class ClimbUpNextWallBehaviour : IBehaviour {

	private GeneralBehaviour generalBehaviour;

	private bool pathToNextWallSet;

	private bool wallFound;

	private float targetYPos;

    private Vector3 currLookRotation, targetLookRotation;

    private Quaternion currRotation, targetRotation;

    private float t2;

    private float timeToLerpUpMovement = 2.0f;

    private float lastRotation = 0.0f;
    
	private static float wallDistanceDefault;

	Vector3 nearestWallPosition;

    public ClimbUpNextWallBehaviour(GeneralBehaviour generalBehaviour)
	{
		generalBehaviour.behaviourChangeLocked = true;
		this.generalBehaviour = generalBehaviour;

        generalBehaviour.isClimbingOnCeiling = false;
		pathToNextWallSet = false;
		wallFound = false;
	}

	public void execute(float timePassed)
	{
		// only execute, if there is a climbable wall in this room
		if (generalBehaviour.currentRoom.climbableWalls.Count > 0)
		{
            if (!wallFound && !generalBehaviour.climbableWallReached)
            {
                FindNextWallToClimb();
                wallFound = true;
            }
            else
            {
                ClimbFoundWall(nearestWallPosition);
            }
		}
		//TODO: implement the case, if no wall is climbable in this room
		else
		{
			Debug.Log ("NO CLIMBABLE WALL IN THIS ROOM");
			generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
		}
	}
	
	// run through registered walls in this room and find the one with minimal distance
	private void FindNextWallToClimb()
	{
		GameObject wall = (GameObject)generalBehaviour.currentRoom.climbableWalls[0];
		nearestWallPosition = wall.transform.position;
		float wallDistance = (generalBehaviour.enemy.transform.position - nearestWallPosition).magnitude;
		float nextWallDistance;
		for (int i = 1; i < generalBehaviour.currentRoom.climbableWalls.Count; i++)
		{
			// if wall isn't that far away, just take this, instead of running through whole list
			if (wallDistance < wallDistanceDefault)
				return;
			wall = (GameObject)generalBehaviour.currentRoom.climbableWalls[i];
			nextWallDistance = (generalBehaviour.enemy.transform.position - wall.transform.position).magnitude;
			// if new distance is shorter, update to nearest wall
			if (wallDistance > nextWallDistance)
			{
				wallDistance = nextWallDistance;
				nearestWallPosition = wall.transform.position;
			}
		}
	}

	//TODO: think of checking, if ceiling is reached, by checking collision with ceiling instead of wallColliderExit
	//TODO: baseOffset uses different units?
	private void ClimbFoundWall(Vector3 nearestWallPosition)
	{
		// set the path to the next wall
		if (!pathToNextWallSet && !generalBehaviour.climbableWallReached)
		{
			generalBehaviour.agent.SetDestination(nearestWallPosition);
			pathToNextWallSet = true;
		}
		// if wall is reached and the enemy is not already climbing the wall, climb it now
		else if (generalBehaviour.climbableWallReached && !generalBehaviour.isClimbingOnWall)
		{
            pathToNextWallSet = true;
            generalBehaviour.enemy.rigidbody.useGravity = false;
			generalBehaviour.agent.Stop();
            generalBehaviour.agent.enabled = false;

			targetYPos = generalBehaviour.currentRoom.ceiling.transform.position.y - 0.5f;

            currLookRotation = generalBehaviour.enemy.transform.forward;
            targetLookRotation = Vector3.up;

            t2 = 0.0f;
            generalBehaviour.enemy.rigidbody.velocity = Vector3.zero;
            generalBehaviour.agent.velocity = Vector3.zero;
            generalBehaviour.enemy.rigidbody.angularVelocity = Vector3.zero;

            generalBehaviour.enemy.rigidbody.velocity = Vector3.up * generalBehaviour.speed;

			generalBehaviour.isClimbingOnWall = true;
            generalBehaviour.updateDelegate += ProcessClimbing;
            generalBehaviour.updateDelegate += ProcessTurnFace;
		}
		// then, if ceiling is reached, change the behaviour
		else if (generalBehaviour.isClimbingOnWall && generalBehaviour.isClimbingOnCeiling)
        {
            currRotation = generalBehaviour.enemy.transform.rotation;
            targetRotation = currRotation;
            targetRotation.x += 90.0f;

			generalBehaviour.isClimbingOnWall = false;
            //generalBehaviour.agent.enabled = true;
            //generalBehaviour.agent.baseOffset = generalBehaviour.enemy.transform.position.y;
			generalBehaviour.behaviourChangeLocked = false;

            generalBehaviour.updateDelegate += ProcessTurnOnCeiling;
		}
	}

    private void ProcessClimbing()
    {
        generalBehaviour.enemy.rigidbody.angularVelocity = Vector3.zero;

        if (generalBehaviour.enemy.transform.position.y >= targetYPos)
            generalBehaviour.isClimbingOnCeiling = true;

        //Vector3 pos = generalBehaviour.enemy.transform.position;
        //pos.y = Mathf.Lerp(startBaseOffset, targetBaseOffset, t1 / timeToLerpUpMovement);

        //generalBehaviour.enemy.transform.position = pos;
        //t1 += Time.deltaTime;

        if (generalBehaviour.isClimbingOnCeiling)//((t1 / timeToLerpUpMovement) > 1.0f)
        {
            //generalBehaviour.isClimbingOnCeiling = true;
            generalBehaviour.enemy.rigidbody.velocity = Vector3.zero;
            generalBehaviour.updateDelegate -= ProcessClimbing;
        }
    }

    private void ProcessTurnFace()
    {
        generalBehaviour.enemy.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(currLookRotation, targetLookRotation, t2));
        t2 += Time.deltaTime;
        if (t2 > 1.0f)
        {
            t2 = 0.0f;
            generalBehaviour.updateDelegate -= ProcessTurnFace;
        }
    }

    private void ProcessTurnOnCeiling()
    {
        generalBehaviour.enemy.transform.rotation = Quaternion.Slerp(currRotation, targetRotation, t2);
        t2 += Time.deltaTime;

        if (t2 > 1.0f)
        {
            t2 = 0.0f;
            generalBehaviour.updateDelegate -= ProcessTurnOnCeiling;
        }
    }
}
