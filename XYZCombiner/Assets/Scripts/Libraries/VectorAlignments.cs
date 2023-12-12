using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Used to Save Vectors between Objects
/// </summary>
public class VectorAlignments
{
    /// <summary>
    /// Game Object Determining the Start of the Vector
    /// </summary>
    public GameObject StartObject { get; set; }

    /// <summary>
    /// Game Object Determining the End of the Vector
    /// </summary>
    public GameObject EndObject { get; set; }

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
                return (EndObject.transform.position - Offset) - (StartObject.transform.position - Offset);
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
                return StartObject.transform.position;
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
                return StartObject.transform.position - ParentObject.transform.position;
            else
                return Vector3.zero;
        }
    }

    /// <summary>
    /// Initializes the Vector Alignment
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public VectorAlignments(GameObject start, GameObject end)
    {
        StartObject = start;
        EndObject = end;

        if (StartObject.GetComponent<Atom>() != null)
            ParentObject = StartObject.GetComponent<Atom>().ParentMolecule.gameObject;
        else if (StartObject.GetComponent<Molecule>() != null)
            ParentObject = StartObject.GetComponent<Molecule>().gameObject;
    }
}
