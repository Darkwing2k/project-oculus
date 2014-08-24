using UnityEngine;
using System.Collections;

public class StartGameGUI : MenuGUI {
	public GameObject monitor1;
	public GameObject monitor2;
	public Material activeMaterial;

	private Material originMaterial;

	void Awake() {
		text = "Start Game";

		if (monitor1 != null) {
			originMaterial = monitor1.renderer.material;
		}
	}

	protected override void OnEnable() {
		base.OnEnable();

		if (monitor1 != null) {
			monitor1.renderer.material = activeMaterial;
			monitor2.renderer.material = activeMaterial;
		}
	}

	protected override void OnDisable() {
		base.OnDisable();

		if (monitor1 != null) {
			monitor1.renderer.material = originMaterial;
			monitor2.renderer.material = originMaterial;
		}
	}
}
