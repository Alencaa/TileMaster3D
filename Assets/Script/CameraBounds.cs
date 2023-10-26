using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private Camera mainCamera;
    private float cameraHeight;
    private float cameraWidth;
    private float boundLeft;
    private float boundRight;
    private float boundBottom;
    private float boundTop;
    public static CameraBounds instance;
    private void Start()
    {
        mainCamera = Camera.main;
        instance = this;

        // Calculate the height and width of the camera frustum at the far clip plane distance
        cameraHeight = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCamera.fieldOfView) * mainCamera.farClipPlane;
        cameraWidth = cameraHeight * mainCamera.aspect;

        Debug.Log("Camera Height: " + cameraHeight);
        Debug.Log("Camera Width: " + cameraWidth);

        // Calculate the camera bounds and extents
        Vector3 cameraPosition = mainCamera.transform.position;
        boundLeft = cameraPosition.x - (cameraWidth * 0.5f);
        boundRight = cameraPosition.x + (cameraWidth * 0.5f);
        boundBottom = cameraPosition.y - (cameraHeight * 0.5f);
        boundTop = cameraPosition.y + (cameraHeight * 0.5f);

        Debug.Log("Camera Bounds Left: " + boundLeft);
        Debug.Log("Camera Bounds Right: " + boundRight);
        Debug.Log("Camera Bounds Bottom: " + boundBottom);
        Debug.Log("Camera Bounds Top: " + boundTop);
    }
    public float BoundLeft
    {
        get { return boundLeft; }
        set { boundLeft = value; }
    }
    public float BoundRight
    {
        get { return boundRight; }
        set { boundRight = value; }
    }
    public float BoundBottom
    {
        get { return boundBottom; }
        set { boundBottom = value; }
    }
    public float BoundTop
    {
        get { return boundTop; }
        set { boundTop = value; }
    }
}