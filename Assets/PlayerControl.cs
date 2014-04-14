using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
	
	public AudioSource flightSoundSource;
	public int Controls; //0 = GamePad, 1 = Keyboard
	
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
	
	private float timer;
	
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
	private float moveVelMax = 15f;
	private float moveAcc = 15f;
	private float strafeAcc = 4f;
	
	// === Variables for Ascending/Descending ===
	private float ascVelTarget = 0f;
	private float ascVelValue = 0f;
	private float ascVelMax = 12f;
	private float ascAcc = 18f;
	
	// Use this for initialization
	void Start () 
	{
        yAxis = new Vector3(0, 1, 0);

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

            if (moveAccDir.sqrMagnitude > 1)
                moveAccDir.Normalize();
        }
        else
        {
            moveAccDir = Vector3.zero;
        }

        // 
        moveVelDir += moveAccDir * strafeAcc * Time.deltaTime;

        if (moveVelDir.sqrMagnitude > 1)
            moveVelDir.Normalize();

        // 
        moveVelTarget = moveAccDir.sqrMagnitude * moveVelMax;

        moveVelValue = calcValue(moveVelValue, moveAcc, moveVelTarget, 0.03f);

        if (moveVelValue == 0)
        {
            moveVelDir = Vector3.zero;
        }

        //this.transform.Translate(moveVelDir * moveVelValue, Space.World);
        //this.rigidbody.velocity = moveVelDir * moveVelValue * 50;
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


        // === Calculating Ascending/Descinding Velocity ==================
        float shoulder = Input.GetAxis(asc);

        if (Mathf.Abs(shoulder) > 0.3f)
        {
            //this.transform.Translate(0, maxAscentVel * shoulder, 0, Space.World);
            ascVelTarget = shoulder * ascVelMax;

            flightSoundSource.pitch = 1 + (0.15f * shoulder);
        }
        else
        {
            ascVelTarget = 0;

            flightSoundSource.pitch = 1;
        }

        ascVelValue = calcValue(ascVelValue, ascAcc, ascVelTarget, 0.03f);

        //this.transform.Translate(new Vector3(0,1,0) * ascVelValue, Space.World);
        // ===========================================================================

        // === Applying Movement & Ascending/Descing Velocity here ===================
        this.rigidbody.velocity = moveVelDir * moveVelValue + yAxis * ascVelValue + yAxis * Mathf.Abs(Physics.gravity.y) * Time.fixedDeltaTime;
        // ===========================================================================

        // === Debug Stuff ===========================================================

        //print (lStickV + " || " + lStickH);
        //print (rStickV + " || " + rStickH);
        //print (shoulder);
        //print (Physics.gravity.y);

        //print (this.transform.forward);

        //print (moveAccDir + " || " + moveAccDir.sqrMagnitude);

        //print ("move >> target: " + moveVelTarget + " | value: " + moveVelValue);

        //print ("yaw >> target: " + yawVelTarget + " | value: " + yawVelValue);

        //print ("roll >> target: " + rollVelTarget + " | value: " + rollVelValue);

        //print("asc >> target: " + ascVelTarget + " | value: " + ascVelValue);

        //print(this.rigidbody.velocity.y);

        Debug.DrawRay(this.transform.position, moveAccDir * 4, Color.red);
        Debug.DrawRay(this.transform.position, moveVelDir * 8, Color.yellow);
        Debug.DrawRay(this.transform.position, xzRight * 2, Color.white);

        //Debug.DrawRay(this.transform.position, this.transform.forward * 10, Color.yellow);
        // ===========================================================================
    }

	// Update is called once per frame
	void Update () 
	{			
		if(timer < 0)
		{
			
			
			timer = -1;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
	
	void OnCollisionEnter(Collision c)
	{
		
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
