using UnityEngine;
using System.Collections;

public class PlayerLooksAtTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		PlayerLooksAtActivator act = other.transform.GetComponent<PlayerLooksAtActivator>();

		if (act != null) {
			act.Activate();
		}
	}

	void OnTriggerExit(Collider other) {
		PlayerLooksAtActivator act = other.transform.GetComponent<PlayerLooksAtActivator>();

		if (act != null) {
			act.Deactivate();
		}
	}
}
