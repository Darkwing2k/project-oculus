using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputState : PlayerState {
	public Terminal terminal;

	public override void enterState(Usable usableRef, List<Usable> all) {
		psm.FlightControlEnabled = false;
		terminal.Activate();
	}

	public override void handleInput(PlayerStateMachine.InputType inputT, List<Usable> usables) {
		if (inputT == PlayerStateMachine.InputType.USE) {
			psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
		}
	}

	public override void exitState() {
		psm.FlightControlEnabled = true;
		terminal.Deactivate();
	}

	// Update is called once per frame
	void Update() {
		
	}
}
