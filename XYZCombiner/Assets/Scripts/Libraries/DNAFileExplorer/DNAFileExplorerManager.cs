using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DNAFileExplorer
{
    public class DNAFileExplorerManager : MonoBehaviour
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

        public bool DebugMode { get; private set; }

        /// <summary>
        /// Initializes the File Explorer Manager
        /// </summary>
        public DNAFileExplorerManager(bool debugMode = false)
        {
            StartPath = "";
            FileExtension = "";
            Description = "";
            DebugMode = debugMode;
        }

        /// <summary>
        /// Gets the file path of the chosen file in the Explorer Window
        /// </summary>
        /// <returns> Path of the file chosen </returns>
        public string GetFilePath(string fileExtension = null, string startPath = null, string description = null)
        {
            fileExtension = fileExtension != null ? fileExtension : FileExtension;
            startPath = startPath != null ? startPath : StartPath;
            description = description != null ? description : Description;

            LastPath = EditorUtility.OpenFilePanel(description, startPath, fileExtension);

            if (DebugMode)
                Debug.Log(LastPath);

            return LastPath;
        }

        /// <summary>
        /// Loads a file from a specified file path as JSON and returns it as a specified type
        /// </summary>
        /// <typeparam name="T"> Return Type </typeparam>
        /// <param name="filePath">  Path to the File Being Loaded </param>
        /// <returns></returns>
        public T LoadFromJSON<T> (string filePath = null)
        {
            string jsonData = "";
            filePath = filePath != null ? filePath : LastPath;

            if (IsViablePath(filePath))
                jsonData = File.ReadAllText(filePath);

            if (DebugMode)
                Debug.Log(jsonData);

            return JsonUtility.FromJson<T>(jsonData);
        }

        /// <summary>
        /// Loads the file from a specified file path or the last loaded file path and returns it as a string
        /// </summary>
        /// <param name="filePath"> Path to the File Being Loaded </param>
        /// <returns></returns>
        public string LoadFileAsString (string filePath = null)
        {
            string file = "";
            filePath = filePath != null ? filePath : LastPath;

            if (IsViablePath(filePath))
                file = File.ReadAllText(filePath);

            if (DebugMode)
                Debug.Log(file);

            return file;
        }

        /// <summary>
        /// Verifies if the File Path leads to a File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsViablePath (string filePath = null)
        {
            filePath = filePath != null ? filePath : LastPath;

            try
            {
                File.ReadAllText(filePath);
                return true;
            }
            catch
            {
                Debug.Log("Not a Viable Path");
                return false;
            }
        }
    }
}

