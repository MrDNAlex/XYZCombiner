using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformManager
{
    /// <summary>
    /// Enum to diffirentiate between Transformation Actions
    /// </summary>
    public enum Transformation
    {
        None,
        Translation,
        Rotation,
        Scaling
    }

    /// <summary>
    /// Boolean Toggle Determining if Rotation can now be Applied
    /// </summary>
    private bool _goodToRotate = false;

    /// <summary>
    /// Counter to determine how many loops have happened for the Rotation
    /// </summary>
    private int _count = 0;

    /// <summary>
    /// Plane that used to determine Translation and Rotation Deltas
    /// </summary>
    public GameObject TranslationPlane { get; set; }

    /// <summary>
    /// Selected Game Object that will have the Transformations Applied to
    /// </summary>
    public GameObject SelectedObject { get; set; }

    /// <summary>
    /// Previous Delta Vector between Mouse and Object Position for Rotation Transformation
    /// </summary>
    public Vector3 LastDelta { get; set; }

    /// <summary>
    /// Delegate Function type that references to a function to Update a GUI
    /// </summary>
    public delegate void UpdateUI();

    /// <summary>
    /// Delegate Function that references a Function to Update the GUI
    /// </summary>
    public UpdateUI UpdateGUI { get; set; }

    /// <summary>
    /// Enum describing the current Transformation Action occuring
    /// </summary>
    public Transformation TransformationAction { get; set; }

    /// <summary>
    /// Layer Mask to Isolate Raycasts to the TranslationPlane
    /// </summary>
    public LayerMask TranslationLayerMask { get { return LayerMask.GetMask("TranslationPlane"); } }

    /// <summary>
    /// Initializes the Transformation Manager
    /// </summary>
    public TransformManager()
    {
        if (GameObject.Find("TranslationPlane") == null)
        {
            //Not Functional
            TranslationPlane = new GameObject("Plane");
            TranslationPlane.name = "TranslationPlane";
            TranslationPlane.layer = LayerMask.NameToLayer("TranslationPlane");

            TranslationPlane.AddComponent<MeshFilter>();
            TranslationPlane.AddComponent<MeshRenderer>();
            TranslationPlane.AddComponent<MeshCollider>();

            TranslationPlane.GetComponent<MeshCollider>().sharedMesh = TranslationPlane.GetComponent<MeshFilter>().mesh;
            TranslationPlane.GetComponent<Transform>().localScale = new Vector3(10, 1, 10);
            TranslationPlane.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            TranslationPlane = GameObject.Find("TranslationPlane");
        }
    }

    /// <summary>
    /// Rotates the Selected Object
    /// </summary>
    public void RotateSelectedObject()
    {
        (Vector3 currentMousePosition, bool success) = GetRayPosition(TranslationLayerMask);
        Vector3 deltaMousePosition = currentMousePosition - SelectedObject.transform.position;

        float angle = Vector3.Angle(LastDelta, deltaMousePosition);
        Vector3 rotationAxis = Vector3.Cross(LastDelta, deltaMousePosition).normalized;

        if (_goodToRotate)
            SelectedObject.transform.Rotate(rotationAxis, angle, Space.World);

        LastDelta = deltaMousePosition;
    }

    /// <summary>
    /// Translates the Selected Object
    /// </summary>
    public void TranslateSelectedObject()
    {
        Vector3 currentMousePosition = GetRayPosition(TranslationLayerMask).position;

        SelectedObject.transform.position = currentMousePosition;
    }

    /// <summary>
    /// Sends a Ray Cast to the Translation plane and Receives the hit position and a boolean confirming if the ray cast was successful
    /// </summary>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public (Vector3 position, bool success) GetRayPosition(LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float maxRaycastDistance = 1000000000000;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
        {
            if (hit.collider != null)
            {
                return (hit.point, true);
            }
            else
                return (Vector3.zero, false);
        }
        else
            return (Vector3.zero, false);
    }

    /// <summary>
    /// Aligns the Translation Planes Normal Vector to be in parallel with the Camera
    /// </summary>
    void AlignWithCameraDirection()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            TranslationPlane.transform.up = -1 * cameraForward;
            TranslationPlane.transform.position = SelectedObject.transform.position;
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }

    /// <summary>
    /// Sets the Selected Object to Apply Transformations to
    /// </summary>
    /// <param name="obj"></param>
    public void SetSelectedObj(GameObject obj)
    {
        SelectedObject = obj;
    }

    /// <summary>
    /// Sets the Delegate Update GUI Function
    /// </summary>
    /// <param name="UpdateFunc"></param>
    public void SetUpdateFunction(UpdateUI UpdateFunc)
    {
        UpdateGUI = UpdateFunc;
    }

    /// <summary>
    /// Toggles the Transformation Action Mode
    /// </summary>
    /// <param name="transformation"></param>
    private void ToggleTransformationMode(Transformation transformation)
    {
        if (TransformationAction == transformation)
            TransformationAction = Transformation.None;
        else
            TransformationAction = transformation;
    }

    /// <summary>
    /// Handles Keybinds and Inputs to change Transformation Actions
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleTransformationMode(Transformation.None);
            UpdateGUI?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTransformationMode(Transformation.Translation);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleTransformationMode(Transformation.Rotation);
            _goodToRotate = false;
            _count = 0;
        }
    }

    /// <summary>
    /// Updates the Transformations that Occur
    /// </summary>
    public void UpdateTransformations()
    {
        HandleInput();

        if (TransformationAction == Transformation.Translation)
        {
            AlignWithCameraDirection();
            TranslateSelectedObject();
            UpdateGUI?.Invoke();
        }

        if (TransformationAction == Transformation.Rotation)
        {
            AlignWithCameraDirection();
            RotateSelectedObject();
            _count++;
            if (_count > 4)
                _goodToRotate = true;
            UpdateGUI?.Invoke();
        }
    }
}
