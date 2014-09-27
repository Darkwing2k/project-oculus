using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{

    public AudioSource voiceSource;
    public VoiceTrigger tutVoiceTrigger;

    public bool useOculus;
    public bool useGamepad;

    public Camera[] normalCameras;
    public OVRCameraController vrCamera;

    public Transform eyeCenter;

    public Checkpoint checkpoint;

    public bool allowNoClip;
    public bool noClip;
    public bool tutorial;

	// Use this for initialization
	void Start () 
    {
        noClip = false;

        if (useOculus)
        {
            vrCamera.gameObject.SetActive(true);

            foreach (Camera c in normalCameras)
            {
                c.gameObject.SetActive(false);
            }
        }
        else
        {
            vrCamera.gameObject.SetActive(false);

            foreach (Camera c in normalCameras)
            {
                c.gameObject.SetActive(true);
            }
        }
	}

	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            useGamepad = !useGamepad;
        }
        if (tutorial)
        {
            if(GameManager.Instance != null)
                GameManager.Instance.Fader.FadeIn();

            gameObject.GetComponent<PlayerStateMachine>().changePlayerState(PlayerStateMachine.PlayerStateEnum.TUTORIAL);

            tutVoiceTrigger.trigger();

            tutorial = false;
        }

        if (allowNoClip)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button4) && Input.GetKeyDown(KeyCode.Joystick1Button5) || Input.GetKeyDown(KeyCode.Space))
            {
                noClip = !noClip;
                this.gameObject.collider.enabled = !noClip;
            }
        }        

        if (Input.GetKeyDown(KeyCode.R))
        {
            checkpoint.Respawn();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            gameObject.GetComponent<PlayerStateMachine>().changePlayerState(PlayerStateMachine.PlayerStateEnum.TUTORIAL);
        }
	}
}
