using UnityEngine;
using System.Collections;
using System;

//TODO: think of: LookForPlayerNear - looking for player near his last position after losing contact
//TODO: after jumpAttack or fallDown, enemy is possibly not on NavMesh anymore -> error calling setDestination
public class SpiderControl : MonoBehaviour {

    public bool DEBUG_ExtendedBehaviourActive = true;
    public bool DEBUG_FollowOnCommand = false;

	public EnemyBehaviour generalBehaviour;

    public Animation anim;

	public float speed = 2.0f;

	float timer = 0.0f;
	float timeToExecute = 0.4f;

	float jumpDistance = 4.0f;
	float fallDownDistance = 4.0f;
	float maxPlayerHeightForJumpAttack = 4.0f;

	public GameObject player;
	public GameObject spiderEnemy;

	public GameObject testRoom;

    public delegate void executeInUpdateMethod();
    public executeInUpdateMethod updateDelegate;
    void dummy()
    {
    }
	void Start () {
        updateDelegate = new executeInUpdateMethod(dummy);
        anim = this.gameObject.GetComponent<Animation>();

		spiderEnemy = GameObject.FindWithTag("Spider");
		player = GameObject.FindWithTag("Player");
		NavMeshAgent navMeshAgent = spiderEnemy.GetComponent<NavMeshAgent>();
		navMeshAgent.speed = speed;

		generalBehaviour = new EnemyBehaviour(spiderEnemy, player, navMeshAgent, anim);
        generalBehaviour.DEBUG_startPos = spiderEnemy.transform.position;
        generalBehaviour.speed = speed;
		generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, this));

		generalBehaviour.setCurrentRoom(testRoom.GetComponent<RoomInfo>());
		//InvokeRepeating("Execute", 0.0f, 0.05f);
	}

	// Update is called once per frame
	void FixedUpdate () {

        try
        {
            if (timer >= timeToExecute)
            {
                Debug.Log("IS BEHAVIOUR CHANGE LOCKED: " + generalBehaviour.behaviourChangeLocked);
                if (!generalBehaviour.behaviourChangeLocked)
                {
                    Vector3 playerPosition = player.transform.position;
                    Vector3 enemyPosition = spiderEnemy.transform.position;
                    float verticalDistance = playerPosition.y - enemyPosition.y;
                    playerPosition.y = 0.0f;
                    enemyPosition.y = 0.0f;
                    float positionDelta = (playerPosition - enemyPosition).magnitude;


                    if (generalBehaviour.currentBehaviour is WanderBehaviour)
                    {
                        if (generalBehaviour.playerPositionKnown)
                        {
                            generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour, this));
                            generalBehaviour.anim.Play();
                        }
                    }
                    else if (generalBehaviour.currentBehaviour is FollowBehaviour)
                    {
                        if (!generalBehaviour.playerPositionKnown)
                        {
                            generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, this));
                        }
                        else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.isClimbingOnCeiling && FallDownAttackBehaviour.isPlayerReachable(verticalDistance, positionDelta))
                        {
                            generalBehaviour.changeBehaviour(new FallDownAttackBehaviour(generalBehaviour));
                        }
                        else if (!generalBehaviour.isClimbingOnCeiling && verticalDistance < maxPlayerHeightForJumpAttack &&
                                 positionDelta < jumpDistance &&
                                 !JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                        {
                            generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                        }
                        else if (DEBUG_ExtendedBehaviourActive && verticalDistance > maxPlayerHeightForJumpAttack)
                        {
                            generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour, this));
                        }
                    }
                    else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.currentBehaviour is ClimbUpNextWallBehaviour)
                    {
                        if (!generalBehaviour.playerPositionKnown)
                        {
                            generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, this));
                        }
                        else if (FallDownAttackBehaviour.isPlayerReachable(verticalDistance, positionDelta))
                        {
                            generalBehaviour.changeBehaviour(new FallDownAttackBehaviour(generalBehaviour));
                        }
                        else
                        {
                            generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour, this));
                        }
                    }
                    else if (generalBehaviour.currentBehaviour is JumpAttackBehaviour)
                    {
                        if (!generalBehaviour.playerPositionKnown)
                        {
                            generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, this));
                        }
                        else if (DEBUG_ExtendedBehaviourActive && verticalDistance > maxPlayerHeightForJumpAttack)
                        {
                            generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour, this));
                        }
                        else if (positionDelta > jumpDistance
                                 || JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                        {
                            generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour, this));
                        }
                        else
                        {
                            generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                        }
                    }
                    else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.currentBehaviour is FallDownAttackBehaviour)
                    {
                        if (!generalBehaviour.playerPositionKnown)
                        {
                            generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour, this));
                        }
                        else if (positionDelta < jumpDistance && verticalDistance < maxPlayerHeightForJumpAttack
                                 && !JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                        {
                            generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                        }
                        else
                        {
                            generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour, this));
                        }
                    }


                    //Debug.Log ("IS CLIMBING ON CEILING: " + generalBehaviour.isClimbingOnCeiling);
                    //Debug.Log ("PLAYER POSITION KNOWN: " + generalBehaviour.playerPositionKnown);
                    //Debug.Log ("POSITION DELTA: " + positionDelta);
                    /*
                    if (generalBehaviour.isClimbingOnCeiling && generalBehaviour.playerPositionKnown && positionDelta < jumpDistance && !(generalBehaviour.currentBehaviour is FallDownAttackBehaviour))
                    {
                        Debug.Log("FALL DOWN");
                        generalBehaviour.changeBehaviour(new FallDownAttackBehaviour(generalBehaviour));
                    }
                    //Debug.Log("PositionDelta" + positionDelta);
                    else if (!generalBehaviour.isClimbingOnCeiling && generalBehaviour.playerPositionKnown && positionDelta < jumpDistance && !(generalBehaviour.currentBehaviour is JumpAttackBehaviour))
                    {
                        generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                    }
                    else if (generalBehaviour.playerPositionKnown && !(generalBehaviour.currentBehaviour is FollowBehaviour))
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
                    }
                    else if (!generalBehaviour.playerPositionKnown && !(generalBehaviour.currentBehaviour is WanderBehaviour))
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }*/
                }
                generalBehaviour.execute(timer);
                timer = 0.0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        catch (Exception e)
        {
            generalBehaviour.DEBUG_resetSpider();
        }
	}

	
	void Update()
	{
        try
        {
            // Debug Code
            if (Input.GetKeyDown(KeyCode.C))
            {
                generalBehaviour.playerPositionKnown = !generalBehaviour.playerPositionKnown;
            }
            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour, this));
            //}
            if (Input.GetKeyDown(KeyCode.U))
            {
                generalBehaviour.DEBUG_resetSpider();
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                this.DEBUG_FollowOnCommand = !this.DEBUG_FollowOnCommand;
            }
            /*
            if (generalBehaviour.currentBehaviour is FallDownAttackBehaviour)
            {
                Debug.Log (generalBehaviour.enemy.rigidbody.velocity);
            }*/
            //

            checkIfEnemyHitsPlayer();

            updateDelegate();

            if (!DEBUG_FollowOnCommand)
                isEnemyInSight();
        }
        catch (Exception e)
        {
            generalBehaviour.DEBUG_resetSpider();
        }
	}

	private void checkIfEnemyHitsPlayer()
	{
		if (generalBehaviour.enemy.collider.bounds.Intersects(generalBehaviour.player.collider.bounds))
		{
			//Debug.Log("PLAYER IS HIT");
			generalBehaviour.playerIsHit = true;
		}
	}

    private void isEnemyInSight()
    {
        Ray ray = new Ray(generalBehaviour.enemy.transform.position, generalBehaviour.player.transform.position - generalBehaviour.enemy.transform.position);
        RaycastHit hit;

        Debug.DrawRay(generalBehaviour.enemy.transform.position, generalBehaviour.player.transform.position - generalBehaviour.enemy.transform.position);
        if (Physics.Raycast(ray, out hit, EnemyBehaviour.ownLayerMask))
        {
            if (hit.collider.gameObject.tag.Equals("Player"))
            {
                generalBehaviour.playerPositionKnown = true;
            }
            else
            {
                generalBehaviour.playerPositionKnown = false;
            }
        }
        
    }

    

}
