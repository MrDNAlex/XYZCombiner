using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Class Used to Save Vectors between Objects
    /// </summary>
    public class VectorAlignments
    {
        /// <summary>
        /// Game Object Determining the Start of the Vector
        /// </summary>
        public TransformableObject StartObject { get; set; }

        /// <summary>
        /// Game Object Determining the End of the Vector
        /// </summary>
        public TransformableObject EndObject { get; set; }

        /// <summary>
        /// Parent Object of the Start Object
        /// </summary>
        public GameObject ParentObject { get; set; }

        /// <summary>
        /// Returns the Vector Between the Start Object and End Object
        /// </summary>
        public Vector3 Vector
        {
            get
            {
                if (EndObject != null && StartObject != null)
                    return (EndObject.Position - Offset) - (StartObject.Position - Offset);
                else
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the Starting Objects Position
        /// </summary>
        public Vector3 Start
        {
            get
            {
                if (StartObject != null)
                    return StartObject.Position;
                else
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the Offset Vector Between the Parents Position and the Starting objects Position
        /// </summary>
        public Vector3 Offset
        {
            get
            {
                if (ParentObject != null && StartObject != null)
                    return StartObject.Position - ParentObject.transform.position;
                else
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Initializes the Vector Alignment
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public VectorAlignments(TransformableObject start, TransformableObject end, GameObject parentObject)
        {
            StartObject = start;
            EndObject = end;
            ParentObject = parentObject;
        }
    }
}