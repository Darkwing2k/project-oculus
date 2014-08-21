using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateMachine : MonoBehaviour 
{
    public GameObject buttonInfo;
    public List<Usable> currentUsables;

    public enum PlayerStateEnum { IDLE, LIFT, LASER, HACK };
    public PlayerStateEnum currentState;

    public enum InputType { USE };

    public Dictionary<PlayerStateEnum, PlayerState> states;

    private bool flightControlActive;
    public bool FlightControlEnabled
    {
        get { return flightControlActive; }
        set { flightControlActive = value; }
    }

	// Use this for initialization
	void Start () 
    {  
        states = new Dictionary<PlayerStateEnum, PlayerState>();

        states.Add(PlayerStateEnum.IDLE, gameObject.GetComponent<IdlePlayerState>());
        states.Add(PlayerStateEnum.LIFT, gameObject.GetComponent<LiftState>());
        states.Add(PlayerStateEnum.LASER, gameObject.GetComponent<LaserState>());
        states.Add(PlayerStateEnum.HACK, gameObject.GetComponent<HackState>());

        currentState = PlayerStateEnum.IDLE;

        FlightControlEnabled = true;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(Input.GetButtonDown("Use"))
        {
            states[currentState].handleInput(InputType.USE, currentUsables);
        }
	}

    public void registerUsable(Usable u)
    {
        if (!currentUsables.Contains(u))
        {
            Debug.Log("added usable");
            currentUsables.Add(u);
            buttonInfo.SetActive(true);
        }
    }

    public void deregisterUsable(Usable u)
    {
        Debug.Log("removed usable");
        currentUsables.Remove(u);

        if (currentUsables.Count == 0)
        {
            buttonInfo.SetActive(false);
        }
    }

    public void changePlayerState(PlayerStateEnum newState, Usable passOnUsable)
    {
        Debug.Log("playerstate changed to: " + newState);

        states[currentState].exitState();

        currentState = newState;

        states[currentState].enterState(passOnUsable, currentUsables);
    }

    public void changePlayerState(PlayerStateEnum newState)
    {
        Debug.Log("playerstate changed to: " + newState);

        states[currentState].exitState();

        currentState = newState;

        states[currentState].enterState(null, currentUsables);
    }
}
