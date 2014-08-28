using UnityEngine;
using System.Collections;

public class ObjectZoom : MonoBehaviour {
	public Transform target;

	public float minDistanceToTarget;
	public float scaleFactor;

	public float time;

	private Vector3 startPos;
	private Quaternion startRot;

	private Vector3 targetPos;
	private Vector3 targetRot;

	private float t;

	private bool zoomIn = false;
	private bool zoomOut = false;

	void Awake() {
		startPos = transform.position;
		startRot = transform.rotation;
	}

	void OnEnable() {
		t = 0.0f;
	}

	void OnDisable() {
		transform.position = startPos;
		transform.rotation = startRot;
	}

	void Update() {
		if (target != null && (zoomIn || zoomOut)) {
			if (t <= 1.0f && t >= 0.0f) {
				if (zoomIn) {
					t += Time.deltaTime / time;
				} else {
					t -= Time.deltaTime / time;
				}
			} else {
				zoomIn = false;
				zoomOut = false;
			}

			if (zoomIn || zoomOut) {
				transform.position = Vector3.Slerp(startPos, targetPos, t);
				//transform.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation(target.position - startPos), t);
			}
		}
	}

	public void ZoomIn() {
		if (t < 0.0) {
			t = 0.0f;
		}

		targetPos = target.position - ((target.position - transform.position).normalized * minDistanceToTarget);

		zoomIn = true;
		zoomOut = false;
	}

	public void ZoomOut() {
		if (t > 1.0f) {
			t = 1.0f;
		}
		zoomIn = false;
		zoomOut = true;
	}

	public bool IsZoomingIn {
		get { return zoomIn; }
	}

	public bool IsZoomingOut {
		get { return zoomOut; }
	}

	public bool IsZooming {
		get { return (zoomIn || zoomOut); }
	}
}
