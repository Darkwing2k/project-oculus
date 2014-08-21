using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerState : MonoBehaviour
{
    public GameObject playerRef;
    public Player playerScript;
    public PlayerStateMachine psm;

    public abstract void handleInput(PlayerStateMachine.InputType inputT, List<Usable> usables);

    public abstract void enterState(Usable closest, List<Usable> all);

    public abstract void exitState();

    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerRef.GetComponent<Player>();
        psm = playerRef.GetComponent<PlayerStateMachine>();
    }
}


