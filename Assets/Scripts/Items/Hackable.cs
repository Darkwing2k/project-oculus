using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hackable : Usable 
{
    public List<Triggerable> objectsToTrigger;

    public bool pressed;
    public bool toggle;

    public override void use()
    {
        if (!pressed)
        {
            foreach (Triggerable t in objectsToTrigger)
                t.trigger();

            pressed = true;
        }
        else if (pressed && toggle)
        {
            foreach (Triggerable t in objectsToTrigger)
                t.trigger();

            pressed = false;
        }
    }

    public override PlayerStateMachine.PlayerStateEnum getDesiredState()
    {
        return PlayerStateMachine.PlayerStateEnum.HACK;
    }
}
