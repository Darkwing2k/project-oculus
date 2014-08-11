using UnityEngine;

using System.Collections;
using System;

public class WanderBehaviour : IBehaviour {

	private static float radius = 10.0f;
	private static float minWalkDistance = 5.0f;
	private static Vector3 minDistance = new Vector3(minWalkDistance, 0.0f, minWalkDistance);
	private float timeToWait = 1.0f;
	private float timer = 0.0f;
	private int layerMask = 1 << 8;

	private bool nextPositionReached = false;
	private Vector3 nextPosition;

	public bool pathSet;

	private EnemyBehaviour generalBehaviour;

    private SpiderControl control;

	public WanderBehaviour(EnemyBehaviour generalBehaviour, SpiderControl control)
	{
		this.generalBehaviour = generalBehaviour;
		this.pathSet = false;
        this.control = control;
	}

	public void execute(float timePassed)
	{
        try
        {
            if (!this.pathSet)
            {
                // get next random position
                this.nextPosition = (UnityEngine.Random.insideUnitSphere * radius);
                this.nextPosition.y = 0.0f;

                int random = UnityEngine.Random.Range(0, 4);
                if (random == 0)
                    minDistance = new Vector3(-minWalkDistance, 0.0f, -minWalkDistance);
                else if (random == 1)
                    minDistance = new Vector3(minWalkDistance, 0.0f, -minWalkDistance);
                else if (random == 2)
                    minDistance = new Vector3(-minWalkDistance, 0.0f, minWalkDistance);
                else
                    minDistance = new Vector3(minWalkDistance, 0.0f, minWalkDistance);

                //Debug.Log (random + ", " + minDistance.ToString());
                Vector3 enemyPosition = generalBehaviour.enemy.transform.position;
                this.nextPosition += enemyPosition + minDistance;

                // if a point near to nextPosition is found, set it as destination
                NavMeshHit hit;
                if (NavMesh.SamplePosition(nextPosition, out hit, radius, 1))
                {
                    nextPosition = hit.position;
                    generalBehaviour.agent.SetDestination(nextPosition);
                    Debug.Log("Next Position: " + nextPosition);
                    this.pathSet = true;
                    nextPosition.y = generalBehaviour.enemy.transform.position.y;

                }
            }
            else if (isNextPositionReached())
            {
                generalBehaviour.agent.Stop();
                wait(timePassed);
            }

            //TODO: if spider is on ceiling, traverse link won't work, so first jump on floor
            OffMeshLinkData data = generalBehaviour.agent.nextOffMeshLinkData;
            if (data.valid && !generalBehaviour.processingOffMeshLink)
            {
                generalBehaviour.targetLinkData = data;
                control.updateDelegate += generalBehaviour.ProcessOffMeshLink;
                generalBehaviour.processingOffMeshLink = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            generalBehaviour.DEBUG_resetSpider();
        }
	}

    /// <summary>
    /// Compare two OffMeshLink objects and return if they are the same.
    /// </summary>
    /// <returns></returns>
    //private static bool CompareOffMeshLinks(OffMeshLink link1, OffMeshLink link2)
    //{
    //    if (link1.startTransform.position.Equals(link2.startTransform.position))
    //        return true;
    //    return false;
    //}

	private bool isNextPositionReached()
	{
		if (!nextPositionReached)
		{
			Vector3 posDelta = this.nextPosition - generalBehaviour.enemy.transform.position;
			if (posDelta.magnitude <= 2.0f)
			{
				nextPositionReached = true;
			}
		}
		return nextPositionReached;
	}

	private void wait(float timePassed)
	{
        generalBehaviour.anim.Stop();
		timer += timePassed;
		if (timer >= timeToWait)
		{
			this.pathSet = false;
			this.nextPositionReached = false;
			timer = 0.0f;
            timeToWait = UnityEngine.Random.Range(0.0f, 3.0f);
            generalBehaviour.anim.Play();
		}
	}

}
