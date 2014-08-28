using UnityEngine;
using System.Collections;

public class ReallyUsable : Usable {
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void use() {
	}

	public override PlayerStateMachine.PlayerStateEnum getDesiredState() {
		return PlayerStateMachine.PlayerStateEnum.INPUT;
	}
}
