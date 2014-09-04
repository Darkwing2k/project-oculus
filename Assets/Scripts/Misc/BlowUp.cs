using UnityEngine;
using System.Collections;

public class BlowUp : Triggerable {

	public override void trigger() {
		if (!triggered) {
			rigidbody.useGravity = true;
			rigidbody.AddForce(0.0f, 500.0f, -1000.0f);
			rigidbody.AddTorque(-500.0f, 0.0f, 0.0f);
			triggered = true;
		}
	}
}
