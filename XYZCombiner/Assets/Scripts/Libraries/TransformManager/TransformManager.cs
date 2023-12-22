using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Manager Class that handles all Transformation Related Activities
    /// </summary>
    public class TransformManager
    {
        /// <summary>
        /// List of Keybinds that map to Transform Actions
        /// </summary>
        public KeyBindManager KeyBinds { get; set; }

        /// <summary>
        /// Manager Storing and Handling all Axis Related Operations
        /// </summary>
        public AxisManager AxisManager { get; set; }

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
        public TransformableObject SelectedObject { get; set; }

        /// <summary>
        /// Last Selected Game Object 
        /// </summary>
        public TransformableObject LastObject { get; set; }

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

        /// <summary>
        /// Stores the Current Mouse Position on the Screen in Pixels
        /// </summary>
        public Vector3 CurrentMouseScreenPosition { get; set; }

        /// <summary>
        /// Stores the Last Mouse position on the Screen in Pixels
        /// </summary>
        public Vector3 LastMouseScreenPosition { get; set; }

        /// <summary>
        /// Stores the Rotation Point on the Screen Relative to the World Space
        /// </summary>
        public Vector3 RotationPoint { get; set; }

        /// <summary>
        /// Stores the Origin Position in World Space
        /// </summary>
        public Vector3 Origin { get; set; }

        /// <summary>
        /// Initializes the Transformation Manager
        /// </summary>
        public TransformManager()
        {
            Origin = Vector3.zero;
            SavedVectors = new List<VectorAlignments>();
            KeyBinds = new KeyBindManager();
            AxisManager = new AxisManager(this);
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
                LastPosition = SelectedObject.Position;
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
                SavedVectors.Add(new VectorAlignments(LastObject, SelectedObject, LastObject.Parent));

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
            Vector3 cameraToObject = SelectedObject.Position - Camera.main.transform.position;

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
            RotationPoint = Camera.main.WorldToScreenPoint(SelectedObject.Position);

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
            Vector3 rotationAxis = AxisManager.GetAxisVector();

            SelectedObject.transform.Rotate(rotationAxis, GetAngleOfRotation(), Space.World);

            AxisManager.AxisLine.SetPositions(new Vector3[] { SelectedObject.Position + rotationAxis * 100, SelectedObject.Position - rotationAxis * 100 });
        }

        /// <summary>
        /// Returns a Directional Multiplier of 1 or -1 based on if the controls are needed to be inverted due to the Camera's position
        /// </summary>
        /// <param name="axisVector"></param>
        /// <returns></returns>
        private float GetDirectionMultiplier()
        {
            Vector3 axisVector = AxisManager.GetAxisVector();
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(cameraForward, axisVector.normalized).normalized;

            return Vector3.Dot(axisVector, projectedCameraForward) > 0 ? -1 : 1;
        }

        /// <summary>
        /// Translates the Selected Object
        /// </summary>
        public void TranslateSelectedObject()
        {
            if (AxisManager.TransformAxis == AxisManager.Axis.None)
            {
                AxisManager.AxisLine.enabled = false;
                SelectedObject.Position += GetMovementVector();
            }
            else
            {
                Vector3 movementVector = GetMovementVector();
                float magnitude = Vector3.Dot(movementVector, AxisManager.GetAxisVector());

                SelectedObject.transform.Translate(AxisManager.GetAxisVector().normalized * magnitude * GetDirectionMultiplier(), Space.World);
                AxisManager.AxisLine.SetPositions(new Vector3[] { SelectedObject.Position + AxisManager.GetAxisVector() * 100, SelectedObject.Position - AxisManager.GetAxisVector() * 100 });
            }
        }

        /// <summary>
        /// Sets the Selected Object to Apply Transformations to
        /// </summary>
        /// <param name="obj"></param>
        public void SetSelectedObj(TransformableObject obj)
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
            SelectedObject.Rotation *= Quaternion.FromToRotation(GetLastVector(0).Vector * -1, GetLastVector(1).Vector);

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
                AxisManager.AxisLine.enabled = false;
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
                SelectedObject.Position = Origin;
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
                    SelectedObject.Position = GetLastVector().Start;
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
                AxisManager.ToggleXAxis();
                UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                AxisManager.ToggleYAxis();
                UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                AxisManager.ToggleZAxis();
                UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                AxisManager.ToggleVectorAxis();
                UpdateGUI?.Invoke();
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
                TranslateSelectedObject();
                UpdateGUI?.Invoke();
            }

            if (TransformationAction == Transformation.Rotation)
            {
                RotateSelectedObject();
                UpdateGUI?.Invoke();
            }
        }
    }
}