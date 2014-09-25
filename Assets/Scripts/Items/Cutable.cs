using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cutable : Usable
{
    public float duration;

    public bool isCut;

    public List<Component> destroyWhenCut;

    public List<Triggerable> triggerOnCut;

    public GameObject model;

    private bool canInterpolate;
    private bool firstCall = true;

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

        Destroy(this);
    }

    public void interpolateMats(float time)
    {
        if (firstCall)
        {
            canInterpolate = model != null && duration != 0;
            firstCall = false;
        }

        if (canInterpolate)
        {
            model.renderer.material.color = Color.Lerp(new Color(0, 0, 0), new Color(1, 1, 1), time / duration);
        }
    }
}
