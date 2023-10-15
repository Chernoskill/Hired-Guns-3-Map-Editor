
namespace Hired_Guns_3_Map_Editor
{
    partial class WorldmapWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldmapWindow));
            this.WorldmapImgPanel = new System.Windows.Forms.Panel();
            this.WorldPosPictureBox = new Hired_Guns_3_Map_Editor.PictureBoxWithInterpolationMode();
            this.WorldmapImgPictureBox = new Hired_Guns_3_Map_Editor.PictureBoxWithInterpolationMode();
            this.WorldmapImgPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorldPosPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldmapImgPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // WorldmapImgPanel
            // 
            this.WorldmapImgPanel.AutoScroll = true;
            this.WorldmapImgPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WorldmapImgPanel.Controls.Add(this.WorldPosPictureBox);
            this.WorldmapImgPanel.Controls.Add(this.WorldmapImgPictureBox);
            this.WorldmapImgPanel.Location = new System.Drawing.Point(13, 13);
            this.WorldmapImgPanel.Name = "WorldmapImgPanel";
            this.WorldmapImgPanel.Size = new System.Drawing.Size(512, 417);
            this.WorldmapImgPanel.TabIndex = 0;
            // 
            // WorldPosPictureBox
            // 
            this.WorldPosPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.WorldPosPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.WorldPosPictureBox.Enabled = false;
            this.WorldPosPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("WorldPosPictureBox.Image")));
            this.WorldPosPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.WorldPosPictureBox.Location = new System.Drawing.Point(265, 188);
            this.WorldPosPictureBox.Name = "WorldPosPictureBox";
            this.WorldPosPictureBox.Size = new System.Drawing.Size(7, 7);
            this.WorldPosPictureBox.TabIndex = 1;
            this.WorldPosPictureBox.TabStop = false;
            // 
            // WorldmapImgPictureBox
            // 
            this.WorldmapImgPictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("WorldmapImgPictureBox.BackgroundImage")));
            this.WorldmapImgPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.WorldmapImgPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            this.WorldmapImgPictureBox.Location = new System.Drawing.Point(16, 16);
            this.WorldmapImgPictureBox.Name = "WorldmapImgPictureBox";
            this.WorldmapImgPictureBox.Size = new System.Drawing.Size(481, 384);
            this.WorldmapImgPictureBox.TabIndex = 0;
            this.WorldmapImgPictureBox.TabStop = false;
            this.WorldmapImgPictureBox.Click += new System.EventHandler(this.WorldmapImgPictureBox_Click);
            this.WorldmapImgPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WorldmapImgPictureBox_MouseMove);
            // 
            // WorldmapWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 445);
            this.Controls.Add(this.WorldmapImgPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(720, 613);
            this.Name = "WorldmapWindow";
            this.Text = "Worldmap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorldmapWindow_FormClosing);
            this.WorldmapImgPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.WorldPosPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WorldmapImgPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel WorldmapImgPanel;
        private PictureBoxWithInterpolationMode WorldmapImgPictureBox;
        private PictureBoxWithInterpolationMode WorldPosPictureBox;
    }
}