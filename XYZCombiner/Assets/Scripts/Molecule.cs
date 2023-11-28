using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Describes a Molecule in Chemistry
/// </summary>
public class Molecule : MonoBehaviour
{
    /// <summary>
    /// Describes the Molecules Name
    /// </summary>
    public string MoleculeName { get; set; }
    /// <summary>
    /// Describes the Molecules Position Relative to World Space
    /// </summary>
    public Vector3 Position { get { return transform.position; } }
    /// <summary>
    /// A List of Atoms tht belong to the Molecules
    /// </summary>
    public List<Atom> Atoms { get; set; }
    /// <summary>
    /// Returns the Number of Atoms in the Molecule
    /// </summary>
    public int NumOfAtoms { get { return Atoms.Count; } }

    /// <summary>
    /// Returns and Sets the Molecules Hit Box Center Position
    /// </summary>
    public Vector3 MoleculeCenter { get { return GetComponent<BoxCollider>().center; } private set { GetComponent<BoxCollider>().center = value; } }

    /// <summary>
    /// Returns and Sets the Molecules Hit Box Dimensions
    /// </summary>
    public Vector3 MoleculeDimensions { get { return GetComponent<BoxCollider>().size; } private set { GetComponent<BoxCollider>().size = value; } }

    public Molecule()
    {
        Atoms = new List<Atom>();
    }
    /// <summary>
    /// Creates the Molecule from a .XYZ File
    /// </summary>
    /// <param name="XYZFile"></param>
    public void CreateMoleculeFromXYZ(string XYZFile)
    {
        //First line = num of molecules [0]
        //Second line = name [1]
        //Rest of lines are atoms [2 -> Infinity]
        string[] fileLines = XYZFile.Split("\n");
        int numOfAtoms = int.Parse(fileLines[0]);
        MoleculeName = fileLines[1];
        this.name = MoleculeName;

        for (int i = 2; i < 2 + numOfAtoms; i++)
        {
            string atomInfo = fileLines[i];
            string[] info = atomInfo.Split(" ");
            Vector3 pos = new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3]));
            GameObject atomPrefab = Resources.Load<GameObject>("Atom");
            GameObject atomObject = GameObject.Instantiate(atomPrefab, pos, new Quaternion(), this.transform);
            Atom atom = atomObject.GetComponent<Atom>();
            atom.SetInfo(info[0], pos);
            Atoms.Add(atom);
        }

        SetHitBoxCenter();
        SetHitBoxDimensions();
    }

    /// <summary>
    /// Sets the Molecules Hit Box Center
    /// </summary>
    public void SetHitBoxCenter()
    {
        Vector3 avgPosition = new Vector3(0, 0, 0);

        foreach (Atom atom in Atoms)
            avgPosition += atom.Position;

        avgPosition = avgPosition / Atoms.Count;

        MoleculeCenter = avgPosition;
    }

    /// <summary>
    /// Sets the Molecules Hit Box Dimensions from the center
    /// </summary>
    public void SetHitBoxDimensions()
    {
        Vector3 avgPosition = MoleculeCenter;

        Vector3 hitboxDim = new Vector3(0, 0, 0);

        foreach (Atom atom in Atoms)
        {
            Vector3 truePosition = atom.Position - avgPosition;
            hitboxDim.x = Mathf.Max(hitboxDim.x, Mathf.Abs(truePosition.x));
            hitboxDim.y = Mathf.Max(hitboxDim.y, Mathf.Abs(truePosition.y));
            hitboxDim.z = Mathf.Max(hitboxDim.z, Mathf.Abs(truePosition.z));
        }

        hitboxDim = hitboxDim * 2;
        hitboxDim = hitboxDim + Vector3.one;

        MoleculeDimensions = hitboxDim;
    }

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {

    }
}