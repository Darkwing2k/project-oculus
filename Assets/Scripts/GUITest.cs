using UnityEngine;
using System.Collections;

public class GUITest : MonoBehaviour 
{
    private OVRCameraController vrCamera;
    public Texture img;
    public string txt;
    private int distance = 370;

    private bool oculusGUI = true;

    public OVRGUI ovrGui;

    private float displayW = 0;
    private float displayH = 0;

    private float pixelW = 0;
    private float pixelH = 0;

    private int testX = 640;
    private int testY = 310;

    private RenderTexture GUIRenderTexture;
    private GameObject GUIRenderObject;

    public GameObject myRenderObject;
    public RenderTexture myRenderTexture;

	// Use this for initialization
	void Start () 
    {
        vrCamera = this.GetComponent<Player>().vrCamera;

        Resolution r = Screen.currentResolution;

        ovrGui = new OVRGUI();
        ovrGui.SetCameraController(ref vrCamera);

        GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
        GUIRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);

        ovrGui.SetPixelResolution(Screen.width, Screen.height);
        //GUIHelper.SetDisplayResolution(OVRDevice.HResolution, OVRDevice.VResolution);
        ovrGui.SetDisplayResolution(1280.0f, 800.0f);

        GUIRenderObject.renderer.material.mainTexture = GUIRenderTexture;

        // Grab transform of GUI object
        Transform t = GUIRenderObject.transform;
        // Attach the GUI object to the camera
        vrCamera.AttachGameObjectToCamera(ref GUIRenderObject);
        // Reset the transform values (we will be maintaining state of the GUI object
        // in local state)
        OVRUtils.SetLocalTransform(ref GUIRenderObject, ref t);
        // Deactivate object until we have completed the fade-in
        // Also, we may want to deactive the render object if there is nothing being rendered
        // into the UI
        // we will move the position of everything over to the left, so get
        // IPD / 2 and position camera towards negative X
        Vector3 lp = GUIRenderObject.transform.localPosition;
        float ipd = 0.0f;
        vrCamera.GetIPD(ref ipd);
        lp.x -= ipd * 0.5f;
        GUIRenderObject.transform.localPosition = lp + new Vector3(0, 0, -0.45f);
        

        GUIRenderObject.SetActive(false);

        print("GUIRenderTexture size: " + GUIRenderTexture.width + " " + GUIRenderTexture.height);
        print("CrosshairTexture size: " + img.width + " " + img.height);
        print("Crosshair Position : " + testX + " " + testY);


        myRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);

        myRenderObject.renderer.material.mainTexture = myRenderTexture;

        myRenderObject.SetActive(false);
	}

    void OnGUI()
    {
        GUIRenderObject.SetActive(true);

        // Cache current active render texture
        RenderTexture previousActive = RenderTexture.active;

        // if set, we will render to this texture
        if (GUIRenderTexture != null)
        {
            RenderTexture.active = GUIRenderTexture;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }

        //ovrGui.StereoDrawTexture(490, 300, img.width, img.height, ref img, Color.yellow);
        //ovrGui.StereoDrawTexture(testX - 3, testY - 3, 6, 6, ref img, Color.red);


        myRenderObject.SetActive(true);

        // Restore active render texture
        RenderTexture.active = previousActive;

        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = myRenderTexture;
        GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));



        GUI.DrawTexture(new Rect(0, 0, myRenderTexture.width, myRenderTexture.height), img);
        //ovrGui.StereoDrawTexture(0, 0, myRenderTexture.width, myRenderTexture.height, ref img, Color.yellow);

        RenderTexture.active = previous;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            distance += 1;
            testY -= 1;

            //print(testY);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            distance -= 1;
            testY += 1;

            //print(testY);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            testX -= 1;

            //print(testX);
            //oculusGUI = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            testX += 1;

            //print(testX);
            //oculusGUI = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            print(testX);
            print(testY);
        }
	}
}
