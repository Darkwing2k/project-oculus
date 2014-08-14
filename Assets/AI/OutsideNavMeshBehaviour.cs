using UnityEngine;
using System.Collections;

public class OutsideNavMeshBehaviour : IBehaviour {

	private EnemyBehaviour generalBehaviour;

	private bool pathToNavMeshSet;

	private bool followingPathToNavMesh;

	private static float radiusToSearchForNavMeshPosition = 10.0f;

	private Vector3 foundPosition;


	public OutsideNavMeshBehaviour(EnemyBehaviour generalBehaviour)
	{
		this.generalBehaviour = generalBehaviour;
		this.pathToNavMeshSet = false;
		this.followingPathToNavMesh = false;
		generalBehaviour.behaviourChangeLocked = true;
	}

	public void execute(float timePassed)
	{
		if (!pathToNavMeshSet)
		{
			foundPosition = findPathToNavMesh();
			if (foundPosition != null)
			{
				pathToNavMeshSet = true;
			}
		}
		else if (!followingPathToNavMesh)
		{
			followPathToNavMesh(foundPosition);
			followingPathToNavMesh = true;
		}
		else if (isPathOnNavMeshReached())
		{
			generalBehaviour.behaviourChangeLocked = false;
		}
	}

	private Vector3 findPathToNavMesh()
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition(generalBehaviour.enemy.transform.position, out hit, radiusToSearchForNavMeshPosition, 1))
		{
			return hit.position;
		}
		else
		{
			return findPathToNavMesh();
		}
	}

	private void followPathToNavMesh(Vector3 foundPosition)
	{
        Vector3 velocityVector = generalBehaviour.speed * (foundPosition - generalBehaviour.enemy.transform.position).normalized;
		generalBehaviour.enemy.rigidbody.AddForce(velocityVector, ForceMode.VelocityChange);
		//generalBehaviour.enemy.transform.position = foundPosition;
	}

	private bool isPathOnNavMeshReached()
	{
		if ((foundPosition - generalBehaviour.enemy.transform.position).magnitude < 2.0f)
		{
			return true;
		}
		return false;
	}
}
