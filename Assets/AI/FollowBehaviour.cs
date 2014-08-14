using UnityEngine;
using System.Collections;
using System;


public class FollowBehaviour : IBehaviour {

	EnemyBehaviour generalBehaviour;

    SpiderControl control;

    private float t;

    private float timer;

    private bool processingLookAtPlayer = false;

    private bool pathSet;

    private Vector3 currLookDirection, targetLookDirection;

	public FollowBehaviour(EnemyBehaviour generalBehaviour, SpiderControl control)
	{
        this.control = control;
        this.timer = 0.0f;
        this.generalBehaviour = generalBehaviour;
		if (generalBehaviour.isClimbingOnCeiling)
		{
			generalBehaviour.agent.enabled = false;
		}
	}

	//TODO: check, if player is reachable
	public void execute(float timePassed)
	{
		Vector3 playerPosition = generalBehaviour.player.transform.position;
		if (!generalBehaviour.isClimbingOnCeiling)
		{
            this.timer += timePassed;
            if (!pathSet)
            {
                playerPosition.y = 0.0f;
                generalBehaviour.agent.SetDestination(playerPosition);
                pathSet = true;
            }
            if (this.timer > 1.0f)
            {
                this.timer = 0.0f;
                pathSet = false;
            }

            OffMeshLinkData data = generalBehaviour.agent.nextOffMeshLinkData;
            if (data.valid && !generalBehaviour.processingOffMeshLink)
            {
                generalBehaviour.targetLinkData = data;
                control.updateDelegate += generalBehaviour.ProcessOffMeshLink;
                generalBehaviour.processingOffMeshLink = true;
            }
		}
		else
		{
			playerPosition.y = generalBehaviour.enemy.transform.position.y;
            Vector3 targetVelocity = (playerPosition - generalBehaviour.enemy.transform.position).normalized * generalBehaviour.speed;
			generalBehaviour.enemy.rigidbody.velocity = targetVelocity;

            if (!processingLookAtPlayer)
            {
                targetLookDirection = generalBehaviour.player.transform.position - generalBehaviour.enemy.transform.position;
                currLookDirection = generalBehaviour.enemy.transform.forward;

                control.updateDelegate += ProcessTurnFaceToPlayer;
                processingLookAtPlayer = true;
            }

		}
	}

    public void ProcessTurnFaceToPlayer()
    {
        generalBehaviour.enemy.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(currLookDirection, targetLookDirection, t));
        t += Time.deltaTime;

        if (t > 1.0f)
        {
            t = 0.0f;
            control.updateDelegate -= ProcessTurnFaceToPlayer;
            processingLookAtPlayer = false;
        }
    }
}
