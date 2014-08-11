using UnityEngine;
using System.Collections;

public class LookForPlayerAfterLostContact : IBehaviour {

	private EnemyBehaviour generalBehaviour;
	private Vector3 lastPlayerPosition;


	public LookForPlayerAfterLostContact(EnemyBehaviour generalBehaviour, Vector3 lastPlayerPosition)
	{
		this.generalBehaviour = generalBehaviour;
		this.lastPlayerPosition = lastPlayerPosition;
	}

	public void execute(float timePassed)
	{

	}




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
