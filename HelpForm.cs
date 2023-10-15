using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Hired_Guns_3_Map_Editor
{
    public partial class HelpForm : Form
    {
        private MainForm _mainForm;

        public HelpForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            InitializeHelpText();
        }
        private void InitializeHelpText()
        {
            HelpTextBox.Clear();
            /*
            HelpTextBox.AppendText("HOTKEYS\r\n\r\n");
            HelpTextBox.AppendText("Delete:\t\t\tClear selected cells\r\n");
            HelpTextBox.AppendText("End:\t\t\tReset cell controls\r\n");
            HelpTextBox.AppendText("Page up:\t\t\tZ level +\r\n");
            HelpTextBox.AppendText("Page down:\t\tZ level -\r\n");
            HelpTextBox.AppendText("Add/Plus:\t\t\tZoom map view in\r\n");
            HelpTextBox.AppendText("Subtract/Minus:\t\tZoom map view out\r\n");
            HelpTextBox.AppendText("A:\t\t\tToggle active mode\r\n");
            HelpTextBox.AppendText("M:\t\t\tMode: Select\r\n");
            HelpTextBox.AppendText("E:\t\t\tMode: Erase\r\n");
            HelpTextBox.AppendText("B:\t\t\tMode: Paint\r\n");
            HelpTextBox.AppendText("G:\t\t\tMode: Fill\r\n");
            HelpTextBox.AppendText("Middle mouse button:\t\t\tPick cell\r\n");
            HelpTextBox.AppendText("r\nBASIC USAGE\r\n\r\n");
            */
            if (File.Exists("README.txt") == false)
            {
                HelpTextBox.Text += "README.txt not found.";
                HelpTextBox.SelectionStart = 0;
                HelpTextBox.SelectionLength = 0;
                return;
            }
            //HelpTextBox.SelectionTabs = new int[] { 80, 160, 240, 320 };
            //HelpTextBox.AcceptsTab = true;

            HelpTextBox.Text += File.ReadAllText("README.txt");
            HelpTextBox.SelectionStart = 0;
            HelpTextBox.SelectionLength = 0;
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
