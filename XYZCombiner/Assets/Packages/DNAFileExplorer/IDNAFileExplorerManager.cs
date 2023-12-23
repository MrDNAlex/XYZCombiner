using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDNAFileExplorerManager 
{
    /// <summary>
    /// Determines the Starting path of the File Explorer page. Leaving Blank leads to last explored page
    /// </summary>
    public string StartPath { get; set; }

    /// <summary>
    /// Determines the file extension you can open
    /// </summary>
    public string FileExtension { get; set; }

    /// <summary>
    /// Describes the Goal of the File Explorer instance
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Stores the last path to a file found
    /// </summary>
    public string LastPath { get; set; }

    /// <summary>
    /// Toggle Determining if Debug Commands will be displayed
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    /// Gets the file path of the chosen file in the Explorer Window
    /// </summary>
    /// <returns> Path of the file chosen </returns>
    public string GetFilePath(string fileExtension = null, string startPath = null, string description = null);

    /// <summary>
    /// Loads a file from a specified file path as JSON and returns it as a specified type
    /// </summary>
    /// <typeparam name="T"> Return Type </typeparam>
    /// <param name="filePath">  Path to the File Being Loaded </param>
    /// <returns></returns>
    public T LoadFromJSON<T>(string filePath = null);

    /// <summary>
    /// Loads the file from a specified file path or the last loaded file path and returns it as a string
    /// </summary>
    /// <param name="filePath"> Path to the File Being Loaded </param>
    /// <returns></returns>
    public string LoadFileAsString(string filePath = null);

    /// <summary>
    /// Verifies if the File Path leads to a File
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool IsViableFile(string filePath = null);

    /// <summary>
    /// Verifies if the Path leads to a Folder
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public bool IsViableFolder(string filePath = null);

    /// <summary>
    /// Saves Content into a File with specified Extansion through a File Explorer Window
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileExtension"></param>
    /// <param name="startPath"></param>
    /// <param name="description"></param>
    public void SaveToFile(string file, string fileExtension = null, string startPath = null, string description = null);

}
