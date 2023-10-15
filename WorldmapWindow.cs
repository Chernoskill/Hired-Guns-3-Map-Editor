using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HiredGunsData;
using HGHelperUtilities;

namespace Hired_Guns_3_Map_Editor
{
    public partial class WorldmapWindow : Form
    {
        //private double posMultiplier = 1.3306;
        private double posMultiplier = 1.0;
        private MainForm _mainForm;
        public MainForm Mainform
        {
            get
            {
                return _mainForm;
            }
            set
            {
                _mainForm = value;
            }
        }
        private Point _worldmapMousePos;
        public WorldmapWindow(MainForm mainForm)
        {
            Mainform = mainForm;
            //Mainform.Enabled = false;
            InitializeComponent();
            WorldmapImgPanel.Controls.Remove(WorldPosPictureBox);
            WorldmapImgPictureBox.Controls.Add(WorldPosPictureBox);
            WorldPosPictureBox.Parent = WorldmapImgPictureBox;
        }

        private void WorldmapWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Mainform == null)
            {
                Close();
                return;
            }
            //Mainform.Enabled = true;
        }
        public void UpdateWorldmap(HGMapData mapData)
        {
            if(mapData == null || Mainform == null)
            {
                Close();
                return;
            }
            if(WorldmapImgPictureBox.Image != null)
            {
                WorldmapImgPictureBox.Size = WorldmapImgPictureBox.Image.Size;
            }
            WorldPosPictureBox.Location = new Point((int)Math.Round((mapData.worldMapPosX - 3) * posMultiplier), (int)Math.Round((mapData.worldMapPosY - 3) * posMultiplier));
        }
        private void WorldmapImgPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            _worldmapMousePos = e.Location;
        }
        private void WorldmapImgPictureBox_Click(object sender, EventArgs e)
        {
            if (Mainform == null)
            {
                Close();
                return;
            }
            Mainform.SetWorldPos(_worldmapMousePos.X, _worldmapMousePos.Y);
            WorldPosPictureBox.Location = new Point((int)Math.Round((_worldmapMousePos.X - 3) * posMultiplier), (int)Math.Round((_worldmapMousePos.Y - 3) * posMultiplier));
        }
    }
}
