using UnityEngine;
using System.Collections;

public abstract class MenuGUI : Menu, IMenu {
	public GUIText menuGUIText;

	protected float cursorTimeout = 0.5f;
	protected float cursorTimer = 0.0f;
	protected string cursor = "|";

	protected string text;
	protected string currentText = "";
	protected string currentCursorState = " ";
	protected float textTimeout = 0.1f;
	protected float textTimer = 0.0f;
	protected int currentTextLength;

	/*
	void Awake() {
		Debug.Log("script was awaked");
	}

	void Start() {
		Debug.Log("script was started");
	}
	*/

	protected virtual void OnEnable() {
		currentTextLength = 0;
		currentCursorState = cursor;
		cursorTimer = cursorTimeout;
		textTimer = textTimeout;
		++currentTextLength;
		currentText = text.Substring(0, currentTextLength);
		Debug.Log("script was enabled");
	}

	protected virtual void OnDisable() {
		if (menuGUIText != null) {
			menuGUIText.text = "";
		}
		Debug.Log("script was disabled");
	}

	protected virtual void Update() {
		if (menuGUIText != null) {
			menuGUIText.text = currentText + currentCursorState;

			if (cursorTimer < 0.0f) {
				cursorTimer = cursorTimeout;
				if (currentCursorState.Trim().Length > 0) {
					currentCursorState = " ";
				} else {
					currentCursorState = cursor;
				}
			}

			if (textTimer < 0.0f && currentTextLength < text.Length) {
				++currentTextLength;
				currentText = text.Substring(0, currentTextLength);
				textTimer = textTimeout;
			}
		}

		cursorTimer -= Time.deltaTime;
		textTimer -= Time.deltaTime;
	}
}
