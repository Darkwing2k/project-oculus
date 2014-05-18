using UnityEngine;
using System.Collections;
using System;

public enum FlyBehavior
{
	Line,
	HermiteSpline
}

public class CameraFly : MonoBehaviour {

	public Transform flyPoints;
	public FlyBehavior flyBehavior;

	private int currentChildIndex;

	private Transform destination;

	private FlyPositionSteering flyPosSteering;

	// Use this for initialization
	void Start() {
		currentChildIndex = -1;
		setNextDestination();
	}

	private bool setNextDestination() {
		flyPosSteering = null;
		++currentChildIndex;

		if (flyPoints != null) {
			if (flyPoints.childCount > 0 && currentChildIndex < flyPoints.childCount) {
				destination = flyPoints.FindChild(Convert.ToString(currentChildIndex));
				CameraFlyChild attr = destination.GetComponent<CameraFlyChild>();
				flyPosSteering = new LineFlyPositionSteering(transform, destination.position, destination.rotation, attr.velocity, DurationType.Velocity);
				flyPosSteering.Start();
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update() {
		if (flyPosSteering != null && !flyPosSteering.IsFinished) {
			flyPosSteering.Update();

			if (flyPosSteering.IsFinished) {
				setNextDestination();
			}
		}
	}

	/*
	// Use this for initialization
	void Start () {
		this.startTime = Time.time;
		journeyTime = 4.0f;

		sourcePosition = transform.position;
		sourceRotation = transform.rotation;
		
		destination = null;

		currentChildIndex = -1;

		if (flyPoints != null) {
			if (flyPoints.childCount > 0) {
				currentChildIndex = 0;
				destination = flyPoints.GetChild(currentChildIndex);
				CameraFlyChild attr = destination.GetComponent<CameraFlyChild>();
				travelingTime = (destination.position - sourcePosition).magnitude / attr.velocity;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (destination != null) {
			float fracComplete = (Time.time - startTime) / travelingTime;

			transform.rotation = Quaternion.Slerp(sourceRotation, destination.rotation, fracComplete);
			transform.position = Vector3.Lerp(sourcePosition, destination.position, fracComplete);

			if (fracComplete >= 1.0f) {
				Debug.Log("ziel erreicht ");
				sourcePosition = transform.position;
				sourceRotation = transform.rotation;
				if (++currentChildIndex < flyPoints.childCount) {
					destination = flyPoints.FindChild(Convert.ToString(currentChildIndex));
					startTime = Time.time;
					CameraFlyChild attr = destination.GetComponent<CameraFlyChild>();
					travelingTime = (destination.position - sourcePosition).magnitude / attr.velocity;
				} else {
					currentChildIndex = -1;
					destination = null;
				}
			}
		}
	}
	*/
}
