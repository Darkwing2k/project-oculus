using UnityEngine;
using System.Collections;

public class ActivateTrigger : MonoBehaviour 
{
    public GameObject playerRef;

    public GameObject[] toSet;

    public bool valueToBeSet;
    public bool used;

    public bool triggerOnButton;
    private bool playerInside;

    // Use this for initialization
    void Start()
    {
        playerRef = FindObjectOfType<Player>().gameObject;
        used = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider c)
    {
        if (!used && playerRef.GetInstanceID() == c.gameObject.GetInstanceID())
        {
            if (!triggerOnButton)
            {
                foreach (GameObject current in toSet)
                {
                    current.SetActive(valueToBeSet);
                }

                used = true;
            }

            playerInside = true;
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (playerInside && Input.GetButtonDown("Use"))
        {
            foreach (GameObject current in toSet)
            {
                current.SetActive(valueToBeSet);
            }

            used = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (playerRef.GetInstanceID() == c.gameObject.GetInstanceID())
        {
            playerInside = false;
        }
    }
}
