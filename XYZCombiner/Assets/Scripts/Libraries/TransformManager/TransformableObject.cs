using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Class Describing a Transformable Object that can be Trasnformed using the Transform Manager
    /// </summary>
    public class TransformableObject : MonoBehaviour
    {
        /// <summary>
        /// Gets and Sets the Transformable Objects Position
        /// </summary>
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

        /// <summary>
        /// Gets and Sets the Transformable Objects Local Position
        /// </summary>
        public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }

        /// <summary>
        /// Gets and Sets the Transformable Objects Rotation
        /// </summary>
        public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }

        /// <summary>
        /// Gets and Sets the Transformable Objects Local Rotation
        /// </summary>
        public Quaternion LocalRotation { get { return transform.localRotation; } set { transform.localRotation = value; } }

        /// <summary>
        /// Gets the Transformable Objects Parent
        /// </summary>
        public GameObject Parent { get { return transform.parent.gameObject; } }
    }
}