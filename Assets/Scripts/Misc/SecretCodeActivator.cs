using UnityEngine;
using System.Collections;

public class SecretCodeActivator : PlayerLooksAtActivator {
	private enum InternalState { NONE, FADE_IN, FADE_OUT };

	public float timeToFade = 1.5f;

	private InternalState state = InternalState.NONE;

	private float currentAlpha;

	private Material material;

	private Color fromColor;
	private Color toColor;

	private float t = 0.0f;

	void Awake() {
		material = transform.renderer.material;
		fromColor = material.color;
		fromColor.a = 0.0f;
		material.color = fromColor;
		toColor = new Color(fromColor.r, fromColor.g, fromColor.b, 1.0f);
	}

	public override void Activate() {
		state = InternalState.FADE_IN;
	}

	public override void Deactivate() {
		state = InternalState.FADE_OUT;
	}

	void Update() {
		if (state != InternalState.NONE) {
			
			switch (state) {
				case InternalState.FADE_IN:
					t += Time.deltaTime / timeToFade;
					break;
				case InternalState.FADE_OUT:
					t -= Time.deltaTime / timeToFade;
					break;
			}

			material.color = Color.Lerp(fromColor, toColor, t);
			
			if (t <= 0.0f || t >= 1.0f) {
				state = InternalState.NONE;
				t = t <= 0.0f ? 0.0f : 1.0f;
			}
		}
	}
}
