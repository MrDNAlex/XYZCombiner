using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target object to orbit around
    public float rotationSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 20f;
    public float panSpeed = 20f;

    private float currentZoomDistance;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned for the orbital camera!");
        }

        currentZoomDistance = Vector3.Distance(transform.position, target.position);
    }

    void Update()
    {
        HandleMouseInput();
    }

    /// <summary>
    /// Handles Mouse Based Controls
    /// </summary>
    void HandleMouseInput()
    {
        UpdateOrbit();
        UpdatePan();
        UpdateZoom();

        // Update camera position based on zoom distance
        transform.position = target.position - transform.forward * currentZoomDistance;
    }

    /// <summary>
    /// Handles Camera Rotation around the Cursor 
    /// </summary>
    public void UpdateOrbit ()
    {
        // Orbit around the target
        if (target != null && Input.GetMouseButton(1)) // Right mouse button
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.RotateAround(target.position, Vector3.up, horizontal);
            transform.RotateAround(target.position, transform.right, vertical);
        }
    }

    /// <summary>
    /// Handles the Panning of the Cursor which leads to the panning of the Camera
    /// </summary>
    public void UpdatePan ()
    {
        // Pan the target
        if (Input.GetMouseButton(2)) // Middle mouse button
        {
            float deltaX = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float deltaY = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            // Move the target in the plane of the camera's forward and right vectors
            target.Translate(transform.right * -deltaX + transform.up * -deltaY, Space.World);
        }
    }

    /// <summary>
    /// Updates the Zoom level of the Camera
    /// </summary>
    public void UpdateZoom()
    {
        // Zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoomDistance = Mathf.Clamp(currentZoomDistance - scroll * zoomSpeed, minZoomDistance, maxZoomDistance);
    }
}
