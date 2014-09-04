using UnityEngine;
using System.Collections;

public abstract class PlayerLooksAtActivator : MonoBehaviour {
	public bool onlyOnce = false;

	public abstract void Activate();
	public abstract void Deactivate();
}
