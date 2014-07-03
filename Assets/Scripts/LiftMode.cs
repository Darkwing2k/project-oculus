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

    public GameObject shortestDistanceTo(List<LiftModeTarget> targets)
    {
        float minDist = float.MaxValue;
        LiftModeTarget candidate = null;

        foreach (LiftModeTarget t in targets)
        {
            float magnitude = (t.gameObject.transform.position - this.gameObject.transform.position).magnitude;

            if(magnitude < minDist)
            {
                candidate = t;
                minDist = magnitude;
            }
        }

        return candidate.transform.parent.gameObject;
    }

    public void attachObject(GameObject liftable)
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

    public void releaseObject()
    {
        print("releasing object");

        Destroy(lifted.GetComponent<FixedJoint>());

        lifted.transform.parent = null;
        lifted.collider.enabled = true;

        lifted = null;
    }
}
