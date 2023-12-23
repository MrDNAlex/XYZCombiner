using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Manager Class Handling and Storing all Axis related Operations
    /// </summary>
    public class AxisManager
    {
        /// <summary>
        /// Stores a Reference to the parent Transform Manager
        /// </summary>
        public TransformManager TransformManager { get; set; }

        /// <summary>
        /// Stores the Line Rendere Component of the Line Object
        /// </summary>
        public LineRenderer AxisLine { get; set; }

        /// <summary>
        /// Stores the GameObject for the Axis line
        /// </summary>
        public GameObject Line { get; set; }

        /// <summary>
        /// Axis Upon which the Transformation Will Occur
        /// </summary>
        public Axis TransformAxis { get; set; }

        /// <summary>
        /// Stores the Default Axis of Transformation to Use
        /// </summary>
        public Vector3 DefaultAxis { get { return Camera.main.transform.forward * -1; } }

        /// <summary>
        /// Initializes the Axis Manager
        /// </summary>
        public AxisManager(TransformManager transformManager)
        {
            TransformManager = transformManager;
            TransformAxis = Axis.None;

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
                    return TransformManager.SelectedObject.transform.right;
                case Axis.Y:
                    AxisLine.enabled = true;
                    return Vector3.up;
                case Axis.SelfY:
                    AxisLine.enabled = true;
                    return TransformManager.SelectedObject.transform.up;
                case Axis.Z:
                    AxisLine.enabled = true;
                    return Vector3.forward;
                case Axis.SelfZ:
                    AxisLine.enabled = true;
                    return TransformManager.SelectedObject.transform.forward;
                case Axis.Vector:
                    AxisLine.enabled = true;
                    return TransformManager.GetLastVector().Vector;
                default:
                    AxisLine.enabled = false;
                    return DefaultAxis;
            }
        }

        /// <summary>
        /// Toggles the Transform Axis Between None, X and SelfX
        /// </summary>
        public void ToggleXAxis()
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
        public void ToggleYAxis()
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
        public void ToggleZAxis()
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
        public void ToggleVectorAxis()
        {
            if (TransformAxis == Axis.None)
                TransformAxis = Axis.Vector;
            else if (TransformAxis == Axis.Vector)
                TransformAxis = Axis.None;
            else
                TransformAxis = Axis.Vector;
        }

        /// <summary>
        /// Sets the Transformation Axis to the One Specified 
        /// </summary>
        public void GetTransformAxis()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ToggleXAxis();
                TransformManager.UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                ToggleYAxis();
                TransformManager.UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                ToggleZAxis();
                TransformManager.UpdateGUI?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                ToggleVectorAxis();
                TransformManager.UpdateGUI?.Invoke();
            }
        }
    }
}