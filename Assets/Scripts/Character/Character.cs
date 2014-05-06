using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour {
	// linear motion kinematics
	protected Vector3 velocity;
	protected Vector3 accel;

	// angular motion kinematics
	protected Vector3 angularVelocity;
	protected Vector3 angularAccel;

	// velocity and acceleration limits
	protected float maxVelocity;
	protected float maxAngularVelocity;
	protected float maxAccel;
	protected float maxAngularAccel;

	protected bool useRigidbody;

	public Vector3 Position {
		get { return transform.position; }
		set { transform.position = value;  }
	}

	public Vector3 Velocity {
		get { return this.velocity; }
		set { this.velocity = value; }
	}

	public Vector3 Acceleration {
		get { return this.accel; }
		set { this.accel = value; }
	}

	public Vector3 AngularVelocity {
		get { return this.angularVelocity; }
		set { this.angularVelocity = value; }
	}

	public Vector3 AngularAcceleration {
		get { return this.angularAccel; }
		set { this.angularAccel = value; }
	}

	public float MaxVelocity {
		get { return this.maxVelocity;  }
		set { this.maxVelocity = value; }
	}

	public float MaxAngularVelocity {
		get { return this.maxAngularVelocity; }
		set { this.maxAngularVelocity = value; }
	}

	public float MaxAcceleration {
		get { return this.maxAccel; }
		set { this.maxAccel = value; }
	}

	public float MaxAngularAcceleration {
		get { return this.maxAngularAccel; }
		set { this.maxAngularAccel = value; }
	}

	#region --- Public Methods ---

	public void LookAt(Vector3 target) {
		this.transform.LookAt(target);
	}

	public void LookAt(Transform target) {
		this.transform.LookAt(target);
	}

	public void LookAtSlerp(Vector3 target, float rotationSpeed) {
		Vector3 vdistance = target - this.transform.position;
		Quaternion rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(vdistance), rotationSpeed * Time.deltaTime);
		this.transform.rotation = rotation;
	}

	#endregion

	#region === Unity Stuff ===
	/*
	// Use this for initialization
	protected virtual void Start () {
	}
	*/
	// Update is called once per frame
	protected virtual void Update () {
		this.velocity = Time.deltaTime * this.accel;
		this.angularVelocity = Time.deltaTime * this.angularAccel;

		if (useRigidbody) {
			this.rigidbody.velocity = this.velocity;
			this.rigidbody.angularVelocity = this.angularVelocity;
		}
	}
	
	#endregion

}
