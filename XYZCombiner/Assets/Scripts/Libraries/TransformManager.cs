using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformManager
{
    // allow for axial editing

    //(Specify translation or rotation along x, y, z only (shift modifier for local instead))

    //Have Self Generated Lines, modify coordinates based on movement

    private LineRenderer AxisLine { get; set; }

    private GameObject Line { get; set; }

    /// <summary>
    /// List of Keybinds that map to Transform Actions
    /// </summary>
    public KeyBindManager KeyBinds { get; set; }

    /// <summary>
    /// Reference to the World Space Manager
    /// </summary>
    private WorldSpaceManager _worldSpaceManager { get; set; }

    /// <summary>
    /// Enum to Differentiate between Transformation Axes
    /// </summary>
    public enum Axis
    {
        None,
        X,
        SelfX,
        Y,
        SelfY,
        Z,
        SelfZ,
        Vector
    }

    /// <summary>
    /// Axis Upon which the Transformation Will Occur
    /// </summary>
    public Axis TransformAxis { get; set; }

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
    /// Selected Game Object that will have the Transformations Applied to
    /// </summary>
    public GameObject SelectedObject { get; set; }

    /// <summary>
    /// Last Selected Game Object 
    /// </summary>
    public GameObject LastObject { get; set; }

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
    /// Saves the Last Position to Determine the Saved Vector
    /// </summary>
    public Vector3 LastPosition { get; set; }

    /// <summary>
    /// Saves the Vector Used to Translate
    /// </summary>
    public List<VectorAlignments> SavedVectors { get; set; }

    public Vector3 CurrentMouseScreenPosition { get; set; }

    public Vector3 LastMouseScreenPosition { get; set; }

    public Vector3 RotationPoint { get; set; }

    public Vector3 DefaultAxis { get { return Camera.main.transform.forward * -1; } }

    public Vector3 Origin { get; set; }

    /// <summary>
    /// Initializes the Transformation Manager
    /// </summary>
    public TransformManager(WorldSpaceManager worldSpaceManager)
    {
        _worldSpaceManager = worldSpaceManager;
        Origin = Vector3.zero;
        TransformAxis = Axis.None;
        SavedVectors = new List<VectorAlignments>();
        KeyBinds = new KeyBindManager();

        Line = new GameObject("AxisLine");
        Line.AddComponent<LineRenderer>();
        AxisLine = Line.GetComponent<LineRenderer>();
        AxisLine.startColor = Color.cyan;
        AxisLine.endColor = Color.cyan;
        AxisLine.startWidth = 0.1f;
        AxisLine.endWidth = 0.1f;
    }

    /// <summary>
    /// Returns the Vector for the Specified Axis of Transformation
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAxisVector()
    {
        switch (TransformAxis)
        {
            case Axis.None:
                AxisLine.enabled = false;
                return DefaultAxis;
            case Axis.X:
                AxisLine.enabled = true;
                return Vector3.right;
            case Axis.SelfX:
                AxisLine.enabled = true;
                return SelectedObject.transform.right;
            case Axis.Y:
                AxisLine.enabled = true;
                return Vector3.up;
            case Axis.SelfY:
                AxisLine.enabled = true;
                return SelectedObject.transform.up;
            case Axis.Z:
                AxisLine.enabled = true;
                return Vector3.forward;
            case Axis.SelfZ:
                AxisLine.enabled = true;
                return SelectedObject.transform.forward;
            case Axis.Vector:
                AxisLine.enabled = true;
                return GetLastVector().Vector;
            default:
                AxisLine.enabled = false;
                return DefaultAxis;
        }
    }

    /// <summary>
    /// Returns the List of Saved Vectors
    /// </summary>
    /// <returns></returns>
    public string GetVectors()
    {
        string vectors = "";
        foreach (VectorAlignments vector in SavedVectors)
        {
            if (vector.StartObject != null && vector.EndObject != null)
                vectors += vector.Vector;
        }

        return vectors;
    }

    /// <summary>
    /// Returns the Last Vector in the Saved Vector list or Last Vector offset by an index if specified
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public VectorAlignments GetLastVector(int index = 0)
    {
        if (SavedVectors.Count > index)
            return SavedVectors[SavedVectors.Count - index - 1];
        else
            return null;
    }

    /// <summary>
    /// Saves the Previously Selected Object Position
    /// </summary>
    /// <param name="gameObj"></param>
    public void SaveLastPosition()
    {
        if (SelectedObject != null)
        {
            LastPosition = SelectedObject.transform.position;
            LastObject = SelectedObject;
        }
    }

    /// <summary>
    /// Saves the Vector Between the Currently Selected Object and the 
    /// </summary>
    /// <param name="position"></param>
    public void SaveVector()
    {
        if (LastPosition != null || LastPosition != Vector3.zero)
        {
            SavedVectors.Add(new VectorAlignments(LastObject, SelectedObject));

            if (SavedVectors.Count >= 3)
                SavedVectors.RemoveAt(0);

            Debug.DrawRay(LastPosition, GetLastVector().Vector, Color.red, 1);
        }
    }

    /// <summary>
    /// Gets the Movement Vector for the Selected Object based on the Delta of the Mouse Position
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMovementVector()
    {
        CurrentMouseScreenPosition = Input.mousePosition;

        Vector3 cameraForward = Camera.main.transform.up.normalized;
        Vector3 cameraRight = Camera.main.transform.right.normalized;
        Vector3 cameraToObject = SelectedObject.transform.position - Camera.main.transform.position;

        float distanceInCameraDirection = Vector3.Project(cameraToObject, Camera.main.transform.forward).magnitude;
        float fov = Camera.main.fieldOfView;
        float scalingFactor = (distanceInCameraDirection * Mathf.Tan(fov * Mathf.Deg2Rad / 2f)) * 2f / Screen.height; // Adjust this formula based on your requirements

        Vector3 deltaMousePosition = (CurrentMouseScreenPosition - LastMouseScreenPosition) * scalingFactor;

        LastMouseScreenPosition = CurrentMouseScreenPosition;

        return cameraForward * deltaMousePosition.y + cameraRight * deltaMousePosition.x;
    }

    /// <summary>
    /// Gets the Angle of Rotation based on the Delta Mouse Movement
    /// </summary>
    /// <returns></returns>
    private float GetAngleOfRotation()
    {
        RotationPoint = Camera.main.WorldToScreenPoint(SelectedObject.transform.position);

        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 deltaMousePosition = currentMousePosition - RotationPoint;

        float angle = Vector3.Angle(LastDelta, deltaMousePosition);
        int clockwiseMult = Vector3.Dot(Vector3.forward, Vector3.Cross(LastDelta, deltaMousePosition)) > 0 ? -1 : 1;

        LastDelta = deltaMousePosition;

        return angle * clockwiseMult;
    }

    /// <summary>
    /// Rotates the Selected Object
    /// </summary>
    public void RotateSelectedObject()
    {
        Vector3 rotationAxis = GetAxisVector();

        SelectedObject.transform.Rotate(rotationAxis, GetAngleOfRotation(), Space.World);

        AxisLine.SetPositions(new Vector3[] { SelectedObject.transform.position + rotationAxis * 100, SelectedObject.transform.position - rotationAxis * 100 });
    }

    /// <summary>
    /// Returns a Directional Multiplier of 1 or -1 based on if the controls are needed to be inverted due to the Camera's position
    /// </summary>
    /// <param name="axisVector"></param>
    /// <returns></returns>
    private float GetDirectionMultiplier()
    {
        Vector3 axisVector = GetAxisVector();
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 projectedCameraForward = Vector3.ProjectOnPlane(cameraForward, axisVector.normalized).normalized;

        return Vector3.Dot(axisVector, projectedCameraForward) > 0 ? -1 : 1;
    }

    /// <summary>
    /// Translates the Selected Object
    /// </summary>
    public void TranslateSelectedObject()
    {
        if (TransformAxis == Axis.None)
        {
            AxisLine.enabled = false;
            SelectedObject.transform.position += GetMovementVector();
        }
        else
        {
            Vector3 movementVector = GetMovementVector();
            float magnitude = Vector3.Dot(movementVector, GetAxisVector());

            SelectedObject.transform.Translate(GetAxisVector().normalized * magnitude * GetDirectionMultiplier(), Space.World);
            AxisLine.SetPositions(new Vector3[] { SelectedObject.transform.position + GetAxisVector() * 100, SelectedObject.transform.position - GetAxisVector() * 100 });
        }
    }

    /// <summary>
    /// Sets the Selected Object to Apply Transformations to
    /// </summary>
    /// <param name="obj"></param>
    public void SetSelectedObj(GameObject obj)
    {
        SaveLastPosition();
        SelectedObject = obj;
    }

    /// <summary>
    /// Sets the Delegate Update GUI Function
    /// </summary>
    /// <param name="UpdateFunc"></param>
    public void SetUpdateFunction(UpdateUI UpdateFunc)
    {
        UpdateGUI = UpdateFunc;
        UpdateGUI?.Invoke();
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
    /// Removes the Selected Object from the World Space
    /// </summary>
    private void RemoveSelectedObject()
    {
        if (SelectedObject.GetComponent<Molecule>() != null)
        {
            Molecule molecule = SelectedObject.GetComponent<Molecule>();
            molecule.DestroyMolecule();
        }

        if (SelectedObject.GetComponent<Atom>() != null)
        {
            Atom atom = SelectedObject.GetComponent<Atom>();
            atom.DestroyAtom();
        }
    }

    /// <summary>
    /// Rotates an Object so that the Selected Vectors become Parallel and Translates it so that their Starting positions are aligned
    /// </summary>
    private void VectorAlignment()
    {
        SelectedObject.transform.rotation *= Quaternion.FromToRotation(GetLastVector(0).Vector * -1, GetLastVector(1).Vector);

        SelectedObject.transform.Translate((GetLastVector().Start - GetLastVector(1).Start), Space.World);
    }

    /// <summary>
    /// Handles Keybinds and Inputs to change Transformation Actions
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleTransformationMode(Transformation.None);
            AxisLine.enabled = false;
            UpdateGUI?.Invoke();
        }

        //Translation
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTransformationMode(Transformation.Translation);
            LastMouseScreenPosition = Input.mousePosition;
            UpdateGUI?.Invoke();
        }

        //Rotation
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleTransformationMode(Transformation.Rotation);
            UpdateGUI?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            VectorAlignment();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SelectedObject.transform.position = Origin;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveVector();
            UpdateGUI?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SavedVectors = new List<VectorAlignments>();
            UpdateGUI?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveSelectedObject();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (GetLastVector() != null)
            {
                SelectedObject.transform.position = GetLastVector().Start;
            }
        }

        GetTransformAxis();
    }

    /// <summary>
    /// Sets the Transformation Axis to the One Specified 
    /// </summary>
    private void GetTransformAxis()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleXAxis();
            UpdateGUI?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleYAxis();
            UpdateGUI?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ToggleZAxis();
            UpdateGUI?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleVectorAxis();
            UpdateGUI?.Invoke();
        }
    }

    /// <summary>
    /// Toggles the Transform Axis Between None, X and SelfX
    /// </summary>
    private void ToggleXAxis()
    {
        if (TransformAxis == Axis.None)
            TransformAxis = Axis.X;
        else if (TransformAxis == Axis.X)
            TransformAxis = Axis.SelfX;
        else if (TransformAxis == Axis.SelfX)
            TransformAxis = Axis.None;
        else
            TransformAxis = Axis.X;
    }

    /// <summary>
    /// Toggles the Transform Axis Between None, Y and SelfY
    /// </summary>
    private void ToggleYAxis()
    {
        if (TransformAxis == Axis.None)
            TransformAxis = Axis.Y;
        else if (TransformAxis == Axis.Y)
            TransformAxis = Axis.SelfY;
        else if (TransformAxis == Axis.SelfY)
            TransformAxis = Axis.None;
        else
            TransformAxis = Axis.Y;
    }

    /// <summary>
    /// Toggles the Transform Axis Between None, Z and SelfZ
    /// </summary>
    private void ToggleZAxis()
    {
        if (TransformAxis == Axis.None)
            TransformAxis = Axis.Z;
        else if (TransformAxis == Axis.Z)
            TransformAxis = Axis.SelfZ;
        else if (TransformAxis == Axis.SelfZ)
            TransformAxis = Axis.None;
        else
            TransformAxis = Axis.Z;
    }

    /// <summary>
    /// Toggles the Transform Axis Between None and Vector
    /// </summary>
    private void ToggleVectorAxis()
    {
        if (TransformAxis == Axis.None)
            TransformAxis = Axis.Vector;
        else if (TransformAxis == Axis.Vector)
            TransformAxis = Axis.None;
        else
            TransformAxis = Axis.Vector;
    }

    /// <summary>
    /// Updates the Transformations that Occur
    /// </summary>
    public void UpdateTransformations()
    {
        HandleInput();

        if (TransformationAction == Transformation.Translation)
        {
            TranslateSelectedObject();
            UpdateGUI?.Invoke();
        }

        if (TransformationAction == Transformation.Rotation)
        {
            RotateSelectedObject();
            UpdateGUI?.Invoke();
        }
    }

    /// <summary>
    /// Returns the Info of the Selected Atom
    /// </summary>
    /// <returns></returns>
    public string GetSelectedAtom()
    {
        if (SelectedObject?.GetComponent<Atom>() != null)
        {
            Atom atom = SelectedObject.GetComponent<Atom>();
            return $" {atom.Element} : {atom.Position} : {atom.FilePosition}";
        }
        else
            return "";
    }

    /// <summary>
    /// Returns the Info of the Selected Molecule
    /// </summary>
    /// <returns></returns>
    public string GetSelectedMolecule()
    {
        if (SelectedObject?.GetComponent<Molecule>() != null)
        {
            Molecule molecule = SelectedObject.GetComponent<Molecule>();
            return $" {molecule.MoleculeName} : {molecule.Position}";
        }

        else
            return "";
    }
}
