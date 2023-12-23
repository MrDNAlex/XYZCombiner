using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    public class TranslationManager
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
        /// Stores the Current Mouse Position on the Screen in Pixels
        /// </summary>
        public Vector3 CurrentMouseScreenPosition { get { return TransformManager.CurrentMouseScreenPosition; } set { TransformManager.CurrentMouseScreenPosition = value; } }

        /// <summary>
        /// Stores the Last Mouse position on the Screen in Pixels
        /// </summary>
        public Vector3 LastMouseScreenPosition { get { return TransformManager.LastMouseScreenPosition; } set { TransformManager.LastMouseScreenPosition = value; } }

        /// <summary>
        /// Initializes the Translation Manager
        /// </summary>
        /// <param name="transformManager"></param>
        public TranslationManager(TransformManager transformManager)
        {
            TransformManager = transformManager;
            AxisManager = TransformManager.AxisManager;
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
        /// Translates the Selected Object
        /// </summary>
        public void TranslateSelectedObject()
        {
            if (AxisManager.TransformAxis == Axis.None)
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
        /// Returns a Directional Multiplier of 1 or -1 based on if the controls are needed to be inverted due to the Camera's position
        /// </summary>
        /// <param name="axisVector"></param>
        /// <returns></returns>
        private float GetDirectionMultiplier()
        {
            Vector3 axisVector = AxisManager.GetAxisVector();
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(cameraForward, axisVector.normalized).normalized;

            return Vector3.Dot(axisVector, projectedCameraForward) > 0 ? 1 : -1;
        }

        /// <summary>
        /// Handles the Key Bind inputs related to Translation
        /// </summary>
        public void HandleInput()
        {
            //Translation
            if (Input.GetKeyDown(KeyCode.T))
            {
                TransformManager.ToggleTransformationMode(Transformation.Translation);
                LastMouseScreenPosition = Input.mousePosition;
                TransformManager.UpdateGUI?.Invoke();
            }
        }

        /// <summary>
        /// Updates the Translation Operations
        /// </summary>
        public void UpdateTranslation()
        {
            HandleInput();

            if (TransformManager.TransformationAction == Transformation.Translation)
            {
                TranslateSelectedObject();
                TransformManager.UpdateGUI?.Invoke();
            }
        }
    }
}
