using UnityEngine;
using System.Collections;
using System;

public class CameraFly : MonoBehaviour {

	public Transform flyPoints;
	private int currentChildIndex;

	private float journeyTime;
	private float startTime;

	private Vector3 sourcePosition;
	private Quaternion sourceRotation;
	
	private Transform destination;

	private float travelingTime;

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
}
