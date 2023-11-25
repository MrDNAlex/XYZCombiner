using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAFileExplorer;
using UnityEngine.UI;


public class WorldSpaceManager : MonoBehaviour
{
    /// <summary>
    /// Stores the list of Molecules in the World Space
    /// </summary>
    public List<Molecule> Molecules { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Molecules = new List<Molecule>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Opens a new XYZFile and saves it
    /// </summary>
    public string GetXYZFile()
    {
        DNAFileExplorerManager fileExplore = new DNAFileExplorerManager(true);
        fileExplore.FileExtension = "XYZ";
        fileExplore.Description = "Select a XYZ File";

        fileExplore.GetFilePath();
        return fileExplore.LoadFileAsString();
    }

    /// <summary>
    /// Imports a Molecule from a XYZ File
    /// </summary>
    public void ImportMolecule()
    {
        GameObject MoleculePrefab = Resources.Load<GameObject>("Molecule");

        GameObject moleculeObject = GameObject.Instantiate(MoleculePrefab, GameObject.Find("WorldSpace").transform);
        Molecule molecule = moleculeObject.GetComponent<Molecule>();
        molecule.CreateMoleculeFromXYZ(GetXYZFile());

        Molecules.Add(molecule);
    }

    /// <summary>
    /// Formats the List of Molecules
    /// </summary>
    /// <returns></returns>
    public List<string> GetMoleculeList ()
    {
        List<string> moleculeList = new List<string>();

        foreach(Molecule molecule in Molecules)
        {
           string moleculeInfo = molecule.MoleculeName + $"   ({molecule.Position.x}, {molecule.Position.y}, {molecule.Position.z})";

            moleculeList.Add(moleculeInfo);
        }

        return moleculeList;
    }
}
