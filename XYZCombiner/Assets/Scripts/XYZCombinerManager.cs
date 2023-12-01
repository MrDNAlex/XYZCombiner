using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DNAFileExplorer;


public class XYZCombinerManager : MonoBehaviour
{
    /// <summary>
    /// Import Molecule Button
    /// </summary>
    [SerializeField] Button importMoleculeBTN;

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
    /// Text Box Displaying if the Translation Mode is Active
    /// </summary>
    [SerializeField] GameObject TranslationMode;

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

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;

        WorldSpaceManager = WorldSpaceManagerObject.GetComponent<WorldSpaceManager>();

        importMoleculeBTN.onClick.AddListener(ImportNewMolecule);

        WorldSpaceManager.UpdateGUI = UpdateUI;
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

        TranslationMode.GetComponent<Text>().text = $"Translation : {WorldSpaceManager.TranslateMode}";
        RotationMode.GetComponent<Text>().text = $"Rotation : {WorldSpaceManager.RotateMode}";
    }

    /// <summary>
    /// Updates the info for the selected Atom
    /// </summary>
    public void UpdateSelectedAtom()
    {
        GameObject infoPrefab = Resources.Load<GameObject>("ListInfo");

        string info = WorldSpaceManager.GetSelectedAtom();

        if (info != "")
        {
            GameObject infoLine = GameObject.Instantiate(infoPrefab, SelectedList.transform);
            infoLine.GetComponent<Text>().text = info;
        }
      
        info = WorldSpaceManager.GetSelectedMolecule();

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
