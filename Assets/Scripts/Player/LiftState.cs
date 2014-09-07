using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LiftState : PlayerState 
{
    // === Variables for using Objects ===
    public Liftable lifted;

    public EnergyStation closeStation;

    public Vector3 jointPos;

    public override void enterState(Usable usableRef, List<Usable> all)
    {
        lifted = usableRef.gameObject.GetComponent<Liftable>();

        if (closeStation != null)
        {
            closeStation.takeCell();
            closeStation.energyCell = null;
        }

        attachObject(lifted);
    }

    public override void handleInput(PlayerStateMachine.InputType inputT, List<Usable> usables)
    {
        if (inputT == PlayerStateMachine.InputType.USE)
        {
            psm.changePlayerState(PlayerStateMachine.PlayerStateEnum.IDLE);
        }
    }

    public override void InternalUpdate()
    {

    }

    public override void exitState()
    {
        print("releasing object");

        Destroy(lifted.transform.parent.gameObject.GetComponent<FixedJoint>());

        lifted.prepareRelease();

        if (closeStation != null && lifted.imAnEnergyCell)
        {
            closeStation.insertCell(lifted);
            closeStation.energyCell = lifted;
        }

        lifted = null;
    }

    public void attachObject(Liftable liftable)
    {
        if (liftable != null)
        {
            print("attaching object");

            liftable.prepareLifting(this.transform, jointPos, this.transform.eulerAngles, false);

            // === Attach a Joint ===================================
            FixedJoint joint = liftable.transform.parent.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = playerScript.rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = jointPos;

            Mesh m = liftable.gameObject.transform.parent.gameObject.GetComponentInChildren<MeshFilter>().mesh;

            float anchorY = m.bounds.size.y / 2;

            joint.anchor = new Vector3(0, 0, 0);
            // ======================================================

            lifted = liftable;
        }
    }

    public void releaseObject()
    {
        print("releasing object");

        Destroy(lifted.transform.parent.gameObject.GetComponent<FixedJoint>());

        lifted.prepareRelease();

        lifted = null;
    }
}
