using UnityEngine;
using System.Collections;

public class CrosshairBehaviour : MonoBehaviour 
{
    public Player playerRef;

    private RenderTexture crosshairRenderTexture;
    public Texture crosshairImg;

    private float desiredCrosshairDist;
    public float crosshairDistChangeRate;
    public float maxDistance;
    public float gapToWall;

	// Use this for initialization
	void Start () 
    {
        crosshairRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);

        this.renderer.material.mainTexture = crosshairRenderTexture;

        Vector3 localPos = this.transform.localPosition;
        Vector3 localRot = this.transform.localEulerAngles;

        this.transform.parent = playerRef.eyeCenter;

        this.transform.localPosition = localPos;
        this.transform.localEulerAngles = localRot;



        this.gameObject.renderer.enabled = false;
	}

    void OnGUI()
    {
        RenderTexture previous = RenderTexture.active;

        RenderTexture.active = crosshairRenderTexture;
        GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));

        GUI.DrawTexture(new Rect(0, 0, crosshairRenderTexture.width, crosshairRenderTexture.height), crosshairImg);

        RenderTexture.active = previous;

        this.gameObject.renderer.enabled = true;
    }

	// Update is called once per frame
	void Update () 
    {

	}

    public void crosshairAdaptDistance(RaycastHit hit)
    {
        float dist = (hit.point - playerRef.eyeCenter.position).magnitude;

        if (dist > maxDistance)
            dist = maxDistance;

        desiredCrosshairDist = dist - gapToWall;

        float diff = desiredCrosshairDist - this.transform.localPosition.z;

        if (diff > crosshairDistChangeRate)
        {
            this.transform.localPosition += new Vector3(0, 0, crosshairDistChangeRate);
        }
        else if (diff < -crosshairDistChangeRate)
        {
            this.transform.localPosition -= new Vector3(0, 0, crosshairDistChangeRate);
        }
        else
        {
            Vector3 tmp = this.transform.localPosition;
            this.transform.localPosition = new Vector3(tmp.x, tmp.y, desiredCrosshairDist);
        }
    }

    public void snapToTarget(Vector3 targetPosition)
    {
        Vector3 eyeCenterToTarget = targetPosition - playerRef.eyeCenter.position;

        this.transform.parent = null;
        this.transform.position = playerRef.eyeCenter.position + eyeCenterToTarget;
    }
}
