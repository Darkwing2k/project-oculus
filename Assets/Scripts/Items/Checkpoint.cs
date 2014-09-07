using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public Checkpoint nextCheckpoint;

	public GameObject playerRef;
	public GameObject spiderRef;

	public GameObject[] objectsToReset;
	public GameObject[] templates;

	public GameObject[] referencingObjects;
	public GameObject[] referencingObjectTemplates;

	public Transform respawnPosPlayer;	
	public Transform respawnPosSpider;	

	public bool active = false;
	public bool triggered = false;
	
	void Start()
	{
		this.gameObject.SetActive (active);
		playerRef = FindObjectOfType<Player>().gameObject;
		spiderRef = FindObjectOfType<SpiderControl>().gameObject;
	}

	void OnTriggerEnter(Collider c)
	{
		if (!triggered && this.gameObject.activeSelf == true && playerRef.GetInstanceID () == c.gameObject.GetInstanceID ()) 
		{
			playerRef.GetComponent<Player> ().checkpoint = this;

			if(nextCheckpoint != null)
			{
				nextCheckpoint.gameObject.SetActive(true);
				nextCheckpoint.active = true;
			}
			triggered = true;
			active = false;
		}
	}

	public void Respawn()
	{
		for (int i = 0; i < templates.Length; i++) {

			Destroy(objectsToReset[i]);

			GameObject obj = (GameObject) Instantiate(templates[i], templates[i].transform.position, templates[i].transform.rotation);
			obj.name = templates[i].name;
			obj.SetActive(true);

			objectsToReset[i] = obj;
		}

		for (int i = 0; i < referencingObjects.Length; i++) {
			Destroy (referencingObjects[i]);

			GameObject obj = (GameObject)Instantiate (referencingObjectTemplates [i], referencingObjectTemplates [i].transform.position, referencingObjectTemplates [i].transform.rotation);
			obj.name = referencingObjectTemplates[i].name;
			obj.SetActive(true);

			referencingObjects[i] = obj;

			Resettable res = referencingObjects[i].GetComponent(typeof(Resettable)) as Resettable;
			res.SetReferencesOnRespawn();
		}

		Vector3 spawnPosSpider = respawnPosSpider.position;
		spiderRef.transform.position = spawnPosSpider;
		
		Quaternion spawnRotSpider = respawnPosSpider.rotation;
		spiderRef.transform.rotation = spawnRotSpider;

		Vector3 spawnPosPlayer = respawnPosPlayer.position;
		playerRef.transform.position = spawnPosPlayer;

		Quaternion spawnRotPlayer = respawnPosPlayer.rotation;
		playerRef.transform.rotation = spawnRotPlayer;
	}
}
