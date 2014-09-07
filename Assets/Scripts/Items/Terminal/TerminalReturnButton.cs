using UnityEngine;
using System.Collections;

public class TerminalReturnButton : TerminalButton {
	public override void Press(Terminal terminal) {
		if (terminal.Text.Length > 0) {
			string n = terminal.Text.Remove(terminal.Text.Length - 1);
			Debug.Log("curr: " + terminal.Text + "| n: " + n);
			terminal.Text = n;
		}
	}
}
