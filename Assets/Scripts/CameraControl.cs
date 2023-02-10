using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dragSpeed = 0.0001f;
    private Vector3 dragOrigin;
    private int cameraXmin, cameraXmax, cameraYmin, cameraYmax;
    private float scrollSpeed = 5;
    private Camera ZoomCamera;

    void Start()
    {
        ZoomCamera = Camera.main;
    }


    public void setCameraBound(int xmin,int xmax,int ymin,int ymax)
    {
        cameraXmax = xmax;
        cameraXmin = xmin;
        cameraYmin = ymin;
        cameraYmax = ymax;
    }
    private void ZoomIn()
    {
        if (ZoomCamera.orthographic)
        {
            //Debug.Log(ZoomCamera.orthographicSize+"   "+transform.position);
            ZoomCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }
        else
        {
      
            ZoomCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }

    }

    private void DragMouse()
    {

        if (Input.GetKey("space"))
        {
            ZoomCamera.orthographicSize = cameraYmax / 2.0f + 1.0f;
            ZoomCamera.transform.position = new Vector3(cameraXmax / 2.0f - 0.5f, cameraYmax / 2.0f - 0.5f, -100.0f);
            return;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;
        




        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);

        Vector3 capPosition = transform.position;
        capPosition.x = Mathf.Clamp(capPosition.x, 0, cameraXmax);
        capPosition.y = Mathf.Clamp(capPosition.y, 0, cameraYmax);
        transform.position = capPosition;

    }


    void Update()
    {

        ZoomIn();
        DragMouse();
     

       
    }
}
