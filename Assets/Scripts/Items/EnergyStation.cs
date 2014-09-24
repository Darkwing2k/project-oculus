using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyStation : MonoBehaviour 
{
    public GameObject playerRef;
    public PlayerStateMachine psm;

    public Animation animation;

    public List<Triggerable> objectsToTrigger;
    public List<Light> lightsToTrigger;
    public bool powered;

    public Liftable energyCell;

    public Vector3 cellPosition;
    public Vector3 cellRotation;

	// Use this for initialization
	void Start () 
    {
        if (energyCell != null && energyCell.isEnergyCell)
        {
            energyCell.prepareLifting(this.transform.parent, cellPosition, cellRotation, this);
            powered = true;
        }
        else
        {
            if (animation != null)
            {
                animation.Play();
            }
        }

        foreach (Light currentLight in lightsToTrigger)
        {
            currentLight.enabled = powered;
        }

        playerRef = GameObject.FindGameObjectWithTag("Player");
        psm = playerRef.GetComponent<PlayerStateMachine>();
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void trigger()
    {
        powered = !powered;

        foreach (Triggerable currentT in objectsToTrigger)
        {
            currentT.trigger();
        }

        foreach (Light currentL in lightsToTrigger)
        {
            currentL.enabled = powered;
        }
    }

    public void takeCell()
    {
        energyCell.prepareRelease();

        if (animation != null)
        {
            animation["Take 001"].speed = 1;
            animation.Play();
        }

        trigger();
    }

    public void insertCell(Liftable cell)
    {
        energyCell = cell;
        energyCell.prepareLifting(this.transform.parent, cellPosition, cellRotation, this);

        if (animation != null)
        {
            animation["Take 001"].speed = -1;
            animation["Take 001"].time = animation["Take 001"].length;
            animation.Play();
        }

        trigger();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            LiftState ls = playerRef.GetComponent<LiftState>();
            ls.closeStation = this;

            if (ls.lifted != null && ls.lifted.isEnergyCell)
            { 
                psm.buttonInfo.SetActive(true);
            }
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            if (! psm.buttonInfo.activeSelf)
            {
                LiftState ls = playerRef.GetComponent<LiftState>();

                if (ls.lifted != null && ls.lifted.isEnergyCell)
                {
                    psm.buttonInfo.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            LiftState ls = playerRef.GetComponent<LiftState>();

            ls.closeStation = null;

            psm.buttonInfo.SetActive(false);
        }
    }
}
