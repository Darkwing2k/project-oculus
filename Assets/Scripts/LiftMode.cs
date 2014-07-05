using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LiftMode : MonoBehaviour 
{
    public Player playerRef;

    // === Variables for using Objects ===
    private GameObject lifted;

    public Vector3 jointPos;

	// Use this for initialization
	void Start () 
    {
        playerRef = FindObjectOfType<Player>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (playerRef.LiftModeEnabled)
        {
            //lifted.transform.localPosition = jointPos;
        }
	}

    public LiftModeTarget shortestDistanceTo(List<LiftModeTarget> targets)
    {
        float minDist = float.MaxValue;
        LiftModeTarget candidate = null;

        foreach (LiftModeTarget current in targets)
        {
            float sqrMagnitude = (current.gameObject.transform.position - this.gameObject.transform.position).sqrMagnitude;

            if (sqrMagnitude < minDist)
            {
                RaycastHit hit;
                Physics.Raycast(playerRef.transform.position, current.transform.parent.position - playerRef.transform.position, out hit, 100);

                if (hit.collider.gameObject.GetInstanceID() == current.transform.parent.gameObject.GetInstanceID())
                {
                    candidate = current;
                    minDist = sqrMagnitude;
                }
            }
        }

        return candidate;
    }

    public void attachObject(GameObject liftable)
    {
        if (liftable != null)
        {
            print("attaching object");

            liftable.collider.enabled = false;
            liftable.transform.parent = this.transform;
            liftable.transform.localPosition = jointPos;
            liftable.transform.rotation = this.transform.rotation;

            // === Attach a Joint ===================================
            FixedJoint joint = liftable.AddComponent<FixedJoint>();
            joint.connectedBody = playerRef.rigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = jointPos;

            Mesh m = liftable.GetComponentInChildren<MeshFilter>().mesh;

            float anchorY = m.bounds.size.y / 2;

            joint.anchor = new Vector3(0, 0, 0);
            // ======================================================

            lifted = liftable;
            liftable = null;
        }
    }

    public void releaseObject()
    {
        print("releasing object");

        Destroy(lifted.GetComponent<FixedJoint>());

        lifted.transform.parent = null;
        lifted.collider.enabled = true;

        lifted = null;
    }
}
