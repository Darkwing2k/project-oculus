using UnityEngine;
using System.Collections;

public class TerminalValueButton : TerminalButton {
	public string value = "";


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Press(Terminal terminal) {
		terminal.Text += value;
	}
}
