using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
	
	public AudioSource flightSoundSource;
	public int Controls; //0 = GamePad, 1 = Keyboard
    public float scale;
	
	private string moveZGamepad = "LStickV";
	private string moveXGamepad = "LStickH";
	private string yawGamepad = "RStickH";
	private string ascGamepad = "Shoulder";
	
	private string moveZKeyboard = "ZAxis";
	private string moveXKeyboard = "XAxis";
	private string yawKeyboard = "YAngular";
	private string ascKeyboard = "YAxis";
	
	private string moveZ;
	private string moveX;
	private string yaw;
	private string asc;
	
	// === Variables for Yaw ===
	private float yawVelTarget = 0f;
	private float yawVelValue = 0f;
	private float yawVelMax = 1.8f;
	private float yawAcc = 3.6f;
	
	//=== Variables for Roll ===	
	private float rollAngleMax = 8;
	private float rollAngleFactor = 0f;
	
	// === Variables for XZ Movement ===
	private Vector3 moveAccDir = Vector3.zero;
    private Vector3 moveVelDir = Vector3.zero;
	private Vector3 xzRight = Vector3.zero;
    private Vector3 yAxis = Vector3.zero;
	
	private float moveVelTarget = 0f;
	private float moveVelValue = 0f;
	private float moveVelMax = 1.35f;
	private float moveAcc = 0.6f;
    private float moveAgility = 0.08f;
	
	// === Variables for Ascending/Descending ===
	private float ascVelTarget = 0f;
	private float ascVelValue = 0f;
	private float ascVelMax = 0.8f;
    private float ascVelMin = 0f;
	private float ascAcc = 1.3f;

    // === Variables for using Objects ===
    public GameObject liftable;
    public GameObject lifted;
	
	// Use this for initialization
	void Start () 
	{
        yAxis = new Vector3(0, 1, 0);

        moveVelMax *= scale;
        moveAcc *= scale;

        ascVelMax *= scale;
        ascAcc *= scale;

        ascVelMin = Mathf.Abs(Physics.gravity.y * Time.fixedDeltaTime);

		switch(Controls)
		{
			case 0: 
			{
				moveZ = moveZGamepad;
				moveX = moveXGamepad;
				yaw = yawGamepad;
				asc = ascGamepad;
				break;
			}
			case 1: 
			{
				moveZ = moveZKeyboard;
				moveX = moveXKeyboard;
				yaw = yawKeyboard;
				asc = ascKeyboard;
				break;
			}
		}
	}

    void FixedUpdate()
    {
        // == Calculating Movement Velocity ==============================
        float lStickV = Input.GetAxis(moveZ);
        float lStickH = Input.GetAxis(moveX);

        xzRight = this.transform.right;
        xzRight.y = 0;
        xzRight.Normalize();

        if (Mathf.Abs(lStickV) > 0.3f || Mathf.Abs(lStickH) > 0.3f)
        {
            moveAccDir = (this.transform.forward * lStickV) + (xzRight * lStickH);

            moveVelTarget = moveAccDir.magnitude;

            if (moveVelTarget > 1)
                moveVelTarget = 1;

            moveVelTarget *= moveVelMax;

            moveAccDir.Normalize();
        }
        else
        {
            moveAccDir = Vector3.zero;
            moveVelTarget = 0;
        }

        // 
        moveVelDir = ((moveAccDir - moveVelDir) * moveAgility) + moveVelDir;

        moveVelValue = calcValue(moveVelValue, moveAcc, moveVelTarget, 0.03f);

        // ===========================================================================

        // === Calculating Ascending/Descinding Velocity ==================
        float shoulder = Input.GetAxis(asc);

        if (Mathf.Abs(shoulder) > 0.3f)
        {
            ascVelTarget = shoulder * ascVelMax;

            flightSoundSource.pitch = 1 + (0.15f * shoulder);
        }
        else
        {
            ascVelTarget = ascVelMin;

            flightSoundSource.pitch = 1;
        }

        ascVelValue = calcValue(ascVelValue, ascAcc, ascVelTarget, 0.03f);
        // ===========================================================================

        // === Applying Movement & Ascending/Descing Velocity here ===================
        this.rigidbody.velocity = moveVelDir * moveVelValue + yAxis * ascVelValue;
        // ===========================================================================

        // === Calculating & Applying Yaw Velocity ==============================
        float rStickV = Input.GetAxis("RStickV");
        float rStickH = Input.GetAxis(yaw);

        if (Mathf.Abs(rStickV) > 0.3f)
        {
            //this.transform.Rotate(0.8f * rStickV, 0, 0, Space.Self);
        }

        if (Mathf.Abs(rStickH) > 0.3f)
        {
            yawVelTarget = rStickH * yawVelMax;
        }
        else
        {
            yawVelTarget = 0;
        }

        yawVelValue = calcValue(yawVelValue, yawAcc, yawVelTarget, 0.03f);

        this.transform.Rotate(0, yawVelValue, 0, Space.World);
        //this.rigidbody.angularVelocity = new Vector3(0, yawVelValue, 0);
        // ===========================================================================

        // === Calculating & Applying Roll Behaviour when flying curves ==============
        rollAngleFactor = (yawVelValue / yawVelMax) * (moveVelValue / moveVelMax);

        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, -rollAngleFactor * rollAngleMax);
        // ===========================================================================

        // === Debug Stuff ===========================================================

        //print (lStickV + " || " + lStickH);
        //print (rStickV + " || " + rStickH);
        //print (shoulder);
        //print (Physics.gravity.y);

        //print (this.transform.forward);

        //print (moveAccDir + " || " + moveAccDir.sqrMagnitude);

        //print("move> t: " + moveVelTarget + "| v: " + moveVelValue);

        //print ("yaw> t: " + yawVelTarget + "| v: " + yawVelValue);

        //print ("roll> t: " + rollVelTarget + "| v: " + rollVelValue);

        //print("asc> t: " + ascVelTarget + "| v: " + (Physics.gravity.y * Time.fixedDeltaTime + ascVelValue));

        //print(this.rigidbody.velocity.magnitude);

        //print(moveAccDir + "  " + moveVelDir);

        Debug.DrawRay(this.transform.position, moveAccDir * moveVelTarget, Color.red);
        Debug.DrawRay(this.transform.position, moveVelDir * moveVelValue, Color.green);

        //Debug.DrawRay(this.transform.position, this.transform.forward * 10, Color.yellow);
        // ===========================================================================
    }

	// Update is called once per frame
	void Update () 
	{
        if (lifted != null && Input.GetButtonDown("Use"))
        {
            print("releasing object");

            Destroy(lifted.GetComponent<SpringJoint>());
            Destroy(lifted.GetComponent<FixedJoint>());

            this.GetComponent<BoxCollider>().isTrigger = true;
            lifted.GetComponent<Collider>().isTrigger = false;

            lifted = null;
        }

        if (liftable != null && Input.GetButtonDown("Use"))
        {
            print("attaching object");

            // === Attach as SpringJoint ============================
            SpringJoint sj = liftable.AddComponent<SpringJoint>();
            sj.connectedBody = this.rigidbody;
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = new Vector3(0, -0.5f, 0);
            sj.spring = 30;
            sj.maxDistance = 0.5f;
            // ======================================================

            // === Attach as FixedJoint =============================
            //FixedJoint fj = liftable.AddComponent<FixedJoint>();
            //fj.connectedBody = this.rigidbody;
            // ======================================================

            liftable.GetComponent<Collider>().isTrigger = true;
            this.GetComponent<BoxCollider>().isTrigger = false;

            //liftable.transform.rotation = this.transform.rotation;
            //liftable.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.1f, this.transform.position.z);

            lifted = liftable;
            liftable = null;
        }
	}
	
	void OnCollisionEnter(Collision c)
	{
        foreach(ContactPoint cp in c.contacts)
        {
            print(cp.point.y - this.transform.position.y);
        }
	}

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Liftable" && c.rigidbody && lifted == null)
        {
            print("liftable object in range");
            liftable = c.gameObject;
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Liftable" && c.rigidbody && lifted == null)
        {
            print("lifted object left range");
            liftable = null;
        }
    }
	
	private float calcValue(float val, float acc, float target, float tolerance)
	{
		if(target - val > tolerance)
		{
			val += acc * Time.deltaTime;
		}
		else if(target - val < -tolerance)
		{
			val -= acc * Time.deltaTime;
		}
		else
		{
			val = target;
		}
		
		return val;
	}
}
