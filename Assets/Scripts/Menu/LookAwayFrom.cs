using UnityEngine;
using System.Collections;

public class LookAwayFrom : MonoBehaviour {
	public Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			transform.rotation = Quaternion.LookRotation(transform.position - target.position);
		}
	}
}
