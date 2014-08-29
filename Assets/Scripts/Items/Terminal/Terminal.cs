using UnityEngine;
using System.Collections;

public class Terminal : MonoBehaviour {

	private enum TerminalState { ENABLED, DISABLED };

	public TerminalButton activeButton;

	private bool buttonTriggeredOnce;
	public TextMesh inputfield;

	private ObjectZoom zoomer;

	private TerminalState currentState;

	void Awake() {
		inputfield = GetComponentInChildren<TextMesh>();
		zoomer = GetComponentInChildren<ObjectZoom>();
		inputfield.text = "";
		currentState = TerminalState.DISABLED;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (currentState == TerminalState.ENABLED) {
			float lStickV = Input.GetAxis("LStickV");
			float lStickH = Input.GetAxis("LStickH");

			if (Mathf.Abs(lStickV) > 0.5f || Mathf.Abs(lStickH) > 0.5f) {
				if (!buttonTriggeredOnce) {
					TerminalButton nextButton = null;

					if (Mathf.Abs(lStickV) > Mathf.Abs(lStickH)) {
						nextButton = Input.GetAxis("LStickV") > 0 ? activeButton.top : activeButton.bottom;
					} else {
						nextButton = Input.GetAxis("LStickH") > 0 ? activeButton.right : activeButton.left;
					}

					if (nextButton != null) {
						activeButton.Deselect();
						activeButton = nextButton;
						activeButton.Select();
					}
				}
				buttonTriggeredOnce = true;
			} else {
				buttonTriggeredOnce = false;
			}

			if (Input.GetButtonDown("Action")) {
				activeButton.Press(this);
			}
		}
	}

	public string Text {
		get { return inputfield.text; }
		set { 
			if (inputfield.text.Length < 8)
				inputfield.text = value; 
		}
	}

	public void Activate() {
		activeButton.Select();
		inputfield.text = "";
		currentState = TerminalState.ENABLED;
		zoomer.ZoomIn();
	}

	public void Deactivate() {
		activeButton.Deselect();
		currentState = TerminalState.DISABLED;
		zoomer.ZoomOut();
	}
}
