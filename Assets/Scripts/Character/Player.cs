using UnityEngine;
using System.Collections;

public class Player : Character
{
	private static Player instance;

	public static Player Instance {
		get { return instance; }
	}


	void Awake() {
		if (instance != null) {
			Debug.LogError("Another Player instance is already running!");
		}
		instance = this;

		this.maxVelocity = 100.0f;
		this.maxAccel = 100.0f;
		this.maxAngularVelocity = 30.0f;
		this.maxAngularAccel = 30.0f;
	}

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	protected override void Update() {

	}
}
