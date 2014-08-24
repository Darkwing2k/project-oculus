using UnityEngine;
using System.Collections;

public class MenuTrigger : MonoBehaviour {

	private Menu activeMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f)) {
		}
		Debug.DrawRay(transform.position, transform.forward, Color.red, 100.0f);
		*/
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log("TriggerEntered: " + other.gameObject.name);
		activateMenu(other);
	}
	
	void OnTriggerExit(Collider other) {
		Debug.Log("TriggerExited: " + other.gameObject.name);
		deactivateMenu(other);
	}

	private void activateMenu(Collider collider) {
		Menu menu = collider.gameObject.GetComponent<Menu>();

		if (menu != null) {
			activeMenu = menu;
			activeMenu.Activate();
		}
	}

	private void deactivateMenu(Collider collider) {
		Menu menu = collider.gameObject.GetComponent<Menu>();

		if (menu != null && menu == activeMenu) {
			activeMenu.Deactivate();
			activeMenu = null;
		}
	}
}
