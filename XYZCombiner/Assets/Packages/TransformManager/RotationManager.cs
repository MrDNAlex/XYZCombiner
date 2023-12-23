using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    public class RotationManager
    {
        /// <summary>
        /// Stores a Reference to the parent Transform Manager
        /// </summary>
        public TransformManager TransformManager { get; set; }

        /// <summary>
        /// Manager Storing and Handling all Axis Related Operations
        /// </summary>
        public AxisManager AxisManager { get; set; }

        /// <summary>
        /// Selected Game Object that will have the Transformations Applied to
        /// </summary>
        public TransformableObject SelectedObject { get { return TransformManager.SelectedObject; } set { TransformManager.SelectedObject = value; } }

        /// <summary>
        /// Stores the Rotation Point on the Screen Relative to the World Space
        /// </summary>
        public Vector3 RotationPoint { get; set; }

        /// <summary>
        /// Previous Delta Vector between Mouse and Object Position for Rotation Transformation
        /// </summary>
        public Vector3 LastDelta { get; set; }

        /// <summary>
        /// Stores the Current Mouse Position on the Screen in Pixels
        /// </summary>
        public Vector3 CurrentMouseScreenPosition { get { return TransformManager.CurrentMouseScreenPosition; } set { TransformManager.CurrentMouseScreenPosition = value; } }

        /// <summary>
        /// Stores the Last Mouse position on the Screen in Pixels
        /// </summary>
        public Vector3 LastMouseScreenPosition { get { return TransformManager.LastMouseScreenPosition; } set { TransformManager.LastMouseScreenPosition = value; } }

        /// <summary>
        /// Initializes the Rotation Manager
        /// </summary>
        /// <param name="transformManager"></param>
        public RotationManager(TransformManager transformManager)
        {
            TransformManager = transformManager;
            AxisManager = TransformManager.AxisManager;
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
        /// Handles the Key Bind inputs related to Rotation
        /// </summary>
        public void HandleInput()
        {

            //Rotation
            if (Input.GetKeyDown(KeyCode.R))
            {
                TransformManager.ToggleTransformationMode(Transformation.Rotation);
                TransformManager.UpdateGUI?.Invoke();
            }

        }

        /// <summary>
        /// Updates the Rotation Operations
        /// </summary>
        public void UpdateRotation ()
        {
            HandleInput();

            if (TransformManager.TransformationAction == Transformation.Rotation)
            {
                RotateSelectedObject();
                TransformManager.UpdateGUI?.Invoke();
            }
        }
    }
}
