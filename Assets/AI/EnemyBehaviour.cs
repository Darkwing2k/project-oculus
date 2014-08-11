using UnityEngine;
using System.Collections;

public class EnemyBehaviour : IBehaviour {

	public GameObject enemy;
	public GameObject player;
	public NavMeshAgent agent;
	public static int ownLayerMask = 1 << 9;


	public IBehaviour currentBehaviour;

	public bool behaviourChangeLocked = false;

	public bool playerIsHit = false;

	public bool climbableWallReached = false;

	public bool isClimbingOnWall = false;

	public bool isClimbingOnCeiling = false;

	public bool playerPositionKnown = false;

    public bool collisionWithObject = false;

	public static float defaultNavMeshBaseOffset;

	public float speed;

	public RoomInfo currentRoom;

    public Animation anim;

    public OffMeshLinkData targetLinkData;
    public OffMeshLink endLink;
    public bool processingOffMeshLink;
    public bool traversingLink = false;

    public Vector3 DEBUG_startPos;

	protected EnemyBehaviour()
	{}

	public EnemyBehaviour(GameObject enemy, GameObject player, NavMeshAgent enemyNavMeshAgent, Animation anim)
	{
        this.anim = anim;
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

    public void DEBUG_resetSpider()
    {
        enemy.transform.position = DEBUG_startPos;
        agent.baseOffset = defaultNavMeshBaseOffset;
        changeBehaviour(new WanderBehaviour(this, GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>()));
    }

}
