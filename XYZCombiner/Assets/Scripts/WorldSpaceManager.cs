using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DNAFileExplorer;
using UnityEngine.UI;


public class WorldSpaceManager : MonoBehaviour
{
    [SerializeField] GameObject TranslationPlane;

    /// <summary>
    /// Delegate Function Type for the Update GUI Function
    /// </summary>
    public delegate void UpdateUI();

    /// <summary>
    /// Delegate Function storing a reference to the UpdateGUI Function in the XYZCombinerManager
    /// </summary>
    public UpdateUI UpdateGUI { get; set; }

    /// <summary>
    /// Transform Manager that handles all Transform related actions such as Rotation and Translation
    /// </summary>
    public TransformManager TransformManager { get; set; }

    /// <summary>
    /// Stores the list of Molecules in the World Space
    /// </summary>
    public List<Molecule> Molecules { get; set; }

    /// <summary>
    /// Returns the Layer Mask Number for Molecules
    /// </summary>
    public int MoleculeLayer { get { return LayerMask.NameToLayer("Molecule"); } }

    /// <summary>
    /// Returns the Layer Mask Number for Atoms
    /// </summary>
    public int AtomLayer { get { return LayerMask.NameToLayer("Atom"); } }

    /// <summary>
    /// Layer Mask for Selecting Molecules Only
    /// </summary>
    public LayerMask MoleculeLayerMask { get { return LayerMask.GetMask("Molecule"); } }

    /// <summary>
    /// Layer Mask for Selecting Atoms Only
    /// </summary>
    public LayerMask AtomLayerMask { get { return LayerMask.GetMask("Atom"); } }

    /// <summary>
    /// Currently Selected Atom
    /// </summary>
    public Atom SelectedAtom { get; set; }

    /// <summary>
    /// Currently Selected Molecule
    /// </summary>
    public Molecule SelectedMolecule { get; set; }

    private void Awake()
    {
        Molecules = new List<Molecule>();
        TransformManager = new TransformManager();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            SelectAtom();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RemoveAtom();
        }

        TransformManager.UpdateTransformations();
    }

    /// <summary>
    /// Sends a Raycast in the hopes of hitting and selecting an Atom
    /// </summary>
    public void SelectAtom()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float maxRaycastDistance = 10000f;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRaycastDistance, AtomLayerMask))
        {
            if (hit.collider != null)
            {
                SelectedAtom = hit.collider.gameObject.GetComponent<Atom>();
                SelectedMolecule = SelectedAtom.ParentMolecule;
                TransformManager.SetSelectedObj(SelectedAtom.gameObject);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, MoleculeLayerMask))
            {
                if (hit.collider != null)
                {
                    SelectedMolecule = hit.collider.gameObject.GetComponent<Molecule>();
                    TransformManager.SetSelectedObj(SelectedMolecule.gameObject);
                    SelectedAtom = null;
                }
            }
            else
            {
                SelectedAtom = null;
                SelectedMolecule = null;
            }
        }

        UpdateGUI.Invoke();
    }

    /// <summary>
    /// Removes the Atom from the Molecule
    /// </summary>
    private void RemoveAtom ()
    {
        //Not quite right
        SelectedMolecule.Atoms.Remove(SelectedAtom);
        GameObject.Destroy(SelectedAtom.gameObject);
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

        UpdateGUI.Invoke();
    }

    /// <summary>
    /// Loops infinitely until there are no longer colliding molecules
    /// </summary>
    /// <param name="gameObj"></param>
    void RemoveOvelapping(GameObject gameObj)
    {
        bool overlapping = true;
        int loop = 0;

        while (overlapping)
        {
            // Check for collisions
            Collider[] colliders = Physics.OverlapBox(gameObj.transform.position, gameObj.GetComponent<BoxCollider>().size, gameObj.transform.rotation, MoleculeLayerMask);

            if (colliders.Length == 0 || loop >= 100)
                overlapping = false;

            // Adjust position if there are collisions
            foreach (Collider collider in colliders)
            {
                if (collider != gameObj.GetComponent<Collider>() && collider.gameObject.layer == MoleculeLayer)
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
    public List<string> GetMoleculeList()
    {
        List<string> moleculeList = new List<string>();

        foreach (Molecule molecule in Molecules)
        {
            string moleculeInfo = molecule.MoleculeName + $"   ({molecule.Position.x}, {molecule.Position.y}, {molecule.Position.z})";

            moleculeList.Add(moleculeInfo);
        }

        return moleculeList;
    }

    /// <summary>
    /// Returns the Info of the Selected Atom
    /// </summary>
    /// <returns></returns>
    public string GetSelectedAtom()
    {
        if (SelectedAtom != null)
            return $" {SelectedAtom.name} : {SelectedAtom.Position} : {SelectedAtom.FilePosition}";
        else
            return "";
    }

    /// <summary>
    /// Returns the Info of the Selected Molecule
    /// </summary>
    /// <returns></returns>
    public string GetSelectedMolecule()
    {
        if (SelectedMolecule != null)
            return $" {SelectedMolecule.name} : {SelectedMolecule.Position}";
        else
            return "";
    }
}
