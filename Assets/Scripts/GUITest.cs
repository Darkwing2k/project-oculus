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

	// Use this for initialization
	void Start () 
    {
        vrCamera = this.GetComponent<Player>().vrCamera;

        Resolution r = Screen.currentResolution;

        ovrGui = new OVRGUI();
        ovrGui.SetCameraController(ref vrCamera);
        //ovrGui.SetDisplayResolution(Screen.width, Screen.height);
        //ovrGui.SetPixelResolution(Screen.width, Screen.height);

        ovrGui.GetDisplayResolution(ref displayW, ref displayH);
        ovrGui.GetPixelResolution(ref pixelW, ref pixelH);

        print("Screen w/h: " + Screen.width + " " + Screen.height);
        print("Display w/h: " + displayW + " " + displayH);
        print("Pixel w/h: " + pixelW + " " + pixelH);        
	}

    void OnGUI()
    {

        Resolution r = Screen.currentResolution;

        int imgW = img.width / 2;
        int imgH = img.height / 2;

        int posX = Screen.width / 2 - imgW;
        int posY = Screen.height / 2 - imgH;

        if (oculusGUI)
        {
            ovrGui.StereoDrawTexture(displayW/2, displayH/2, img.width, img.height, ref img, Color.yellow);
            ovrGui.StereoDrawTexture(posX - distance, posY, img.width, img.height, ref img, Color.red);
        }
        else
        {
            Rect rect1 = new Rect(posX + distance, posY, img.width, img.height);
            Rect rect2 = new Rect(posX - distance, posY, img.width, img.height);

            GUI.color = Color.green;
            GUI.DrawTexture(rect1, img);
            GUI.DrawTexture(rect2, img);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            distance += 1;

            print(distance);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            distance -= 1;

            print(distance);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            oculusGUI = true;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            oculusGUI = false;
        }
	}
}
