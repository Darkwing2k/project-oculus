using UnityEngine;
using System.Collections;

public abstract class FlySteering {
	protected bool isFinished;

	public bool IsFinished {
		get { return this.isFinished; }
	}

	public abstract void Start();
	public abstract void Update();
}
