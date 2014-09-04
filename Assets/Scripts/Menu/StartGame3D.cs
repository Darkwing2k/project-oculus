using UnityEngine;
using System.Collections;

public class StartGame3D : Menu3D {
	public Transform origin;
	public Transform target;

	public TextMesh text3D;

	public GameObject monitor1;
	public GameObject monitor2;
	public Material activeMaterial;

	private Material originMaterial;

	private Vector3 targetPos;
	private float t;

	void Awake () {
		if (origin == null) {
			origin = transform;
		}
		text3D.gameObject.SetActive(false);

		if (monitor1 != null) {
			originMaterial = monitor1.renderer.material;
		}
	}

	void OnEnable() {
		text3D.gameObject.SetActive(true);
		text3D.transform.position = origin.position;

		if (!GameManager.Instance.IsLevelLoaded) {
			text3D.text = "Start Game";
		} else {
			text3D.text = "Resume Game";
		}

		t = 0.0f;

		if (monitor1 != null) {
			monitor1.renderer.material = activeMaterial;
			monitor2.renderer.material = activeMaterial;
		}
	}

	void OnDisable() {
		text3D.gameObject.SetActive(false);

		if (monitor1 != null) {
			monitor1.renderer.material = originMaterial;
			monitor2.renderer.material = originMaterial;
		}
	}

	void Update () {
		if (t < 0.7f) {
			text3D.transform.position = Vector3.Lerp(origin.position, target.position, t);
			t += Time.deltaTime / 2.0f;
		}

		if (Input.GetButtonDown("Action")) {
			GameManager.Instance.LoadLevel(1);
			this.enabled = false;
		}
	}


}
