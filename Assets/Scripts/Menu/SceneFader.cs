using UnityEngine;
using System.Collections;

public class SceneFader {

	private Texture2D fadeTexture;
	private float fadeSpeed = 1.0f;
	
	private Color currentColor;
	private bool isFadingIn;
	private bool isFadingOut;
	private bool keepFading;
	
	public SceneFader () {
		fadeTexture = Resources.Load("Textures/fadeTexture") as Texture2D;
		currentColor = Color.black;
	}
	
	public bool IsFading {
		get { return (isFadingIn || isFadingOut); }
	}
	
	public bool IsFadingIn {
		get { return isFadingIn; }
	}
	
	public bool IsFadingOut {
		get { return isFadingOut; }
	}
	
	// Update is called once per frame
	public void Update () {
		if (isFadingIn) {
			if (currentColor.a <= 0.05f) {
				currentColor = Color.clear;
				isFadingIn = false;
			} else {
				fading(Color.clear);
			}
		}
		
		if (isFadingOut) {
			if (currentColor.a >= 0.95f) {
				currentColor = Color.black;
				isFadingOut = false;
			} else {
				fading(Color.black);
			}
		}
	}
	
	public void OnGUI () {
		if (isFadingIn || isFadingOut || keepFading) {
			GUI.depth = 0;
			GUI.color = currentColor;
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
		}
	}
	
	public void FadeIn () {
		isFadingIn = true;
		isFadingOut = false;
		keepFading = false;
		currentColor = Color.black;
		
		Debug.Log("Start fading in");
		
		
	}
	
	public void FadeOut () {
		isFadingIn = false;
		isFadingOut = true;
		keepFading = false;
		currentColor = Color.clear;
		
		Debug.Log("Start fading out");
	}
	
	public void FadeOutAndStay () {
		FadeOut();
		keepFading = true;
	}
	
	public void StopFading() {
		isFadingIn = false;
		isFadingOut = false;
		keepFading = false;
	}
	
	#region Private Methods
	
	private void fading (Color targetColor) {
		currentColor = Color.Lerp(currentColor, targetColor, fadeSpeed * Time.deltaTime);
	}
	
	#endregion
}
