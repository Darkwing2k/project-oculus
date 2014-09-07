using UnityEngine;
using System.Collections;

public class RestartGame3D : Menu3D {
	public Transform origin;
	public Transform target;

	public TextMesh text3D;

	public GameObject tower;

	private Shader originShader;

	private Vector3 targetPos;
	private float t;

	void Awake () {
		if (origin == null) {
			origin = transform;
		}
		text3D.gameObject.SetActive(false);

		if (tower != null) {
			originShader = tower.renderer.material.shader;
		}
	}

	void OnEnable() {
		text3D.gameObject.SetActive(true);
		text3D.transform.position = origin.position;

		t = 0.0f;

		if (tower != null) {
			tower.renderer.material.shader = Shader.Find("Reflective/Bumped Diffuse");
		}
	}

	void OnDisable() {
		text3D.gameObject.SetActive(false);

		if (tower != null) {
			tower.renderer.material.shader = originShader;
		}
	}

	void Update () {
		if (t < 0.7f) {
			text3D.transform.position = Vector3.Lerp(origin.position, target.position, t);
			t += Time.deltaTime / 2.0f;
		}

		if (Input.GetButtonDown("Action")) {
			GameManager.Instance.ReloadLevel();
			this.enabled = false;
		}
	}

	public override void Activate() {
		if (GameManager.Instance.IsLevelLoaded) {
			base.Activate();
		}
	}
}
