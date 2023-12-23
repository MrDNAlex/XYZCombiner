using UnityEngine;

namespace DNATransformManager
{
    /// <summary>
    /// Class Describing a Transformable Object that can be Trasnformed using the Transform Manager
    /// </summary>
    public class TransformableObject : MonoBehaviour, ITransformableObject
    {
        /// <inheritdoc/>
        public Vector3 Position { get { return transform.position; } set { transform.position = value; } }

        /// <inheritdoc/>
        public Vector3 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }

        /// <inheritdoc/>
        public Quaternion Rotation { get { return transform.rotation; } set { transform.rotation = value; } }

        /// <inheritdoc/>
        public Quaternion LocalRotation { get { return transform.localRotation; } set { transform.localRotation = value; } }

        /// <inheritdoc/>
        public GameObject Parent { get { return transform.parent.gameObject; } }

        /// <inheritdoc/>
        public virtual void DestroyObject()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}