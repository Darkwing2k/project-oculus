using UnityEngine;
using System.Collections;

public class Seek : PositionSteering {
	private Character target;
	private float distance;

	public Seek(Character target) : this(target, 0.0f) {
	}

	public Seek(Character target, float distance) {
		this.target = target;
		this.distance = distance;
	}

	public override void Update(Character self) {
		Vector3 desiredVelocity = target.Position - self.Position;
		Vector3 accel = desiredVelocity - self.Velocity;

		accel.Normalize();
		accel *= self.MaxAcceleration;

		self.Acceleration = accel;
	}
}
