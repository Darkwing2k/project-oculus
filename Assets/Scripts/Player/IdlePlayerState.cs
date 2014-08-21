using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdlePlayerState : PlayerState
{
    public override void enterState(Usable usableRef, List<Usable> all)
    {

    }

    public override void handleInput(PlayerStateMachine.InputType t, List<Usable> usables)
    {
        if(t == PlayerStateMachine.InputType.USE && usables.Count > 0)
        {
            Usable closest = shortestDistanceTo(usables);

            psm.changePlayerState(closest.getDesiredState(), closest);
        }
    }

    public override void exitState()
    {

    }

    private Usable shortestDistanceTo(List<Usable> currentUsable)
    {
        Usable candidate = currentUsable[0];
        float dist = (currentUsable[0].transform.position - playerRef.transform.position).sqrMagnitude;

        for (int i = 1; i < currentUsable.Count; i++)
        {
            if ((currentUsable[i].transform.position - playerRef.transform.position).sqrMagnitude < dist)
            {
                dist = (currentUsable[i].transform.position - playerRef.transform.position).sqrMagnitude;
                candidate = currentUsable[i];
            }
        }

        return candidate;
    }
}
