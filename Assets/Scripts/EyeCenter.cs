using UnityEngine;
using System.Collections;

public class EyeCenter : MonoBehaviour 
{
    public float ipd;
    public Camera rightCamera;

	// Use this for initialization
	void Start () 
    {
        OVRCameraController camCtrl = this.transform.parent.transform.parent.GetComponent<OVRCameraController>();

        ipd = camCtrl.IPD;

        this.transform.localPosition = rightCamera.transform.localPosition + new Vector3(-(ipd / 2), 0, 0);
	}
	
	// Update is called once per frame
	void Update () 
    {
        
	}
}
