using UnityEngine;
using System.Collections;
using System;

//TODO: think of: LookForPlayerNear - looking for player near his last position after losing contact
//TODO: after jumpAttack or fallDown, enemy is possibly not on NavMesh anymore -> error calling setDestination
public class SpiderControl : MonoBehaviour {

    private bool started = false;

    public bool DEBUG_ExtendedBehaviourActive = true;
    public bool DEBUG_FollowOnCommand = false;

    private bool m_WaitingOnCheckpoint;
    public bool WaitingOnCheckpoint {
        get
        {
            return m_WaitingOnCheckpoint;
        }
        set
        { 
            if (value)
            {
                m_WaitingOnCheckpoint = value;
                generalBehaviour.enemy.rigidbody.useGravity = false;
                generalBehaviour.agent.enabled = false;
                anim.Stop();
            }
            else
            {
                generalBehaviour.lastYPos = generalBehaviour.enemy.transform.position.y;
                generalBehaviour.enemy.rigidbody.useGravity = true;
                generalBehaviour.updateDelegate += waitUntilFallDownFinished;
            }
        } 
    }

	public GeneralBehaviour generalBehaviour;

    public Animation anim;

    public AudioSource soundSource;
    public AudioClip walkSound;
    public AudioClip jumpSound;

	public float speed = 2.0f;

	float timer = 0.0f;
	float timeToExecute = 0.2f;

	float jumpDistance = 4.0f;
	float fallDownDistance = 4.0f;
	float maxPlayerHeightForJumpAttack = 4.0f;

    

	public GameObject player;
	public GameObject spiderEnemy;

	public GameObject testRoom;

    
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {

        if (!started)
        {
            DisableRenderOnStart();

            anim = this.gameObject.GetComponent<Animation>();

            soundSource = this.gameObject.GetComponent<AudioSource>();

            spiderEnemy = GameObject.FindWithTag("Spider");
            player = GameObject.FindWithTag("Player");
            NavMeshAgent navMeshAgent = spiderEnemy.GetComponent<NavMeshAgent>();
            navMeshAgent.speed = speed;

            //check in which room spider is
            RoomInfo[] roomScripts = FindObjectsOfType<RoomInfo>();

            foreach (var obj in roomScripts)
            {
                if (obj.gameObject.collider.bounds.Intersects(spiderEnemy.collider.bounds))
                {
                    this.testRoom = obj.gameObject;
                    break;
                }
            }


            //generalBehaviour = new EnemyBehaviour(this, spiderEnemy, player, navMeshAgent, anim, soundSource, walkSound, jumpSound);
            generalBehaviour.SetEnemyBehaviour(spiderEnemy, player, navMeshAgent, anim, soundSource, walkSound, jumpSound);
            generalBehaviour.DEBUG_startPos = spiderEnemy.transform.position;
            generalBehaviour.speed = speed;
            generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));

            generalBehaviour.setCurrentRoom(testRoom.GetComponent<RoomInfo>());

            WaitingOnCheckpoint = true;

