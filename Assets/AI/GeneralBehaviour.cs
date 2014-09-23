using UnityEngine;
using System.Collections;

public class GeneralBehaviour : MonoBehaviour {
    
    public static bool playerIsHit = false;
    public static bool playerPositionKnown = false;
    public static float defaultNavMeshBaseOffset;

    public static int jumpCount;
    public static int maxJumps = 5;

	public GameObject enemy;
	public GameObject player;
	public NavMeshAgent agent;
    public IBehaviour currentBehaviour;
    public string DEBUG_currentBehaviourStr;
    public RoomInfo currentRoom;

    public Animation anim;

    public AudioSource soundSource;
    public AudioClip walkSound;
    public AudioClip jumpSound;

    public OffMeshLinkData currLinkData, nextLinkData;
    public bool processingOffMeshLink = false;
    public bool offMeshLinkStartPosReached = false;
    public bool reachingOffMeshLinkStartPos = false;
    public bool traversingOffMeshLink = false;

	public static int OWN_LAYER_MASK = 1 << 9;
    public int layerMaskToIgnore = ~OWN_LAYER_MASK & ~RoomInfo.ROOM_COLLIDER_LAYER_MASK;

	public bool behaviourChangeLocked = false;

	public bool climbableWallReached = false;

	public bool isClimbingOnWall = false;

	public bool isClimbingOnCeiling = false;

    public bool collisionWithObject = false;

	
	public float speed;

    // for following player a fixed time after losing contact
    float timerAfterLostPlayerSight = 0.0f;
    float timeToFollowPlayerAfterLostSight = 10.0f;
    public bool timeoutLostPlayerSight = false;

    // for interpolation of turn while falling
    float t, lastRotationAngle;

    //for checking if enemy is on ground after falling
    public float lastYPos;
	
    public Vector3 DEBUG_startPos;

    public delegate void executeInUpdateMethod();
    public executeInUpdateMethod updateDelegate;
    void dummy()
    {
    }


	protected GeneralBehaviour()
	{}

	public void SetEnemyBehaviour(GameObject enemy, GameObject player, NavMeshAgent enemyNavMeshAgent, Animation anim, AudioSource ss, AudioClip walk, AudioClip jump)
	{
        this.anim = anim;
        updateDelegate = new executeInUpdateMethod(dummy);

        this.soundSource = ss;
        this.walkSound = walk;
        this.jumpSound = jump;

		this.enemy = enemy;
		this.player = player;
		this.agent = enemyNavMeshAgent;
		defaultNavMeshBaseOffset = enemyNavMeshAgent.baseOffset;
        //enemyNavMeshAgent.autoTraverseOffMeshLink = true;
	}

	public void execute(float timePassed)
	{
        //Debug.Log(currentBehaviour.ToString());

		currentBehaviour.execute(timePassed);

        if (this.agent.enabled && this.agent.nextOffMeshLinkData.valid && !this.processingOffMeshLink)
        {
            nextLinkData = this.agent.nextOffMeshLinkData;
            this.updateDelegate += ProcessOffMeshLink;
            processingOffMeshLink = true;
        }
	}

	public void setCurrentRoom(RoomInfo room)
	{
		this.currentRoom = room;
	}

	public void changeBehaviour(IBehaviour newBehaviour)
	{
		currentBehaviour = newBehaviour;
        DEBUG_currentBehaviourStr = currentBehaviour.GetType().Name;
	}

