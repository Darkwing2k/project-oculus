using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cutable : Usable 
{
    public float duration;

    public bool isCut;

    public List<Component> destroyWhenCut;

    public List<Triggerable> triggerOnCut;

    public override void use()
    {
        
    }

    public override PlayerStateMachine.PlayerStateEnum getDesiredState()
    {
        return PlayerStateMachine.PlayerStateEnum.LASER;
    }

    public void Cut()
    {
        foreach (Component c in destroyWhenCut)
            Destroy(c);

        destroyWhenCut.Clear();

        foreach (Triggerable current in triggerOnCut)
        {
            current.trigger();
        }

        isCut = true;

        this.collider.enabled = false;
        psm.deregisterUsable(this);
    }
}
