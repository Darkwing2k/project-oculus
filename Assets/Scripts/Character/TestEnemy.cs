using UnityEngine;
using System.Collections;

public class TestEnemy : Enemy
{

	protected void Awake() {
		this.maxVelocity = 100.0f;
		this.maxAccel = 100.0f;
		this.maxAngularVelocity = 30.0f;
		this.maxAngularAccel = 30.0f;
		this.useRigidbody = true;
	}

	// Use this for initialization
	protected override void Start() {
		base.Start();
		this.positionSteering = new Seek(Player.Instance);
		this.angularSteering = new LookWhereYoureGoing();
	}

	// Update is called once per frame
	protected override void Update() {
		base.Update();
	}
}
