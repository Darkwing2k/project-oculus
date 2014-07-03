using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FuseBox : MonoBehaviour 
{
    public bool isActive;

    public int currentlySelected;

    public List<GameObject> cables;
    public List<Transform> startPositions;

    public List<LaserModeTarget> cutables;

	// Use this for initialization
	void Start () 
    {
        foreach (GameObject cable in cables)
        {
            startPositions.Add(cable.transform);
        }

        foreach (LaserModeTarget cutable in cutables)
        {
            cutable.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isActive)
        {
            float lStickV = Input.GetAxis("LStickV");

            if (lStickV > 0.5f)
            {
                currentlySelected = (currentlySelected + 1) % cables.Count;
            }
            else if (lStickV < -0.5f)
            {
                currentlySelected = (currentlySelected - 1 + cables.Count) % cables.Count;
            }
        }
	}

    void OnTriggerEnter(Collider c)
    {
        
    }
}
