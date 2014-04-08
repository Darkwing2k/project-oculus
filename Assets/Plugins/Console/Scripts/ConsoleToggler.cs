using UnityEngine;
using System.Collections;

public class ConsoleToggler : MonoBehaviour {
    void Update () {
        if (Input.GetKeyDown(KeyCode.F12)) {
            ConsoleGUI.Instance.Toggle();
        }
    }
}
