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
        /// Manager Storing and Handling all Translation Related Operations
        /// </summary>
        public TranslationManager TranslationManager { get; set; }

        /// <summary>
        /// Manager Storing and Handling All Rotation Related Operations
        /// </summary>
        public RotationManager RotationManager { get; set; }

        /// <summary>
        /// Selected Game Object that will have the Transformations Applied to
        /// </summary>
        public TransformableObject SelectedObject { get; set; }

        /// <summary>
        /// Last Selected Game Object 
        /// </summary>
        public TransformableObject LastObject { get; set; }

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
            TranslationManager = new TranslationManager(this);
            RotationManager = new RotationManager(this);
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
        public void ToggleTransformationMode(Transformation transformation)
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
            SelectedObject.DestroyObject();
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

            AxisManager.GetTransformAxis();
        }

        /// <summary>
        /// Updates the Transformations that Occur
        /// </summary>
        public void UpdateTransformations()
        {
            HandleInput();

            TranslationManager.UpdateTranslation();
            RotationManager.UpdateRotation();
        }
    }
}