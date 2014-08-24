using UnityEngine;
using System.Collections;

public abstract class Menu : MonoBehaviour, IMenu {

	public virtual void Activate() {
		this.enabled = true;
	}

	public virtual void Deactivate() {
		this.enabled = false;
	}
}
