using UnityEngine;
using System.Collections;

public abstract class TerminalButton : MonoBehaviour {

	public TerminalButton top;
	public TerminalButton right;
	public TerminalButton bottom;
	public TerminalButton left;

	public virtual void Select() {
		gameObject.renderer.material.color = Color.blue;
	}

	public virtual void Deselect() {
		gameObject.renderer.material.color = Color.white;
	}

	public abstract void Press(Terminal terminal);
}
