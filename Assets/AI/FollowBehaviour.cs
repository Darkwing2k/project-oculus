using UnityEngine;
using System.Collections;
using System;


public class FollowBehaviour : IBehaviour {

	GeneralBehaviour generalBehaviour;

    private float t;

    private float timer;

    private bool processingLookAtPlayer = false;

    private bool pathSet;

    private Vector3 currLookDirection, targetLookDirection;

    private float lastRotationAngle, targetAngle;

	public FollowBehaviour(GeneralBehaviour generalBehaviour)
	{
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
                generalBehaviour.updateDelegate += generalBehaviour.ProcessOffMeshLink;
                generalBehaviour.processingOffMeshLink = true;
            }
		}
		else
		{
			playerPosition.y = generalBehaviour.enemy.transform.position.y;
            Vector3 targetVelocity = (playerPosition - generalBehaviour.enemy.transform.position).normalized * generalBehaviour.speed;
			generalBehaviour.enemy.rigidbody.velocity = targetVelocity;


            //if (!processingLookAtPlayer)
            //{
                //targetLookDirection = (generalBehaviour.player.transform.position - generalBehaviour.enemy.transform.position).normalized;
                //currLookDirection = generalBehaviour.enemy.transform.forward;

                //targetAngle = Vector3.Angle(currLookDirection, targetLookDirection);
                //Debug.Log("Target Angle: " + targetAngle);



                targetLookDirection = (generalBehaviour.player.transform.position - generalBehaviour.enemy.transform.position).normalized;
                targetLookDirection.y = 0.0f;
                currLookDirection = generalBehaviour.enemy.transform.forward;
                currLookDirection.y = 0.0f;
                Debug.Log(targetLookDirection + ", " + currLookDirection);
                targetAngle = Vector3.Angle(currLookDirection, targetLookDirection);

                generalBehaviour.enemy.transform.Rotate(Vector3.up, targetAngle);

                //generalBehaviour.updateDelegate += ProcessTurnFaceToPlayer;
                //processingLookAtPlayer = true;
            //}

		}
	}

    public void ProcessTurnFaceToPlayer()
    {
        //processingLookAtPlayer = false;
        //generalBehaviour.updateDelegate -= ProcessTurnFaceToPlayer;

        //float angle = (1.0f - t) * 0.0f + t * targetAngle;
        //generalBehaviour.enemy.transform.Rotate(Vector3.up, angle - lastRotationAngle);
        //lastRotationAngle = angle;
        //t += Time.deltaTime;

        //if (t > 1.0f)
        //{
        //    t = 0.0f;
        //    lastRotationAngle = 0.0f;
        //    generalBehaviour.updateDelegate -= ProcessTurnFaceToPlayer;
        //    processingLookAtPlayer = false;
        //}
    }
}
