using UnityEngine;
using System.Collections;

public class GeneralBehaviour : MonoBehaviour {
    
    public static bool playerIsHit = false;
    public static bool playerPositionKnown = false;
    public static float defaultNavMeshBaseOffset;


	public GameObject enemy;
	public GameObject player;
	public NavMeshAgent agent;
    public IBehaviour currentBehaviour;
    public RoomInfo currentRoom;

    public Animation anim;

    public AudioSource soundSource;
    public AudioClip walkSound;
    public AudioClip jumpSound;

    public OffMeshLinkData targetLinkData;
    public OffMeshLink endLink;
    public bool processingOffMeshLink;
    public bool traversingLink = false;

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
        enemyNavMeshAgent.autoTraverseOffMeshLink = true;
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
	}

    //TODO: check if next offmeshlink is reached
    public void ProcessOffMeshLink()
    {
        //if (this.agent.isOnOffMeshLink && !traversingLink)
        //{
        //    this.enemy.rigidbody.AddForce((targetLinkData.endPos - targetLinkData.startPos).normalized * speed, ForceMode.VelocityChange);
        //    traversingLink = true;
        //}
        //else if (agent.isOnOffMeshLink)
        //{
        //    SpiderControl control = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
        //    enemy.rigidbody.velocity = Vector3.zero;
        //    agent.CompleteOffMeshLink();
        //    agent.Resume();
        //    control.updateDelegate -= ProcessOffMeshLink;
        //    traversingLink = false;
        //    processingOffMeshLink = false;
        //}
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
            Debug.Log("Fall completed!");
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
