﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyStation : MonoBehaviour 
{
    public GameObject playerRef;
    public PlayerStateMachine psm;

    public List<Triggerable> objectsToTrigger;
    public List<Light> lightsToTrigger;
    public bool powered;

    public Liftable energyCell;

    public Vector3 cellPosition;
    public Vector3 cellRotation;

	// Use this for initialization
	void Start () 
    {
        if (energyCell != null && energyCell.imAnEnergyCell)
        {
            energyCell.prepareLifting(this.transform.parent, cellPosition, cellRotation, this);
            powered = true;
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
        trigger();
    }

    public void insertCell(Liftable cell)
    {
        energyCell = cell;
        energyCell.prepareLifting(this.transform.parent, cellPosition, cellRotation, this);
        trigger();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            LiftState ls = playerRef.GetComponent<LiftState>();

            ls.closeStation = this;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            LiftState ls = playerRef.GetComponent<LiftState>();

            ls.closeStation = null;
        }
    }
}