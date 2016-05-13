using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void RenameAllFiles(bool matchCase)
        {
            foreach (string s in listFiles)
            {
                RenameFile(s, matchCase);
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
            rootPath = null;
            numberOfFiles = 0;
            numberofFilesRenamed = 0;
            listFiles = new List<string>();
        }

        //--------------------------------------------------------------------------------
        // Private Functions
        //--------------------------------------------------------------------------------

        private string ReplaceFileNameSubstring(string path, bool matchCase)
        {
            string fileName = Path.GetFileName(path);
            string extension = Path.GetExtension(path);
            string fileNameWithoutExtension = fileName.Remove(fileName.Length - extension.Length);
            string updatedName;

            if (matchCase)
            {
                updatedName = fileNameWithoutExtension.Replace(oldName, newName);
            }
            else
            {
                var regex = new Regex(oldName, RegexOptions.IgnoreCase);
                updatedName = regex.Replace(fileNameWithoutExtension, newName);
            }

            return (updatedName + extension);
        }

        //--------------------------------------------------------------------------------

        private bool RenameFile(string path, bool matchCase)
        {
            //Find the position of the last occurence of '\' in order to know which directory it's in
            int position = path.LastIndexOf('\\');

            //Get the directory of the file
            string pathWithoutFileName = path.Substring(0, position + 1);

            //Delete the old file and create a new one with the new name.
            try
            {
                File.Move(path, pathWithoutFileName + ReplaceFileNameSubstring(path, matchCase));
                //Count the number of files renamed by checking if the file names are diff.
                if (path != (pathWithoutFileName + ReplaceFileNameSubstring(path, matchCase)))
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
