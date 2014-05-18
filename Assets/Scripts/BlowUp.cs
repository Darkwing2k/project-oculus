using UnityEngine;
using System.Collections;

public class BlowUp : MonoBehaviour {

	public Rigidbody rigidbody;

	private bool triggered = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.GetComponent<CharacterController>() != null && !triggered) {
			rigidbody.AddForce(0.0f, 500.0f, -1000.0f);
			rigidbody.AddTorque(-500.0f, 0.0f, 0.0f);
			rigidbody.useGravity = true;
			triggered = true;
		}
	}
}
