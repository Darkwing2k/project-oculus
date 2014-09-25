using UnityEngine;
using System.Collections;

public class ReallyUsable : Usable {
	

	public override void use() {
	}

	public override PlayerStateMachine.PlayerStateEnum getDesiredState() {
		return PlayerStateMachine.PlayerStateEnum.INPUT;
	}
}
