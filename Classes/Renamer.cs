using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RenamerPro.Classes
{
    class Renamer
    {
        //--------------------------------------------------------------------------------
        // Variables
        //--------------------------------------------------------------------------------

        public string oldName { get; set; }
        public string newName { get; set; }
        public string rootPath { get; set; }

        public bool isAppendMode { get; set; }
        public bool matchCase { get; set; }
        public bool appendAtStart { get; set; }

        private int numberOfFiles = 0;
        private int numberofFilesRenamed = 0;
        private List<string> listFiles = new List<string>();

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public Renamer() {}
        
        //--------------------------------------------------------------------------------
        // Public Functions
        //--------------------------------------------------------------------------------

        public void RenameAllFiles()
        {
            foreach (string s in listFiles)
            {
                RenameFile(s);
                Console.WriteLine("Parsed file: " + s);
            }
        }

        //--------------------------------------------------------------------------------

        public void CollectAllFiles(bool includeSubFolders)
        {
            try
            {
                if (includeSubFolders)
                    CollectAllFilesIncludingSubfolders();
                else
                    CollectAllFilesExcludingSubfolders();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                Logger.WriteWarning(ex.ToString());

            }
        }

        //--------------------------------------------------------------------------------

        public void ResetAppState()
        {
            oldName = null;
            newName = null;
            numberOfFiles = 0;
            numberofFilesRenamed = 0;
            listFiles = new List<string>();
        }

        //--------------------------------------------------------------------------------
        // Private Functions
        //--------------------------------------------------------------------------------
        
        private void CollectAllFilesIncludingSubfolders()
        {
            Queue<string> queue = new Queue<string>();
            string subPath = null;
            queue.Enqueue(rootPath);

            while (queue.Count > 0)
            {
                subPath = queue.Dequeue();

                // add all the sub directories to the queue
                try
                {
                    foreach (string subDir in Directory.GetDirectories(subPath))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteWarning(ex.ToString());
                }

                // collect all files for the current sub directory
                try
                {
                    foreach (string file in Directory.GetFiles(subPath))
                    {
                        listFiles.Add(file);
                        numberOfFiles++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteWarning(ex.ToString());
                }
            }
        }

        //--------------------------------------------------------------------------------

        private void CollectAllFilesExcludingSubfolders()
        {
            foreach (string s in Directory.GetFiles(rootPath))
            {
                listFiles.Add(s);
                numberOfFiles++;
            }
        }

        //--------------------------------------------------------------------------------

        private bool RenameFile(string path)
        {
            //Find the position of the last occurence of '\' in order to know which directory it's in
            int position = path.LastIndexOf('\\');

            //Get the directory of the file
            string pathWithoutFileName = path.Substring(0, position + 1);

            //Delete the old file and create a new one with the new name.
            try
            {
                File.Move(path, pathWithoutFileName + UpdateFileName(path));
                //Count the number of files renamed by checking if the file names are diff.
                if (path != (pathWithoutFileName + UpdateFileName(path)))
                {
                    numberofFilesRenamed++;                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read file. Please ensure all files have an extension",
                                "Error");
                Logger.WriteWarning(ex.ToString());
                return false;
            }
            return true;
        }

        //--------------------------------------------------------------------------------

        private string UpdateFileName(string path)
        {
            string fileName = Path.GetFileName(path);
            string extension = Path.GetExtension(path);
            string fileNameWithoutExtension = fileName.Remove(fileName.Length - extension.Length);
            string updatedName;

            if (isAppendMode)
            {
                updatedName = AppendModeFileName(fileNameWithoutExtension, appendAtStart);
            }
            else
            {
                updatedName = ReplaceModeFileName(fileNameWithoutExtension, matchCase);
            }

            return (updatedName + extension);
        }        

        //--------------------------------------------------------------------------------

        private string AppendModeFileName(string oldFileName, bool atStart)
        {
            if (atStart)
            {
                return oldFileName.Insert(0, newName);
            }
            else
            {
                return oldFileName.Insert(oldFileName.Length, newName);
            }
        }

        //--------------------------------------------------------------------------------

        private string ReplaceModeFileName(string oldFileName, bool matchCase)
        {
            if (matchCase)
            {
                return oldFileName.Replace(oldName, newName);
            }
            else
            {
                var regex = new Regex(oldName, RegexOptions.IgnoreCase);
                return regex.Replace(oldFileName, newName);
            }
        }

        //--------------------------------------------------------------------------------
        // Getters
        //--------------------------------------------------------------------------------

        public int GetNumberOfFilesFound()
        {
            return numberOfFiles;
        }

        //--------------------------------------------------------------------------------

        public int GetNumberOfFilesRenamed()
        {
            return numberofFilesRenamed;
        }

        //--------------------------------------------------------------------------------

        public List<string> GetListOfFiles()
        {
            return listFiles;
        }

        //--------------------------------------------------------------------------------
    }
}
