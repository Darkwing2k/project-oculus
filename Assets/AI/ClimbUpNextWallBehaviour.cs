using UnityEngine;
using System.Collections;

public class ClimbUpNextWallBehaviour : IBehaviour {

	private EnemyBehaviour generalBehaviour;

	private bool pathToNextWallSet;

	private bool wallFound;

	private float startBaseOffset, targetBaseOffset, t1;

    private Vector3 currLookRotation, targetLookRotation;

    private Quaternion currRotation, targetRotation;

    private float t2;

    private float timeToLerpUpMovement = 2.0f;

    private float lastRotation = 0.0f;

	private static float wallDistanceDefault;

	Vector3 nearestWallPosition;

    SpiderControl control;

    public ClimbUpNextWallBehaviour(EnemyBehaviour generalBehaviour, SpiderControl control)
	{
        this.control = control;
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
			if (!wallFound)
			{
				FindNextWallToClimb();
				wallFound = true;
			}

			ClimbFoundWall(nearestWallPosition);
		}
		//TODO: implement the case, if no wall is climbable in this room
		else
		{
			Debug.Log ("NO CLIMBABLE WALL IN THIS ROOM");
			generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, control));
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
		if (!pathToNextWallSet)
		{
			generalBehaviour.agent.SetDestination(nearestWallPosition);
			pathToNextWallSet = true;
		}
		// if wall is reached and the enemy is not already climbing the wall, climb it now
		else if (generalBehaviour.climbableWallReached && !generalBehaviour.isClimbingOnWall)
		{
            //generalBehaviour.agent.enabled = false;
            generalBehaviour.enemy.rigidbody.useGravity = false;
			generalBehaviour.agent.Stop();
            generalBehaviour.agent.enabled = false;
            startBaseOffset = generalBehaviour.enemy.transform.position.y;
			targetBaseOffset = generalBehaviour.currentRoom.ceiling.transform.position.y;

            currLookRotation = generalBehaviour.enemy.transform.forward;
            targetLookRotation = Vector3.up;

            Debug.Log(currLookRotation + ", " + targetLookRotation);

			t1 = 0.0f;
            t2 = 0.0f;
            generalBehaviour.enemy.rigidbody.velocity = Vector3.zero;
            generalBehaviour.agent.velocity = Vector3.zero;
            generalBehaviour.enemy.rigidbody.angularVelocity = Vector3.zero;

			//generalBehaviour.enemy.rigidbody.AddForce(Vector3.up * SpiderControl.speed, ForceMode.VelocityChange);
			generalBehaviour.isClimbingOnWall = true;
            control.updateDelegate += ProcessClimbing;
            control.updateDelegate += ProcessTurnFace;
		}
		// then, if ceiling is reached, change the behaviour
		else if (generalBehaviour.isClimbingOnWall && generalBehaviour.isClimbingOnCeiling)
		{
            currRotation = generalBehaviour.enemy.transform.rotation;
            targetRotation = currRotation;
            targetLookRotation.x -= 90.0f;
			generalBehaviour.isClimbingOnWall = false;
            generalBehaviour.agent.enabled = true;
            generalBehaviour.agent.baseOffset = generalBehaviour.enemy.transform.position.y;
			generalBehaviour.behaviourChangeLocked = false;
            control.updateDelegate -= ProcessClimbing;
            control.updateDelegate += ProcessTurnOnCeiling;
		}
	}

    private void ProcessClimbing()
    {
        generalBehaviour.enemy.rigidbody.angularVelocity = Vector3.zero;

        Vector3 pos = generalBehaviour.enemy.transform.position;
        pos.y = Mathf.Lerp(startBaseOffset, targetBaseOffset, t1 / timeToLerpUpMovement);

        generalBehaviour.enemy.transform.position = pos;
        t1 += Time.deltaTime;

        if ((t1 / timeToLerpUpMovement) > 1.0f)
        {
            generalBehaviour.isClimbingOnCeiling = true;
        }
    }

    private void ProcessTurnFace()
    {
        generalBehaviour.enemy.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(currLookRotation, targetLookRotation, t2));
        t2 += Time.deltaTime;
        if (t2 > 1.0f)
        {
            t2 = 0.0f;
            control.updateDelegate -= ProcessTurnFace;
        }
    }

    private void ProcessTurnOnCeiling()
    {
        generalBehaviour.enemy.transform.rotation = Quaternion.Slerp(currRotation, targetRotation, t2);
        t2 += Time.deltaTime;

        if (t2 > 1.0f)
        {
            t2 = 0.0f;
            control.updateDelegate -= ProcessTurnOnCeiling;
        }
    }
}
