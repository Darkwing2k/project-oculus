using UnityEngine;
using System.Collections;

public class ExitGame3D : Menu3D {
	public Transform origin;
	public Transform target;

	public TextMesh text3D;

	public MenuTrigger trigger;

	private Vector3 targetPos;
	private float t;

	private SceneFader fader;

	public GameObject door;
	public Material activeMaterial;

	private Material originMaterial;

	private bool exit = false;

	void Awake () {
		if (origin == null) {
			origin = transform;
		}
		text3D.gameObject.SetActive(false);
		fader = new SceneFader();

		if (door != null) {
			originMaterial = door.renderer.material;
		}
	}

	void OnEnable() {
		text3D.gameObject.SetActive(true);
		text3D.transform.position = origin.position;
		t = 0.0f;

		if (door != null) {
			door.renderer.material = activeMaterial;
		}
	}

	void OnDisable() {
		text3D.gameObject.SetActive(false);

		if (door != null) {
			door.renderer.material = originMaterial;
		}
	}

	void Update () {
		if (t < 0.7f) {
			text3D.transform.position = Vector3.Lerp(origin.position, target.position, t);
			t += Time.deltaTime / 2.0f;
		}

		if (exit && !fader.IsFadingOut) {
			Debug.Log("Game exited");
			Application.Quit();
		}

		if (Input.GetButtonDown("Use") && !exit) {
			exit = true;
			Component.Destroy(trigger);
			fader.FadeOutAndStay();
		}

		fader.Update();
	}

	void OnGUI() {
		fader.OnGUI();
	}
}
