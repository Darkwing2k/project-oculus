using UnityEngine;
using System.Collections;

public class WallReachedTrigger : MonoBehaviour {

	private SpiderControl control;

	void OnTriggerEnter(Collider c)
	{
		if (c.gameObject.tag.Equals("Spider"))
		{
			control.generalBehaviour.climbableWallReached = true;
		}
	}


	void OnTriggerExit(Collider c)
	{
		if (c.gameObject.tag.Equals("Spider"))
		{
			control.generalBehaviour.climbableWallReached = false;
		}
	}


	// Use this for initialization
	void Start () {
		control = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
	}
}
