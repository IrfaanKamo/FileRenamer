using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RenamerPro.Classes;

namespace RenamerPro
{
    public partial class MainForm : Form
    {
        Renamer renamer;
        bool isAppendMode = false;

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

            //If the textboxes are empty we cannot proceed to replace names.
            if (!isAppendMode && txtFind.Text == "")
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
            if (AreIllegalCharactersUsed(txtReplace.Text))
            {
                MessageBox.Show("A file name can't contain any of the following characters: \\ / : * ? < > |", "Illegal Characters");
                return;
            }
                    
            renamer.oldName = txtFind.Text;
            renamer.newName = txtReplace.Text;
            renamer.isAppendMode = isAppendMode;
            renamer.appendAtStart = startRadioButton.Checked;
            renamer.CollectAllFiles(chckSubFolders.Checked);
            renamer.RenameAllFiles();
 
            MessageBox.Show(renamer.GetNumberOfFilesRenamed() + " of " + renamer.GetNumberOfFilesFound() +" files renamed", 
                            "Renaming Complete");
            renamer.ResetAppState();
        }

        //--------------------------------------------------------------------------------

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAppendMode = false;
            appendToolStripMenuItem.BackColor = SystemColors.Control;
            appendToolStripMenuItem.Enabled = true;
            replaceToolStripMenuItem.BackColor = SystemColors.ActiveCaption;
            replaceToolStripMenuItem.Enabled = false;
            label1.Text = "Find what:";
            txtFind.Show();
            startRadioButton.Hide();
            endRadioButton.Hide();
            label2.Text = "Replace with:";
            chckMatchCase.Enabled = true;
        }

        //--------------------------------------------------------------------------------

        private void appendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isAppendMode = true;
            appendToolStripMenuItem.BackColor = SystemColors.ActiveCaption;
            appendToolStripMenuItem.Enabled = false;
            replaceToolStripMenuItem.BackColor = SystemColors.Control;
            replaceToolStripMenuItem.Enabled = true;
            label1.Text = "Append at:";
            txtFind.Hide();
            startRadioButton.Show();
            endRadioButton.Show();
            label2.Text = "Append with:";
            chckMatchCase.Enabled = false;
            chckMatchCase.Checked = false;
        }
        //--------------------------------------------------------------------------------

        private bool AreIllegalCharactersUsed(string fileName)
        {
            char[] illegalChars = { '<', '>', ':', '/', '\\', '|', '?', '*', '"' };
            foreach (char c in illegalChars)
            {
                if (fileName.Contains(c))
                {
                    return true;
                }
            }
            return false;
        }

        //--------------------------------------------------------------------------------
    }
}
