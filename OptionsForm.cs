using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hired_Guns_3_Map_Editor
{
    public partial class OptionsForm : Form
    {
        private MainForm _mainForm;
        public OptionsForm(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;

            if(ShowExtendedCursorInfoToggleButton.Checked != _mainForm.ShowExtendedCursorInfo)
            {
                ShowExtendedCursorInfoToggleButton.Checked = _mainForm.ShowExtendedCursorInfo;
            }
            if (OriginalCellLightingMethodToggleButton.Checked != _mainForm.OriginalCellLightingMethod)
            {
                OriginalCellLightingMethodToggleButton.Checked = _mainForm.OriginalCellLightingMethod;
            }
            ShowWaterToggleButton.Checked = _mainForm.ShowWater;
            ShowDoorToggleButton.Checked = _mainForm.ShowDoor;
            ShowLiftToggleButton.Checked = _mainForm.ShowLift;
            ShowPushableToggleButton.Checked = _mainForm.ShowPushable;
            if (ClearRegistryOnExitToggleButton.Checked != _mainForm.menuClearRegistryOnExit)
            {
                ClearRegistryOnExitToggleButton.Checked = _mainForm.menuClearRegistryOnExit;
            }
        }

        private void ShowExtendedCursorInfoToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            _mainForm.ShowExtendedCursorInfo = ShowExtendedCursorInfoToggleButton.Checked;
        }

        private void OriginalCellLightingMethodToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            _mainForm.OriginalCellLightingMethod = OriginalCellLightingMethodToggleButton.Checked;
        }
        private void ShowWaterToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_mainForm.ShowWater != ShowWaterToggleButton.Checked)
            {
                _mainForm.ShowWater = ShowWaterToggleButton.Checked;
            }
        }

        private void ShowDoorToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if(_mainForm.ShowDoor != ShowDoorToggleButton.Checked)
            {
                _mainForm.ShowDoor = ShowDoorToggleButton.Checked;
            }
        }

        private void ShowLiftToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_mainForm.ShowLift != ShowLiftToggleButton.Checked)
            {
                _mainForm.ShowLift = ShowLiftToggleButton.Checked;
            }
        }

        private void ShowPushableToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_mainForm.ShowPushable != ShowPushableToggleButton.Checked)
            {
                _mainForm.ShowPushable = ShowPushableToggleButton.Checked;
            }
        }

        private void ClearRegistryOnExitToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if(_mainForm.menuClearRegistryOnExit != ClearRegistryOnExitToggleButton.Checked)
            {
                _mainForm.menuClearRegistryOnExit = ClearRegistryOnExitToggleButton.Checked;
            }
        }
    }
}
