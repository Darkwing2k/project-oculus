using UnityEngine;
using System.Collections;

public class SecretCodeGenerator : MonoBehaviour {

	#region Statics

	private static SecretCodeGenerator instance;

	public static SecretCodeGenerator Instance {
		get { return instance; }
	}

	#endregion

	private TextMesh textMesh;

	void Awake() {
		if (instance != null && instance != this) {
			DestroyImmediate(gameObject);
			return;
		} else {
			instance = this;
		}

		textMesh = transform.GetComponent<TextMesh>();
		textMesh.text = Random.Range(10000, 99999).ToString();
	}

	public string Code {
		get { return textMesh.text; }
	}
}
