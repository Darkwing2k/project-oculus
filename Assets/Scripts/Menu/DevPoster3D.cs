using UnityEngine;
using System.Collections;

public class DevPoster3D : Menu {
	public Transform target;

	private Vector3 startPos;
	private Quaternion startRot;

	private float t;

	private bool zoomIn = false;
	private bool zoomOut = false;

	void Awake () {
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

	void Update () {
		if (target != null && (zoomIn || zoomOut)) {
			if (t <= 1.0f && t >= 0.0f) {
				if (zoomIn) {
					t += Time.deltaTime / 2.0f;
				} else {
					t -= Time.deltaTime / 2.0f;
				}
			} else {
				zoomIn = false;
				zoomOut = false;
			}

			if ((zoomIn || zoomOut) && t < 0.65f) {
				transform.position = Vector3.Slerp(startPos, target.position, t);
				//transform.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation(target.position - startPos), t);
			}
			transform.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation(target.position - startPos), t);
			//transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, 10 * Time.deltaTime);
		}
	}

	public override void Activate() {
		if (t < 0.0) {
			t = 0.0f;
		}
		zoomIn = true;
		zoomOut = false;
	}

	public override void Deactivate() {
		if (t > 1.0f) {
			t = 1.0f;
		}
		zoomIn = false;
		zoomOut = true;
	}
}
