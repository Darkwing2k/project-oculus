using UnityEngine;
using System.Collections;

public class CustomOffMeshLinkContainer : MonoBehaviour {

    public CustomOffMeshLinkMarker[] MarkerList;

	// Use this for initialization
	void Start ()
    {
        for (int i = 0; i < MarkerList.Length; i++)
        {
            MarkerList[i].ID = i;
        }
	}
	
}
