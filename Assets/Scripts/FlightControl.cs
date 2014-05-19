using UnityEngine;
using System.Collections;

public class FlightControl : MonoBehaviour 
{
    private Player playerRef;

    #region Control Variables
    public bool useGamepad; //0 = GamePad, 1 = Keyboard

    private bool controlsActivated;
    public bool ControlsActivated
    {
        set
        {
            controlsActivated = value;
            playerRef.flightControlActive = value;
        }
        get { return controlsActivated; }

    }

    // for gamepad, use these input axes
    private string moveZGamepad = "LStickV";
    private string moveXGamepad = "LStickH";
    private string yawGamepad = "RStickH";
    private string ascGamepad = "Shoulder";

    // for keyboard, use these input axes
    private string moveZKeyboard = "ZAxis";
    private string moveXKeyboard = "XAxis";
    private string yawKeyboard = "YAngular";
    private string ascKeyboard = "YAxis";

    // contain the names of the used axes
    private string moveZ;
    private string moveX;
    private string yaw;
    private string asc; 
    #endregion

    #region Y-Rotate Variables
    private float yawVelTarget = 0f; // Desired Velocity for Y-Rotation
    private float yawVelValue = 0f; // Current Velocity for Y-Rotation
    private float yawVelMax = 1.2f; // Maximum Velocity for Y-Rotation
    private float yawAcc = 3.6f; // Acceleration for Y-Rotation
    #endregion

    #region Z-Rotate Variables
    // local Z-Rotation is applied when flying in steep curves with high velocity
    private float rollAngleMax = 0; // Maximum Z-Rotation (0 for oculus. prevents simulator sickness)
    private float rollAngleFactor = 0f; // Lies between 0 (= 0°) and 1 (= rollAngleMax). Calculated with the yawVelValue and the moveVelValue
    #endregion

    #region XZ-Movement Variables
    private Vector3 moveAccDir = Vector3.zero; // Accelerate in this direction (Desired Direction)
    private Vector3 moveVelDir = Vector3.zero; // Actually moving in this direction (Current Direction)
    private Vector3 xzRight = Vector3.zero; // Vector for strafing left/right. Y value is always 0

    private float moveVelTarget = 0f; // Desired Velocity for XZ-Movement
    private float moveVelValue = 0f; // Current Velocity for XZ-Movement
    private float moveVelMax = 1.15f; // Maximum Velocity for XZ-Movement
    private float moveAcc = 0.6f; // Acceleration in XZ-Direction
    private float moveAgility = 0.08f; // How fast a new Direction is applied (Changerate from Current to Desired Direction)
    #endregion

    #region Y-Movement Variables
    private Vector3 yAxis = Vector3.zero; // Up Vector. Is always (0,1,0)

    private float ascVelTarget = 0f; // Desired Velocity for Y-Movement
    private float ascVelValue = 0f; // Current Velocity for Y-Movement
    private float ascVelMax = 0.8f; // Maximum Velocity for Y-Movement
    private float ascVelMin = 0f; // Minimum Velocity for Y-Movement (equals gravity for hover flight)
    private float ascAcc = 1.3f; // Acceleration in Y-Direction
    #endregion

    #region Other Variables
		public float scale;
        public AudioSource flightSoundSource;
	#endregion

	// Use this for initialization
	void Start () 
    {
        playerRef = this.GetComponent<Player>();
        ControlsActivated = true;

        yAxis = new Vector3(0, 1, 0);

        moveVelMax *= scale;
        moveAcc *= scale;

        ascVelMax *= scale;
        ascAcc *= scale;

        ascVelMin = Mathf.Abs(Physics.gravity.y * Time.fixedDeltaTime);

        if(useGamepad)
        {
            moveZ = moveZGamepad;
            moveX = moveXGamepad;
            yaw = yawGamepad;
            asc = ascGamepad;
        }
        else
        {
            moveZ = moveZKeyboard;
            moveX = moveXKeyboard;
            yaw = yawKeyboard;
            asc = ascKeyboard;
        }
	}

    void FixedUpdate()
    {
        float lStickV = 0;
        float lStickH = 0;

        float shoulder = 0;

        float rStickV = 0;
        float rStickH = 0;

        // == Get all Joystick Positions =================================
        if (controlsActivated)
        {
            lStickV = Input.GetAxis(moveZ);
            lStickH = Input.GetAxis(moveX);

            shoulder = Input.GetAxis(asc);

            rStickV = Input.GetAxis("RStickV");
            rStickH = Input.GetAxis(yaw);
        }
        // ===============================================================

        /*
         * All velocity calculations basically work the same way:
         * A desired velocity is calculated by the position of the associated input-axis.
         * After that, the actual velocity tries to reach the desired velocity with the given acceleration.
         * Then the current velocity is applied to the rigidbody.
         * 
         * In case of the XZ-Movement, the same also happens for the desired/current direction
         **/

        // == Calculating XZ-Movement Velocity ==============================
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

        moveVelDir = ((moveAccDir - moveVelDir) * moveAgility) + moveVelDir;

        moveVelValue = calcValue(moveVelValue, moveAcc, moveVelTarget, 0.03f);

        // ===========================================================================

        // === Calculating Ascending/Descinding Velocity ==================
        if (Mathf.Abs(shoulder) > 0.3f)
        {
            ascVelTarget = shoulder * ascVelMax;
            
            if(flightSoundSource != null)
                flightSoundSource.pitch = 1 + (0.15f * shoulder);
        }
        else
        {
            ascVelTarget = ascVelMin;

            if (flightSoundSource != null)
                flightSoundSource.pitch = 1;
        }

        ascVelValue = calcValue(ascVelValue, ascAcc, ascVelTarget, 0.03f);
        // ===========================================================================

        // === Applying Movement & Ascending/Descing Velocity here ===================
        this.rigidbody.velocity = moveVelDir * moveVelValue + yAxis * ascVelValue;
        // ===========================================================================

        // === Calculating & Applying Yaw Velocity ==============================
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
        #region Debug
		
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
	    #endregion
        // ===========================================================================
    }

	// Update is called once per frame
	void Update () 
    {
	
	}

    private float calcValue(float val, float acc, float target, float tolerance)
    {
        if (target - val > tolerance)
        {
            val += acc * Time.deltaTime;
        }
        else if (target - val < -tolerance)
        {
            val -= acc * Time.deltaTime;
        }
        else
        {
            val = target;
        }

        return val;
    }

    void OnCollisionEnter(Collision c)
    {
        print(c.collider.name);
    }
}
