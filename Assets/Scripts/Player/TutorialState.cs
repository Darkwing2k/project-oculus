using UnityEngine;
using System.Collections.Generic;

public class TutorialState : PlayerState {

	public CrosshairBehaviour tutorialGUI;
	public Texture[] guiTextures;

	public enum Mode {PAN, ROTATE, ELEVATE};
	public Mode currentMode;

	public bool panningDone;
	public bool rotatingDone;
	public bool elevatingDone;

	private FlightControl2 control;

	public override void enterState(Usable usableRef, List<Usable> all) {
		control = playerRef.GetComponent<FlightControl2>();

		panningDone = false;
		rotatingDone = false;
		elevatingDone = false;

		currentMode = Mode.PAN;
		tutorialGUI.gameObject.SetActive(true);
	}

	public override void handleInput(PlayerStateMachine.InputType inputT, List<Usable> usables) {
	}
	
	public override void exitState() {
		tutorialGUI.gameObject.SetActive(false);
	}

	public override void InternalUpdate() 
	{
		switch(currentMode)
		{
			case Mode.PAN:
				control.rotationLocked = true;
				control.verticalMovementLocked = true;
				tutorialGUI.crosshairImg = guiTextures[0];

				if(panningDone) {
					currentMode = Mode.ROTATE;
				}
				break;

			case Mode.ROTATE:
				control.rotationLocked = false;
				tutorialGUI.crosshairImg = guiTextures[1];

				if(rotatingDone) {
					currentMode = Mode.ELEVATE;
				}
				break;

			case Mode.ELEVATE:
				control.verticalMovementLocked = false;
				tutorialGUI.crosshairImg = guiTextures[2];
				
				if(elevatingDone) {
					psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
				}
				break;
		}
	}
}
