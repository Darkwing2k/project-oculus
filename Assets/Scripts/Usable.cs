using UnityEngine;
using System.Collections;

public abstract class Usable : MonoBehaviour 
{
    public Player playerRef;

    public bool canUseMultiple;

    public int usePriority;

    public abstract void prepareInteraction();

    public void setPlayerRef(Player p)
    {
        playerRef = p;
    }

    public bool playerVisible()
    {
        RaycastHit hit;
        Vector3 playerPos = playerRef.transform.position;

        Physics.Raycast(this.transform.position, playerPos - this.transform.position, out hit);

        if (hit.collider.gameObject.GetInstanceID() == playerRef.gameObject.GetInstanceID())
        {
            return true;
        }

        return false;
    }
}
