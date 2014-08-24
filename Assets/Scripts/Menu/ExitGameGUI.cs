using UnityEngine;
using System.Collections;

public class ExitGameGUI : MenuGUI {
	public GameObject door;
	public Material activeMaterial;

	private Material originMaterial;

	void Awake() {
		text = "Exit Game";

		if (door != null) {
			originMaterial = door.renderer.material;
		}
	}

	protected override void OnEnable() {
		base.OnEnable();

		if (door != null) {
			door.renderer.material = activeMaterial;
		}
	}

	protected override void OnDisable() {
		base.OnDisable();

		if (door != null) {
			door.renderer.material = originMaterial;
		}
	}

	protected override void Update() {
		base.Update();

		if (Input.GetButtonDown("Use")) {
			Application.Quit();
		}
	}
}