    private void ProcessOffMeshLink()
    {
        if (!this.traversingOffMeshLink && this.agent.isOnOffMeshLink && this.agent.currentOffMeshLinkData.startPos.Equals(this.nextLinkData.startPos))
        {
            currLinkData = this.agent.currentOffMeshLinkData;
            if (currLinkData.valid)
            {
                if (Mathf.Abs(currLinkData.startPos.y - currLinkData.endPos.y) <= 1.0f)
                {
                    this.updateDelegate += ReachOffMeshLinkStartPos;

                    this.traversingOffMeshLink = true;
                }
                else
                {
                    this.agent.CompleteOffMeshLink();
                    traversingOffMeshLink = false;
                    processingOffMeshLink = false;
                    this.updateDelegate -= ProcessOffMeshLink;
                }
            }
        }
        else if (offMeshLinkStartPosReached)
        {
            Debug.Log("Curr Velocity: " + enemy.rigidbody.velocity);
            if (currLinkData.offMeshLink.endTransform.collider.bounds.Intersects(enemy.collider.bounds))
            {
                traversingOffMeshLink = false;
                this.updateDelegate -= ProcessOffMeshLink;
                processingOffMeshLink = false;
                this.enemy.rigidbody.useGravity = true;
                this.agent.CompleteOffMeshLink();
                reachingOffMeshLinkStartPos = false;
                offMeshLinkStartPosReached = false;
            }
        }
    }

    private void ReachOffMeshLinkStartPos()
    {
        if (!this.reachingOffMeshLinkStartPos)
        {
            Vector3 targetVel = (this.currLinkData.startPos - this.enemy.transform.position).normalized * this.speed;

            this.enemy.rigidbody.useGravity = false;
            this.enemy.rigidbody.velocity = targetVel;
            reachingOffMeshLinkStartPos = true;
        }
        else
        {
            Debug.Log((this.currLinkData.startPos - this.enemy.transform.position).magnitude);
            if ((this.currLinkData.startPos - this.enemy.transform.position).magnitude < 0.1f)
            {
                offMeshLinkStartPosReached = true;
                this.updateDelegate -= ReachOffMeshLinkStartPos;

                Vector3 targetVel = (currLinkData.endPos - currLinkData.startPos).normalized * this.speed;
                this.enemy.rigidbody.velocity = targetVel;
            }
        }
    }


    public void ProcessTurnWhileFalling()
    {
        float angle = (1.0f - t) * 0.0f + t * 180.0f;
        enemy.transform.Rotate(enemy.transform.forward, angle - lastRotationAngle);
        lastRotationAngle = angle;
        t += Time.deltaTime;

        if (t > 1.0f)
        {
            t = 0.0f;
            lastRotationAngle = 0.0f;
            updateDelegate -= ProcessTurnWhileFalling;
        }
    }

    public void DEBUG_resetSpider()
    {
        enemy.transform.position = DEBUG_startPos;
        agent.baseOffset = defaultNavMeshBaseOffset;
        changeBehaviour(new WanderBehaviour(this));
        this.behaviourChangeLocked = false;
    }


    public void isEnemyInSight()
    {
        Ray ray = new Ray(enemy.transform.position, player.transform.position - enemy.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMaskToIgnore))
        {
            if (hit.collider.gameObject.tag.Equals("Player"))
            {
                GeneralBehaviour.playerPositionKnown = true;
                timerAfterLostPlayerSight = 0.0f;
                timeoutLostPlayerSight = false;
            }
            else
            {
                GeneralBehaviour.playerPositionKnown = false;
                if (!timeoutLostPlayerSight)
                {
                    if (timerAfterLostPlayerSight < timeToFollowPlayerAfterLostSight)
                    {
                        timerAfterLostPlayerSight += Time.deltaTime;
                    }
                    else
                    {
                        Debug.Log("Timeout Lost Player Sight!");
                        timeoutLostPlayerSight = true;
                    }
                }
            }
        }
    }

    public void checkIfEnemyHitsPlayer()
    {
        if (enemy.collider.bounds.Intersects(player.collider.bounds))
        {
            GeneralBehaviour.playerIsHit = true;
        }
    }

    public bool isFallDownFinished()
    {
        float heightDelta = System.Math.Abs(lastYPos - enemy.transform.position.y);

        if (heightDelta < 0.05f && collisionWithObject)
        {
            return true;
        }
        //else if (collisionWithObject)
        //{
        //    return true;
        //}
        lastYPos = enemy.transform.position.y;
        
        return false;
    }

}
