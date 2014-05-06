using UnityEngine;
using System.Collections;

public class NightVision : MonoBehaviour
{
	public bool bright = true;
	public float brightness = 2.0f;
	public Shader nightvisionShader;

	public void OnEnable() {
		nightvisionShader = Shader.Find("UnityFree/NightVision");
	}

	public void OnPreCull() {
		if (bright) {
			Shader.SetGlobalFloat("_Brightness", brightness);
			Shader.SetGlobalFloat("_Bright", 1.0f);
		} else {
			Shader.SetGlobalFloat("_Bright", 0.0f);
		}

		if (nightvisionShader)
			transform.camera.SetReplacementShader(nightvisionShader, null);
	}
}