            started = true;
            //InvokeRepeating("Execute", 0.0f, 0.05f);
        }


        if (timer >= timeToExecute && !WaitingOnCheckpoint)
        {
            if (!generalBehaviour.behaviourChangeLocked)
            {
                Vector3 playerPosition = player.transform.position;
                Vector3 enemyPosition = spiderEnemy.transform.position;
                float verticalDistance = Math.Abs(playerPosition.y - enemyPosition.y);
                Debug.Log("Vert. Dist. " + verticalDistance);
                
                playerPosition.y = 0.0f;
                enemyPosition.y = 0.0f;
                float positionDelta = (playerPosition - enemyPosition).magnitude;


                if (generalBehaviour.currentBehaviour is WanderBehaviour)
                {
                    if (GeneralBehaviour.playerPositionKnown)
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
                        generalBehaviour.anim.Play();

                        generalBehaviour.soundSource.clip = generalBehaviour.walkSound;
                        generalBehaviour.soundSource.loop = true;
                        generalBehaviour.soundSource.Play();
                    }
                }
                else if (generalBehaviour.currentBehaviour is FollowBehaviour)
                {
                    Debug.Log(DEBUG_ExtendedBehaviourActive + ", " + GeneralBehaviour.playerPositionKnown + ", " + (verticalDistance > maxPlayerHeightForJumpAttack));
                    if (generalBehaviour.timeoutLostPlayerSight && !GeneralBehaviour.playerPositionKnown && !generalBehaviour.isClimbingOnCeiling)
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }
                    else if (generalBehaviour.timeoutLostPlayerSight && !GeneralBehaviour.playerPositionKnown && generalBehaviour.isClimbingOnCeiling)
                    {
                        generalBehaviour.changeBehaviour(new FallOnFloorBehaviour(generalBehaviour));
                    }
                    else if (DEBUG_ExtendedBehaviourActive && GeneralBehaviour.playerPositionKnown && generalBehaviour.isClimbingOnCeiling && FallDownAttackBehaviour.isPlayerReachable(verticalDistance, positionDelta))
                    {
                        generalBehaviour.changeBehaviour(new FallDownAttackBehaviour(generalBehaviour));
                    }
                    else if (!generalBehaviour.isClimbingOnCeiling && verticalDistance < maxPlayerHeightForJumpAttack &&
                                positionDelta < jumpDistance && GeneralBehaviour.playerPositionKnown &&
                                !JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                    {
                        generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                    }
                    else if (DEBUG_ExtendedBehaviourActive && GeneralBehaviour.playerPositionKnown && verticalDistance > maxPlayerHeightForJumpAttack && !generalBehaviour.isClimbingOnCeiling)
                    {
                        generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour));
                    }
                }
                else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.currentBehaviour is ClimbUpNextWallBehaviour)
                {
                    if (!GeneralBehaviour.playerPositionKnown && !generalBehaviour.isClimbingOnCeiling)
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }
                    else if (FallDownAttackBehaviour.isPlayerReachable(verticalDistance, positionDelta))
                    {
                        generalBehaviour.changeBehaviour(new FallDownAttackBehaviour(generalBehaviour));
                    }
                    else
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
                    }
                }
                else if (generalBehaviour.currentBehaviour is JumpAttackBehaviour)
                {
                    if (!GeneralBehaviour.playerPositionKnown)
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }
                    else if (DEBUG_ExtendedBehaviourActive && verticalDistance > maxPlayerHeightForJumpAttack)
                    {
                        generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour));
                    }
                    else if (positionDelta > jumpDistance
                                || JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
                    }
                    else
                    {
                        generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                    }
                }
                else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.currentBehaviour is FallDownAttackBehaviour)
                {
                    if (!GeneralBehaviour.playerPositionKnown)
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }
                    else if (positionDelta < jumpDistance && verticalDistance < maxPlayerHeightForJumpAttack
                                && !JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                    {
                        generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                    }
                    else
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
                    }
                }
                else if (DEBUG_ExtendedBehaviourActive && generalBehaviour.currentBehaviour is FallOnFloorBehaviour)
                {
                    if (!GeneralBehaviour.playerPositionKnown)
                    {
                        generalBehaviour.changeBehaviour(new WanderBehaviour(generalBehaviour));
                    }
                    else if (!generalBehaviour.isClimbingOnCeiling && verticalDistance < maxPlayerHeightForJumpAttack &&
                                positionDelta < jumpDistance &&
                                !JumpAttackBehaviour.isObstacleInWay(spiderEnemy, player.transform.position))
                    {
                        generalBehaviour.changeBehaviour(new JumpAttackBehaviour(generalBehaviour));
                    }
                    else if (DEBUG_ExtendedBehaviourActive && verticalDistance > maxPlayerHeightForJumpAttack)
                    {
                        generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour));
                    }
                    else
                    {
                        generalBehaviour.changeBehaviour(new FollowBehaviour(generalBehaviour));
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

	
	void Update()
	{
        // Debug Code
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    GeneralBehaviour.playerPositionKnown = !GeneralBehaviour.playerPositionKnown;
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    generalBehaviour.changeBehaviour(new ClimbUpNextWallBehaviour(generalBehaviour, this));
        //}
        if (Input.GetKeyDown(KeyCode.U))
        {
            generalBehaviour.DEBUG_resetSpider();
        }
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    this.DEBUG_FollowOnCommand = !this.DEBUG_FollowOnCommand;
        //}
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    this.gameObject.GetComponent<TriggerSpider>().trigger();
        //}
        /*
        if (generalBehaviour.currentBehaviour is FallDownAttackBehaviour)
        {
            Debug.Log (generalBehaviour.enemy.rigidbody.velocity);
        }*/
        //



        generalBehaviour.checkIfEnemyHitsPlayer();

        generalBehaviour.updateDelegate();

        if (!DEBUG_FollowOnCommand)
            generalBehaviour.isEnemyInSight();
        
	}


    public void waitUntilFallDownFinished()
    {
        if (generalBehaviour.isFallDownFinished() && generalBehaviour.collisionWithObject)
        {
            generalBehaviour.agent.enabled = true;
            m_WaitingOnCheckpoint = false;
            generalBehaviour.updateDelegate -= waitUntilFallDownFinished;
        }
    }

    private void DisableRenderOnStart()
    {
        GameObject[] objectsToDisable = GameObject.FindGameObjectsWithTag("DisableRenderOnStart");

        foreach (var obj in objectsToDisable)
        {
            obj.renderer.enabled = false;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.GetInstanceID() == player.GetInstanceID())
        {
            Player playerScript = player.GetComponent<Player>();
            playerScript.checkpoint.Respawn();
        }
    }

}
