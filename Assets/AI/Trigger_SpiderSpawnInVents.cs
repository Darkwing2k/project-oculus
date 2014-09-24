using UnityEngine;
using System.Collections;

public class Trigger_SpiderSpawnInVents : MonoBehaviour {

    SpiderControl control;
    public RoomInfo ventsRoomInfo;
    public GameObject spawnPointInVents;

    private bool alreadyTriggered;

	void Start () {
        control = GameObject.FindGameObjectWithTag("Spider").GetComponent<SpiderControl>();
        alreadyTriggered = false;
	}

    void OnTriggerEnter(Collider pCollider)
    {
        if (!alreadyTriggered && pCollider.tag.Equals("Player"))
        {
            control.WaitingOnCheckpoint = true;
            control.generalBehaviour.spawnSpider(spawnPointInVents.transform.position, ventsRoomInfo, true);

            control.generalBehaviour.soundSource.clip = control.generalBehaviour.jumpSound;
            control.generalBehaviour.soundSource.loop = false;
            control.generalBehaviour.soundSource.Play();

            control.WaitingOnCheckpoint = false;

            alreadyTriggered = true;
        }
    }
}
