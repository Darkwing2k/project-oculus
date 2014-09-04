using UnityEngine;
using System.Collections;

public class TerminalEnterButton : TerminalButton {

	public override void Press(Terminal terminal) {
		terminal.ValidateAndTrigger();
	}
}
