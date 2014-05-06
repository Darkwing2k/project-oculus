using UnityEngine;
using System.Collections;

public abstract class Enemy : Character
{
	protected PositionSteering positionSteering;
	protected AngularSteering angularSteering;

	#region === Unity Stuff ====
	// Use this for initialization
	protected virtual void Start () {
	
	}
	
	// Update is called once per frame
	protected override void Update () {
		if (this.positionSteering != null) {
			this.positionSteering.Update(this);
		}

		if (this.angularSteering != null) {
			this.angularSteering.Update(this);
		}

		// Do the base.Update() at last
		base.Update();
	}
	#endregion
}
