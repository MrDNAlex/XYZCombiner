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
    public UpdateUI UpdateGUI;

    /// <summary>
    /// Toggle for the Translation Edit Mode
    /// </summary>
    public bool TranslateMode { get; set; }


    public bool RotateMode { get; set; }

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


    public LayerMask TranslationLayerMask { get { return LayerMask.GetMask("TranslationPlane"); } }

    /// <summary>
    /// Currently Selected Atom
    /// </summary>
    public Atom SelectedAtom { get; set; }

    /// <summary>
    /// Currently Selected Molecule
    /// </summary>
    public Molecule SelectedMolecule { get; set; }

    public Vector3 lastDelta;
    // Start is called before the first frame update
    void Start()
    {
        Molecules = new List<Molecule>();

        TranslateMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectAtom();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            TranslateMode = !TranslateMode;

            UpdateGUI.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateMode = !RotateMode;
            lastDelta = SelectedMolecule.Position - GetRayPosition(TranslationLayerMask);
            //StartRotation = SelectedMolecule.transform.rotation;
            UpdateGUI.Invoke();
        }

        //Make it so that there is a plane perpendicular to the camera (normal of plane is looking at camera) and the plane is located at the molecule or atoms center, then use a Hit Ray scan to move it around on the plane. Have the plane rotate around always normal 

        if (TranslateMode)
        {
            AlignWithCameraDirection();
            TranslateSelectedObject();

            UpdateGUI.Invoke();
        }

        if (RotateMode)
        {
            AlignWithCameraDirection();
            RotateSelectedObject();
            UpdateGUI.Invoke();
        }

    }

    public void RotateSelectedObject()
    {

        //This is not quite right still

        
        Vector3 currentMousePosition = GetRayPosition(TranslationLayerMask);
        Vector3 deltaMousePosition = SelectedMolecule.Position - currentMousePosition;

        Vector3 rotationAxis = TranslationPlane.transform.up; // You can change the rotation axis as needed

        // Use Quaternion to calculate rotation
        Quaternion rotation = Quaternion.FromToRotation(lastDelta, deltaMousePosition);

        // Define a maximum angle to rotate in one frame
        float maxAngle = 1000f * Time.deltaTime;

        // Use RotateTowards to limit the rotation angle
        Quaternion targetRotation = Quaternion.RotateTowards(SelectedMolecule.transform.rotation, SelectedMolecule.transform.rotation * rotation, maxAngle);

        SelectedMolecule.transform.rotation = targetRotation;

        lastDelta = deltaMousePosition;
        
        /*
        Vector3 currentMousePosition = GetRayPosition(TranslationLayerMask);
        Vector3 deltaMousePosition = SelectedMolecule.Position - currentMousePosition;

        Vector3 rotationAxis = TranslationPlane.transform.up;

        // Project the rotation axis onto the plane defined by TranslationPlane.up
        rotationAxis = Vector3.ProjectOnPlane(rotationAxis, deltaMousePosition.normalized).normalized;

        // Use Quaternion to calculate rotation
        Quaternion rotation = Quaternion.FromToRotation(lastDelta, deltaMousePosition);

        // Define a maximum angle to rotate in one frame
        float maxAngle = 500f * Time.deltaTime;

        // Use RotateTowards to limit the rotation angle
        Quaternion targetRotation = Quaternion.RotateTowards(SelectedMolecule.transform.rotation, SelectedMolecule.transform.rotation * rotation, maxAngle);

        SelectedMolecule.transform.rotation = targetRotation;

        lastDelta = deltaMousePosition;
        */
        //Not right either, work off of this though, we will probably have to develop the 
    }

    public Vector3 GetRayPosition(LayerMask layerMask)
    {
        // Create a ray from the camera's position pointing forward
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Set the maximum distance the ray can travel
        float maxRaycastDistance = 1000000000000;

        // Create a RaycastHit variable to store information about the hit
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, layerMask))
        {
            if (hit.collider != null)
            {
                return hit.point;
            }
            else
                return Vector3.zero;
        }
        else
            return Vector3.zero;
    }

    public void TranslateSelectedObject()
    {
        // Create a ray from the camera's position pointing forward
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Set the maximum distance the ray can travel
        float maxRaycastDistance = 1000000000000;

        // Create a RaycastHit variable to store information about the hit
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, TranslationLayerMask))
        {
            if (hit.collider != null)
                SelectedMolecule.Position = hit.point;
        }
    }

    void AlignWithCameraDirection()
    {
        // Get the main camera
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 cameraForward = mainCamera.transform.forward;
            TranslationPlane.transform.up = -1 * cameraForward;
            TranslationPlane.transform.position = SelectedMolecule.Position;
        }
        else
        {
            Debug.LogError("Main camera not found.");
        }
    }

    /// <summary>
    /// Sends a Raycast in the hopes of hitting and selecting an Atom
    /// </summary>
    public void SelectAtom()
    {
        // Create a ray from the camera's position pointing forward
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Set the maximum distance the ray can travel
        float maxRaycastDistance = 10000f;

        // Create a RaycastHit variable to store information about the hit
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, AtomLayerMask))
        {
            if (hit.collider != null)
            {
                SelectedAtom = hit.collider.gameObject.GetComponent<Atom>();
                SelectedMolecule = SelectedAtom.ParentMolecule;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, MoleculeLayerMask))
            {
                // Check if the object hit is on the specified layer
                if (hit.collider != null)
                {
                    SelectedMolecule = hit.collider.gameObject.GetComponent<Molecule>();
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
