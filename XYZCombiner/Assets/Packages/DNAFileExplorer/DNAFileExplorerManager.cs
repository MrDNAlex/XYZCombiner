using UnityEngine;
using System.IO;
using SFB;

namespace DNAFileExplorer
{
    /// <summary>
    /// Class that Opens and Saves Files using the OS's Native File Explorer
    /// </summary>
    public class DNAFileExplorerManager : IDNAFileExplorerManager
    {
        /// \<inheritdoc/>
        public string StartPath { get; set; }

        /// \<inheritdoc/>
        public string FileExtension { get; set; }

        /// \<inheritdoc/>
        public string Description { get; set; }

        /// \<inheritdoc/>
        public string LastPath { get; set; }

        /// \<inheritdoc/>
        public bool DebugMode { get; set; }

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

        /// \<inheritdoc/>
        public string GetFilePath(string fileExtension = null, string startPath = null, string description = null)
        {
            fileExtension = fileExtension != null ? fileExtension : FileExtension;
            startPath = startPath != null ? startPath : StartPath;
            description = description != null ? description : Description;

            LastPath = StandaloneFileBrowser.OpenFilePanel(description, startPath, fileExtension, false)[0];

            return LastPath;
        }

        /// \<inheritdoc/>
        public T LoadFromJSON<T>(string filePath = null)
        {
            string jsonData = "";
            filePath = filePath != null ? filePath : LastPath;

            if (IsViableFile(filePath))
                jsonData = File.ReadAllText(filePath);

            return JsonUtility.FromJson<T>(jsonData);
        }

        /// \<inheritdoc/>
        public string LoadFileAsString(string filePath = null)
        {
            string file = "";
            filePath = filePath != null ? filePath : LastPath;

            if (IsViableFile(filePath))
                file = File.ReadAllText(filePath);

            return file;
        }

        /// \<inheritdoc/>
        public bool IsViableFile(string filePath = null)
        {
            filePath = filePath != null ? filePath : LastPath;

            try
            {
                File.ReadAllText(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// \<inheritdoc/>
        public bool IsViableFolder(string filePath = null)
        {
            filePath = filePath != null ? filePath : LastPath;

            if (Directory.Exists(filePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// \<inheritdoc/>
        public void SaveToFile(string file, string fileExtension = null, string startPath = null, string description = null)
        {
            fileExtension = fileExtension != null ? fileExtension : FileExtension;
            startPath = startPath != null ? startPath : StartPath;
            description = description != null ? description : Description;

            string savePath = StandaloneFileBrowser.SaveFilePanel(description, startPath, file.Split("\n")[1], fileExtension);

            savePath = savePath.Replace("\\", "/");

            if (IsViableFolder(savePath.Substring(0, savePath.LastIndexOf("/"))) && savePath.Length > 0)
                File.WriteAllText(savePath, file);
        }
    }
}

