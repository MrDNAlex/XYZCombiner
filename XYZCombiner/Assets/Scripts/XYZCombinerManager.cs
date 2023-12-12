using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manager controller the XYZ Combiner Application
/// </summary>
public class XYZCombinerManager : MonoBehaviour
{
    /// <summary>
    /// Displays the Status of the Specified Axis
    /// </summary>
    [SerializeField] Text SpecifiedAxis;

    /// <summary>
    /// Displays Status of the Translation Mode Toggle
    /// </summary>
    [SerializeField] Toggle TranslationToggle;

    /// <summary>
    /// Displays Status of the Rotation Mode Toggle
    /// </summary>
    [SerializeField] Toggle RotationToggle;

    /// <summary>
    /// Displays Status of Translating Along a Vector
    /// </summary>
    [SerializeField] Toggle TranslateAlongVectorToggle;

    /// <summary>
    /// Displays Status Rotating Objects to align their Vectors
    /// </summary>
    [SerializeField] Toggle RotateAlignVectorToggle;

    /// <summary>
    /// Button for Saving the Bundled Molecule to the Device
    /// </summary>
    [SerializeField] Button SaveMoleculeBtn;

    /// <summary>
    /// Button for Bundling all Molecules in Scene into a Single Molecule
    /// </summary>
    [SerializeField] Button BundleMolecules;

    /// <summary>
    /// Import Molecule Button
    /// </summary>
    [SerializeField] Button ImportMoleculeBTN;

    /// <summary>
    /// World Space Manager Object 
    /// </summary>
    [SerializeField] GameObject WorldSpaceManagerObject;

    /// <summary>
    /// Text Box displaying the Molecule List
    /// </summary>
    [SerializeField] GameObject MoleculeList;

    /// <summary>
    /// Text Box Displaying the Selected Molecule or Atom
    /// </summary>
    [SerializeField] GameObject SelectedList;

    /// <summary>
    /// Text Box Displaying if the Rotation Mode is Active
    /// </summary>
    [SerializeField] GameObject RotationMode;

    /// <summary>
    /// Text Box Displaying the Selected Atoms Info
    /// </summary>
    [SerializeField] Text SelectedAtom;

    /// <summary>
    /// World Space Manager Instance
    /// </summary>
    public WorldSpaceManager WorldSpaceManager { get; set; }

    /// <summary>
    /// Manager controlling the Process to Save the Selected Molecule to the Device
    /// </summary>
    SaveToXYZManager SaveToXYZ;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;

        WorldSpaceManager = WorldSpaceManagerObject.GetComponent<WorldSpaceManager>();

        ImportMoleculeBTN.onClick.AddListener(ImportNewMolecule);
        BundleMolecules.onClick.AddListener(Bundle);

        WorldSpaceManager.UpdateGUI = UpdateUI;
        WorldSpaceManager.TransformManager.SetUpdateFunction(UpdateUI);
    }

    /// <summary>
    /// Bundles the Molecules in the Scene into a single
    /// </summary>
    public void Bundle()
    {
        SaveToXYZ = new SaveToXYZManager();

        SaveToXYZ.BundleIntoMolecule(WorldSpaceManager);

        SaveMoleculeBtn.onClick.RemoveAllListeners();
        SaveMoleculeBtn.onClick.AddListener(SaveToXYZ.SaveMolecule);

        UpdateUI();
    }

    /// <summary>
    /// Imports a new Molecule into the World Space
    /// </summary>
    public void ImportNewMolecule()
    {
        WorldSpaceManager.ImportMolecule();
    }

    /// <summary>
    /// Updates the UI Displaying Info about the Scene
    /// </summary>
    public void UpdateUI()
    {
        //Clear Children
        foreach (Transform child in MoleculeList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in SelectedList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        UpdateMoleculeList();
        UpdateSelectedAtom();

        TranslationToggle.isOn = WorldSpaceManager.TransformManager.TransformationAction == TransformManager.Transformation.Translation;
        RotationToggle.isOn = WorldSpaceManager.TransformManager.TransformationAction == TransformManager.Transformation.Rotation;

        SpecifiedAxis.text = $"Axis: {WorldSpaceManager.TransformManager.TransformAxis}";

        RotationMode.GetComponent<Text>().text = $"Vector : {WorldSpaceManager.TransformManager.GetVectors()}";
    }

    /// <summary>
    /// Updates the info for the selected Atom
    /// </summary>
    public void UpdateSelectedAtom()
    {
        GameObject infoPrefab = Resources.Load<GameObject>("ListInfo");

        string info = WorldSpaceManager.TransformManager.GetSelectedAtom();

        if (info != "")
        {
            GameObject infoLine = GameObject.Instantiate(infoPrefab, SelectedList.transform);
            infoLine.GetComponent<Text>().text = info;
        }

        info = WorldSpaceManager.TransformManager.GetSelectedMolecule();

        if (info != "")
        {
            GameObject infoLine = GameObject.Instantiate(infoPrefab, SelectedList.transform);
            infoLine.GetComponent<Text>().text = info;
        }
    }

    /// <summary>
    /// Updates the Molecule List
    /// </summary>
    public void UpdateMoleculeList()
    {
        GameObject infoPrefab = Resources.Load<GameObject>("ListInfo");

        List<string> info = WorldSpaceManager.GetMoleculeList();

        foreach (string molInfo in info)
        {
            GameObject infoLine = GameObject.Instantiate(infoPrefab, MoleculeList.transform);
            infoLine.GetComponent<Text>().text = molInfo;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OpenFolder(string fileName)
    {
        //System.Diagnostics.Process.Start(fileName);

        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo = new System.Diagnostics.ProcessStartInfo("explorer.exe");
        p.Start();
    }
}
