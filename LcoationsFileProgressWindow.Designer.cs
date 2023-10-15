
namespace Hired_Guns_3_Map_Editor
{
    partial class LocationsFileProgressWindow
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
            System.Windows.Forms.Label MapFileBaseLabel;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationsFileProgressWindow));
            System.Windows.Forms.Label GameDirLabel;
            this.ConfirmationButton = new System.Windows.Forms.Button();
            this.ProgressTextBox = new System.Windows.Forms.RichTextBox();
            this.DevDirLabel = new System.Windows.Forms.Label();
            this.DevDirTextBox = new System.Windows.Forms.TextBox();
            this.BrowseDevDirButton = new System.Windows.Forms.Button();
            this.BrowseGameDirButton = new System.Windows.Forms.Button();
            this.GameDirTextBox = new System.Windows.Forms.TextBox();
            this.DevDirCheckBox = new System.Windows.Forms.CheckBox();
            this.DevDirGroupBox = new System.Windows.Forms.GroupBox();
            this.InsertRadioButton = new System.Windows.Forms.RadioButton();
            this.CreateLocationsRadioButton = new System.Windows.Forms.RadioButton();
            this.EditorToolTip = new System.Windows.Forms.ToolTip(this.components);
            MapFileBaseLabel = new System.Windows.Forms.Label();
            GameDirLabel = new System.Windows.Forms.Label();
            this.DevDirGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // MapFileBaseLabel
            // 
            MapFileBaseLabel.Location = new System.Drawing.Point(173, 83);
            MapFileBaseLabel.Name = "MapFileBaseLabel";
            MapFileBaseLabel.Size = new System.Drawing.Size(452, 71);
            MapFileBaseLabel.TabIndex = 7;
            MapFileBaseLabel.Text = resources.GetString("MapFileBaseLabel.Text");
            // 
            // GameDirLabel
            // 
            GameDirLabel.AutoSize = true;
            GameDirLabel.Location = new System.Drawing.Point(19, 18);
            GameDirLabel.Name = "GameDirLabel";
            GameDirLabel.Size = new System.Drawing.Size(83, 13);
            GameDirLabel.TabIndex = 8;
            GameDirLabel.Text = "Game Directory:";
            // 
            // ConfirmationButton
            // 
            this.ConfirmationButton.Location = new System.Drawing.Point(281, 375);
            this.ConfirmationButton.Name = "ConfirmationButton";
            this.ConfirmationButton.Size = new System.Drawing.Size(75, 23);
            this.ConfirmationButton.TabIndex = 0;
            this.ConfirmationButton.Text = "Go!";
            this.ConfirmationButton.UseVisualStyleBackColor = true;
            this.ConfirmationButton.Click += new System.EventHandler(this.ConfirmationButton_Click);
            // 
            // ProgressTextBox
            // 
            this.ProgressTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.ProgressTextBox.Location = new System.Drawing.Point(11, 157);
            this.ProgressTextBox.Name = "ProgressTextBox";
            this.ProgressTextBox.ReadOnly = true;
            this.ProgressTextBox.Size = new System.Drawing.Size(614, 211);
            this.ProgressTextBox.TabIndex = 1;
            this.ProgressTextBox.Text = "";
            this.ProgressTextBox.WordWrap = false;
            // 
            // DevDirLabel
            // 
            this.DevDirLabel.AutoSize = true;
            this.DevDirLabel.Location = new System.Drawing.Point(8, 17);
            this.DevDirLabel.Name = "DevDirLabel";
            this.DevDirLabel.Size = new System.Drawing.Size(75, 13);
            this.DevDirLabel.TabIndex = 2;
            this.DevDirLabel.Text = "Dev Directory:";
            // 
            // DevDirTextBox
            // 
            this.DevDirTextBox.Location = new System.Drawing.Point(97, 14);
            this.DevDirTextBox.Name = "DevDirTextBox";
            this.DevDirTextBox.Size = new System.Drawing.Size(446, 20);
            this.DevDirTextBox.TabIndex = 3;
            this.DevDirTextBox.WordWrap = false;
            this.DevDirTextBox.TextChanged += new System.EventHandler(this.DevDirTextBox_TextChanged);
            // 
            // BrowseDevDirButton
            // 
            this.BrowseDevDirButton.Location = new System.Drawing.Point(549, 14);
            this.BrowseDevDirButton.Name = "BrowseDevDirButton";
            this.BrowseDevDirButton.Size = new System.Drawing.Size(59, 21);
            this.BrowseDevDirButton.TabIndex = 4;
            this.BrowseDevDirButton.Text = "Browse...";
            this.BrowseDevDirButton.UseVisualStyleBackColor = true;
            this.BrowseDevDirButton.Click += new System.EventHandler(this.BrowseDevDirButton_Click);
            // 
            // BrowseGameDirButton
            // 
            this.BrowseGameDirButton.Location = new System.Drawing.Point(560, 15);
            this.BrowseGameDirButton.Name = "BrowseGameDirButton";
            this.BrowseGameDirButton.Size = new System.Drawing.Size(59, 21);
            this.BrowseGameDirButton.TabIndex = 10;
            this.BrowseGameDirButton.Text = "Browse...";
            this.BrowseGameDirButton.UseVisualStyleBackColor = true;
            this.BrowseGameDirButton.Click += new System.EventHandler(this.BrowseGameDirButton_Click);
            // 
            // GameDirTextBox
            // 
            this.GameDirTextBox.Location = new System.Drawing.Point(108, 15);
            this.GameDirTextBox.Name = "GameDirTextBox";
            this.GameDirTextBox.Size = new System.Drawing.Size(446, 20);
            this.GameDirTextBox.TabIndex = 9;
            this.GameDirTextBox.WordWrap = false;
            this.GameDirTextBox.TextChanged += new System.EventHandler(this.GameDirTextBox_TextChanged);
            // 
            // DevDirCheckBox
            // 
            this.DevDirCheckBox.AutoSize = true;
            this.DevDirCheckBox.Location = new System.Drawing.Point(11, 86);
            this.DevDirCheckBox.Name = "DevDirCheckBox";
            this.DevDirCheckBox.Size = new System.Drawing.Size(144, 17);
            this.DevDirCheckBox.TabIndex = 11;
            this.DevDirCheckBox.Text = "Enable development files";
            this.DevDirCheckBox.UseVisualStyleBackColor = true;
            this.DevDirCheckBox.CheckedChanged += new System.EventHandler(this.DevDirCheckBox_CheckedChanged);
            // 
            // DevDirGroupBox
            // 
            this.DevDirGroupBox.Controls.Add(this.DevDirTextBox);
            this.DevDirGroupBox.Controls.Add(this.DevDirLabel);
            this.DevDirGroupBox.Controls.Add(this.BrowseDevDirButton);
            this.DevDirGroupBox.Enabled = false;
            this.DevDirGroupBox.Location = new System.Drawing.Point(11, 36);
            this.DevDirGroupBox.Name = "DevDirGroupBox";
            this.DevDirGroupBox.Size = new System.Drawing.Size(614, 44);
            this.DevDirGroupBox.TabIndex = 12;
            this.DevDirGroupBox.TabStop = false;
            // 
            // InsertRadioButton
            // 
            this.InsertRadioButton.AutoSize = true;
            this.InsertRadioButton.Location = new System.Drawing.Point(11, 110);
            this.InsertRadioButton.Name = "InsertRadioButton";
            this.InsertRadioButton.Size = new System.Drawing.Size(110, 17);
            this.InsertRadioButton.TabIndex = 13;
            this.InsertRadioButton.TabStop = true;
            this.InsertRadioButton.Text = "Insert current map";
            this.EditorToolTip.SetToolTip(this.InsertRadioButton, "Insert the currently opened map\'s data into an existing Locations.dat file");
            this.InsertRadioButton.UseVisualStyleBackColor = true;
            this.InsertRadioButton.CheckedChanged += new System.EventHandler(this.InsertRadioButton_CheckedChanged);
            // 
            // CreateLocationsRadioButton
            // 
            this.CreateLocationsRadioButton.AutoSize = true;
            this.CreateLocationsRadioButton.Location = new System.Drawing.Point(11, 133);
            this.CreateLocationsRadioButton.Name = "CreateLocationsRadioButton";
            this.CreateLocationsRadioButton.Size = new System.Drawing.Size(123, 17);
            this.CreateLocationsRadioButton.TabIndex = 14;
            this.CreateLocationsRadioButton.TabStop = true;
            this.CreateLocationsRadioButton.Text = "Create Locations.dat";
            this.EditorToolTip.SetToolTip(this.CreateLocationsRadioButton, "Create a new Locations.dat file");
            this.CreateLocationsRadioButton.UseVisualStyleBackColor = true;
            this.CreateLocationsRadioButton.CheckedChanged += new System.EventHandler(this.CreateLocationsRadioButton_CheckedChanged);
            // 
            // LocationsFileProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(637, 403);
            this.Controls.Add(this.CreateLocationsRadioButton);
            this.Controls.Add(this.InsertRadioButton);
            this.Controls.Add(this.DevDirGroupBox);
            this.Controls.Add(this.DevDirCheckBox);
            this.Controls.Add(this.BrowseGameDirButton);
            this.Controls.Add(this.GameDirTextBox);
            this.Controls.Add(GameDirLabel);
            this.Controls.Add(MapFileBaseLabel);
            this.Controls.Add(this.ProgressTextBox);
            this.Controls.Add(this.ConfirmationButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LocationsFileProgressWindow";
            this.Text = "Locations.dat writer";
            this.Shown += new System.EventHandler(this.LocationsFileProgressWindow_Shown);
            this.DevDirGroupBox.ResumeLayout(false);
            this.DevDirGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ConfirmationButton;
        private System.Windows.Forms.RichTextBox ProgressTextBox;
        public System.Windows.Forms.TextBox DevDirTextBox;
        private System.Windows.Forms.Button BrowseDevDirButton;
        private System.Windows.Forms.Button BrowseGameDirButton;
        public System.Windows.Forms.TextBox GameDirTextBox;
        private System.Windows.Forms.Label DevDirLabel;
        private System.Windows.Forms.CheckBox DevDirCheckBox;
        private System.Windows.Forms.GroupBox DevDirGroupBox;
        private System.Windows.Forms.RadioButton InsertRadioButton;
        private System.Windows.Forms.RadioButton CreateLocationsRadioButton;
        private System.Windows.Forms.ToolTip EditorToolTip;
    }
}