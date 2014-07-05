using UnityEngine;
using System.Collections;

public class SetActiveOnEnter : MonoBehaviour 
{
    public GameObject playerRef;

    public GameObject[] toSet;

    public bool setValue;
    public bool used;

	// Use this for initialization
	void Start () 
    {
        playerRef = FindObjectOfType<Player>().gameObject;
        used = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnTriggerEnter(Collider c)
    {
        if (!used)
        {
            if (playerRef.GetInstanceID() == c.gameObject.GetInstanceID())
            {
                foreach (GameObject current in toSet)
                {
                    current.SetActive(setValue);
                }
            }
            used = true;
        }
    }
}
