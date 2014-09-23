using UnityEngine;
using System.Collections;

public class JumpAttackBehaviour : IBehaviour {

	GeneralBehaviour generalBehaviour;

	private bool jumpInProgress, checking;

	private static float timeToCheckIfJumpCompleted = 0.8f;
	private float timePassedSinceJump;

	private static Vector3 jumpCorrectionVector = new Vector3(0.0f, 5.0f, 0.0f);

	public JumpAttackBehaviour(GeneralBehaviour generalBehaviour)
	{
		this.generalBehaviour = generalBehaviour;
		jumpInProgress = false;
		timePassedSinceJump = 0.0f;
        checking = false;
	}

	public void execute(float timePassed)
	{
        if (!jumpInProgress)
        {
            if (GeneralBehaviour.jumpCount < GeneralBehaviour.maxJumps)
            {
                attackWithJump();
                GeneralBehaviour.jumpCount++;
            }
            else
            {
                GeneralBehaviour.jumpCount = 0;
                generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour));
            }
        }
        else if (!checking)
        {
            generalBehaviour.updateDelegate += checkJumpCompleted;
            checking = true;
        }
	}

	private void attackWithJump()
	{
		jumpInProgress = true;
		generalBehaviour.behaviourChangeLocked = true;
		Vector3 playerPosition = generalBehaviour.player.transform.position;
		Vector3 enemyPosition = generalBehaviour.enemy.transform.position;
		
		// Vector3 circleCenter = enemyPosition + (0.5 * (playerPosition - enemyPosition))
		Vector3 positionDelta = playerPosition - enemyPosition;

		/*
		playerPosition.y = 0.0f;
		Vector3 positionFloorDelta = playerPosition - enemyPosition;
		float angle = Vector3.Angle(positionFloorDelta, positionDelta);
		
		if (angle < minJumpAngle)
			angle = minJumpAngle;
		*/
		Vector3 jumpVector = positionDelta + jumpCorrectionVector;
		//generalBehaviour.agent.Stop();
		generalBehaviour.agent.enabled = false;

		generalBehaviour.enemy.rigidbody.useGravity = true;
		generalBehaviour.enemy.rigidbody.AddForce(jumpVector, ForceMode.Impulse);
        generalBehaviour.anim.Stop();
        generalBehaviour.collisionWithObject = false;

        generalBehaviour.soundSource.clip = generalBehaviour.jumpSound;
        generalBehaviour.soundSource.loop = false;
        generalBehaviour.soundSource.Play();
	}

    private void checkJumpCompleted()
    {
        timePassedSinceJump += Time.deltaTime;
        if (isJumpCompleted())
        {
            generalBehaviour.enemy.rigidbody.useGravity = false;
            generalBehaviour.agent.enabled = true;
            generalBehaviour.behaviourChangeLocked = false;
            generalBehaviour.enemy.rigidbody.velocity = Vector3.zero;
            timePassedSinceJump = 0.0f;
            jumpInProgress = false;
            generalBehaviour.anim.Play();

            generalBehaviour.soundSource.clip = generalBehaviour.walkSound;
            generalBehaviour.soundSource.loop = true;
            generalBehaviour.soundSource.Play();

            generalBehaviour.updateDelegate -= checkJumpCompleted;
        }
    }

	private bool isJumpCompleted()
	{
		if (timePassedSinceJump > timeToCheckIfJumpCompleted)
		{
            if (generalBehaviour.isFallDownFinished())
                return true;
            //if (generalBehaviour.enemy.transform.position.y < 2.0f)
            //{
            //    return true;
            //}
		}
		return false;
	}

	public static bool isObstacleInWay(GameObject enemy, Vector3 playerPosition)
	{
		RaycastHit hit;
		Ray ray = new Ray(enemy.transform.position, playerPosition - enemy.transform.position);

        
		if (Physics.SphereCast(ray, ((BoxCollider)enemy.collider).size.x, out hit, Mathf.Infinity, ~GeneralBehaviour.OWN_LAYER_MASK))
		{
			if (!hit.transform.gameObject.tag.Equals("Player"))
			{
				return true;
			}
		}
		return false;
	}
}
