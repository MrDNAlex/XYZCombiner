using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNATransformManager;

/// <summary>
/// Describes a Molecule in Chemistry
/// </summary>
public class Molecule : TransformableObject
{
    /// <summary>
    /// Describes the Molecules Name
    /// </summary>
    public string MoleculeName { get { return this.name; } set { this.name = value; } }

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

    /// <summary>
    /// Initializes the Molecule
    /// </summary>
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

        for (int i = 2; i < 2 + numOfAtoms; i++)
        {
            string atomInfo = fileLines[i];
            string[] info = atomInfo.Split(" ");
            Vector3 pos = new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3]));
            GameObject atomPrefab = Resources.Load<GameObject>("Atom");
            GameObject atomObject = GameObject.Instantiate(atomPrefab, pos, new Quaternion(), this.transform);
            Atom atom = atomObject.GetComponent<Atom>();
            atom.SetInfo(info[0], pos, this);
            Atoms.Add(atom);
        }

        SetHitBoxDimensions();
    }

    /// <summary>
    /// Sets the Molecules Hit Box Center
    /// </summary>
    public Vector3 GetHitBoxCenter()
    {
        Vector3 avgPosition = new Vector3(0, 0, 0);

        foreach (Atom atom in Atoms)
            avgPosition += atom.Position;

        MoleculeCenter = Vector3.zero;

        return avgPosition / Atoms.Count;
    }

    /// <summary>
    /// Sets the Molecules Hit Box Dimensions from the center
    /// </summary>
    public void SetHitBoxDimensions()
    {
        Vector3 avgPosition = GetHitBoxCenter();

        Vector3 hitboxDim = new Vector3(0, 0, 0);

        foreach (Atom atom in Atoms)
        {
            Vector3 truePosition = atom.Position - avgPosition;
            hitboxDim.x = Mathf.Max(hitboxDim.x, Mathf.Abs(truePosition.x));
            hitboxDim.y = Mathf.Max(hitboxDim.y, Mathf.Abs(truePosition.y));
            hitboxDim.z = Mathf.Max(hitboxDim.z, Mathf.Abs(truePosition.z));

            atom.SetOffsetPosition(avgPosition);
        }

        hitboxDim = hitboxDim * 2;
        hitboxDim = hitboxDim + Vector3.one;

        MoleculeDimensions = hitboxDim;
    }

    /// <summary>
    /// Sets the Molecules Name to a custom Value
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        MoleculeName = name;
    }

    /// <summary>
    /// Adds More Atoms to the Molecule
    /// </summary>
    /// <param name="atoms"></param>
    public void AddAtoms(List<Atom> atoms)
    {
        //List through every atom, convert to world space, and then convert to local space of this molecule
        foreach (Atom atom in atoms)
        {
            Vector3 worldPos = atom.Position;

            atom.transform.parent = this.transform;

            atom.SetInfo(atom.Element, worldPos, this);

            Atoms.Add(atom);
        }
    }

    /// <summary>
    /// Destroys the Molecule
    /// </summary>
    public void DestroyMolecule()
    {
        WorldSpaceManager worldSpace = GameObject.Find("WorldSpace").GetComponent<WorldSpaceManager>();
        worldSpace.Molecules.Remove(this);
        GameObject.Destroy(this.gameObject);
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