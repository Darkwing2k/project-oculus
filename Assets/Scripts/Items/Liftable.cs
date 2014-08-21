using UnityEngine;
using System.Collections;

public class Liftable : Usable 
{
    public bool lifted;

    public bool imAnEnergyCell;

    public override void use()
    {
        
    }

    public override PlayerStateMachine.PlayerStateEnum getDesiredState()
    {
        return PlayerStateMachine.PlayerStateEnum.LIFT;
    }

    public void prepareLifting(Transform p, Vector3 position, Vector3 rotation, bool useKinematic)
    {
        GameObject myParent = this.transform.parent.gameObject;

        myParent.collider.enabled = false;
        myParent.transform.parent = p;
        myParent.transform.localPosition = position;
        myParent.transform.localEulerAngles = rotation;

        myParent.rigidbody.isKinematic = useKinematic;

        lifted = true;
    }

    public void prepareRelease()
    {
        GameObject myParent = this.transform.parent.gameObject;

        myParent.transform.parent = null;
        myParent.collider.enabled = true;

        lifted = false;
    }
}
