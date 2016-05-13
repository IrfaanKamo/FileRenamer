using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RenamerPro.Classes;

namespace RenamerPro
{
    public partial class MainForm : Form
    {
        Renamer renamer;

        //--------------------------------------------------------------------------------
        public MainForm()
        {
            InitializeComponent();
            renamer = new Renamer();
        }

        //--------------------------------------------------------------------------------

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                //Allow the user to replace the names since a folder was selected.
                btnReplace.Enabled = true;
                renamer.rootPath = folderBrowser.SelectedPath;
                txtDirectory.Text = renamer.rootPath;
            }
        }
        //--------------------------------------------------------------------------------

        private void btnReplace_Click(object sender, EventArgs e)
        {
            //If the list has items in it, do not attempt to replace names.
            if (renamer.GetListOfFiles().Count != 0)
                return;

            //If the textboxes are empty we cannot procede to replace names.
            if (txtFind.Text == "")
            {
                MessageBox.Show("Please provide a search criteria", "Search Criteria Missing");
                return;
            }
            if (txtReplace.Text == "")
            {
                MessageBox.Show("Please provide a replacement criteria", "Replacement Criteria Missing");
                return;
            }

            //If the replacement text contains any illegal characters we cannot proceed.
            char[] illegalChars = {'<', '>', ':', '/', '\\', '|', '?', '*', '"' };
            foreach (char c in illegalChars)
            {
                if (txtReplace.Text.Contains(c))
                {
                    MessageBox.Show("Illegal Characters used in replacement text", "Illegal Characters");
                    return;
                }
            }
                    
            renamer.oldName = txtFind.Text;
            renamer.newName = txtReplace.Text;

            renamer.CollectAllFiles(chckSubFolders.Checked);
            renamer.RenameAllFiles(chckMatchCase.Checked);
 
            MessageBox.Show(renamer.GetNumberOfFilesRenamed() + " of " + renamer.GetNumberOfFilesFound() +" files renamed", 
                            "Renaming Complete");
            renamer.ResetAppState();
        }
        //--------------------------------------------------------------------------------

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //--------------------------------------------------------------------------------

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        //--------------------------------------------------------------------------------

        private void howDoIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }
        //--------------------------------------------------------------------------------
    }
}
