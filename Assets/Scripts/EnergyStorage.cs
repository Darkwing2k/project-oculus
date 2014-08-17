using UnityEngine;
using System.Collections;

public class EnergyStorage : MonoBehaviour 
{
    private GameObject playerRef;

    public Vector3 targetPos;

    public Vector3 targetRot;

    public GameObject package;

    public bool packageInside;

    public bool PackageInside
    {
        get { return packageInside; }
        set
        {
            packageInside = value;
            objectToTrigger.trigger();
        }
    }

    public Triggerable objectToTrigger;

	// Use this for initialization
	void Start () 
    {
        if(PackageInside)
            objectToTrigger.trigger();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (PackageInside)
        {
            package.transform.localPosition = targetPos;
            package.transform.localEulerAngles = targetRot;

            if (package != null && !package.collider.enabled)
            {
                package.transform.parent = null;
                package = null;
                PackageInside = false;
            }

        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef = c.gameObject;

            Player playerScript = playerRef.GetComponent<Player>();

            if (playerScript.LiftModeEnabled)
            {
                LiftModeTarget lifted = playerRef.GetComponentInChildren<LiftModeTarget>();

                if (lifted != null && lifted.imAnEnergyPackage)
                {
                    playerScript.buttonInfo.gameObject.SetActive(true);
                }
            }
        }

        LiftModeTarget target;

        if ((target = c.gameObject.GetComponentInChildren<LiftModeTarget>()) != null)
        {
            if (target.transform.parent.collider.enabled && target.imAnEnergyPackage)
            {
                package = target.transform.parent.gameObject;
                package.transform.parent = this.transform.parent;
                PackageInside = true;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            playerRef.GetComponent<Player>().buttonInfo.gameObject.SetActive(false);

            playerRef = null;
        }
    }
}
