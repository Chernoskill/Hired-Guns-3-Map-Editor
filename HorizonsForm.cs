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
using System.IO;
using HGHelperUtilities;

namespace Hired_Guns_3_Map_Editor
{
    public partial class HorizonsForm : Form
    {
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
        public HorizonsForm(MainForm mainForm)
        {
            Mainform = mainForm;
            //Mainform.Enabled = false;
            InitializeComponent();
            SkyStyleDropdownList.SelectedIndex = 0;
            UpdateSkies();
        }
        private void HorizonsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Mainform == null)
            {
                Close();
                return;
            }
            //Mainform.Enabled = true;
        }

        public void UpdateHorizons(HGMapData mapData)
        {
            if(mapData != null && mapData.horizons != null && mapData.horizons.Length == 4)
            {
                if(HorizonNPictureBoxWithInterpolationMode.Image != null)
                {
                    HorizonNPictureBoxWithInterpolationMode.Image.Dispose();
                }
                if (HorizonEPictureBoxWithInterpolationMode.Image != null)
                {
                    HorizonEPictureBoxWithInterpolationMode.Image.Dispose();
                }
                if (HorizonSPictureBoxWithInterpolationMode.Image != null)
                {
                    HorizonSPictureBoxWithInterpolationMode.Image.Dispose();
                }
                if (HorizonWPictureBoxWithInterpolationMode.Image != null)
                {
                    HorizonWPictureBoxWithInterpolationMode.Image.Dispose();
                }
                HorizonNPictureBoxWithInterpolationMode.Image = mapData.horizons[0];
                HorizonEPictureBoxWithInterpolationMode.Image = mapData.horizons[1];
                HorizonSPictureBoxWithInterpolationMode.Image = mapData.horizons[2];
                HorizonWPictureBoxWithInterpolationMode.Image = mapData.horizons[3];
            }
            //Invalidate();
        }

        private void HorizonsImportButton_Click(object sender, EventArgs e)
        {
            if(Mainform == null)
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Mainform.editorDir;
            openFileDialog.Filter = "PNG files (*.png)|*.png";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream inStream;
                if ((inStream = openFileDialog.OpenFile()) != null)
                {
                    ImportHorizons(openFileDialog.FileName);
                }
                inStream.Close();
            }
        }

        private void ImportHorizons(string path)
        {
            if(Mainform == null)
            {
                return;
            }
            HGMapData mapData = Mainform.GetMapData();
            if (mapData == null)
            {
                return;
            }
            Bitmap original = new Bitmap(path);
            if(original.Width < HGMapData.MAP_HORIZONWIDTH || original.Height < HGMapData.MAP_HORIZONHEIGHT * 4)
            {
                return;
            }
            Bitmap img = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            HelperUtil.CopyRegionIntoImage(original, new Rectangle(0, 0, original.Width, original.Height), ref img, new Rectangle(0, 0, original.Width, original.Height));
            original.Dispose();
            if (img == null)
            {
                return;
            }
            mapData.horizons = new Bitmap[4];
            Color transColor = Color.Transparent;
            for (int i = 0; i < mapData.horizons.Length; i++)
            {
                using (BmpPixelSnoop snoopImg = new BmpPixelSnoop(img))
                {
                    Bitmap bm = new Bitmap(HGMapData.MAP_HORIZONWIDTH, HGMapData.MAP_HORIZONHEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    
                    using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                    {
                        for (int y = 0; y < bm.Height; y++)
                        {
                            for (int x = 0; x < bm.Width; x++)
                            {
                                //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                                //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                                snoop.SetPixel(x, y, (snoopImg.GetPixel(x, HGMapData.MAP_HORIZONHEIGHT * i + y).GetBrightness() < 0.3 && x < HGMapData.MAP_HORIZONWIDTH - 2) ? Color.Black : transColor);
                            }
                        }
                    }
                    if(mapData.horizons[i] != null)
                    {
                        mapData.horizons[i].Dispose();
                    }
                    mapData.horizons[i] = bm;
                }   
            }
            UpdateHorizons(Mainform.GetMapData());
        }

        private void SkyStyleDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSkies();
        }
        public void UpdateSkies()
        {
            if (Mainform == null)
            {
                return;
            }
            /*
            if(HorizonNPictureBoxWithInterpolationMode.BackgroundImage != null)
            {
                HorizonNPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
            }
            if (HorizonEPictureBoxWithInterpolationMode.BackgroundImage != null)
            {
                HorizonEPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
            }
            if (HorizonSPictureBoxWithInterpolationMode.BackgroundImage != null)
            {
                HorizonSPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
            }
            if (HorizonWPictureBoxWithInterpolationMode.BackgroundImage != null)
            {
                HorizonWPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
            }
            */
            if(SkyStyleDropdownList.SelectedIndex == 2)
            {
                /*
                if (HorizonNPictureBoxWithInterpolationMode.BackgroundImage != null)
                {
                    HorizonNPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
                }
                if (HorizonEPictureBoxWithInterpolationMode.BackgroundImage != null)
                {
                    HorizonEPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
                }
                if (HorizonSPictureBoxWithInterpolationMode.BackgroundImage != null)
                {
                    HorizonSPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
                }
                if (HorizonWPictureBoxWithInterpolationMode.BackgroundImage != null)
                {
                    HorizonWPictureBoxWithInterpolationMode.BackgroundImage.Dispose();
                }
                */
                HorizonNPictureBoxWithInterpolationMode.BackgroundImage = null;
                HorizonNPictureBoxWithInterpolationMode.BackgroundImage = null;
                HorizonEPictureBoxWithInterpolationMode.BackgroundImage = null;
                HorizonSPictureBoxWithInterpolationMode.BackgroundImage = null;
                HorizonWPictureBoxWithInterpolationMode.BackgroundImage = null;
            }
            else
            {
                int sky = Mainform.GetMapData().sky;
                HorizonNPictureBoxWithInterpolationMode.BackgroundImage = Mainform.GetSkyImg(sky, SkyStyleDropdownList.SelectedIndex == 0 ? false : true);
                HorizonNPictureBoxWithInterpolationMode.BackgroundImage = Mainform.GetSkyImg(sky, SkyStyleDropdownList.SelectedIndex == 0 ? false : true);
                HorizonEPictureBoxWithInterpolationMode.BackgroundImage = Mainform.GetSkyImg(sky, SkyStyleDropdownList.SelectedIndex == 0 ? false : true);
                HorizonSPictureBoxWithInterpolationMode.BackgroundImage = Mainform.GetSkyImg(sky, SkyStyleDropdownList.SelectedIndex == 0 ? false : true);
                HorizonWPictureBoxWithInterpolationMode.BackgroundImage = Mainform.GetSkyImg(sky, SkyStyleDropdownList.SelectedIndex == 0 ? false : true);
            }
        }

        private void ExportToBitmapButton_Click(object sender, EventArgs e)
        {
            HGMapData mapData = _mainForm.GetMapData();
            if (mapData == null)
            {
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select output filename and directory...";
            saveFileDialog.Filter = "PNG files (*.png)|*.png";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bm = new Bitmap(HorizonsBlankPictureBox.Image);
                for (int i = 0; i < mapData.horizons.Length; i++)
                {
                    if(mapData.horizons[i] == null)
                    {
                        continue;
                    }
                    HelperUtil.CopyRegionIntoImage(mapData.horizons[i], new Rectangle(0, 0, 142, 32), ref bm, new Rectangle(0,i * 32,142,32));
                    bm.Save(saveFileDialog.FileName);

                }
                bm.Dispose();
            }
        }
    }
}
