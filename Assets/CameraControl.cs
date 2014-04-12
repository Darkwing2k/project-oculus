using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		RaycastHit hit;
		
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
		{
			this.transform.LookAt(hit.point);
		}
		
	}
}
