using UnityEngine;
using System.Collections;

public class HackState : PlayerState 
{
    public override void enterState(Usable closest, System.Collections.Generic.List<Usable> all)
    {
        Hackable h = closest.gameObject.GetComponent<Hackable>();

        h.use();

        psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
    }

    public override void handleInput(PlayerStateMachine.InputType inputT, System.Collections.Generic.List<Usable> usables)
    {
        
    }

    public override void exitState()
    {
        
    }
}
