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
using HiredGunsData;

namespace Hired_Guns_3_Map_Editor
{
    public partial class LocationsFileProgressWindow : Form
    {
        bool ready = true;
        private MainForm _mainForm;
        public LocationsFileProgressWindow(MainForm mf)
        {
            InitializeComponent();
            _mainForm = mf;
            if(GameDirTextBox.Text != _mainForm.gameDir)
            {
                GameDirTextBox.Text = _mainForm.gameDir;
            }
            if (DevDirTextBox.Text != _mainForm.devDir)
            {
                DevDirTextBox.Text = _mainForm.devDir;
            }
            DevDirCheckBox.Checked = _mainForm.useDevFiles;
            if(_mainForm.insertMapData)
            {
                InsertRadioButton.Checked = _mainForm.insertMapData;
            }
            else
            {
                CreateLocationsRadioButton.Checked = true;
            }

            //Static AddContextMenu function is located in Program.cs
            ProgressTextBox.AddContextMenu();
        }
        private void LocationsFileProgressWindow_Shown(object sender, EventArgs e)
        {
            ConfirmationButton.Text = "Go!";
            ConfirmationButton.Enabled = true;
        }
        public void ClearProgressText()
        {
            ProgressTextBox.Text = "";
        }
        public void AddProgressText(string line)
        {
            ProgressTextBox.AppendText($"{line}\r\n", ProgressTextBox.ForeColor);
        }
        public void AddProgressText(string line, Color col)
        {
            ProgressTextBox.AppendText($"{line}\r\n", col);
        }
        public void StartProcess()
        {
            ready = false;
            ConfirmationButton.Text = "working...";
            ConfirmationButton.Enabled = false;
            ClearProgressText();
            
            if (_mainForm.insertMapData)
            {
                string locationsPath = $"{_mainForm.gameDir}\\World\\Locations.dat";
                if (File.Exists(locationsPath) == false)
                {
                    MessageBox.Show($"Inserting current map data into Locations.dat: : {locationsPath} does not exist! Create a new Locations.dat file instead. Operation cancelled.");
                    AddProgressText($"Inserting current map data into Locations.dat: : {locationsPath} does not exist! Create a new Locations.dat file instead. Operation cancelled.", Color.Red);
                    ready = true;
                    ConfirmationButton.Text = "Go!";
                    ConfirmationButton.Enabled = true;
                    return;
                }
                HGMapData mapData = _mainForm.GetMapData();
                int mapNum = -1;
                string mapName = Path.GetFileNameWithoutExtension(_mainForm.MapFileName);
                if (int.TryParse(mapName, out mapNum) == false || mapName.Length != 2)
                {
                    DialogResult res = MessageBox.Show($"Map filename {mapName} does not correspond to the Hired Guns map file naming scheme (two-digit number)!\r\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if(res == DialogResult.No)
                    {
                        AddProgressText($"Inserting current map data into Locations.dat: Operation cancelled.", Color.Red);
                        ready = true;
                        ConfirmationButton.Text = "Go!";
                        ConfirmationButton.Enabled = true;
                        return;
                    }
                }
                if (mapNum != mapData.file)
                {
                    DialogResult res = MessageBox.Show($"Map filename {mapName} differs from the mapdata file property ({mapData.file}) (\"Map\" register -> File #)!\r\nContinue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (res == DialogResult.No)
                    {
                        AddProgressText($"Inserting current map data into Locations.dat: Operation cancelled.", Color.Red);
                        ready = true;
                        ConfirmationButton.Text = "Go!";
                        ConfirmationButton.Enabled = true;
                        return;
                    }
                }
                string initialDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory($"{_mainForm.editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                File.Copy(locationsPath, "Locations.dat", true);
                _mainForm.DecompressFile("Locations.dat");
                byte[] bytes = File.ReadAllBytes("Locations.dat");

                string fileName = $"{_mainForm.editorDir}\\misc\\" + (mapData.file == 13 ? "dropzone.map" : "default.map");
                if (File.Exists(fileName) == false)
                {
                    MessageBox.Show($"[StartProcess] Base map file {fileName} not found!\r\nOperation cancelled.");
                    AddProgressText($"[StartProcess] Base map file {fileName} not found!\r\nOperation cancelled.", Color.Red);
                    _mainForm.AddConsoleMessage($"[StartProcess] Base map file {fileName} not found!\r\nOperation cancelled.", true);
                    FinishProcess();
                    return;
                }
                byte[] mapBytes = File.ReadAllBytes(fileName);
                mapBytes = HGMapData.MapDataToByteArray(mapBytes, mapData);
                Array.Copy(mapBytes, 0, bytes, (mapData.file - 1) * HGMapData.MAPLOCATION_BYTES, HGMapData.MAPLOCATION_BYTES);//entries start at file number - 1
                File.WriteAllBytes("Locations.dat", bytes.ToArray());
                _mainForm.CompressFile("Locations.dat");
                OriginalLocationFileCheck(locationsPath);
                File.Copy("Locations.dat", locationsPath, true);
                File.Delete("Locations.dat");
                AddProgressText($"Inserted data from opened map (file #{mapData.file}) into\r\n{locationsPath}.");
                FinishProcess();
                _mainForm.AddConsoleMessage($"Updated {locationsPath}");
                Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
            }
            else
            {
                string devGameMapsDir = $"{_mainForm.devDir}\\Maps\\Game";
                AddProgressText($"Maps are read from {_mainForm.GetMapDir()}" + (_mainForm.useDevFiles && Directory.Exists(devGameMapsDir) ? $" and {devGameMapsDir}." : "."));
                AddProgressText("Loading map files...");
                List<byte> bytes = new List<byte>();
                string[] fileNames = Directory.GetFiles(_mainForm.GetMapDir(), "*.map", searchOption: SearchOption.AllDirectories);
                Dictionary<int, string> fileNameDict = new Dictionary<int, string>();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    string fn = fileNames[i];
                    int mapNum = 0;
                    if (int.TryParse(Path.GetFileNameWithoutExtension(fn), out mapNum) == false)
                    {
                        continue;
                    }
                    if (fileNameDict.ContainsKey(mapNum) == false)
                    {
                        fileNameDict.Add(mapNum, fn);
                    }
                }

                Dictionary<int, string> fileNameDictDev = new Dictionary<int, string>();
                if (_mainForm.useDevFiles)
                {
                    if (Directory.Exists(devGameMapsDir))
                    {
                        string[] fileNamesDev = Directory.GetFiles(devGameMapsDir, "*.map", searchOption: SearchOption.AllDirectories);
                        for (int i = 0; i < fileNamesDev.Length; i++)
                        {
                            string fn = fileNamesDev[i];
                            int mapNum = 0;
                            if (int.TryParse(Path.GetFileNameWithoutExtension(fn), out mapNum) == false)
                            {
                                continue;
                            }
                            if (fileNameDictDev.ContainsKey(mapNum) == false)
                            {
                                fileNameDictDev.Add(mapNum, fn);
                            }
                        }
                    }
                    else
                    {
                        AddProgressText($"Development map files should be used, but the development files game maps directory '{devGameMapsDir}' does not exit!", Color.Red);
                    }
                }

                for (int i = 1; i <= 47; i++)
                {
                    string fileName = "";
                    if (fileNameDict.TryGetValue(i, out fileName) == false && fileNameDictDev.TryGetValue(i, out fileName) == false)
                    {
                        string fallbackMapName = i == 13 ? "dropzone.map" : "default.map";
                        fileName = $"{_mainForm.editorDir}\\misc\\{fallbackMapName}";
                        if (File.Exists(fileName) == false)
                        {
                            _mainForm.AddConsoleMessage($"[StartProcess] fileName {fileName} not found!\r\nOperation cancelled.", true);
                            FinishProcess();
                            return;
                        }
                        byte[] fallbackBytes = File.ReadAllBytes(fileName).Skip(0).Take(HGMapData.MAPLOCATION_BYTES).ToArray();
                        if (i != 13)
                        {
                            fallbackBytes[9] = 1;//game type set to Action so it won't be shown on the world map.
                        }
                        fallbackBytes[525] = (byte)HGGameData.GetDiskFromFile(i);
                        fallbackBytes[526] = (byte)i;

                        bytes.AddRange(fallbackBytes);
                        AddProgressText($"{i.ToString().PadLeft(2, '0')} Loading {fileName} (fallback)");
                        continue;
                    }
                    if (new FileInfo(fileName).Length < HGMapData.MAP_BYTES)//must be rnc compressed
                    {
                        string initialDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory($"{_mainForm.editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                        string tempFile = Path.GetFileName(fileName);
                        File.Copy(fileName, tempFile, true);
                        _mainForm.DecompressFile(tempFile);
                        byte[] fileBytes = File.ReadAllBytes(tempFile);
                        bytes.AddRange(fileBytes.Skip(0).Take(HGMapData.MAPLOCATION_BYTES));
                        File.Delete(tempFile);
                        Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                        AddProgressText($"{i.ToString().PadLeft(2, '0')} Loading {fileName} (compressed)");
                    }
                    else
                    {
                        byte[] fileBytes = File.ReadAllBytes(fileName);
                        bytes.AddRange(fileBytes.Skip(0).Take(HGMapData.MAPLOCATION_BYTES));
                        AddProgressText($"{i.ToString().PadLeft(2, '0')} Loading {fileName} (uncompressed)");
                    }
                }
                if (true)
                {
                    string initialDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory($"{_mainForm.editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                    File.WriteAllBytes("Locations.dat", bytes.ToArray());
                    _mainForm.CompressFile("Locations.dat");
                    string locationsPath = $"{_mainForm.gameDir}\\World\\Locations.dat";
                    OriginalLocationFileCheck(locationsPath);
                    File.Copy("Locations.dat", locationsPath, true);
                    File.Delete("Locations.dat");
                    AddProgressText($"Created {locationsPath}");
                    FinishProcess();
                    AddProgressText("Done!");
                    _mainForm.AddConsoleMessage($"Created {locationsPath}");
                    Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                }
            }
            
            //ready = true;
        }
        public void FinishProcess()
        {
            ConfirmationButton.Text = "Close";
            ConfirmationButton.Enabled = true;
        }
        private void OriginalLocationFileCheck(string path)
        {
            DateTime fileDate = new FileInfo(path).LastWriteTimeUtc;
            if (fileDate < new DateTime(1993, 12, 31))
            {
                for (int i = 1; i < 32000; i++)
                {
                    string fileName = $"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}_{i}.dat";
                    if(File.Exists(fileName) == false)
                    {
                        File.Copy(path, fileName, false);
                        MessageBox.Show($"The Location.dat file seems to be from the original game.\r\nDate of last file writing: {fileDate}\r\nA backup has been created: {fileName}.");
                        break;
                    }
                }
            }
        }

        private void ConfirmationButton_Click(object sender, EventArgs e)
        {
            if(ready)
            {
                StartProcess();
            }
            else
            {
                Close();
            }
        }

        private void BrowseGameDirButton_Click(object sender, EventArgs e)
        {
            _mainForm.SelectHiredGunsDirectory();
        }
        private void BrowseDevDirButton_Click(object sender, EventArgs e)
        {
            _mainForm.SelectDevDirectory();
        }

        private void GameDirTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mainForm.gameDir != GameDirTextBox.Text)
            {
                _mainForm.gameDir = GameDirTextBox.Text;
            }
           
        }
        private void DevDirTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mainForm.devDir != DevDirTextBox.Text)
            {
                _mainForm.devDir = DevDirTextBox.Text;
            }

        }

        private void DevDirCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DevDirGroupBox.Enabled = DevDirCheckBox.Checked;
            if(_mainForm.useDevFiles != DevDirCheckBox.Checked)
            {
                _mainForm.useDevFiles = DevDirCheckBox.Checked;
            }
        }

        private void InsertRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_mainForm.insertMapData != InsertRadioButton.Checked)
            {
                _mainForm.insertMapData = InsertRadioButton.Checked;
            }
        }

        private void CreateLocationsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_mainForm.insertMapData != InsertRadioButton.Checked)
            {
                _mainForm.insertMapData = InsertRadioButton.Checked;
            }
        }
    }
}
