using UnityEngine;
using System.Collections;

public class KeepTriggerScale : MonoBehaviour 
{

    /*
     * this script prevents the scaling of a trigger when attached to another gameobject
     * */

	// Use this for initialization
	void Start () 
    {
        this.transform.localScale = new Vector3(1, 1, 1);

        float x = 1 / this.transform.lossyScale.x;
        float y = 1 / this.transform.lossyScale.y;
        float z = 1 / this.transform.lossyScale.z;

        this.transform.localScale = new Vector3(x, y, z);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
