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

    /*
    void InstantiateMolecule(GameObject gameObj)
    {
        Debug.Log(gameObj.GetComponent<BoxCollider>().size);

        // Check for collisions
        Collider[] colliders = Physics.OverlapBox(gameObj.transform.position, gameObj.GetComponent<BoxCollider>().size);

        // Adjust position if there are collisions
        foreach (Collider collider in colliders)
        {
            if (collider != gameObj.GetComponent<Collider>()) // Skip self
            {
                Vector3 newPos = FindNonCollidingPosition(gameObj, collider);
                gameObj.transform.position = newPos;
            }
        }
    }

    Vector3 FindNonCollidingPosition(GameObject obj, Collider otherCollider)
    {
        // Implement a logic to find a non-colliding position
        // This can be as simple as moving the object along one axis until no collision is detected
        Vector3 newPosition = obj.transform.position;

        Vector3 hitboxSize = obj.GetComponent<BoxCollider>().size;

        if (hitboxSize.x < hitboxSize.y && hitboxSize.x < hitboxSize.z)
        {
            newPosition.x += hitboxSize.x * 0.5f;
        }
        else if (hitboxSize.y < hitboxSize.x && hitboxSize.y < hitboxSize.z)
        {
            newPosition.y += hitboxSize.y * 0.5f;
        }
        else if (hitboxSize.z < hitboxSize.y && hitboxSize.z < hitboxSize.x)
        {
            newPosition.z += hitboxSize.z * 0.5f;
        }

        return newPosition;
    }
    */

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
