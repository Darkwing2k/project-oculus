using UnityEngine;
using System.Collections;

public class CameraFly2 : MonoBehaviour
{
	public Camera cam;
	public bool repeat;

	private Transform currentChild;
	private int currentChildIndex;

	// Use this for initialization
	void Start() {
		currentChildIndex = -1;
		if (transform.childCount > 0) {
			currentChildIndex = 0;
			currentChild = transform.GetChild(currentChildIndex);
		}
		//Debug.Log(transform.childCount);
	}

	// Update is called once per frame
	void Update() {
		CameraFlyChild attr = currentChild.GetComponent<CameraFlyChild>();
		
		float speed = attr.velocity * Time.deltaTime;
		cam.transform.position = Vector3.MoveTowards(cam.transform.position, currentChild.position, speed);

		if (cam.transform.position == currentChild.position) {
			if (++currentChildIndex < transform.childCount) {
				Debug.Log(currentChildIndex);
				currentChild = transform.GetChild(currentChildIndex);
			} else {
				if (repeat) {
					currentChildIndex = 0;
					currentChild = transform.GetChild(currentChildIndex);
				}
			}
		}
	}
}
