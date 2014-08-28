using UnityEngine;
using System.Collections;

public abstract class Usable : MonoBehaviour 
{
    public GameObject playerRef;
    public PlayerStateMachine psm;

    public abstract void use();

    public abstract PlayerStateMachine.PlayerStateEnum getDesiredState();

    public void registerOnPlayer()
    {
        psm.registerUsable(this);
    }

    public void deregisterOnPlayer()
    {
        psm.deregisterUsable(this);
    }

    // Monobehaviour stuff
    protected virtual void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        psm = playerRef.GetComponent<PlayerStateMachine>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            Debug.Log("player entered usable trigger");
            registerOnPlayer();
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            Debug.Log("player left usable trigger");
            deregisterOnPlayer();
        }
    }
}
