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

        RemoveOvelapping(moleculeObject);

        Molecules.Add(molecule);
    }

    /// <summary>
    /// Loops infinitely until there are no longer colliding molecules
    /// </summary>
    /// <param name="gameObj"></param>
    void RemoveOvelapping(GameObject gameObj)
    {
        bool overlapping = true;
        int loop = 0;

        while(overlapping)
        {
            // Check for collisions
            Collider[] colliders = Physics.OverlapBox(gameObj.transform.position, gameObj.GetComponent<BoxCollider>().size);

            if (colliders.Length == 0 || loop >= 100)
                overlapping = false;

            // Adjust position if there are collisions
            foreach (Collider collider in colliders)
            {
                if (collider != gameObj.GetComponent<Collider>() && collider.gameObject.tag == "Molecule")
                    gameObj.transform.position = FindNonCollidingPosition(gameObj, collider);
            }

            loop++;
        }
       
    }

    /// <summary>
    /// Finds the Shortest change in position necessary to remove the collision between the molecule and another
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="otherCollider"></param>
    /// <returns></returns>
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
