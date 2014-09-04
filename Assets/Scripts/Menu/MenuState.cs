using UnityEngine;
using System.Collections;

public class MenuState : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Start") && GameManager.Instance.IsLevelLoaded) {
			GameManager.Instance.LoadLevel(1);
		}
	}
}
