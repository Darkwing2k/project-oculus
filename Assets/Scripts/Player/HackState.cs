using UnityEngine;
using System.Collections;

public class HackState : PlayerState 
{
    public override void enterState(Usable closest, System.Collections.Generic.List<Usable> all)
    {
        Hackable h = closest.gameObject.GetComponent<Hackable>();

        h.use();

        psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);

        if (h.pressed && !h.toggle)
        {
            psm.deregisterUsable(h);
        }
    }

    public override void handleInput(PlayerStateMachine.InputType inputT, System.Collections.Generic.List<Usable> usables)
    {
        
    }

    public override void InternalUpdate()
    {
     
    }

    public override void exitState()
    {
        
    }
}
