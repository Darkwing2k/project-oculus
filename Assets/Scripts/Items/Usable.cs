using UnityEngine;
using System.Collections;

public abstract class Usable : MonoBehaviour 
{
    public GameObject playerRef;
    public PlayerStateMachine psm;

    public bool playerClose;
    public bool registered;

    private float raycastTimer = 0;
    private float raycastTimerMax = 0.3f;

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

        playerClose = false;
        registered = false;
    }

    void Update()
    {
        if (playerClose)
        {
            raycastTimer += Time.deltaTime;

            if (raycastTimer > raycastTimerMax)
            {
                raycastTimer = 0;

                RaycastHit hit;

                if (Physics.Raycast(this.transform.position, playerRef.transform.position - this.transform.position, out hit, 10))
                {
                    if (hit.collider.gameObject.tag.Equals("Player"))
                    {
                        if (!registered)
                        {
                            Debug.Log("player entered usable trigger");
                            registerOnPlayer();

                            registered = true;
                        }
                    }
                    else
                    {
                        if (registered)
                        {
                            Debug.Log("player left usable trigger");
                            deregisterOnPlayer();

                            registered = false;
                        }
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            playerClose = true;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.GetInstanceID() == playerRef.GetInstanceID())
        {
            playerClose = false;
            registered = false;

            Debug.Log("player left usable trigger");
            deregisterOnPlayer();
        }
    }
}
