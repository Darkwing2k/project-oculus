using UnityEngine;
using System.Collections;
using System;

public enum DurationType {
	Velocity,
	TimeInSeconds
}

public class LineFlyPositionSteering : FlyPositionSteering {
	private Vector3 sourcePos;
	private Vector3 destinationPos;

	protected float elapsedTime;

	private float travelTime;
	private float velocity;

	protected Transform toMove;

	protected bool isStarted;

	protected FlyAngularSteering flyAngularSteering;

	public LineFlyPositionSteering(Transform toMove, Vector3 destinationPos, float duration, DurationType dType) {
		this.toMove = toMove;
		Reset(destinationPos, duration, dType);
	}

	public LineFlyPositionSteering(Transform toMove, Vector3 destinationPos, Quaternion destinationRot, float duration, DurationType dType) {
		this.toMove = toMove;
		Reset(destinationPos, duration, dType);

		this.flyAngularSteering = new LerpFlyAngularStreering(this.toMove, destinationRot, this.travelTime);
	}

	public void Reset(Vector3 destinationPos, float duration, DurationType dType) {
		this.sourcePos = this.toMove.position;
		this.destinationPos = destinationPos;
		
		switch (dType) {
			case DurationType.Velocity:
				this.velocity = duration;
				this.travelTime = (destinationPos - sourcePos).magnitude / duration;
				break;
			case DurationType.TimeInSeconds:
				this.travelTime = duration;
				break;
		}

		this.isStarted = false;
		this.isFinished = false;
	}

	public override void Start() {
		this.elapsedTime = 0.0f;
		this.isStarted = true;
		this.isFinished = false;

		if (this.flyAngularSteering != null) {
			this.flyAngularSteering.Start();
		}
	}

	public override void Update() {
		if (!this.isFinished && this.isStarted) {
			float fracComplete = elapsedTime / travelTime;

			if (this.flyAngularSteering != null) {
				this.flyAngularSteering.Update();
			}

			toMove.position = Vector3.Lerp(sourcePos, destinationPos, fracComplete);

			if (fracComplete >= 1.0f) {
				Debug.Log("ziel erreicht ");
				this.isFinished = true;
			} else {
				elapsedTime += Time.deltaTime;
			}
		}
	}
}
