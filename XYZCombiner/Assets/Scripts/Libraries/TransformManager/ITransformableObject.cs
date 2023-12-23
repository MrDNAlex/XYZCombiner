using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DNATransformManager
{
    public interface ITransformableObject
    {
        /// <summary>
        /// Gets and Sets the Transformable Objects Position
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets and Sets the Transformable Objects Local Position
        /// </summary>
        public Vector3 LocalPosition { get; set; }

        /// <summary>
        /// Gets and Sets the Transformable Objects Rotation
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Gets and Sets the Transformable Objects Local Rotation
        /// </summary>
        public Quaternion LocalRotation { get; set; }

        /// <summary>
        /// Gets the Transformable Objects Parent
        /// </summary>
        public GameObject Parent { get; }

        /// <summary>
        /// Method for Destroying or Deleting the Transformable Object
        /// </summary>
        public abstract void DestroyObject();
    }
}
