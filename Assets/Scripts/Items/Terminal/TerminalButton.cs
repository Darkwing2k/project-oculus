using UnityEngine;
using System.Collections;

public class TerminalButton : MonoBehaviour {

	public string value = "";

	public TerminalButton top;
	public TerminalButton right;
	public TerminalButton bottom;
	public TerminalButton left;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Select() {
		gameObject.renderer.material.color = Color.blue;
	}

	public void Deselect() {
		gameObject.renderer.material.color = Color.white;
	}

	public void Press(Terminal terminal) {
		terminal.Text += value;
	}
}
