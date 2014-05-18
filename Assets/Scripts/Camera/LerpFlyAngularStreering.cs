using UnityEngine;
using System.Collections;

public class LerpFlyAngularStreering : FlyAngularSteering {
	private Quaternion sourceRot;
	private Quaternion destinationRot;

	protected float elapsedTime;

	private float travelTime;
	private float velocity;

	protected Transform toRotate;

	protected bool isStarted;

	public LerpFlyAngularStreering(Transform toRotate, Quaternion destinationRot, float duration) {
		this.toRotate = toRotate;
		Reset(destinationRot, duration);
	}

	public void Reset(Quaternion destinationRot, float duration) {
		this.sourceRot = this.toRotate.rotation;
		this.destinationRot = destinationRot;
		this.travelTime = duration;

		this.isStarted = false;
		this.isFinished = false;
	}

	public override void Start() {
		this.elapsedTime = 0.0f;
		this.isStarted = true;
		this.isFinished = false;
	}

	public override void Update() {
		if (!this.isFinished && this.isStarted) {
			float fracComplete = elapsedTime / travelTime;

			toRotate.rotation = Quaternion.Slerp(sourceRot, destinationRot, fracComplete);

			if (fracComplete >= 1.0f) {
				this.isFinished = true;
			} else {
				elapsedTime += Time.deltaTime;
			}
		}
	}
}
