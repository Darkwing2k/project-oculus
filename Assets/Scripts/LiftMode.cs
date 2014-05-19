using UnityEngine;
using System.Collections;

public class LiftMode : MonoBehaviour 
{
    private Player playerRef;

    // === Variables for using Objects ===
    private GameObject liftable;
    private GameObject lifted;

    public Vector3 jointPos;

	// Use this for initialization
	void Start () 
    {
        playerRef = this.transform.parent.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Use"))
        {
            if (lifted != null)
            {
                playerRef.liftModeActive = false;

                print("releasing object");

                Destroy(lifted.GetComponent<FixedJoint>());

                lifted = null;
            }

            if (liftable != null)
            {
                playerRef.liftModeActive = true;

                print("attaching object");

                // === Attach a Joint ===================================
                FixedJoint joint = liftable.AddComponent<FixedJoint>();
                joint.connectedBody = this.transform.parent.rigidbody;
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = jointPos;

                Mesh m = liftable.GetComponent<MeshFilter>().mesh;

                float anchorY = m.bounds.size.y / 2;

                joint.anchor = new Vector3(0, anchorY, 0);
                
                // ======================================================

                lifted = liftable;
                liftable = null;
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Liftable" && c.rigidbody && lifted == null)
        {
            print("liftable object in range");
            liftable = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Liftable" && c.rigidbody && lifted == null)
        {
            print("lifted object left range");
            liftable = null;
        }
    }
}
