using UnityEngine;
using System.Collections;

public class LiftMode : MonoBehaviour 
{

    // === Variables for using Objects ===
    public BoxCollider interactionTrigger;

    private GameObject liftable;
    private GameObject lifted;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetButtonDown("Use"))
        {
            if (lifted != null)
            {
                print("releasing object");

                Destroy(lifted.GetComponent<SpringJoint>());
                //Destroy(lifted.GetComponent<FixedJoint>());

                interactionTrigger.isTrigger = true;
                lifted.GetComponent<Collider>().isTrigger = false;

                lifted = null;
            }

            if (liftable != null)
            {
                print("attaching object");

                // === Attach as SpringJoint ============================
                SpringJoint sj = liftable.AddComponent<SpringJoint>();
                sj.connectedBody = this.rigidbody;
                sj.autoConfigureConnectedAnchor = false;
                sj.connectedAnchor = new Vector3(0, -0.5f, 0);
                sj.spring = 80;
                sj.maxDistance = 0.01f;
                // ======================================================

                // === Attach as FixedJoint =============================
                //FixedJoint fj = liftable.AddComponent<FixedJoint>();
                //fj.connectedBody = this.rigidbody;
                // ======================================================

                liftable.GetComponent<Collider>().isTrigger = true;
                interactionTrigger.isTrigger = false;

                //liftable.transform.rotation = this.transform.rotation;
                //liftable.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.1f, this.transform.position.z);

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
