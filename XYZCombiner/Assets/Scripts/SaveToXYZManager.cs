using DNAFileExplorer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveToXYZManager
{
    /// <summary>
    /// Molecule that is Created, Bundled and Saved
    /// </summary>
    public Molecule Molecule { get; set; }

    /// <summary>
    /// Initializes the SaveToXYZ Manager
    /// </summary>
    public SaveToXYZManager()
    {
        GameObject MoleculePrefab = Resources.Load<GameObject>("Molecule");
        GameObject moleculeObject = GameObject.Instantiate(MoleculePrefab, GameObject.Find("WorldSpace").transform);

        Molecule = moleculeObject.GetComponent<Molecule>();
    }

    /// <summary>
    /// Bundles all Molecules in the Scene into a Single Molecule
    /// </summary>
    /// <param name="WorldSpaceManager"></param>
    public void BundleIntoMolecule(WorldSpaceManager WorldSpaceManager)
    {
        Molecule.MoleculeName = "Bundled Molecule";

        List<Molecule> removalList = new List<Molecule>();
        Vector3 AvgPosition = Vector3.zero;

        foreach (Molecule molecule in WorldSpaceManager.Molecules)
        {
            AvgPosition += molecule.Position;

            Molecule.AddAtoms(molecule.Atoms);
            removalList.Add(molecule);
        }

        foreach (Molecule molecule in removalList)
        {
            WorldSpaceManager.Molecules.Remove(molecule);
            molecule.DestroyMolecule();
        }

        WorldSpaceManager.Molecules.Add(Molecule);

        Molecule.SetHitBoxDimensions();
        Molecule.Position = Molecule.Atoms[0].FilePositionOffset;
    }

    /// <summary>
    /// Returns the Molecules Information in XYZ Format
    /// </summary>
    public string GetXYZFile ()
    {
        string XYZFile = "";

        XYZFile += $"{Molecule.Atoms.Count}\n";
        XYZFile += $"{Molecule.MoleculeName}";

        foreach (Atom atom in Molecule.Atoms)
            XYZFile += $"\n{atom.XYZFormat()}";

        return XYZFile;
    }

    /// <summary>
    /// Saves the Molecule to the Device in XYZ Format
    /// </summary>
    public void SaveMolecule ()
    {
        DNAFileExplorerManager fileExplore = new DNAFileExplorerManager(true);
        fileExplore.FileExtension = "XYZ";
        fileExplore.Description = "Save as a XYZ File";

        fileExplore.SaveToFile(GetXYZFile());
    }
}
