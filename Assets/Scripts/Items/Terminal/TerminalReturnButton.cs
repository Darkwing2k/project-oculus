using UnityEngine;
using System.Collections;

public class TerminalReturnButton : TerminalButton {
	public override void Press(Terminal terminal) {
		if (terminal.Text.Length > 0) {
			terminal.Text = terminal.Text.Remove(terminal.Text.Length - 1);
		}
	}
}
