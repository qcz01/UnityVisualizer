using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dragSpeed = 0.01f;
    private Vector3 dragOrigin;
  
    private float scrollSpeed = 5;
    private Camera ZoomCamera;

    void Start()
    {
        ZoomCamera = Camera.main;
    
    }

    private void ZoomIn()
    {
        if (ZoomCamera.orthographic)
        {
            ZoomCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }
        else
        {
            ZoomCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        }

    }

    private void DragMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        transform.Translate(move, Space.World);

    }


    void Update()
    {

        ZoomIn();
        DragMouse();
     

       
    }
}
