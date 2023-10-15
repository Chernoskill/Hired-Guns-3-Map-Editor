
namespace Hired_Guns_3_Map_Editor
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            System.Windows.Forms.GroupBox CellPropertyIconsGroupBox;
            this.EditorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.OriginalCellLightingMethodToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ShowExtendedCursorInfoToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ShowDoorToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ShowLiftToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ShowWaterToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ShowPushableToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            this.ClearRegistryOnExitToggleButton = new Hired_Guns_3_Map_Editor.ToggleButton();
            CellPropertyIconsGroupBox = new System.Windows.Forms.GroupBox();
            CellPropertyIconsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // OriginalCellLightingMethodToggleButton
            // 
            this.OriginalCellLightingMethodToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.OriginalCellLightingMethodToggleButton.Location = new System.Drawing.Point(12, 43);
            this.OriginalCellLightingMethodToggleButton.Name = "OriginalCellLightingMethodToggleButton";
            this.OriginalCellLightingMethodToggleButton.Size = new System.Drawing.Size(160, 24);
            this.OriginalCellLightingMethodToggleButton.TabIndex = 1;
            this.OriginalCellLightingMethodToggleButton.Text = "Original cell lighting method";
            this.EditorToolTip.SetToolTip(this.OriginalCellLightingMethodToggleButton, resources.GetString("OriginalCellLightingMethodToggleButton.ToolTip"));
            this.OriginalCellLightingMethodToggleButton.UseVisualStyleBackColor = true;
            this.OriginalCellLightingMethodToggleButton.CheckedChanged += new System.EventHandler(this.OriginalCellLightingMethodToggleButton_CheckedChanged);
            // 
            // ShowExtendedCursorInfoToggleButton
            // 
            this.ShowExtendedCursorInfoToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ShowExtendedCursorInfoToggleButton.Location = new System.Drawing.Point(13, 13);
            this.ShowExtendedCursorInfoToggleButton.Name = "ShowExtendedCursorInfoToggleButton";
            this.ShowExtendedCursorInfoToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ShowExtendedCursorInfoToggleButton.TabIndex = 0;
            this.ShowExtendedCursorInfoToggleButton.Text = "Show extended cursor info";
            this.EditorToolTip.SetToolTip(this.ShowExtendedCursorInfoToggleButton, "Displays additional data for the cell under the cursor.");
            this.ShowExtendedCursorInfoToggleButton.UseVisualStyleBackColor = true;
            this.ShowExtendedCursorInfoToggleButton.CheckedChanged += new System.EventHandler(this.ShowExtendedCursorInfoToggleButton_CheckedChanged);
            // 
            // ShowDoorToggleButton
            // 
            this.ShowDoorToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ShowDoorToggleButton.Location = new System.Drawing.Point(6, 49);
            this.ShowDoorToggleButton.Name = "ShowDoorToggleButton";
            this.ShowDoorToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ShowDoorToggleButton.TabIndex = 2;
            this.ShowDoorToggleButton.Text = "Locked door / door key";
            this.EditorToolTip.SetToolTip(this.ShowDoorToggleButton, resources.GetString("ShowDoorToggleButton.ToolTip"));
            this.ShowDoorToggleButton.UseVisualStyleBackColor = true;
            this.ShowDoorToggleButton.CheckedChanged += new System.EventHandler(this.ShowDoorToggleButton_CheckedChanged);
            // 
            // CellPropertyIconsGroupBox
            // 
            CellPropertyIconsGroupBox.Controls.Add(this.ShowPushableToggleButton);
            CellPropertyIconsGroupBox.Controls.Add(this.ShowWaterToggleButton);
            CellPropertyIconsGroupBox.Controls.Add(this.ShowLiftToggleButton);
            CellPropertyIconsGroupBox.Controls.Add(this.ShowDoorToggleButton);
            CellPropertyIconsGroupBox.Location = new System.Drawing.Point(13, 73);
            CellPropertyIconsGroupBox.Name = "CellPropertyIconsGroupBox";
            CellPropertyIconsGroupBox.Size = new System.Drawing.Size(181, 140);
            CellPropertyIconsGroupBox.TabIndex = 3;
            CellPropertyIconsGroupBox.TabStop = false;
            CellPropertyIconsGroupBox.Text = "Cell property icons";
            // 
            // ShowLiftToggleButton
            // 
            this.ShowLiftToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ShowLiftToggleButton.Location = new System.Drawing.Point(6, 79);
            this.ShowLiftToggleButton.Name = "ShowLiftToggleButton";
            this.ShowLiftToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ShowLiftToggleButton.TabIndex = 3;
            this.ShowLiftToggleButton.Text = "Lift min z / max z";
            this.EditorToolTip.SetToolTip(this.ShowLiftToggleButton, resources.GetString("ShowLiftToggleButton.ToolTip"));
            this.ShowLiftToggleButton.UseVisualStyleBackColor = true;
            this.ShowLiftToggleButton.CheckedChanged += new System.EventHandler(this.ShowLiftToggleButton_CheckedChanged);
            // 
            // ShowWaterToggleButton
            // 
            this.ShowWaterToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ShowWaterToggleButton.Location = new System.Drawing.Point(6, 19);
            this.ShowWaterToggleButton.Name = "ShowWaterToggleButton";
            this.ShowWaterToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ShowWaterToggleButton.TabIndex = 4;
            this.ShowWaterToggleButton.Text = "Water";
            this.EditorToolTip.SetToolTip(this.ShowWaterToggleButton, resources.GetString("ShowWaterToggleButton.ToolTip"));
            this.ShowWaterToggleButton.UseVisualStyleBackColor = true;
            this.ShowWaterToggleButton.CheckedChanged += new System.EventHandler(this.ShowWaterToggleButton_CheckedChanged);
            // 
            // ShowPushableToggleButton
            // 
            this.ShowPushableToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ShowPushableToggleButton.Location = new System.Drawing.Point(6, 109);
            this.ShowPushableToggleButton.Name = "ShowPushableToggleButton";
            this.ShowPushableToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ShowPushableToggleButton.TabIndex = 5;
            this.ShowPushableToggleButton.Text = "Pushable";
            this.EditorToolTip.SetToolTip(this.ShowPushableToggleButton, resources.GetString("ShowPushableToggleButton.ToolTip"));
            this.ShowPushableToggleButton.UseVisualStyleBackColor = true;
            this.ShowPushableToggleButton.CheckedChanged += new System.EventHandler(this.ShowPushableToggleButton_CheckedChanged);
            // 
            // ClearRegistryOnExitToggleButton
            // 
            this.ClearRegistryOnExitToggleButton.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            this.ClearRegistryOnExitToggleButton.Location = new System.Drawing.Point(12, 219);
            this.ClearRegistryOnExitToggleButton.Name = "ClearRegistryOnExitToggleButton";
            this.ClearRegistryOnExitToggleButton.Size = new System.Drawing.Size(160, 24);
            this.ClearRegistryOnExitToggleButton.TabIndex = 4;
            this.ClearRegistryOnExitToggleButton.Text = "Clear recent files on exit";
            this.EditorToolTip.SetToolTip(this.ClearRegistryOnExitToggleButton, resources.GetString("ClearRegistryOnExitToggleButton.ToolTip"));
            this.ClearRegistryOnExitToggleButton.UseVisualStyleBackColor = true;
            this.ClearRegistryOnExitToggleButton.CheckedChanged += new System.EventHandler(this.ClearRegistryOnExitToggleButton_CheckedChanged);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ClearRegistryOnExitToggleButton);
            this.Controls.Add(CellPropertyIconsGroupBox);
            this.Controls.Add(this.OriginalCellLightingMethodToggleButton);
            this.Controls.Add(this.ShowExtendedCursorInfoToggleButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsForm";
            this.Text = "OptionsForm";
            CellPropertyIconsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ToggleButton ShowExtendedCursorInfoToggleButton;
        private ToggleButton OriginalCellLightingMethodToggleButton;
        private System.Windows.Forms.ToolTip EditorToolTip;
        private ToggleButton ShowDoorToggleButton;
        private ToggleButton ShowLiftToggleButton;
        private ToggleButton ShowWaterToggleButton;
        private ToggleButton ShowPushableToggleButton;
        private ToggleButton ClearRegistryOnExitToggleButton;
    }
}