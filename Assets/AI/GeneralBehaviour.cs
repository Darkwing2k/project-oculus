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
    public CustomOffMeshLinkMarker[] markerArray;
    public Vector3 nextPosition;
    public bool nextPositionSet = false;
    public bool countUp;
    public int currentIndexPosition;
    public bool processingOffMeshLink = false;
    public bool offMeshLinkStartPosReached = false;
    public bool reachingOffMeshLinkStartPos = false;
    public bool traversingOffMeshLink = false;

	public static int OWN_LAYER_MASK = 1 << 9;
    public LayerMask layerMaskToIgnore;// = ~(RoomInfo.ROOM_COLLIDER_LAYER_MASK | OWN_LAYER_MASK);

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
        updateDelegate += CheckForOffMeshLinks;

        this.soundSource = ss;
        this.walkSound = walk;
        this.jumpSound = jump;

        layerMaskToIgnore = ~layerMaskToIgnore;

		this.enemy = enemy;
		this.player = player;
		this.agent = enemyNavMeshAgent;
		defaultNavMeshBaseOffset = enemyNavMeshAgent.baseOffset;
	}

	public void execute(float timePassed)
	{
        Debug.Log(currentBehaviour.ToString());

		currentBehaviour.execute(timePassed);
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
        // if not already traversing and the right OffMeshLink is reached...
        if (!this.traversingOffMeshLink && this.agent.isOnOffMeshLink && this.agent.currentOffMeshLinkData.startPos.Equals(this.nextLinkData.startPos))
        {
            currLinkData = this.agent.currentOffMeshLinkData;
            if (currLinkData.valid)
            {
                // ...if height difference is low...
                if (Mathf.Abs(currLinkData.startPos.y - currLinkData.endPos.y) <= 1.0f)
                {
                    // ...get to the right start position of the OffMeshLink.
                    this.updateDelegate += ReachOffMeshLinkStartPos;

                    this.traversingOffMeshLink = true;
                }
                    // ...else complete the OffMeshLink with Unity's solution (custom solution still needed)
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
            if (!this.nextPositionSet)
            {
                Vector3 targetVel = (currLinkData.endPos - currLinkData.startPos).normalized * this.speed;
                this.enemy.rigidbody.velocity = targetVel;
                this.nextPositionSet = true;
            }
            // check if the end position of the link is reached and in this case, link is completed
            if (currLinkData.offMeshLink.endTransform.collider.bounds.Intersects(enemy.collider.bounds))
            {
                traversingOffMeshLink = false;
                this.updateDelegate -= ProcessOffMeshLink;
                processingOffMeshLink = false;
                this.enemy.rigidbody.useGravity = true;
                this.agent.CompleteOffMeshLink();
                reachingOffMeshLinkStartPos = false;
                offMeshLinkStartPosReached = false;
                this.nextPositionSet = false;
            }
        }
    }

    private void ReachOffMeshLinkStartPos()
    {
        // if not already on the way to the start position, set the velocity to reach it
        if (!this.reachingOffMeshLinkStartPos)
        {
            Vector3 targetVel = (this.currLinkData.startPos - this.enemy.transform.position).normalized * this.speed;

            this.enemy.rigidbody.useGravity = false;
            this.enemy.rigidbody.velocity = Vector3.zero;
            this.enemy.rigidbody.velocity = targetVel;
            reachingOffMeshLinkStartPos = true;
        }
        // else check whether start position is reached and in this case, set the velocity to traverse the link
        else
        {
            if (this.isPositionReached(this.currLinkData.startPos))
            {
                offMeshLinkStartPosReached = true;
                this.updateDelegate -= ReachOffMeshLinkStartPos;
            }
        }
    }


    private void ProcessCustomOffMeshLink()
    {
        if (!this.traversingOffMeshLink && this.agent.isOnOffMeshLink && this.agent.currentOffMeshLinkData.startPos.Equals(this.nextLinkData.startPos))
        {
            currLinkData = this.agent.currentOffMeshLinkData;
            if (currLinkData.valid)
            {
                this.updateDelegate += ReachOffMeshLinkStartPos;
                this.traversingOffMeshLink = true;

                this.markerArray = currLinkData.offMeshLink.transform.parent.GetComponent<CustomOffMeshLinkContainer>().MarkerList;
                this.currentIndexPosition = currLinkData.offMeshLink.gameObject.GetComponent<CustomOffMeshLinkMarker>().ID;
                if (currentIndexPosition == 0)
                {
                    this.currentIndexPosition++;
                    this.countUp = true;
                }
                else
                {
                    this.currentIndexPosition--;
                    this.countUp = false;
                }
                this.nextPosition = markerArray[this.currentIndexPosition].transform.position;
            }
        }
        else if (this.offMeshLinkStartPosReached)
        {
            if (!this.nextPositionSet)
            {
                Vector3 targetVel = (this.nextPosition - this.enemy.transform.position).normalized * this.speed;
                this.enemy.rigidbody.velocity = Vector3.zero;
                this.enemy.rigidbody.velocity = targetVel;
                this.nextPositionSet = true;
            }
            else
            {
                //this.enemy.transform.LookAt(nextPosition);
                if (isPositionReached(nextPosition))
                {
                    if (countUp)
                        currentIndexPosition++;
                    else
                        currentIndexPosition--;

                    if (currentIndexPosition < 0 || currentIndexPosition >= markerArray.Length)
                    {
                        traversingOffMeshLink = false;
                        this.updateDelegate -= ProcessCustomOffMeshLink;
                        processingOffMeshLink = false;
                        this.enemy.rigidbody.useGravity = true;
                        this.agent.CompleteOffMeshLink();
                        reachingOffMeshLinkStartPos = false;
                        offMeshLinkStartPosReached = false;
                        this.nextPositionSet = false;
                        this.agent.autoTraverseOffMeshLink = true;
                    }
                    else
                    {
                        this.nextPosition = markerArray[currentIndexPosition].transform.position;
                        this.nextPositionSet = false;
                    }
                }
            }
        }

    }



    private void CheckForOffMeshLinks()
    {
        // check if current path leads through an OffMeshLink...
        if (this.agent.enabled && this.agent.nextOffMeshLinkData.valid && !this.processingOffMeshLink)
        {
            // ...and process it in this case
            nextLinkData = this.agent.nextOffMeshLinkData;
            //this.enemy.rigidbody.useGravity = false;
            if (nextLinkData.offMeshLink.tag.Equals("CustomOffMeshLink"))
            {
                this.agent.autoTraverseOffMeshLink = false;
                Debug.Log("Now traverse " + nextLinkData.offMeshLink.gameObject.name);
                this.updateDelegate += ProcessCustomOffMeshLink;
                processingOffMeshLink = true;
            }
            else
            {
                if (nextLinkData.offMeshLink.gameObject.GetComponent<OffMeshLinkStatus>() != null && !nextLinkData.offMeshLink.gameObject.GetComponent<OffMeshLinkStatus>().traversable)
                {
                    Debug.Log("Reset Behaviour");
                    System.Type[] types = { this.GetType() };
                    object[] parameters = { this };
                    this.changeBehaviour((IBehaviour)currentBehaviour.GetType().GetConstructor(types).Invoke(parameters));
                }
                //this.updateDelegate += ProcessOffMeshLink;
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
        agent.enabled = true;
        changeBehaviour(new WanderBehaviour(this));
        this.behaviourChangeLocked = false;
    }


    public void isEnemyInSight()
    {
        Ray ray = new Ray(enemy.transform.position, player.transform.position - enemy.transform.position);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layerMaskToIgnore))
        {
            Debug.Log(hit.transform.gameObject.name + ", Layer: " + hit.transform.gameObject.layer);
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

        if (heightDelta < 0.02f && collisionWithObject)
        {
            Debug.Log("Fall Down Finished");
            return true;
        }
        //else if (collisionWithObject)
        //{
        //    return true;
        //}
        lastYPos = enemy.transform.position.y;
        
        return false;
    }

    public bool isPositionReached(Vector3 position)
    {
        if ((position - this.enemy.transform.position).magnitude < 0.1f)
        {
            return true;
        }
        return false;
    }

    public void spawnSpider(Vector3 position, RoomInfo room, bool waitingOnCheckpoint)
    {
        if (agent.enabled)
        {
            Debug.Log("Disable agent");
            agent.Stop();
            agent.enabled = false;
        }
        enemy.transform.position = position;
        this.changeBehaviour(new WanderBehaviour(this));

        if (!waitingOnCheckpoint)
        {
            agent.enabled = true;
            enemy.rigidbody.useGravity = false;
            agent.Resume();
        }
        else
        {
            this.collisionWithObject = false;
        }
        currentRoom = room;
    }

}
