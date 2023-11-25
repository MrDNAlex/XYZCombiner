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
    public Vector3 Position { get; set; }

    /// <summary>
    /// A List of Atoms tht belong to the Molecules
    /// </summary>
    public List<Atom> Atoms { get; set; }

    /// <summary>
    /// Returns the Number of Atoms in the Molecule
    /// </summary>
    public int NumOfAtoms { get { return Atoms.Count; } }

    public Molecule()
    {
        Debug.Log("Initialize");
        Atoms = new List<Atom>();
    }

    /// <summary>
    /// Creates the Molecule from a .XYZ File
    /// </summary>
    /// <param name="XYZFile"></param>
    public void CreateMoleculeFromXYZ (string XYZFile)
    {
        //First line = num of molecules [0]
        //Second line = name [1]

        //Rest of lines are atoms [2 -> Infinity]

        string[] fileLines = XYZFile.Split("\n");

        int numOfAtoms = int.Parse(fileLines[0]);
        Debug.Log(numOfAtoms);

        MoleculeName = fileLines[1];
        Debug.Log(MoleculeName);

        this.name = MoleculeName;
        
        for (int i = 2; i < 2 + numOfAtoms; i ++)
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
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
