using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a Atom in Chemistry
/// </summary>
public class Atom : MonoBehaviour
{
    /// <summary>
    /// Reference to the Molecule that the Atom in Contained in
    /// </summary>
    public Molecule ParentMolecule { get; set; }

    /// <summary>
    /// Describes the Atoms Element 
    /// </summary>
    public string Element { get; set; }

    /// <summary>
    /// Describes the Atoms Position Relative to the Molecule
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Sets the Atoms Information
    /// </summary>
    /// <param name="element"></param>
    /// <param name="position"></param>
    public void SetInfo(string element, Vector3 position, Molecule parentMolecule)
    {
        Element = element;
        Position = position;
        ParentMolecule = parentMolecule;

        ApplyInfoToModel();
    }

    /// <summary>
    /// Applies all the Info to the Atom Model
    /// </summary>
    public void ApplyInfoToModel ()
    {
        this.name = Element;
        transform.localPosition = Position;

        GetComponent<MeshRenderer>().material = GetAtomColor();

        if (Element == "H")
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    /// <summary>
    /// Loads the Atoms Material Color from files
    /// </summary>
    /// <returns></returns>
    public Material GetAtomColor()
    {
        switch (Element)
        {
            case "H":
                return Resources.Load<Material>("Colors/Hydrogen");
            case "C":
                return Resources.Load<Material>("Colors/Carbon");
            case "N":
                return Resources.Load<Material>("Colors/Nitrogen");
            case "O":
                return Resources.Load<Material>("Colors/Oxygen");
            default:
                return Resources.Load<Material>("Colors/Hydrogen");
        }
    }

    //https://periodictable.com/Properties/A/AtomicRadius.an.pr.html

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
