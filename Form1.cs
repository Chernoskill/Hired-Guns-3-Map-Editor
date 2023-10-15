using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using PDNIFFPlugin;
using HiredGunsData;
using HGHelperUtilities;
using JWC;//MRU
using Microsoft.Win32;//MRU
using System.Threading;
using System.Diagnostics;

namespace Hired_Guns_3_Map_Editor
{
    public partial class MainForm : Form
    {
        protected MruStripMenu mruMenu;
        public bool menuClearRegistryOnExit = false;
        private int _recentFilesAmount = 10;
        static string mruRegKey = "SOFTWARE\\HiredGunsMapEditor\\HiredGunsMapEditor";
        //static string mruRegKey = "SOFTWARE\\JWC\\MruMenuDemo";
        //public string openFileDiagDir = "F:\\WinUAE2\\Hard Drives\\Games\\HiredGuns\\Dev\\Maps\\Game";
        public string editorDir;
        public string gameDir;
        public string devDir;
        private string _untitledMapName = "Untitled";
        private string _programName = "Hired Guns Map Editor for Windows";
        private string _defaultMapPath;
        private Byte[] defaultBytes = new byte[HGMapData.MAP_BYTES];
        private string _mapFileName;
        public string MapFileName
        {
            get
            {
                return _mapFileName;
            }
            set
            {
                _mapFileName = value;
                Text = $"{_programName} [{Path.GetFileName(_mapFileName)}]";
            }
        }
        private bool _mapChanged;
        public bool MapChanged
        {
            get
            {
                return _mapChanged;
            }
            set
            {
                _mapChanged = value;
                if(_mapChanged && Text.EndsWith("*") == false)
                {
                    Text += "*";
                }
                else if (_mapChanged == false && Text.EndsWith("*"))
                {
                    Text = Text.TrimEnd('*');
                }
            }
        }
        public enum WorkingDirType
        {
            Game = 0,
            Dev = 1
        }
        public bool useDevFiles = false;
        public bool insertMapData = true;
        private Cursor _cursorSelect;
        private Cursor _cursorPaint;
        private Cursor _cursorFill;
        private Cursor _cursorErase;
        private Cursor _cursorSelectTargetIn;
        private Cursor _cursorSelectTargetOut;
        private int windowWidthNoDebug = 1179;
        private int windowWidthDebug = 1452;
        private HorizonsForm _horizonsForm;
        private WorldmapWindow _worldmapWindow;
        private ConversationsForm _conversationsForm;
        private LocationsFileProgressWindow _locationsFileProgressWindow;
        private OptionsForm _optionsForm;
        private HelpForm _helpForm;
        private StateManager _stateManager;
        private bool blockUpdateMapView = false;
        private bool _showExtendedCursorInfo = true;
        public bool ShowExtendedCursorInfo
        {
            get
            {
                return _showExtendedCursorInfo;
            }
            set
            {
                _showExtendedCursorInfo = value;
            }
        }
        private bool _originalCellLightingMethod = true;
        public bool OriginalCellLightingMethod
        {
            get
            {
                return _originalCellLightingMethod;
            }
            set
            {
                _originalCellLightingMethod = value;
                //accessed when saving the map.
            }
        }
        private bool _showWater = true;
        public bool ShowWater 
        { 
            get { return _showWater; } 
            set 
            { 
                _showWater = value; 
                if(ShowWaterToggleButton.Checked != _showWater)
                {
                    ShowWaterToggleButton.Checked = _showWater;
                }
                if(blockUpdateMapView == false)
                {
                    UpdateMapView();
                }
            } 
        }
        private bool _showDoor = true;
        public bool ShowDoor 
        { 
            get { return _showDoor; } 
            set 
            { 
                _showDoor = value;
                if (blockUpdateMapView == false)
                {
                    UpdateMapView();
                }
            } 
        }
        private bool _showLift = true;
        public bool ShowLift 
        { 
            get { return _showLift; } 
            set 
            { 
                _showLift = value;
                if (blockUpdateMapView == false)
                {
                    UpdateMapView();
                }
            } 
        }
        private bool _showPushable = true;
        public bool ShowPushable 
        { 
            get { return _showPushable; } 
            set 
            { 
                _showPushable = value;
                if (blockUpdateMapView == false)
                {
                    UpdateMapView();
                }
            } 
        }
        private CellClipboard _cellClipboard = new CellClipboard();
        private bool _pasting;
        public bool blockHandler;
        public bool blockHandlerKeep;
        private int listViewStringLength = 64;
        private int _mapTileSize = 16;
        private int _mapBlocksSize = 128;
        private int _mapTileScale = 2;
        private int MapTileScale
        {
            get
            {
                return _mapTileScale;
            }
            set
            {
                _mapTileScale = HelperUtil.Clamp<int>(value, 1, 4);
                //MapViewPanel.Size = new Size(HGMapData.MAP_WIDTH * _mapTileScale * _mapTileSize, HGMapData.MAP_HEIGHT * _mapTileScale * _mapTileSize);
                //MapViewPanel.MaximumSize = MapViewPanel.Size;
                /*
                int padding = 6;
                int posX = Math.Max(MapViewPanel.Location.X + MapViewPanel.Width, ShowSkyToggleButton.Location.X + ShowSkyToggleButton.Width) + padding;
                ZLevelGroupBox.Location = new Point(posX, ZLevelGroupBox.Location.Y);
                SetCellPropertyDropdownList.Location = new Point(ZLevelGroupBox.Location.X + ZLevelGroupBox.Width + padding, SetCellPropertyDropdownList.Location.Y);
                EditorTabControl.Location = new Point(posX, EditorTabControl.Location.Y);
                DebugToggleButton.Location = new Point(EditorTabControl.Location.X + EditorTabControl.Width - DebugToggleButton.Width, DebugToggleButton.Location.Y);
                */
                UpdateZoomedMapView();
                UpdateMapViewCursor();
                
            }
        }

        private int MapTileSizeScaled
        {
            get
            {
                return _mapTileSize * _mapTileScale;
            }
        }
        private int MapBlocksWidth
        {
            get
            {
                return _mapBlocksSize / _mapTileSize;
            }
        }
        private int _mapViewScrollV = 0;
        private int MapViewScrollV
        {
            get
            {
                return _mapViewScrollV;
            }
            set
            {
                if(value > 0)
                {
                    PanelExtension.ScrollDown(MapViewPanel, value);
                }
                else if (value < 0)
                {
                    PanelExtension.ScrollUp(MapViewPanel, value);
                }
            }
        }
        private HGGameData _hgGameData;
        private HGMapData _mapData = new HGMapData();
        private string[] _gameTypeNames = new string[] { "0: Normal", "1: Action", "2: Start", "3: End" };
        private readonly string[] _skyNames = new string[] { "0: Black (no sky)", "1: Black to torquiose", "2: Dark blue to violet", "3: Brown to black", "4: Black to blue", "5: Pale green to white" };
        private readonly string[] _atmosNames = new string[] { "0: Crickets", "1: Wind", "2: Heartbeat", "3: Rain", "4: Drums", "5: Electric hum", "6: Aliens echo", "7: Industrial breathing" };
        private readonly string[] _musicNames = new string[] { "0: ???", "1: ???", "2: ???", "3: ???", "4: ???", "5: ???", "6: ???" };
        private readonly string[] _styleNames = new string[] { "1: Brown Stone", "2: Limestone", "3: Hi-tec", "4: Aliens bio-growth", "5: Aliens bio-growth" };

        private HGCharacter[] _monsters = HGCharacter.CreateMonsters();
        Dictionary<string, Bitmap> monsterPortraits = new Dictionary<string, Bitmap>();
        private readonly string[] _blockTypes = new string[] { "None", "Stone", "Push", "Fitness Boost", "Exit", "Tree", "Barrier", "Teleport", "Hydraulic", "Stairs North", "Stairs East", "Stairs South", "Stairs West", "Door NS", "Door EW",""};
        private readonly string[] _floorTypes = new string[] { "None", "Grass", "Button", "Lift", "Puddle", "" };
        private readonly string[] _panelTypes = new string[] { "None", "Image", "Button In", "Button Out","" };
        private readonly string[] _waterTypes = new string[] { "None", "0", "1", "2", "3","" };
        private readonly string[] _auxTypes = new string[] { "None", "Monster", "Egg open", "Container 1", "Container 2", "Container 3", "Container 4", "Container 5", "Stairs N", "Stairs E", "Stairs S", "Stairs W", "Frame NS", "Frame EW", "Dead monster 1", "Dead monster 2", "" };
        private readonly HGItem[] _items = HGItem.CreateItems();
        private readonly string[] buttonActions = new string[]
        {
            "None","Floor on","Floor off","Toggle floor","Block on","Block off","Toggle block","Hatch egg","Unused","lift Up","lift Down","Stop lift","Toggle lift","Unused","Open door","Close door","Toggle door","Lock door","Unlock door"
        };
        private Bitmap _mapLevelImage;
        private Coordinate3 _coordsCursor = new Coordinate3(0,0,0);
        private Coordinate3 _coordsCursorLast = new Coordinate3(-1, -1, -1);
        private List<Coordinate3> _coordsSelected = new List<Coordinate3> { new Coordinate3(0, 0, 0) };
        private Point _mapViewMousePos = new Point(0,0);
        private bool _cursorInsideMapView = false;
        private Coordinate3 _coordsCursorDragStart = new Coordinate3(0,0,0);
        private bool _mouse0Down = false;
        private bool _mouse1Down = false;
        private Color highlightColor = Color.Gold;
        private MonsterButton[] _monsterButtons;
        private int _monsterSelected;
        private int MonsterSelected
        {
            get
            {
                return _monsterSelected;
            }
            set
            {
                int monsterSelectedLast = value;
                _monsterSelected = value;
                UpdateMonsterInfo();
                if (_monsterSelected != monsterSelectedLast)
                {
                    MonsterLevelDropDownList.SelectedIndex = HelperUtil.Clamp<int>(MonsterLevelDropDownList.SelectedIndex, 0,_monsters[_monsterSelected].levels.Length - 1);
                }
                else
                {
                    MonsterLevelDropDownList.SelectedIndex = 0;
                }
                if (_monsterButtons != null && _monsterSelected < _monsterButtons.Length)
                {
                    MonsterLoadButton.Text = _monsterButtons[_monsterSelected].MonsterLoaded ? "Unload" : "Load";
                }
                for(int i = 0; i < _monsterButtons.Length;i++)
                {
                    _monsterButtons[i].Selected = (i == _monsterSelected);
                }
            }
        }
        private Color _monsterButtonColorUnloaded = Color.FromArgb(255, 63, 63, 63);
        private Color _monsterButtonColorLoaded = Color.FromArgb(255, 179, 0, 0);
        private PanelButton[] _panelButtons;
        private Panel[] _panelPanels;
        private int _panelSelected;
        private int PanelSelected
        {
            get
            {
                return _panelSelected;
            }
            set
            {
                _panelSelected = value;
                for (int i = 0; i < _panelButtons.Length; i++)
                {
                    _panelButtons[i].Selected = (i == _panelSelected);
                }
            }
        }
        private Bitmap[] _playerStartImgArray;
        private Bitmap _hydraulicImg;
        private Bitmap _hydraulicNoShadowImg;
        private Bitmap _liftImg;
        private Bitmap _liftUpImg;
        private Bitmap _liftDownImg;
        private Bitmap _liftMinImg;
        private Bitmap _liftMaxImg;
        private Bitmap _auxiliaryImg;
        private Bitmap[] _monsterImgArray;
        private Bitmap _deadMonsterImg;
        private Bitmap _itemImg;
        private Bitmap _itemWeaponImg;
        private Bitmap _itemAmmoImg;
        private Bitmap _itemConsumableImg;
        private Bitmap _itemKeyImg;
        private Bitmap _itemPsiImg;
        private Bitmap _itemUnusableImg;
        private Bitmap _panelImg;
        private Bitmap _buttonInImg;
        private Bitmap _buttonOutImg;
        private Bitmap _floorTriggerImg;
        private Bitmap _lightImg;
        private Bitmap _puddleImg;
        private Bitmap _convImg;
        private Bitmap _monsterShieldedImg;
        private Bitmap _doorLockedImg;
        private Bitmap _doorKeyImg;
        private Bitmap[] _waterLevelImgArray;
        private Bitmap[] _waterLevelTidalImgArray;
        private Bitmap _waterTidalImg;
        private Bitmap _buttonTargetPushedImg;
        private Bitmap _buttonTargetReleasedImg;
        private Bitmap _buttonTargetHigherImg;
        private Bitmap _buttonTargetLowerImg;
        private Bitmap _exitImg;
        private Bitmap _teleportImg;
        private Bitmap _fitnessboostImg;
        private Bitmap _emptyCellImg;
        private Bitmap _invisibleImg;
        private Bitmap _opaqueImg;
        private Bitmap _pushableImg;
        private Bitmap _stairsImg;
        private Bitmap _skynImg;
        private Bitmap _skyeImg;
        private Bitmap _skysImg;
        private Bitmap _skywImg;
        private Bitmap[] _skyImgArray;
        private Bitmap[] _skyImgArrayAGA;
        private int _zLevel = 11;
        public int ZLevel
        {
            get
            {
                return _zLevel;
            }
            set
            {
                _zLevel = HelperUtil.Clamp<int>(value, 0, HGMapData.MAP_LEVELS - 1);
                if(ZLevelDropdownList.SelectedIndex != ZLevelConvertToIndex())
                {
                    ZLevelDropdownList.SelectedIndex = ZLevelConvertToIndex();
                }
                UpdateMapView();
                UpdateMapViewCursor();
            }
        }
        private enum SetCellPropertyType
        {
            All = 0,
            Block = 1,
            Floor = 2,
            Panel = 3,
            Water = 4,
            Auxiliary = 5,
            Egg = 6,
            Sky = 7,
            Item = 8
        }
        public MainForm()
        {
            InitializeComponent();
            InitializeControls();
            
            editorDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if(Directory.Exists(editorDir) == false)
            {
                MessageBox.Show($"There was a problem getting the editor's directory!\r\nClosing application.\r\n{editorDir}");
                Close();
                return;
            }
            _defaultMapPath = $"{editorDir}\\misc\\default.map";
            if(File.Exists(_defaultMapPath) == false)
            {
                MessageBox.Show($"[MainForm] Couldn't find {_defaultMapPath}!");
            }
            else
            {
                defaultBytes = File.ReadAllBytes(_defaultMapPath);
            }
            _stateManager = new StateManager(this);

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(mruRegKey);
            if (regKey != null)
            {
                //menuClearRegistryOnExit = (int)regKey.GetValue("delSubkey", 1) != 0;//handled via config file loading
                regKey.Close();
            }
            mruMenu = new MruStripMenuInline(FileToolStripMenuItem, RecentFilesToolStripMenuItem, new MruStripMenu.ClickedHandler(OnMruFile), mruRegKey + "\\MRU", _recentFilesAmount);
            CreateCursors();
            LoadImages();
            _hgGameData = new HGGameData();
            LoadStyles();
            _mapLevelImage = new Bitmap(HGMapData.MAP_WIDTH * _mapTileSize, HGMapData.MAP_HEIGHT * _mapTileSize);

            CreateMonsterButtons();
            MonsterSelected = 0;
            CreatePanelButtons();
           this.MapViewPanel.MouseWheel += new MouseEventHandler(this.MapViewPanel_MouseWheel);
           this.MapViewPictureBox.MouseWheel += new MouseEventHandler(this.MapViewPictureBox_MouseWheel);

            LoadDefaultMap();
            ZLevel = 11;
            SetCoordsSelected(_coordsCursor);
            SelectToggleButton.Checked = true;

            LoadConfig();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(mruRegKey);
            if (regKey != null)
            {
                regKey.SetValue("delSubkey", menuClearRegistryOnExit ? 1 : 0);
                regKey.Close();
            }

            if (menuClearRegistryOnExit)
            {
                regKey = Registry.CurrentUser;
                regKey.DeleteSubKey(mruRegKey + "\\MRU", false);
                
            }
            if(MapChanged)
            {
                DialogResult res = MessageBox.Show("Save changes to the map before closing?", _programName,MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.OK)
                {
                    e.Cancel = true;
                    SaveMap();
                }
            }
            SaveConfig();
            base.OnClosing(e);
        }
        private void SaveConfig()
        {
            string path = string.Format($"{editorDir}\\config.ini");
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine
                (
                    string.Format
                    (
                        $"gamedir={gameDir}\n" +
                        $"devdir={devDir}\n" +
                        $"usedevfiles={useDevFiles.ToString()}\n" +
                        $"insertmapdata={insertMapData.ToString()}\n" +
                        $"showextendedcursorinfo={ShowExtendedCursorInfo.ToString()}\n" +
                        $"originalcelllightingmethod={OriginalCellLightingMethod.ToString()}\n" +
                        $"autostairs={AutoStairsToggleButton.Checked.ToString()}\n" +
                        $"showwater={ShowWater.ToString()}\n" +
                        $"showdoor={ShowWater.ToString()}\n" +
                        $"showlift={ShowWater.ToString()}\n" +
                        $"maptilescale={MapTileScale}\n" +
                        $"width={Width}\n" +
                        $"height={Height}\n" +
                        $"posx={Location.X}\n" +
                        $"posy={Location.Y}\n" +
                        $"showdebuginfo={DebugToggleButton.Checked.ToString()}\n" +
                        $"clearregistryonexit={menuClearRegistryOnExit.ToString()}"
                    )
                );
            }
        }

        private void LoadConfig()
        {
            string dir = string.Format($"{Path.GetDirectoryName(Application.ExecutablePath)}");
            string path = string.Format($"{dir}\\config.ini");
            if (File.Exists(path) == false)
            {
                AutoStairsToggleButton.Checked = true;
                ShowWaterToggleButton.Checked = true;
                ShowExtendedCursorInfo = true;
                return;
            }
            string[] lines = File.ReadAllLines(path);

            if (lines.Length == 0)
            {
                return;
            }
            blockUpdateMapView = true;
            foreach (string line in lines)
            {
                if (line.ToLower().StartsWith("gamedir="))
                {
                    gameDir = line.Substring(line.LastIndexOf('=') + 1);
                }
                else if (line.ToLower().StartsWith("devdir="))
                {
                    devDir = line.Substring(line.LastIndexOf('=') + 1);
                }
                else if (line.ToLower().StartsWith("usedevfiles="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    useDevFiles = b;
                }
                else if (line.ToLower().StartsWith("insertmapdata="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    insertMapData = b;
                }
                else if (line.ToLower().StartsWith("showextendedcursorinfo="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    ShowExtendedCursorInfo = b;
                }
                else if (line.ToLower().StartsWith("originalcelllightingmethod="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    OriginalCellLightingMethod = b;
                }
                else if (line.ToLower().StartsWith("autostairs="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    AutoStairsToggleButton.Checked = b;
                }
                else if (line.ToLower().StartsWith("showwater="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    ShowWater = b;
                }
                else if (line.ToLower().StartsWith("showdoor="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    ShowDoor = b;
                }
                else if (line.ToLower().StartsWith("showlift="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    ShowLift = b;
                }
                else if (line.ToLower().StartsWith("maptilescale="))
                {
                    int i = MapTileScale;
                    int.TryParse(line.Substring(line.LastIndexOf('=') + 1), out i);
                    if(i != MapTileScale)
                    {
                        MapTileScale = i;
                    }
                }
                else if (line.ToLower().StartsWith("width="))
                {
                    int i = Width;
                    int.TryParse(line.Substring(line.LastIndexOf('=') + 1), out i);
                    Width = i;
                }
                else if (line.ToLower().StartsWith("height="))
                {
                    int i = Height;
                    int.TryParse(line.Substring(line.LastIndexOf('=') + 1), out i);
                    Height = i;
                }
                else if (line.ToLower().StartsWith("posx="))
                {
                    int i = Location.X;
                    int.TryParse(line.Substring(line.LastIndexOf('=') + 1), out i);
                    Location = new Point(i, Location.Y);
                }
                else if (line.ToLower().StartsWith("posy="))
                {
                    int i = Location.Y;
                    int.TryParse(line.Substring(line.LastIndexOf('=') + 1), out i);
                    Location = new Point(Location.X, i);
                }
                else if (line.ToLower().StartsWith("showdebuginfo="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    DebugToggleButton.Checked = b;
                }
                else if (line.ToLower().StartsWith("clearregistryonexit="))
                {
                    bool b = true;
                    Boolean.TryParse(line.Substring(line.LastIndexOf('=') + 1), out b);
                    menuClearRegistryOnExit = b;
                }
            }
            blockUpdateMapView = false;
        }
        public void SetBlockHandler(bool state = true)
        {
            if (state == false && blockHandlerKeep == true)
            {
                return;
            }
            blockHandler = state;
        }
        public void SetBlockHandlerKeep(bool state = true)
        {
            blockHandlerKeep = state;
            blockHandler = state;
        }
        public void AddConsoleMessage(string msg, bool warning = false)
        {
            if(ConsoleTextBox.Text.Length > 0)
            {
                ConsoleTextBox.AppendText("\r\n");
            }
            ConsoleTextBox.AppendText($"{DateTime.Now.ToShortTimeString()} {msg}", warning ? Color.Red : ConsoleTextBox.ForeColor);
        }
        private void ClearConsoleMessages()
        {
            ConsoleTextBox.Text = "";
        }
        private void InitializeControls()
        {
            SetBlockHandlerKeep();

            for (int i = HGMapData.MAP_LEVELS - 1; i >= 0; i--)
            {
                ZLevelDropdownList.Items.Add(i.ToString().PadLeft(2));
            }

            BlockTypeDropDownList.Items.AddRange(_blockTypes);
            BlockTypeDropDownList.SelectedIndex = 0;
            FloorTypeDropDownList.Items.AddRange(_floorTypes);
            FloorTypeDropDownList.SelectedIndex = 0;
            PanelTypeDropDownList.Items.AddRange(_panelTypes);
            PanelTypeDropDownList.SelectedIndex = 0;
            WaterDropDownList.Items.AddRange(_waterTypes);
            WaterDropDownList.SelectedIndex = 0;
            AuxiliaryDropDownList.Items.AddRange(_auxTypes);
            AuxiliaryDropDownList.SelectedIndex = 0;
            EggDelayDropDownList.SelectedIndex = 0;
            for (int i = 0; i < _items.Length; i++)
            {
                ItemDropDownList.Items.Add(_items[i].name);
            }
            ItemDropDownList.Items.Add("");
            ItemDropDownList.SelectedIndex = 0;
            ItemCategoryFilterDropdownList.SelectedIndex = 0;

            GameTypeDropDownList.Items.AddRange(_gameTypeNames);
            GameTypeDropDownList.SelectedIndex = 0;
            MusicDropDownList.Items.AddRange(_musicNames);
            MusicDropDownList.SelectedIndex = 0;
            AtmosDropDownList.Items.AddRange(_atmosNames);
            AtmosDropDownList.SelectedIndex = 0;
            SkyDropDownList.Items.AddRange(_skyNames);
            SkyDropDownList.SelectedIndex = 0;

            ButtonActionPushedDropDownList.Items.AddRange(buttonActions);
            ButtonActionPushedDropDownList.SelectedIndex = 0;
            ButtonActionReleasedDropDownList.Items.AddRange(buttonActions);
            ButtonActionReleasedDropDownList.SelectedIndex = 0;

            for (int i = 0; i < HGMapData.MAP_LIFTS; i++)
            {
                string s = $"{i.ToString().PadLeft(2, '0')}".PadRight(listViewStringLength);
                LiftsListBox.Items.Add(s);
            }
            LiftsListBox.SelectedIndex = 0;
            for (int i = 0; i < HGMapData.MAP_DOORS; i++)
            {
                string s = $"{i.ToString().PadLeft(2, '0')}".PadRight(listViewStringLength);
                DoorsListBox.Items.Add(s);
            }
            DoorsListBox.SelectedIndex = 0;
            for (int i = 0; i < HGMapData.MAP_BUTTONS; i++)
            {
                string s = $"{i.ToString().PadLeft(2, '0')}".PadRight(listViewStringLength);
                ButtonsListBox.Items.Add(s);
            }
            ButtonsListBox.SelectedIndex = 0;

            SetCellPropertyDropdownList.SelectedIndex = 0;

            SetToolTips();

            SetBlockHandlerKeep(false);

            //Static AddContextMenu function is located in Program.cs
            WorldmapDescriptionTextBox.AddContextMenu();
            DetailedDescriptionTextBox.AddContextMenu();
            IntroMessageTextBox.AddContextMenu();
            ConsoleTextBox.AddContextMenu();
        }
        private void SetToolTips()
        {
            EditorToolTip.AutoPopDelay = 15000;
            EditorToolTip.SetToolTip(ActiveModeToggleButton, "Active Mode\r\nWhen active, all changes to a cell control take effect immediately.\r\nWhen inactive, cells are only affected when changing them in the map view window (for example, by painting or filling in a selected area)\r\nShortcut: A");
            EditorToolTip.SetToolTip(ShowWaterToggleButton, "Show Water\r\nShows or hides water level cell icons.");
            EditorToolTip.SetToolTip(AutoStairsToggleButton, "Auto Stairs Mode\r\nAutomatically clears cells and sets stair auxiliaries when creating stair blocks.");
            EditorToolTip.SetToolTip(ShowSkyToggleButton, "Show Sky\r\nShows or hides sky rendering settings for all cells.");
            EditorToolTip.SetToolTip(SelectToggleButton, "Select Mode\r\nDrag a box over several cells to select an area.\r\nHold Shift to do this while in Paint mode.\r\nShortcut: M");
            EditorToolTip.SetToolTip(PaintToggleButton, "Paint Mode\r\nLeft mouse button: Paint cells.\r\nMiddle mouse button: Pick cell properties at cursor position.\r\nRight mouse button: Clear cells.\r\nHold Shift to select an area.\r\nShortcut: B");
            EditorToolTip.SetToolTip(FillToggleButton, "Fill Mode\r\nClick inside the selected area to fill it.\r\nShortcut: G");
            EditorToolTip.SetToolTip(EraseToggleButton, "Erase Mode\r\nClear cells.\r\nCan also be done with the right mouse button in paint mode.\r\nShortcut: E");
            EditorToolTip.SetToolTip(SelectTargetInToggleButton, "Select Pushed / Teleport Target Mode\r\nSelect the target position for a button's pushed action or teleport block.");
            EditorToolTip.SetToolTip(SelectTargetOutToggleButton, "Select Released / Teleport Target Mode\r\nSelect the target position for a button's released or teleport block.");
            EditorToolTip.SetToolTip(NewMapButton, "New Map\r\nShortcut: CTRL+N");
            EditorToolTip.SetToolTip(OpenFileButton, "Load Map\r\nShortcut: CTRL+O");
            EditorToolTip.SetToolTip(SaveMapButton, "Save Map\r\nShortcut: CTRL+S");
            EditorToolTip.SetToolTip(SaveAsButton, "Save Map As...\r\nShortcut: CTRL+SHIFT+S");
            EditorToolTip.SetToolTip(CutButton, "Cut\r\nShortcut: CTRL+X");
            EditorToolTip.SetToolTip(CopyButton, "Copy\r\nShortcut: CTRL+C");
            EditorToolTip.SetToolTip(PasteButton, "Paste\r\nShortcut: CTRL+V");
        }
        private void CreateCursors()
        {
            _cursorSelect = new Cursor(GetType(), "Cursor_select.cur");
            _cursorPaint = new Cursor(GetType(), "Cursor_paint.cur");
            _cursorFill = new Cursor(GetType(), "Cursor_fill.cur");
            _cursorErase = new Cursor(GetType(), "Cursor_erase.cur");
            _cursorSelectTargetIn = new Cursor(GetType(), "Cursor_selecttargetin.cur");
            _cursorSelectTargetOut = new Cursor(GetType(), "Cursor_selecttargetout.cur");
        }
        private void LoadImages()
        {
            string tilesPath = $"{editorDir}\\graphics\\tiles";
            if (Directory.Exists(tilesPath) == false)
            {
                MessageBox.Show($"{tilesPath} does not exist!");
                return;
            }
            _playerStartImgArray = new Bitmap[4];
            for (int i = 0; i < 4; i++)
            {
                if (File.Exists($"{tilesPath}\\playerstart{i + 1}_tile.png"))
                {
                    using (FileStream fs = File.OpenRead($"{tilesPath}\\playerstart{i + 1}_tile.png"))
                    {
                        _playerStartImgArray[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
            }
            if(File.Exists($"{tilesPath}\\hydraulic_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\hydraulic_tile.png"))
                {
                    _hydraulicImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\hydraulicnoshadow_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\hydraulicnoshadow_tile.png"))
                {
                    _hydraulicNoShadowImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\aux_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\aux_tile.png"))
                {
                    _auxiliaryImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            
            _monsterImgArray = new Bitmap[4];
            for (int i = 0; i < _monsterImgArray.Length; i++)
            {
                if (File.Exists($"{tilesPath}\\monster{i + 1}_tile.png"))
                {
                    using (FileStream fs = File.OpenRead($"{tilesPath}\\monster{i + 1}_tile.png"))
                    {
                        _monsterImgArray[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
            }
            if (File.Exists($"{tilesPath}\\deadmonster_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\deadmonster_tile.png"))
                {
                    _deadMonsterImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\item_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\item_tile.png"))
                {
                    _itemImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itemweapon_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itemweapon_tile.png"))
                {
                    _itemWeaponImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itemammo_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itemammo_tile.png"))
                {
                    _itemAmmoImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itemconsumable_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itemconsumable_tile.png"))
                {
                    _itemConsumableImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itemkey_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itemkey_tile.png"))
                {
                    _itemKeyImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itempsi_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itempsi_tile.png"))
                {
                    _itemPsiImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\itemunusable_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\itemunusable_tile.png"))
                {
                    _itemUnusableImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\lift_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\lift_tile.png"))
                {
                    _liftImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\liftup_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\liftup_tile.png"))
                {
                    _liftUpImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\liftdown_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\liftdown_tile.png"))
                {
                    _liftDownImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\liftmin_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\liftmin_tile.png"))
                {
                    _liftMinImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\liftMax_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\liftMax_tile.png"))
                {
                    _liftMaxImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\floortrigger_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\floortrigger_tile.png"))
                {
                    _floorTriggerImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\doorlocked_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\doorlocked_tile.png"))
                {
                    _doorLockedImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\doorkey_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\doorkey_tile.png"))
                {
                    _doorKeyImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\panel_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\panel_tile.png"))
                {
                    _panelImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttonin_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttonin_tile.png"))
                {
                    _buttonInImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttonout_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttonout_tile.png"))
                {
                    _buttonOutImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\light_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\light_tile.png"))
                {
                    _lightImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\puddle_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\puddle_tile.png"))
                {
                    _puddleImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\conv_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\conv_tile.png"))
                {
                    _convImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            _waterLevelImgArray = new Bitmap[4];
            _waterLevelTidalImgArray = new Bitmap[4];
            for (int i = 0; i < 4; i++)
            {
                if (File.Exists($"{tilesPath}\\waterlevel{i}_tile.png"))
                {
                    using (FileStream fs = File.OpenRead($"{tilesPath}\\waterlevel{i}_tile.png"))
                    {
                        _waterLevelImgArray[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
                if (File.Exists($"{tilesPath}\\waterlevel{i}tidal_tile.png"))
                {
                    using (FileStream fs = File.OpenRead($"{tilesPath}\\waterlevel{i}tidal_tile.png"))
                    {
                        _waterLevelTidalImgArray[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
            }
            if (File.Exists($"{tilesPath}\\watertidal_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\watertidal_tile.png"))
                {
                    _waterTidalImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttontargetpushed_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttontargetpushed_tile.png"))
                {
                    _buttonTargetPushedImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttontargetreleased_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttontargetreleased_tile.png"))
                {
                    _buttonTargetReleasedImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttontargethigher_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttontargethigher_tile.png"))
                {
                    _buttonTargetHigherImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\buttontargetlower_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\buttontargetlower_tile.png"))
                {
                    _buttonTargetLowerImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\exit_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\exit_tile.png"))
                {
                    _exitImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\teleport_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\teleport_tile.png"))
                {
                    _teleportImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\fitnessboost_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\fitnessboost_tile.png"))
                {
                    _fitnessboostImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\emptycell_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\emptycell_tile.png"))
                {
                    _emptyCellImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\invisible_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\invisible_tile.png"))
                {
                    _invisibleImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\opaque_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\opaque_tile.png"))
                {
                    _opaqueImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\pushable_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\pushable_tile.png"))
                {
                    _pushableImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\stairs_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\stairs_tile.png"))
                {
                    _stairsImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\skyn_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\skyn_tile.png"))
                {
                    _skynImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\skye_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\skye_tile.png"))
                {
                    _skyeImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\skys_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\skys_tile.png"))
                {
                    _skysImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            if (File.Exists($"{tilesPath}\\skyw_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\skyw_tile.png"))
                {
                    _skywImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            string monsterPath = $"{editorDir}\\graphics\\monsters";
            string[] monsterPortraitPaths = Directory.GetFiles(monsterPath, "*_portrait.png",SearchOption.AllDirectories);
            for(int i = 0; i < monsterPortraitPaths.Length; i++)
            {
                string pathCur = monsterPortraitPaths[i];
                string portraitCur = Path.GetFileNameWithoutExtension(pathCur);
                if (File.Exists(pathCur))
                {
                    using (FileStream fs = File.OpenRead(pathCur))
                    {
                        monsterPortraits.Add(portraitCur, Bitmap.FromStream(File.OpenRead(pathCur)) as Bitmap);
                    }
                }
            }
            if (File.Exists($"{tilesPath}\\monstershielded_tile.png"))
            {
                using (FileStream fs = File.OpenRead($"{tilesPath}\\monstershielded_tile.png"))
                {
                    _monsterShieldedImg = Bitmap.FromStream(fs) as Bitmap;
                }
            }
            _skyImgArray = new Bitmap[6];
            _skyImgArrayAGA = new Bitmap[6];
            string skiesPath = $"{editorDir}\\graphics\\sky";
            for (int i = 0; i < 6; i++)
            {
                if (File.Exists($"{skiesPath}\\sky{i}.png"))
                {
                    using (FileStream fs = File.OpenRead($"{skiesPath}\\sky{i}.png"))
                    {
                        _skyImgArray[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
                if (File.Exists($"{skiesPath}\\sky{i}_aga.png"))
                {
                    using (FileStream fs = File.OpenRead($"{skiesPath}\\sky{i}_aga.png"))
                    {
                        _skyImgArrayAGA[i] = Bitmap.FromStream(fs) as Bitmap;
                    }
                }
            }
        }
        private void LoadStyles()
        {
            string path = $"{editorDir}\\graphics\\styles";
            if(Directory.Exists(path) == false)
            {
                MessageBox.Show($"{path} does not exist!");
                return;
            }
            _hgGameData.styles = new HGStyle[5];
            for (int i = 0; i < 5; i++)
            {
                string stylePath = $"{path}\\style{i+1}";
                string mapBlocksPath = $"{stylePath}\\MapBlocks.png";
                if(File.Exists(mapBlocksPath) == false)
                {
                    MessageBox.Show($"{mapBlocksPath} does not exist!");
                    continue;
                }
                HGStyle style = new HGStyle();
                style.name = _styleNames[i];
                StyleDropDownList.Items.Add(style.name);
                using (FileStream fs = File.OpenRead(mapBlocksPath))
                {
                    Bitmap bm = Bitmap.FromStream(fs) as Bitmap;
                    style.mapTilesImage = HelperUtil.CropImage(bm, new Rectangle(0, 0, _mapBlocksSize, _mapBlocksSize), new Rectangle(0,0, _mapBlocksSize, _mapBlocksSize));
                    style.mapTiles = new Bitmap[(int)Math.Pow(MapBlocksWidth, 2)];
                    UpdateMapTiles(style);
                }
                _hgGameData.styles[i] = style;
            }
            if(_hgGameData.styles.Length > 0)
            {
                SetBlockHandler();
                StyleDropDownList.SelectedIndex = 0;
            }
        }
        private void CreateMonsterButtons()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            int columns = 4;
            _monsterButtons = new MonsterButton[_monsters.Length];
            for (int i = 0; i < _monsters.Length; i++)
            {
                MonsterButton btn = new MonsterButton();
                btn.Parent = MonsterTypesGoupBox;
                MonsterTypesGoupBox.Controls.Add(btn);
                btn.Location = new Point(6 + (i % columns) * 64, 15 + (i / columns) * 64);
                btn.Width = 64;
                btn.Height = 64;
                Bitmap bm = null;
                monsterPortraits.TryGetValue(_monsters[i].portraitName, out bm);
                btn.BackgroundImage = bm;
                btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.BackColor = _monsterButtonColorUnloaded;
                btn.Click += new EventHandler(MonsterButton_Click);
                btn.InterpolationMode = InterpolationMode.NearestNeighbor;
                toolTip.InitialDelay = 0;
                toolTip.SetToolTip(btn, $"{_monsters[i].name}");
                toolTip.InitialDelay = 500;
                btn.monsterIndex = i;
                _monsterButtons[i] = btn;
            }
        }
        private void CreatePanelButtons()
        {
            int columns = 6;
            _panelButtons = new PanelButton[HGMapData.MAP_PANELS];
            _panelPanels = new Panel[HGMapData.MAP_PANELS];
            for (int i = 0; i < _panelButtons.Length; i++)
            {
                /*
                Panel p = new Panel();
                p.Parent = PanelsGroupBox;
                PanelsGroupBox.Controls.Add(p);
                p.BorderStyle = BorderStyle.FixedSingle;
                p.Width = 41;
                p.Height = 44;
                p.Location = new Point(6 + (i % columns) * (p.Width + 4), 15 + (i / columns) * (p.Height + 4));
                _panelPanels[i] = p;

                PanelButton btn = new PanelButton();
                btn.FlatStyle = FlatStyle.Flat;
                btn.Parent = p;
                p.Controls.Add(btn);
                btn.Parent = p;
                //btn.Location = new Point(6 + (i % columns) * 42, 15 + (i / columns) * 40);
                btn.Location = new Point(-11,-4);
                btn.Width = 56;
                btn.Height = 48;
                //btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.BackgroundImageLayout = ImageLayout.None;
                btn.BackColor = _monsterButtonColorUnloaded;
                btn.Click += new EventHandler(PanelButton_Click);
                btn.InterpolationMode = InterpolationMode.NearestNeighbor;
                btn.panelIndex = i;
                _panelButtons[i] = btn;
                */
                Panel p = new Panel();
                p.Parent = PanelsGroupBox;
                PanelsGroupBox.Controls.Add(p);
                p.BorderStyle = BorderStyle.FixedSingle;
                p.BackColor = Color.Black;
                p.Width = 49;
                p.Height = 50;
                p.Location = new Point(6 + (i % columns) * (p.Width + 4), 15 + (i / columns) * (p.Height + 4));
                _panelPanels[i] = p;

                PanelButton btn = new PanelButton();
                btn.FlatStyle = FlatStyle.Flat;
                btn.Parent = p;
                p.Controls.Add(btn);
                btn.Parent = p;
                //btn.Location = new Point(6 + (i % columns) * 42, 15 + (i / columns) * 40);
                btn.Location = new Point(-5,-5);
                btn.Width = 57;
                btn.Height = 58;
                //btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.BackgroundImageLayout = ImageLayout.Center;
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.Transparent;
                btn.Click += new EventHandler(PanelButton_Click);
                btn.InterpolationMode = InterpolationMode.NearestNeighbor;
                btn.panelIndex = i;
                _panelButtons[i] = btn;
                /*
                PanelButton btn = new PanelButton();
                btn.FlatStyle = FlatStyle.Flat;
                btn.Parent = PanelsGroupBox;
                PanelsGroupBox.Controls.Add(btn);
                btn.Width = 44;
                btn.Height = 45;
                btn.Location = new Point(6 + (i % columns) * (btn.Width + 4), 15 + (i / columns) * (btn.Height + 4));
                //btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.BackgroundImageLayout = ImageLayout.Center;
                btn.BackColor = _monsterButtonColorUnloaded;
                btn.Click += new EventHandler(PanelButton_Click);
                btn.InterpolationMode = InterpolationMode.NearestNeighbor;
                btn.panelIndex = i;
                _panelButtons[i] = btn;
                */
            }
        }
        private void UpdatePanels()
        {
            if(_mapData == null)
            {
                return;
            }
            for (int i = 0; i < _panelButtons.Length; i++)
            {
                if(i >= _mapData.panels.Length)
                {
                    break;
                }
                if(_panelButtons[i].Image == null)
                {
                    _panelButtons[i].Image = new Bitmap(39,40);
                }
                Bitmap bm = (Bitmap)_panelButtons[i].Image;
                HelperUtil.CopyRegionIntoImage(_mapData.panels[i], new Rectangle(6, 0, 39, 40), ref bm, new Rectangle(0, 0, 39, 40));

                _panelButtons[i].Image = bm;
                _panelButtons[i].Invalidate();
                //_panelButtons[i].Image = _mapData.panels[i];
            }
        }
        private void UpdateMapTiles(HGStyle style)
        {
            if(style == null || style.mapTiles == null)
            {
                return;
            }
            for (int j = 0; j < style.mapTiles.Length; j++)
            {
                style.mapTiles[j] = HelperUtil.CropImage(style.mapTilesImage, new Rectangle(_mapTileSize * (j % MapBlocksWidth), _mapTileSize * (j / MapBlocksWidth), _mapTileSize, _mapTileSize), new Rectangle(0, 0, _mapTileSize, _mapTileSize));
            }
        }
        public HGMapData GetMapData()
        {
            return _mapData;
        }
        public Bitmap GetSkyImg(int skyNum, bool aga = false)
        {
            if(skyNum >= _skyImgArray.Length)
            {
                return null;
            }
            return aga ? _skyImgArrayAGA[skyNum] : _skyImgArray[skyNum];
        }
        private void AddStep()
        {
            if(_stateManager == null)
            {
                return;
            }
            _stateManager.AddStep();
            MapChanged = true;
        }
        public void UpdateMapView()
        {
            UpdateMapView(0,0,HGMapData.MAP_WIDTH - 1, HGMapData.MAP_HEIGHT - 1);
        }
        public void UpdateMapView(Coordinate3 coords)
        {
            UpdateMapView(coords.x, coords.y, coords.x, coords.y);
        }
        public void UpdateMapView(Coordinate3 coordsStart, Coordinate3 coordsEnd)
        {
            UpdateMapView(coordsStart.x, coordsStart.y, coordsEnd.x, coordsEnd.y);
        }
        public void UpdateMapView(int minX, int minY, int maxX, int maxY)
        {
            if(_mapData == null)
            {
                return;
            }
            minX = HelperUtil.Clamp<int>(minX - 1,0, HGMapData.MAP_WIDTH - 1);
            minY = HelperUtil.Clamp<int>(minY - 1, 0, HGMapData.MAP_HEIGHT - 1);
            maxX = HelperUtil.Clamp<int>(maxX + 1, 0, HGMapData.MAP_WIDTH - 1);
            maxY = HelperUtil.Clamp<int>(maxY + 1, 0, HGMapData.MAP_HEIGHT - 1);
            int z = ZLevel;
            for (int y = 0; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    HGCell cellCur = _mapData.GetCell(x, y, z);
                    HGCell cellBelow = z > 0 ? _mapData.GetCell(x, y, z - 1) : null;
                    if (cellCur == null)
                    {
                        continue;
                    }
                    if(cellCur.forceQuestionMark)
                    {
                        PasteMapTile(GetMapTile(31), x, y);//question mark
                    }
                    else if(cellCur.blockHere == false && z > 0 && cellBelow.blockType == HGCellBlockType.Push)
                    {
                        PasteMapTile(GetMapTile(63), x, y);
                    }
                    else if(cellCur.blockHere && cellCur.blockType == HGCellBlockType.Push)
                    {
                        PasteMapTile(GetMapTile(15), x, y);
                    }
                    else
                    {
                        PasteMapTile(GetMapTile(cellCur.floorTile), x, y);
                    }
                    if(cellCur.blockHere)
                    {
                        if (cellCur.blockType == HGCellBlockType.Hydraulic)
                        {
                            if (z > 0 && (cellBelow.blockHere == false || cellBelow.blockType == HGCellBlockType.Hydraulic))
                            {
                                PasteMapTile(_emptyCellImg, x, y);
                                PasteMapTile(_hydraulicNoShadowImg, x, y);
                            }
                            else
                            {
                                PasteMapTile(_hydraulicImg, x, y);
                            }
                        }
                        else if (cellCur.blockType == HGCellBlockType.Exit)
                        {
                            PasteMapTile(_exitImg, x, y);
                        }
                        else if (cellCur.blockType == HGCellBlockType.Teleport)
                        {
                            PasteMapTile(_teleportImg, x, y);
                        }
                        else if (cellCur.blockType == HGCellBlockType.FitnessBoost)
                        {
                            PasteMapTile(_fitnessboostImg, x, y);
                        }
                    }
                    else if(cellCur.floorHere == false && z > 0 && cellBelow.blockHere == false)
                    {
                        PasteMapTile(_emptyCellImg, x, y);
                    }
                    if (cellCur.floorHere)
                    {
                        if (cellCur.floorType == HGCellFloorType.Button)
                        {
                            PasteMapTile(_floorTriggerImg, x, y);
                        }
                        else if (cellCur.floorType == HGCellFloorType.Lift)
                        {
                            PasteMapTile(_liftImg, x, y);
                        }
                        else if (cellCur.floorType == HGCellFloorType.PuddleLight)
                        {
                            PasteMapTile(cellCur.blockHere ? _lightImg : _puddleImg, x, y);
                        }
                    }
                    if (cellCur.blockHere == false && cellCur.opaque)
                    {
                        PasteMapTile(_opaqueImg, x, y);
                    }
                    if (cellCur.invisible)
                    {
                        PasteMapTile(_invisibleImg, x, y);
                    }
                    if(ShowPushable && cellCur.pushable)
                    {
                        PasteMapTile(_pushableImg, x, y);
                    }
                    if (ShowWater && (cellCur.waterHere || cellCur.tidalWaterFlowing))
                    {
                        PasteMapTile(cellCur.tidalWaterFlowing ? cellCur.waterLevel == 0 ? _waterTidalImg : _waterLevelTidalImgArray[cellCur.waterLevel] : _waterLevelImgArray[cellCur.waterLevel], x, y);
                    }
                    if (cellCur.panelHere)
                    {
                        if (cellCur.panelType == HGCellPanelType.ButtonIn)
                        {
                            PasteMapTile(_buttonInImg, x, y);
                        }
                        else if (cellCur.panelType == HGCellPanelType.ButtonOut)
                        {
                            PasteMapTile(_buttonOutImg, x, y);
                        }
                        else
                        {
                            PasteMapTile(_panelImg, x, y);
                        }
                    }
                    if (cellCur.auxiliaryHere)
                    {
                        if(cellCur.auxiliaryType == HGCellAuxiliaryType.Monster && cellCur.monsterLevel >= 0 && cellCur.monsterLevel < _monsterImgArray.Length)
                        {
                            PasteMapTile(_monsterImgArray[cellCur.monsterLevel], x, y);
                            HGCharacter monster = cellCur.monsterType >= 0 && cellCur.monsterType < _monsters.Length ?  _monsters[cellCur.monsterType] : null;
                            if(monster != null && cellCur.monsterLevel > 0 && cellCur.monsterLevel < monster.levels.Length && monster.levels[cellCur.monsterLevel].shielded)
                            {
                                PasteMapTile(_monsterShieldedImg, x, y);
                            }
                        }
                        else if (cellCur.itemID < _items.Length && (cellCur.auxiliaryType == HGCellAuxiliaryType.Container1 || cellCur.auxiliaryType == HGCellAuxiliaryType.Container2 || cellCur.auxiliaryType == HGCellAuxiliaryType.Container3 || cellCur.auxiliaryType == HGCellAuxiliaryType.Container4 || cellCur.auxiliaryType == HGCellAuxiliaryType.Container5))
                        {
                            HGItemCategory cat = _items[cellCur.itemID].category;
                            PasteMapTile(cat == HGItemCategory.Weapon ? _itemWeaponImg : cat == HGItemCategory.Ammo ? _itemAmmoImg : cat == HGItemCategory.Consumable ? _itemConsumableImg : cat == HGItemCategory.Key ? _itemKeyImg : cat == HGItemCategory.PsiAmp ? _itemPsiImg : cat == HGItemCategory.Unusable ? _itemUnusableImg : _itemImg, x, y);
                        }
                        else if (cellCur.auxiliaryType == HGCellAuxiliaryType.StairsN || cellCur.auxiliaryType == HGCellAuxiliaryType.StairsE || cellCur.auxiliaryType == HGCellAuxiliaryType.StairsS || cellCur.auxiliaryType == HGCellAuxiliaryType.StairsW)
                        {
                            PasteMapTile(_stairsImg, x, y);
                        }
                        else if (cellCur.auxiliaryType == HGCellAuxiliaryType.Dead1 || cellCur.auxiliaryType == HGCellAuxiliaryType.Dead2)
                        {
                            PasteMapTile(_deadMonsterImg, x, y);
                        }
                        else                        
                        {
                            PasteMapTile(_auxiliaryImg, x, y);
                        }
                    }
                    if(ShowDoor)
                    {
                        if (cellCur.blockHere && (cellCur.blockType == HGCellBlockType.DoorNS || cellCur.blockType == HGCellBlockType.DoorEW))
                        {
                            HGDoor door = cellCur.doorIndex >= 0 && cellCur.doorIndex < _mapData.doors.Length ? _mapData.doors[cellCur.doorIndex] : null;
                            if (door != null)
                            {
                                if (door.locked)
                                {
                                    PasteMapTile(_doorLockedImg, x, y);
                                }
                                if (door.keyID > 0)
                                {
                                    PasteMapTile(_doorKeyImg, x, y);
                                }
                            }
                        }
                    }
                    if(ShowLift)
                    {
                        if (cellCur.floorHere && cellCur.floorType == HGCellFloorType.Lift)
                        {
                            HGLift lift = cellCur.liftIndex >= 0 && cellCur.liftIndex < _mapData.lifts.Length ? _mapData.lifts[cellCur.liftIndex] : null;
                            if (lift != null)
                            {
                                if (lift.minZ < ZLevel)
                                {
                                    PasteMapTile(_liftDownImg, x, y);
                                }
                                if (lift.maxZ > ZLevel)
                                {
                                    PasteMapTile(_liftUpImg, x, y);
                                }
                            }
                        }
                    }
                    if (ShowSkyToggleButton.Checked)
                    {
                        if(cellCur.skyRendered[0] == false)
                        {
                            PasteMapTile(_skynImg, x, y);
                        }
                        if (cellCur.skyRendered[1] == false)
                        {
                            PasteMapTile(_skyeImg, x, y);
                        }
                        if (cellCur.skyRendered[2] == false)
                        {
                            PasteMapTile(_skysImg, x, y);
                        }
                        if (cellCur.skyRendered[3] == false)
                        {
                            PasteMapTile(_skywImg, x, y);
                        }
                    }
                }
            }
            if (_mapData.conversations != null && _mapData.conversations.Length > 0)
            {
                for (int i = 0; i < _mapData.conversations.Length; i++)
                {
                    if (_mapData.conversations[i] == null || String.IsNullOrEmpty(_mapData.conversations[i].text) || _mapData.conversations[i].text.Length == 0)
                    {
                        continue;
                    }
                    Coordinate3 c = _mapData.conversations[i].coords;
                    if (c.z == ZLevel)
                    {
                        PasteMapTile(_convImg, c.x, c.y);
                    }
                }
            }

            Coordinate3[] playerStarts = new Coordinate3[] { _mapData.playerStart1, _mapData.playerStart2, _mapData.playerStart3, _mapData.playerStart4 };
            for(int i = 0; i < 4; i++)
            {
                if(playerStarts[i].z == ZLevel)
                {
                    HelperUtil.CopyRegionIntoImage(_playerStartImgArray[i], new Rectangle(0, 0, _mapTileSize, _mapTileSize), ref _mapLevelImage, new Rectangle(playerStarts[i].x * _mapTileSize, playerStarts[i].y * _mapTileSize, _mapTileSize, _mapTileSize));
                }
            }
            
            MapViewPictureBox.Image = _mapLevelImage;
            UpdateZoomedMapView();
        }
        private void PasteMapTile(Bitmap bm, int x, int y)
        {
            HelperUtil.CopyRegionIntoImage(bm, new Rectangle(0, 0, _mapTileSize, _mapTileSize), ref _mapLevelImage, new Rectangle(x * _mapTileSize, y * _mapTileSize, _mapTileSize, _mapTileSize));

        }
        private void UpdateZoomedMapView()
        {
            Size mapImageSize = new Size(_mapLevelImage.Width * _mapTileScale, _mapLevelImage.Height * _mapTileScale);
            MapViewPictureBox.Size = mapImageSize;
        }
        private void UpdateStyle()
        {
            UpdateMapView();
        }
        private void UpdateMapViewCursor()
        {
            if (_mapData == null)
            {
                return;
            }
            SetCoordsCursor(Coordinate3.PointToCoordinate3(_mapViewMousePos, ZLevel, MapTileSizeScaled));
           
            if (_coordsCursorLast.Equals(_coordsCursor) == false)
            {
                if (SelectToggleButton.Checked || Control.ModifierKeys == Keys.Shift)
                {
                    if (_mouse0Down)
                    {
                        SetCoordsSelected(_coordsCursorDragStart, _coordsCursor);
                    }
                    else if (_coordsSelected.Count > 0)
                    {
                        // SetCoordsSelected(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    }
                }
                else if(PaintToggleButton.Checked)
                {
                    if (_mouse0Down)
                    {
                        if (_pasting)
                        {
                            PasteCellsFromClipboard();
                        }
                        else
                        {
                            PaintCell(_coordsCursor);
                        }
                    }
                    else if (_mouse1Down)
                    {
                        ClearCell(_coordsCursor);
                    }
                }
                else if (EraseToggleButton.Checked)
                {
                    if (_mouse0Down)
                    {
                        ClearCell(_coordsCursor);
                    }
                }
                /*
                if(_pasting && _cellClipboard.cells.Length > 0)
                {
                    Coordinate3 coordsStart = GetClipboardUpdateCoordsStart(_coordsCursor);
                    Coordinate3 coordsEnd = GetClipboardUpdateCoordsEnd(_coordsCursor);
                    UpdateMapView(coordsStart.x, coordsStart.y, coordsEnd.x, coordsEnd.y);
                }
                */
            }
            _coordsCursorLast = _coordsCursor;
            MapViewPictureBox.Invalidate();
        }

        private void AddSelectedCoords(Coordinate3 coords)
        {
            coords.ClampCoords();
            _coordsSelected.Add(coords);
        }
        public Coordinate3 GetCoordsSelected()
        {
            return _coordsSelected[0];
        }
        private void SetCoordsCursor(Coordinate3 coords)
        {
            coords.ClampCoords();
            _coordsCursor = coords;
        }
        private void SetCoordsSelected(Coordinate3 coordsStart)
        {
            coordsStart.ClampCoords();
            SetCoordsSelected(coordsStart, coordsStart);
        }
        private void SetCoordsSelected(Coordinate3 coordsStart, Coordinate3 coordsEnd)
        {
            if (_mapData == null || _mapData.cells == null)
            {
                return;
            }
            coordsStart.ClampCoords();
            coordsEnd.ClampCoords();
            int startX = HelperUtil.Clamp<int>(Math.Min(coordsStart.x, coordsEnd.x),0,HGMapData.MAP_WIDTH - 1);
            int startY = HelperUtil.Clamp<int>(Math.Min(coordsStart.y, coordsEnd.y), 0, HGMapData.MAP_HEIGHT - 1);
            int startZ = HelperUtil.Clamp<int>(Math.Min(coordsStart.z, coordsEnd.z), 0, HGMapData.MAP_LEVELS - 1);
            int endX = HelperUtil.Clamp<int>((coordsEnd.x < coordsStart.x ? coordsStart.x : coordsEnd.x),0,HGMapData.MAP_WIDTH - 1);
            int endY = HelperUtil.Clamp<int>((coordsEnd.y < coordsStart.y ? coordsStart.y : coordsEnd.y),0,HGMapData.MAP_HEIGHT - 1);
            int endZ = HelperUtil.Clamp<int>((coordsEnd.z < coordsStart.z ? coordsStart.z : coordsEnd.z),0,HGMapData.MAP_LEVELS - 1);

            //_coordsSelected = new Coordinate3[((endX - startX) + 1) * ((endY - startY) + 1) * ((endZ - startZ) + 1)];
            _coordsSelected.Clear();

            HGCell cellStart = _mapData.cells[startX, startY, startZ];

            if (cellStart == null)
            {
                return;
            }
            SetBlockHandlerKeep();
            int i = 0;
            for(int z = startZ; z <= endZ; z++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++, i++)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        AddSelectedCoords(coords);
                        
                        if (i == 0)
                        {
                            PickCell(coords, setBlockHandlerKeep : false);
                        }
                        else
                        {
                            SetBlockHandlerKeep();
                            HGCell cellCur = _mapData.cells[x, y, z];
                            if (cellCur.blockHere != cellStart.blockHere || cellCur.blockType != cellStart.blockType)
                            {
                                BlockTypeDropDownList.SelectedIndex = BlockTypeDropDownList.Items.Count - 1;
                            }
                            if (BlockPushableCheckBox.CheckState != CheckState.Indeterminate && cellCur.pushable != cellStart.pushable)
                            {
                                BlockPushableCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (BlockInvisibleCheckBox.CheckState != CheckState.Indeterminate && cellCur.invisible != cellStart.invisible)
                            {
                                BlockInvisibleCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (BlockOpaqueCheckBox.CheckState != CheckState.Indeterminate && cellCur.opaque != cellStart.opaque)
                            {
                                BlockOpaqueCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if ((TeleportTargetPosXTextBox.Text != "" || TeleportTargetPosYTextBox.Text != "" || TeleportTargetPosZTextBox.Text != "") && (cellCur.teleportTargetPos.Equals(cellStart.teleportTargetPos) == false))
                            {
                                TeleportTargetPosXTextBox.Text = "";
                                TeleportTargetPosYTextBox.Text = "";
                                TeleportTargetPosZTextBox.Text = "";
                            }
                            if (FitnessBoostValueTextBox.Text != "" && cellCur.healingValue != cellStart.healingValue)
                            {
                                FitnessBoostValueTextBox.Text = "";
                            }
                            if (ForceQuestionMarkCheckBox.CheckState != CheckState.Indeterminate && cellCur.forceQuestionMark != cellStart.forceQuestionMark)
                            {
                                ForceQuestionMarkCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (cellCur.floorHere != cellStart.floorHere || cellCur.floorType != cellStart.floorType)
                            {
                                FloorTypeDropDownList.SelectedIndex = FloorTypeDropDownList.Items.Count - 1;
                                FloorTypeDropDownList.Items[4] = "Puddle / Light";
                            }
                            if (cellCur.panelHere != cellStart.panelHere || cellCur.panelType != cellStart.panelType)
                            {
                                PanelTypeDropDownList.SelectedIndex = PanelTypeDropDownList.Items.Count - 1;
                            }
                            if (cellCur.waterHere != cellStart.waterHere || cellCur.waterLevel != cellStart.waterLevel)
                            {
                                WaterDropDownList.SelectedIndex = WaterDropDownList.Items.Count - 1;
                            }
                            if (TidalWaterFlowingCheckBox.CheckState != CheckState.Indeterminate && cellCur.tidalWaterFlowing != cellStart.tidalWaterFlowing)
                            {
                                TidalWaterFlowingCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (cellCur.auxiliaryHere != cellStart.auxiliaryHere || cellCur.auxiliaryType != cellStart.auxiliaryType)
                            {
                                AuxiliaryDropDownList.SelectedIndex = AuxiliaryDropDownList.Items.Count - 1;
                            }
                            if (EggBlockMonsterCheckBox.CheckState != CheckState.Indeterminate && cellCur.eggBlocked != cellStart.eggBlocked)
                            {
                                EggBlockMonsterCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (EggEraseCheckBox.CheckState != CheckState.Indeterminate && cellCur.eggErase != cellStart.eggErase)
                            {
                                EggEraseCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (EggDelayTextBox.Text != "" && cellCur.eggDelay != cellStart.eggDelay)
                            {
                                EggDelayTextBox.Text = "";
                            }
                            if (SkyNCheckBox.CheckState != CheckState.Indeterminate && cellCur.skyRendered[0] != cellStart.skyRendered[0])
                            {
                                SkyNCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (SkyECheckBox.CheckState != CheckState.Indeterminate && cellCur.skyRendered[1] != cellStart.skyRendered[1])
                            {
                                SkyECheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (SkySCheckBox.CheckState != CheckState.Indeterminate && cellCur.skyRendered[2] != cellStart.skyRendered[2])
                            {
                                SkySCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (SkyWCheckBox.CheckState != CheckState.Indeterminate && cellCur.skyRendered[3] != cellStart.skyRendered[3])
                            {
                                SkyWCheckBox.CheckState = CheckState.Indeterminate;
                            }
                            if (cellCur.itemID != cellStart.itemID)
                            {
                                ItemDropDownList.SelectedIndex = ItemDropDownList.Items.Count - 1;
                            }
                            if (ItemDamagedTextBox.Text != "" && cellCur.itemDamaged != cellStart.itemDamaged)
                            {
                                ItemDamagedTextBox.Text = "";
                            }
                            if (ItemAmmunitionTextBox.Text != "" && cellCur.itemAmmo != cellStart.itemAmmo)
                            {
                                ItemAmmunitionTextBox.Text = "";
                            }
                            if (cellCur.monsterType != cellStart.monsterType || cellCur.monsterLevel != cellStart.monsterLevel)
                            {
                                DeselectMonsterAll();
                            }
                            if(cellCur.panelButtonIndex != cellStart.panelButtonIndex && cellCur.panelButtonIndex < _mapData.buttons.Length && cellStart.panelButtonIndex < _mapData.buttons.Length)
                            {
                                HGButton buttonCur = _mapData.buttons[cellCur.panelButtonIndex];
                                HGButton buttonStart = _mapData.buttons[cellStart.panelButtonIndex];
                                if(buttonCur != null && buttonStart != null)
                                {
                                    if(buttonCur.actionPushed != buttonStart.actionPushed)
                                    {
                                        ButtonActionPushedDropDownList.SelectedIndex = ButtonActionPushedDropDownList.Items.Count - 1;
                                    }
                                    if (buttonCur.actionReleased != buttonStart.actionReleased)
                                    {
                                        ButtonActionReleasedDropDownList.SelectedIndex = ButtonActionReleasedDropDownList.Items.Count - 1;
                                    }
                                    if(buttonCur.targetPosPushed.x != buttonStart.targetPosPushed.x)
                                    {
                                        ButtonTargetInPositionXTextBox.Text = "";
                                    }
                                    if ( buttonCur.targetPosPushed.y != buttonStart.targetPosPushed.y)
                                    {
                                        ButtonTargetInPositionYTextBox.Text = "";
                                    }
                                    if (buttonCur.targetPosPushed.z != buttonStart.targetPosPushed.z)
                                    {
                                        ButtonTargetInPositionZTextBox.Text = "";
                                    }
                                    if(buttonCur.index != buttonStart.index)
                                    {
                                        ButtonIndexTextBox.Text = "";
                                    }
                                }
                            }
                            if (cellCur.doorIndex != cellStart.doorIndex && cellCur.doorIndex < _mapData.doors.Length && cellStart.doorIndex < _mapData.doors.Length)
                            {
                                HGDoor doorCur = _mapData.doors[cellCur.doorIndex];
                                HGDoor doorStart = _mapData.doors[cellStart.doorIndex];
                                if (doorCur != null && doorStart != null)
                                {
                                    if (doorCur.closeDelay != doorStart.closeDelay)
                                    {
                                        DoorCloseDelayTextBox.Text = "";
                                    }
                                    if (doorCur.keyID != doorStart.keyID)
                                    {
                                        DoorKeyIDTextBox.Text = "";
                                    }
                                    if (doorCur.locked != doorStart.locked)
                                    {
                                        DoorLockedCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (doorCur.buttonOnly != doorStart.buttonOnly)
                                    {
                                        DoorButtonOnlyCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (doorCur.index != doorStart.index)
                                    {
                                        DoorIndexTextBox.Text = "";
                                    }
                                }
                            }
                            if (cellCur.liftIndex != cellStart.liftIndex && cellCur.liftIndex < _mapData.lifts.Length && cellStart.liftIndex < _mapData.lifts.Length)
                            {
                                /*
                                HGLift liftCur = _mapData.lifts[cellCur.liftIndex];
                                HGLift liftStart = _mapData.lifts[cellStart.liftIndex];
                                if (liftCur != null && liftStart != null)
                                {
                                    if (liftCur.initial != liftStart.initial)
                                    {
                                        LiftIndexTextBox.Text = "";
                                    }
                                    if (liftCur.maxZ != liftStart.maxZ)
                                    {
                                        LiftMaxZTextBox.Text = "";
                                    }
                                    if (liftCur.minZ != liftStart.minZ)
                                    {
                                        LiftMinZTextBox.Text = "";
                                    }
                                    if (liftCur.carry != liftStart.carry)
                                    {
                                        LiftCarryCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (liftCur.continueAtTop != liftStart.continueAtTop)
                                    {
                                        LiftContinueAtTopCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (liftCur.continueAtBottom != liftStart.continueAtBottom)
                                    {
                                        LiftContinueAtBottomCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (liftCur.automove != liftStart.automove)//Off = 0, bottom = 1, top = 2, both = 3
                                    {
                                        if((liftCur.automove == 1 && liftStart.automove != 3) || (liftStart.automove == 1 && liftCur.automove != 3))
                                        {
                                            LiftAutomoveBottomCheckBox.CheckState = CheckState.Indeterminate;
                                        }
                                        if ((liftCur.automove == 2 && liftStart.automove != 3) || (liftStart.automove == 2 && liftCur.automove != 3))
                                        {
                                            LiftAutomoveTopCheckBox.CheckState = CheckState.Indeterminate;
                                        }
                                    }
                                    if (liftCur.continueAtBottom != liftStart.continueAtBottom)
                                    {
                                        LiftContinueAtBottomCheckBox.CheckState = CheckState.Indeterminate;
                                    }
                                    if (liftCur.index != liftStart.index)
                                    {
                                        LiftIndexTextBox.Text = "";
                                    }
                                }
                                */
                            }
                        }
                    }
                }
            }
            SetBlockHandlerKeep(false);
            UpdateDebugTextBox();
        }
        private Bitmap GetMapTile(int mapTileIndex)
        {
            if(_mapData == null)
            {
                return null;
            }
            return _hgGameData.styles[_mapData.style].mapTiles[mapTileIndex];
        }
        private void UpdateDebugTextBox()
        {
            if(_mapData == null || _coordsSelected == null || _coordsSelected.Count == 0)
            {
                return;
            }
            HGCell cell = _mapData.cells[_coordsSelected[0].x, _coordsSelected[0].y, _coordsSelected[0].z];
            /*
            DebugTextBox.Text =
                $"monsterType1: {_mapData.monsterType1}, monsterType2: {_mapData.monsterType2}" +
                $"\r\nworldMapPosX: {_mapData.worldMapPosX}, worldMapPosY: {_mapData.worldMapPosY}";
            */
            /*
            byte[] b = BitConverter.GetBytes(30);
            byte[] br = b.Reverse().ToArray();
            byte[] b2 = BitConverter.GetBytes(258);
            byte[] b2r = b2.Reverse().ToArray();
            byte[] worldMapPosXBytes = BitConverter.GetBytes(_mapData.worldMapPosX);
            byte[] worldMapPosXBytesR = BitConverter.GetBytes(_mapData.worldMapPosX).Reverse().ToArray();
            byte[] worldMapPosYBytes = BitConverter.GetBytes(_mapData.worldMapPosY);
            byte[] worldMapPosYBytesR = BitConverter.GetBytes(_mapData.worldMapPosY).Reverse().ToArray();
            DebugTextBox.Text +=
               $"\r\n30 -> {b[0]},{b[1]},{b[2]},{b[3]} -> {br[0]},{br[1]},{br[2]},{br[3]}" +
               $"\r\n258 -> {b2[0]},{b2[1]},{b2[2]},{b2[3]} -> {b2r[0]},{b2r[1]},{b2r[2]},{b2r[3]}" +
                $"\r\n{_mapData.worldMapPosX} -> {worldMapPosXBytes[0]},{worldMapPosXBytes[1]},{worldMapPosXBytes[2]},{worldMapPosXBytes[3]} -> {worldMapPosXBytesR[0]},{worldMapPosXBytesR[1]},{worldMapPosXBytesR[2]},{worldMapPosXBytesR[3]}" +
                 $"\r\n{_mapData.worldMapPosY} -> {worldMapPosYBytes[0]},{worldMapPosYBytes[1]},{worldMapPosYBytes[2]},{worldMapPosYBytes[3]} -> {worldMapPosYBytesR[0]},{worldMapPosYBytesR[1]},{worldMapPosYBytesR[2]},{worldMapPosYBytesR[3]}";
            */
            /*
            DebugTextBox.Text += $"\r\n blockType: {cell.blockType}";
            DebugTextBox.Text += $"\r\n pushable: {cell.pushable}";
            DebugTextBox.Text += $"\r\ncell.auxiliaryHere: {cell.auxiliaryHere}, cell.auxiliaryType: {cell.auxiliaryType}";
            int cell1D = HelperUtil.to1D(_coordsSelected[0].x, _coordsSelected[0].y, _coordsSelected[0].z, HGMapData.MAP_WIDTH, HGMapData.MAP_HEIGHT);
            DebugTextBox.Text += $"\r\nCoord1D: : {cell1D}";
            DebugTextBox.Text += $"\r\nCellData1 (1D) : {28478 + cell1D * 4}";
            DebugTextBox.Text += $"\r\nCellData2 (1D) : {71718 + cell1D * 4}";
            DebugTextBox.Text += $"\r\nCellData3 (1D) : {114958 + cell1D * 4}";
            int coords1D = _coordsSelected[0].To1D(HGMapData.MAP_WIDTH, HGMapData.MAP_HEIGHT);
            DebugTextBox.Text += $"\r\nPos (1D) : {coords1D} ({Math.Floor((double)coords1D / 256)} {coords1D % 256})";

            DebugTextBox.Text += $"\r\ncell.panelButtonIndex: {cell.panelButtonIndex}";
            DebugTextBox.Text += $"\r\ncell.healingValue: {cell.healingValue}";
            DebugTextBox.Text += $"\r\ncell.auxiliaryType: {cell.auxiliaryType}({(int)cell.auxiliaryType})";
            DebugTextBox.Text += $"\r\nShowExtendedCursorInfo: {ShowExtendedCursorInfo}";
            */
            /*
            for (int i = 0; i < _mapData.doors.Length; i++)
            {
                HGDoor door = _mapData.doors[i];
                DebugTextBox.Text += $"\r\n[{i}] {door.coords}";
            }
            */
            /*
            DebugTextBox.Text += $"\r\n***LIFTS***";
            for (int i = 0; i < _mapData.lifts.Length; i++)
            {
                HGLift lift = _mapData.lifts[i];
                DebugTextBox.Text += $"\r\n[{lift.index}] {lift.coords}";
            }
            */
            /*
            DebugTextBox.Text += $"\r\n***DOORS***";
            for (int i = 0; i < _mapData.doors.Length; i++)
            {
                HGDoor door = _mapData.doors[i];
                DebugTextBox.Text += $"\r\n[{door.index}] {door.coords}";
            }
            */
            /*
            DebugTextBox.Text += $"\r\npanelButtonIndex: {cell.panelButtonIndex}";
            DebugTextBox.Text += $"\r\n***BUTTONS***";
            for (int i = 0; i < _mapData.buttons.Length; i++)
            {
                HGButton button = _mapData.buttons[i];
                DebugTextBox.Text += $"\r\n[{button.index}] {button.coords} {button.actionPushed} {button.actionReleased}";
            }
            */
            /*
            for (int i = 0; i < _mapData.conversations.Length; i++)
            {
                HGConversation conv = _mapData.conversations[i];
                DebugTextBox.Text += $"\r\n[{i}] {conv.text}";
                DebugTextBox.Text += $" (length: {conv.length}, offset: {conv.offset})";
            }
            */
            /*
            BitArray bits = new BitArray(8);
            bits[0] = false;
            bits[1] = true;
            bits[2] = false;
            bits[3] = true;
            bits[4] = false;
            bits[5] = false;
            bits[6] = false;
            bits[7] = false;
            HelperUtil.BitArrayReverse(bits);
            BitArray bits2 = new BitArray(8);
            bits2[0] = false;
            bits2[1] = false;
            bits2[2] = true;
            bits2[3] = true;
            bits2[4] = false;
            bits2[5] = false;
            bits2[6] = false;
            bits2[7] = false;
            HelperUtil.BitArrayReverse(bits2);
            byte by = (byte)HelperUtil.GetIntFromBitArray(bits);
            byte by2 = (byte)HelperUtil.GetIntFromBitArray(bits2);
            DebugTextBox.Text += "\r\n";
            DebugTextBox.Text += bits[0] == false ? "0" : "1";
            DebugTextBox.Text += bits[1] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[2] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[3] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[4] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[5] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[6] == false ? " 0" : " 1";
            DebugTextBox.Text += bits[7] == false ? " 0" : " 1";
            DebugTextBox.Text += $" = {by}";
            DebugTextBox.Text += "\r\n";
            DebugTextBox.Text += bits2[0] == false ? "0" : "1";
            DebugTextBox.Text += bits2[1] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[2] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[3] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[4] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[5] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[6] == false ? " 0" : " 1";
            DebugTextBox.Text += bits2[7] == false ? " 0" : " 1";
            DebugTextBox.Text += $" = {by2}";
            BitArray bitA0 = new BitArray(2);
            bitA0[0] = bits[0];
            bitA0[1] = bits2[0];
            BitArray bitA1 = new BitArray(2);
            bitA1[0] = bits[1];
            bitA1[1] = bits2[1];
            BitArray bitA2 = new BitArray(2);
            bitA2[0] = bits[2];
            bitA2[1] = bits2[2];
            BitArray bitA3 = new BitArray(2);
            bitA3[0] = bits[3];
            bitA3[1] = bits2[3];
            BitArray bitA4 = new BitArray(2);
            bitA4[0] = bits[4];
            bitA4[1] = bits2[4];
            BitArray bitA5 = new BitArray(2);
            bitA5[0] = bits[5];
            bitA5[1] = bits2[5];
            BitArray bitA6 = new BitArray(2);
            bitA6[0] = bits[6];
            bitA6[1] = bits2[6];
            BitArray bitA7 = new BitArray(2);
            bitA7[0] = bits[7];
            bitA7[1] = bits2[7];
            DebugTextBox.Text += "\r\n";
            DebugTextBox.Text += $"{HelperUtil.GetIntFromBitArray(bitA0)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA1)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA2)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA3)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA4)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA5)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA6)}";
            DebugTextBox.Text += $" {HelperUtil.GetIntFromBitArray(bitA7)}";
            */
            /*
            DebugTextBox.Text =
                $"Map:" +
                $"\r\n" +
                $"\r\ncellData1 bytes: {cell.cellData1_1Byte},{cell.cellData1_2Byte},{cell.cellData1_3Byte},{cell.cellData1_4Byte} " +
                $"\r\npanelButtonIndex: {cell.panelButtonIndex}" +
                $"\r\nwaterHere: {cell.waterHere}" +
                $"\r\nwaterLevel: {cell.waterLevel}" +
                $"\r\n" +
                $"\r\n" +
                $"\r\ncellData2 bytes: {cell.cellData2_1Byte},{cell.cellData2_2Byte},{cell.cellData2_3Byte},{cell.cellData2_4Byte} " +
                $"\r\neggDelay: {cell.eggDelay}" +
                $"\r\nmonsterType: {cell.monsterType}, monsterLevel: {cell.monsterLevel}" +
                $"\r\n(monsterTypes loaded: {_mapData.monsterType1} and {_mapData.monsterType2})" +
                $"\r\ntidalWaterFlowing: {cell.tidalWaterFlowing}" +
                $"\r\nfloorTile: {cell.floorTile}" +
                $"\r\n" +
                $"\r\ncellData3 bytes: {cell.cellData3_1Byte},{cell.cellData3_2Byte},{cell.cellData3_3Byte},{cell.cellData3_4Byte} " +
                $"\r\neggBlocked: {cell.eggBlocked}, eggErase: {cell.eggErase}" +
                $"\r\nskyRendered: North(0): {cell.skyRendered[0]}, East(1): {cell.skyRendered[1]}, South(2): {cell.skyRendered[2]}, West(3): {cell.skyRendered[3]}" +
                $"\r\nitemID: {cell.itemID}, itemDamaged: {cell.itemDamaged}, itemAmmo: {cell.itemAmmo}" +
                $"\r\n"
            ;*/
            /*
            DebugTextBox.Text =
               $"Cell:" +
               $"\r\nblockType: {cell.blockType}, blockHere: {cell.blockHere}," +
               $"\r\npushable: {cell.pushable}, invisible: {cell.invisible}, opaque: {cell.opaque}," +
               $"\r\nfloorTile: {cell.floorTile}";
            */
            /*
            DebugTextBox.Text += $"\r\nConversations ({_mapData.conversations.Length}):";
            for (int i = 0; i < _mapData.conversations.Length; i++)
            {
                HGConversation conv = _mapData.conversations[i];
                if (conv == null)
                {
                    break;
                }
                DebugTextBox.Text += $"\r\n[{i}] {conv.text}";
                DebugTextBox.Text += $"\r\n\tcoords: {conv.coords}, offset: {conv.offset}, length: {conv.length}";
            }
            */
            /*
            DebugTextBox.Text += $"\r\nLifts ({_mapData.lifts.Length}):";
            for (int i = 0; i < _mapData.lifts.Length; i++)
            {
                HGLift lift = _mapData.lifts[i];
                if (lift == null)
                {
                    break;
                }
                DebugTextBox.Text += $"\r\n[{i}] coords: {lift.coords},";
                DebugTextBox.Text += $"\r\nminZ: {lift.minZ}, maxZ: {lift.maxZ}, initial: {lift.initial},";
                DebugTextBox.Text += $"\r\ncarry: {lift.carry}, automove: {lift.automove}";
                DebugTextBox.Text += $"\r\ncontinueAtTop: {lift.continueAtTop}, continueAtBottom: {lift.continueAtBottom}";
            }
            */
            /*
            DebugTextBox.Text += $"\r\nButtons ({_mapData.buttons.Length}):";
            for (int i = 0; i < _mapData.buttons.Length; i++)
            {
                HGButton button = _mapData.buttons[i];
                if (button == null)
                {
                    break;
                }
                DebugTextBox.Text += $"\r\n[{i}] coords: {button.coords},";
                DebugTextBox.Text += $"\r\npushed: {button.actionPushed}, released: {button.actionReleased},";
                DebugTextBox.Text += $"\r\npushed target: {button.targetPosPushed},";
                DebugTextBox.Text += $"\r\nreleased target: {button.targetPosReleased},";
            }
            */
            /*
            DebugTextBox.Text = "";
            if (_mapData != null && _coordsSelected.Count > 0)
            {
                for(int i = 0; i < _coordsSelected.Count;i++)
                {
                    DebugTextBox.Text += $"\r\n{_coordsSelected[i]}: {_mapData.cells[_coordsSelected[i].x, _coordsSelected[i].y, _coordsSelected[i].z].blockType}";
                }
            }
            */

        }
        private void UpdateMonsterInfo()
       {
           MonsterLevelDropDownList.Items.Clear();
           for(int i = 0; i < _monsters[MonsterSelected].levels.Length; i++)
           {
                HGCharacter monster = _monsters[MonsterSelected];
                HGCharacterLevel lvl = monster.levels[i];
                MonsterLevelDropDownList.Items.Add((i + 1).ToString() + (lvl.shielded ? "*" : ""));
           }
       }
       private void LoadMonster(int monsterIndex, bool setMapData = true)
       {
           _monsterButtons[monsterIndex].BackColor = _monsterButtonColorLoaded;
           _monsterButtons[monsterIndex].MonsterLoaded = true;
            if(setMapData)
            {
                int monster1Loaded = -1;
                int monster2Loaded = -1;
                for (int i = 0; i < _monsterButtons.Length; i++)
                {
                    MonsterButton btn = _monsterButtons[i];
                    if(btn.MonsterLoaded)
                    {
                        if(monster1Loaded == -1)
                        {
                            monster1Loaded = btn.monsterIndex;
                            btn.loadedType = 1;
                            continue;
                        }
                        if (monster2Loaded == -1)
                        {
                            monster2Loaded = btn.monsterIndex;
                            btn.loadedType = 2;
                            continue;
                        }
                        if (monster1Loaded != -1 && monster1Loaded != -1)
                        {
                            AddConsoleMessage($"[LoadMonster] Too many monsters loaded! A maximum of 2 monsters may be loaded at the same time!", warning: true);
                            MessageBox.Show($"[LoadMonster] Too many monsters loaded!\r\nA maximum of 2 monsters\r\nmay be loaded at the same time!");
                            break;
                        }
                    }
                }
                _mapData.monsterType1 = monster1Loaded + 1;
                _mapData.monsterType2 = monster2Loaded + 1;
            }
            //make sure to invalidate all monster buttons so the loadedType indicator is updated in Paint()
            for (int i = 0; i < _monsterButtons.Length; i++)
            {
                MonsterButton btn = _monsterButtons[i];
                btn.Invalidate();
            }
        }
       private void UnloadMonster(int monsterIndex, bool resetMapData = true)
       {
            if (resetMapData)
            {
                if (_mapData.monsterType1 == monsterIndex + 1)
                {
                    _mapData.monsterType1 = 0;
                }
                else if (_mapData.monsterType2 == monsterIndex + 1)
                {
                    _mapData.monsterType2 = 0;
                }
            }
            _monsterButtons[monsterIndex].BackColor = _monsterButtonColorUnloaded;
            _monsterButtons[monsterIndex].MonsterLoaded = false;
        }
       private void UnloadMonsterAll(bool resetMapData = false)
       {
           for (int i = 0; i < _monsterButtons.Length; i++)
           {
                UnloadMonster(i, resetMapData);
           }
       }
       private void SelectMonster(int monsterType, int monsterLevel = -1)
       {
           MonsterSelected = monsterType;
           if(monsterLevel != -1)
           {
                MonsterLevelDropDownList.SelectedIndex = monsterLevel;
           }
            else
            {
                SelectedMonsterTypeLabel.Text = "";
            }
            SelectedMonsterTypeLabel.Text = _monsters[monsterType].name;
        }
        private void DeselectMonsterAll()
       {
           if(MonsterLevelDropDownList != null && MonsterLevelDropDownList.Items.Count > 0)
           {
               MonsterLevelDropDownList.SelectedIndex = 0;
           }
           if(_monsterButtons != null)
           {
               for (int i = 0; i < _monsterButtons.Length; i++)
               {
                   _monsterButtons[i].Selected = false;
               }
           }
       }
       private void SelectPanel(int panelType)
       {
           PanelSelected = panelType;
       }
       private void DeselectPanelAll()
       {
           if(_panelButtons != null)
           {
               for (int i = 0; i < _panelButtons.Length; i++)
               {
                   _panelButtons[i].Selected = false;
               }
           }
        }
        public HGLift AddLift(Coordinate3 coords)
        {
            if(_mapData == null)
            {
                return null;
            }
            if(_mapData.CountLifts() >= _mapData.lifts.Length)
            {
                AddConsoleMessage($"[AddLift] Maximum number of lifts reached! ({_mapData.lifts.Length})", warning: true);
                SetFloorType(coords, HGCellFloorType.Grass, false, setControl: false, setCell: true);
                return null;
            }
            HGLift lift = _mapData.GetLift(coords,true);
            lift.Set
            (
                coords : coords, 
                minZ : String.IsNullOrEmpty(LiftMinZTextBox.Text) ? 0 : int.Parse(LiftMinZTextBox.Text), 
                maxZ: String.IsNullOrEmpty(LiftMaxZTextBox.Text) ? 0 : int.Parse(LiftMaxZTextBox.Text), 
                initial: LiftInitialDropDownList.SelectedIndex, 
                carry: LiftCarryCheckBox.Checked, 
                continueOnTop: LiftContinueAtTopCheckBox.Checked, 
                continueAtBottom : LiftContinueAtBottomCheckBox.Checked, 
                automove : GetLiftAutoMoveFromControls()
            );
            _mapData.GetCell(coords).liftIndex = lift.index;
            UpdateLiftsListBox();
            return lift;
        }
        public void RemoveLift(Coordinate3 coords)
        {
            if (_mapData == null)
            {
                return;
            }
            HGLift lift = _mapData.GetLift(coords,false);
            if(lift != null)
            {
                lift.Clear();
                SetLiftInitial(coords, 0, setControl: false, setCell: true);
                SetLiftMaxZ(coords, 0, setControl: false, setCell: true);
                SetLiftMinZ(coords, 0, setControl: false, setCell: true);
                SetLiftCarry(coords, false, setControl: false, setCell: true);
                SetLiftContinueAtTop(coords, false, setControl: false, setCell: true);
                SetLiftContinueAtBottom(coords, false, setControl: false, setCell: true);
                SetLiftAutomoveTop(coords, false, setControl: false, setCell: true);
                SetLiftAutomoveBottom(coords, false, setControl: false, setCell: true);
            }
            _mapData.GetCell(coords).liftIndex = 0;
            UpdateLiftsListBox();
        }
        private void UpdateLiftsListBox()
        {
            for(int i = 0; i < _mapData.lifts.Length; i++)
            {
                string s = "";
                HGLift lift = _mapData.lifts[i];
                HGCell cell = _mapData.GetCell(lift.coords);
                if(cell.floorHere && cell.floorType == HGCellFloorType.Lift)
                {
                    s = $"Lift at ({lift.coords}), minZ: {lift.minZ}, maxZ: {lift.maxZ}, initial: {lift.initial}";
                }
                LiftsListBox.Items[lift.index] = $"{i.ToString().PadLeft(2, '0')} {s}".PadRight(listViewStringLength);
            }
        }
        public HGDoor AddDoor(Coordinate3 coords)
        {
            if (_mapData == null)
            {
                return null;
            }
            HGCell cell = _mapData.GetCell(coords);
            if (_mapData.CountDoors() >= _mapData.doors.Length)
            {
                AddConsoleMessage($"[AddLift] Maximum number of doors reached! ({_mapData.doors.Length})", warning: true);
                SetBlockType(coords, HGCellBlockType.Stone, false, setControl: false, setCell: true);
                return null;
            }
            HGDoor door = _mapData.GetDoor(coords, true);
            door.Set
            (
                coords : coords,
                locked : DoorLockedCheckBox.Checked,
                dirNS : cell.blockType == HGCellBlockType.DoorNS,
                closeDelay : String.IsNullOrEmpty(DoorCloseDelayTextBox.Text) ? 0 : int.Parse(DoorCloseDelayTextBox.Text),
                keyID : String.IsNullOrEmpty(DoorKeyIDTextBox.Text) ? 0 : int.Parse(DoorKeyIDTextBox.Text),
                buttonOnly : DoorButtonOnlyCheckBox.Checked
            );
            _mapData.GetCell(coords).doorIndex = door.index;
            UpdateDoorsListBox();
            return door;
        }
        public void RemoveDoor(Coordinate3 coords)
        {
            if (_mapData == null)
            {
                return;
            }
            HGDoor door = _mapData.GetDoor(coords, false);
            if (door != null)
            {
                door.Clear();
                SetDoorCloseDelay(coords, 0, setControl: false, setCell: true);
                SetDoorKeyID(coords, 0, setControl: false, setCell: true);
                SetDoorLocked(coords, false, setCell: true);
                SetDoorButtonOnly(coords, false, setCell: true);
            }
            _mapData.GetCell(coords).doorIndex = 0;
            UpdateDoorsListBox();
        }
        private void UpdateDoorsListBox()
        {
            for (int i = 0; i < _mapData.doors.Length; i++)
            {
                string s = "";
                HGDoor door = _mapData.doors[i];
                HGCell cell = _mapData.GetCell(door.coords);
                if (cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW))
                {
                    s = $"Door at ({door.coords}), locked: {door.locked}, closeDelay: {door.closeDelay}, keyID: {door.keyID}";
                }
                DoorsListBox.Items[door.index] = $"{i.ToString().PadLeft(2, '0')} {s}".PadRight(listViewStringLength);
            }
        }
        public HGButton AddButton(Coordinate3 coords)
        {
            if (_mapData == null)
            {
                return null;
            }
            HGCell cell = _mapData.GetCell(coords);
            if (_mapData.CountButtons() >= _mapData.buttons.Length)
            {
                if(cell.blockHere && cell.blockType == HGCellBlockType.Stone)
                {
                    SetPanelType(coords, HGCellPanelType.Text, false, setControl: false, setCell: true);
                }
                else if(cell.floorHere && cell.floorType == HGCellFloorType.Button)
                {
                    SetFloorType(coords,HGCellFloorType.Grass,false, setControl: false, setCell: true);
                }
                AddConsoleMessage($"[AddLift] Maximum number of buttons reached! ({_mapData.buttons.Length})", warning: true);
                return null;
            }
            HGButton button = _mapData.GetButton(coords, true);
            button.Set
            (
                coords: coords,
                actionPushed: (HGButtonAction)ButtonActionPushedDropDownList.SelectedIndex,
                actionReleased: (HGButtonAction)ButtonActionReleasedDropDownList.SelectedIndex,
                targetPosPushed: new Coordinate3(),
                targetPosReleased: new Coordinate3()
            );
            Coordinate3 indexCoords = coords;
            if (cell.floorType == HGCellFloorType.Button && coords.z > 0)
            {
                indexCoords = new Coordinate3(coords.x, coords.y, coords.z - 1);
            }
            _mapData.GetCell(indexCoords).panelButtonIndex = button.index;
            UpdateButtonsListBox();
            return button;
        }
        public void RemoveButton(Coordinate3 coords)
        {
            if (_mapData == null)
            {
                return;
            }
            HGCell cell = _mapData.GetCell(coords);
            HGButton button = _mapData.GetButton(coords, false);
            if (button != null)
            {
                button.Clear();
                SetButtonTargetInPositionX(coords, 0, setControl: false, setCell: true);
                SetButtonTargetInPositionY(coords, 0, setControl: false, setCell: true);
                SetButtonTargetInPositionZ(coords, 0, setControl: false, setCell: true);
                SetButtonTargetOutPositionX(coords, 0, setControl: false, setCell: true);
                SetButtonTargetOutPositionY(coords, 0, setControl: false, setCell: true);
                SetButtonTargetOutPositionZ(coords, 0, setControl: false, setCell: true);
                SetButtonActionPushed(coords, HGButtonAction.None, setControl: false, setCell: true);
                SetButtonActionReleased(coords, HGButtonAction.None, setControl: false, setCell: true);
            }
            if(cell.panelHere == false || cell.panelType != HGCellPanelType.Text)
            {
                Coordinate3 indexCoords = coords;
                if (cell.floorType == HGCellFloorType.Button && coords.z > 0)
                {
                    indexCoords = new Coordinate3(coords.x, coords.y, coords.z - 1);
                }
                _mapData.GetCell(indexCoords).panelButtonIndex = 0;
            }
            
            UpdateButtonsListBox();
        }
        private void UpdateButtonsListBox()
        {
            for (int i = 0; i < _mapData.buttons.Length; i++)
            {
                string s = "";
                HGButton button = _mapData.buttons[i];
                HGCell cell = _mapData.GetCell(button.coords);
                if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cell.floorHere && cell.floorType == HGCellFloorType.Button))
                {
                    s = $"Button at ({button.coords}), pushed: {ButtonActionPushedDropDownList.Items[HelperUtil.Clamp<int>((int)button.actionPushed,0, ButtonActionPushedDropDownList.Items.Count - 1)]}, released: {ButtonActionReleasedDropDownList.Items[HelperUtil.Clamp<int>((int)button.actionReleased, 0, ButtonActionReleasedDropDownList.Items.Count - 1)]}";
                }
                ButtonsListBox.Items[button.index] = $"{button.index.ToString().PadLeft(2, '0')} {s}".PadRight(listViewStringLength);
            }
        }
        private void PickCell(Coordinate3 coords, bool setBlockHandlerKeep = true)
        {
            if(_mapData == null)
            {
                return;
            }
            if(setBlockHandlerKeep)
            {
                SetBlockHandlerKeep();
            }
            HGCell cell = _mapData.GetCell(coords);
            //BlockTypeDropDownList.SelectedIndex = cell.blockHere ? HelperUtil.Clamp<int>((int)cell.blockType / 8 + 1 - ((int)cell.blockType >= 128 ? 8 : 0), 0, BlockTypeDropDownList.Items.Count - 1) : 0;
            BlockTypeDropDownList.SelectedIndex = cell.blockHere ? HelperUtil.Clamp<int>(GetIndexFromBlockType(cell.blockType), 0, BlockTypeDropDownList.Items.Count - 1) : 0;
            BlockPushableCheckBox.CheckState = cell.pushable ? CheckState.Checked : CheckState.Unchecked;
            BlockInvisibleCheckBox.CheckState = cell.invisible ? CheckState.Checked : CheckState.Unchecked;
            BlockOpaqueCheckBox.CheckState = cell.opaque ? CheckState.Checked : CheckState.Unchecked;
            
            TeleportTargetPosLabel.Enabled = (cell.blockHere && cell.blockType == HGCellBlockType.Teleport);
            TeleportTargetPosXTextBox.Enabled = TeleportTargetPosLabel.Enabled;
            TeleportTargetPosYTextBox.Enabled = TeleportTargetPosLabel.Enabled;
            TeleportTargetPosZTextBox.Enabled = TeleportTargetPosLabel.Enabled;
            TeleportTargetPosXTextBox.Text = TeleportTargetPosLabel.Enabled ? cell.teleportTargetPos.x.ToString() : "0";
            TeleportTargetPosYTextBox.Text = TeleportTargetPosLabel.Enabled ? cell.teleportTargetPos.y.ToString() : "0";
            TeleportTargetPosZTextBox.Text = TeleportTargetPosLabel.Enabled ? cell.teleportTargetPos.z.ToString() : "0";

            FitnessBoostValueLabel.Enabled = (cell.blockHere && cell.blockType == HGCellBlockType.FitnessBoost);
            FitnessBoostValueTextBox.Enabled = FitnessBoostValueLabel.Enabled;
            FitnessBoostValueTextBox.Text = FitnessBoostValueLabel.Enabled ? _mapData.ConvertFitnessBoostValue(cell.healingValue,false).ToString() : "0";

            ForceQuestionMarkCheckBox.CheckState = cell.forceQuestionMark ? CheckState.Checked : CheckState.Unchecked;

            FloorTypeDropDownList.Items[4] = (cell.blockHere && cell.blockType == HGCellBlockType.Stone) ? "Light" : "Puddle";
            //FloorTypeDropDownList.SelectedIndex = cell.floorHere ? HelperUtil.Clamp<int>((int)cell.floorType / 2 + 1, 0, FloorTypeDropDownList.Items.Count - 1) : 0;
            FloorTypeDropDownList.SelectedIndex = cell.floorHere ? HelperUtil.Clamp<int>(GetIndexFromFloorType(cell.floorType), 0, FloorTypeDropDownList.Items.Count - 1) : 0;
            

            //PanelTypeDropDownList.SelectedIndex = cell.panelHere ? HelperUtil.Clamp<int>((int)cell.panelType / 8 + 1, 0, PanelTypeDropDownList.Items.Count - 1) : 0;
            PanelTypeDropDownList.SelectedIndex = cell.panelHere ? HelperUtil.Clamp<int>(GetIndexFromPanelType(cell.panelType), 0, PanelTypeDropDownList.Items.Count - 1) : 0;

            WaterDropDownList.SelectedIndex = cell.waterHere ? HelperUtil.Clamp<int>(cell.waterLevel + 1, 0, WaterDropDownList.Items.Count - 1) : 0;
            TidalWaterFlowingCheckBox.CheckState = cell.tidalWaterFlowing ? CheckState.Checked : CheckState.Unchecked;

            //AuxiliaryDropDownList.SelectedIndex = cell.auxiliaryHere ? HelperUtil.Clamp<int>((int)cell.auxiliaryType / 16 + 1 - ((int)cell.auxiliaryType >= 128 ? 1 : 0), 0, BlockTypeDropDownList.Items.Count - 1) : 0;
            AuxiliaryDropDownList.SelectedIndex = cell.auxiliaryHere ? HelperUtil.Clamp<int>(GetIndexFromAuxiliaryType(cell.auxiliaryType), 0, BlockTypeDropDownList.Items.Count - 1) : 0;

            EggDelayTextBox.Text = cell.eggDelay.ToString();
            EggDelayDropDownList.SelectedIndex = GetEggDelayIndexFromEggdelay(cell.eggDelay);
            EggBlockMonsterCheckBox.CheckState = cell.eggBlocked ? CheckState.Checked : CheckState.Unchecked;
            EggEraseCheckBox.CheckState = cell.eggErase ? CheckState.Checked : CheckState.Unchecked;

            SkyNCheckBox.CheckState = cell.skyRendered[0] ? CheckState.Checked : CheckState.Unchecked;
            SkyECheckBox.CheckState = cell.skyRendered[1] ? CheckState.Checked : CheckState.Unchecked;
            SkySCheckBox.CheckState = cell.skyRendered[2] ? CheckState.Checked : CheckState.Unchecked;
            SkyWCheckBox.CheckState = cell.skyRendered[3] ? CheckState.Checked : CheckState.Unchecked;

            ItemIDTextBox.Text = cell.itemID.ToString();
            ItemDropDownList.SelectedIndex = HelperUtil.Clamp<int>(cell.itemID, 0, ItemDropDownList.Items.Count - 1);
            ItemDamagedTextBox.Text = cell.itemDamaged.ToString();
            ItemAmmunitionTextBox.Text = cell.itemAmmo.ToString();

            if (cell.auxiliaryHere && cell.auxiliaryType == HGCellAuxiliaryType.Monster)
            {
                SelectMonster(cell.monsterType, cell.monsterLevel);
            }
            else
            {
                DeselectMonsterAll();
            }
            if (cell.panelHere && cell.panelType == HGCellPanelType.Text)
            {
                SelectPanel(cell.panelButtonIndex);
            }
            else
            {
                DeselectPanelAll();
            }
            HGLift lift = _mapData.GetLift(coords,false);
            LiftsListBox.SelectedIndex = HelperUtil.Clamp<int>(lift.index,0, LiftsListBox.Items.Count - 1);
            LiftIndexTextBox.Text = cell.liftIndex.ToString();
            LiftMaxZTextBox.Text = lift.maxZ.ToString();
            LiftMinZTextBox.Text = lift.minZ.ToString();
            LiftInitialDropDownList.SelectedIndex = HelperUtil.Clamp<int>(lift.initial,0, LiftInitialDropDownList.Items.Count - 1);
            LiftCarryCheckBox.CheckState = lift.carry ? CheckState.Checked : CheckState.Unchecked;
            LiftContinueAtTopCheckBox.CheckState = lift.continueAtTop ? CheckState.Checked : CheckState.Unchecked;
            LiftContinueAtBottomCheckBox.CheckState = lift.continueAtBottom ? CheckState.Checked : CheckState.Unchecked;
            LiftAutomoveTopCheckBox.CheckState = (lift.automove == 2 || lift.automove == 3) ? CheckState.Checked : CheckState.Unchecked;
            LiftAutomoveBottomCheckBox.CheckState = (lift.automove == 1 || lift.automove == 3) ? CheckState.Checked : CheckState.Unchecked;
            if (cell.floorHere && cell.floorType == HGCellFloorType.Lift && lift != null)
            {
                /*
                HGLift lift = _mapData.GetLift(coords);
                LiftIndexTextBox.Text = cell.liftIndex.ToString();
                LiftMaxZTextBox.Text = lift.maxZ.ToString();
                LiftMinZTextBox.Text = lift.minZ.ToString();
                LiftInitialDropDownList.SelectedIndex = lift.initial;
                LiftCarryCheckBox.CheckState = lift.carry ? CheckState.Checked : CheckState.Unchecked;
                LiftContinueAtTopCheckBox.CheckState = lift.continueAtTop ? CheckState.Checked : CheckState.Unchecked;
                LiftContinueAtBottomCheckBox.CheckState = lift.continueAtBottom ? CheckState.Checked : CheckState.Unchecked;
                LiftAutomoveTopCheckBox.CheckState = (lift.automove == 2 || lift.automove == 3) ? CheckState.Checked : CheckState.Unchecked;
                LiftAutomoveBottomCheckBox.CheckState = (lift.automove == 1 || lift.automove == 3) ? CheckState.Checked : CheckState.Unchecked;

                LiftsListBox.SelectedIndex = lift.index;
                */
            }
            else
            {
                /*
                LiftIndexTextBox.Text = "0";
                LiftMaxZTextBox.Text = "0";
                LiftMinZTextBox.Text = "0";
                LiftInitialDropDownList.SelectedIndex = 0;
                LiftCarryCheckBox.CheckState = CheckState.Unchecked;
                LiftContinueAtTopCheckBox.CheckState = CheckState.Unchecked;
                LiftContinueAtBottomCheckBox.CheckState = CheckState.Unchecked;
                LiftAutomoveTopCheckBox.CheckState = CheckState.Unchecked;
                LiftAutomoveBottomCheckBox.CheckState = CheckState.Unchecked;
                */
            }
            HGButton button = _mapData.GetButton(coords, false);
            if (button != null)
            {
                ButtonsListBox.SelectedIndex = HelperUtil.Clamp<int>(button.index, 0, ButtonsListBox.Items.Count - 1);
                ButtonIndexTextBox.Text = button.index.ToString();
                ButtonTargetInPositionXTextBox.Text = button.targetPosPushed.x.ToString();
                ButtonTargetInPositionYTextBox.Text = button.targetPosPushed.y.ToString();
                ButtonTargetInPositionZTextBox.Text = button.targetPosPushed.z.ToString();
                ButtonTargetOutPositionXTextBox.Text = button.targetPosReleased.x.ToString();
                ButtonTargetOutPositionYTextBox.Text = button.targetPosReleased.y.ToString();
                ButtonTargetOutPositionZTextBox.Text = button.targetPosReleased.z.ToString();
                ButtonActionPushedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)button.actionPushed, 0, ButtonActionPushedDropDownList.Items.Count - 1);
                ButtonActionReleasedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)button.actionReleased, 0, ButtonActionReleasedDropDownList.Items.Count - 1);

                ButtonTargetPosBytesTextBox.Text = $"{button.targetPosPushedBytes[0]} {button.targetPosPushedBytes[1]} {button.targetPosPushedBytes[2]} {button.targetPosPushedBytes[3]}";
                ButtonTargetPosFlatTextBox.Text = button.targetPosPushedFlat.ToString();
            }
            /*
            if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cell.floorHere && cell.floorType == HGCellFloorType.Button))
            {
                HGButton button = _mapData.GetButton(coords, false);
                if (button != null)
                {
                    ButtonsListBox.SelectedIndex = HelperUtil.Clamp<int>(button.index, 0, ButtonsListBox.Items.Count - 1);
                    ButtonIndexTextBox.Text = button.index.ToString();
                    ButtonTargetInPositionXTextBox.Text = button.targetPosPushed.x.ToString();
                    ButtonTargetInPositionYTextBox.Text = button.targetPosPushed.y.ToString();
                    ButtonTargetInPositionZTextBox.Text = button.targetPosPushed.z.ToString();
                    ButtonTargetOutPositionXTextBox.Text = button.targetPosReleased.x.ToString();
                    ButtonTargetOutPositionYTextBox.Text = button.targetPosReleased.y.ToString();
                    ButtonTargetOutPositionZTextBox.Text = button.targetPosReleased.z.ToString();
                    ButtonActionPushedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)button.actionPushed,0, ButtonActionPushedDropDownList.Items.Count - 1);
                    ButtonActionReleasedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)button.actionReleased, 0, ButtonActionReleasedDropDownList.Items.Count - 1);

                    ButtonTargetPosBytesTextBox.Text = $"{button.targetPosPushedBytes[0]} {button.targetPosPushedBytes[1]} {button.targetPosPushedBytes[2]} {button.targetPosPushedBytes[3]}";
                    ButtonTargetPosFlatTextBox.Text = button.targetPosPushedFlat.ToString();
                }
            }
            else
            {
                ButtonIndexTextBox.Text = "0";
                ButtonTargetInPositionXTextBox.Text = "0";
                ButtonTargetInPositionYTextBox.Text = "0";
                ButtonTargetInPositionZTextBox.Text = "0";
                ButtonTargetOutPositionXTextBox.Text = "0";
                ButtonTargetOutPositionYTextBox.Text = "0";
                ButtonTargetOutPositionZTextBox.Text = "0";
                ButtonActionPushedDropDownList.SelectedIndex = (int)HGButtonAction.None;
                ButtonActionReleasedDropDownList.SelectedIndex = (int)HGButtonAction.None;

                ButtonTargetPosBytesTextBox.Text = "0";
                ButtonTargetPosFlatTextBox.Text = "0";
            }
            */
            HGDoor door = _mapData.GetDoor(coords, false);
            if(door != null)
            {
                DoorsListBox.SelectedIndex = HelperUtil.Clamp<int>(door.index, 0, DoorsListBox.Items.Count - 1);
                DoorIndexTextBox.Text = door.index.ToString();
                DoorCloseDelayTextBox.Text = door.closeDelay.ToString();
                DoorKeyIDTextBox.Text = door.keyID.ToString();
                DoorLockedCheckBox.CheckState = door.locked ? CheckState.Checked : CheckState.Unchecked;
                DoorButtonOnlyCheckBox.CheckState = door.buttonOnly ? CheckState.Checked : CheckState.Unchecked;
            }

            if (/*cell.blockHere && cell.doorIndex < _mapData.doors.Length && */(cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW))
            {
                /*
                HGDoor door = _mapData.doors[cell.doorIndex];
                if (door != null)
                {
                    DoorIndexTextBox.Text = door.index.ToString();
                    DoorLockedCheckBox.CheckState = door.locked ? CheckState.Checked : CheckState.Unchecked;
                    DoorKeyIDTextBox.Text = door.keyID.ToString();
                    DoorButtonOnlyCheckBox.CheckState = door.buttonOnly ? CheckState.Checked : CheckState.Unchecked;

                    DoorIndexTextBox.Text = door.index.ToString();
                    DoorBytesTextBox.Text = $"{door.posBytes[0]} {door.posBytes[1]}";
                    DoorFlatTextBox.Text = door.posFlat.ToString();
                }
                */
            }
            else
            {
                /*
                DoorIndexTextBox.Text = "-";
                DoorLockedCheckBox.CheckState = CheckState.Unchecked;
                DoorKeyIDTextBox.Text = "-";
                DoorButtonOnlyCheckBox.CheckState = CheckState.Unchecked;

                DoorIndexTextBox.Text = "-";
                DoorBytesTextBox.Text = "-";
                DoorFlatTextBox.Text = "-";
                */
            }

            if(setBlockHandlerKeep)
            {
                SetBlockHandlerKeep(false);
                MapViewPictureBox.Invalidate();
            }
        }
        private void SelectTarget(bool pushed, bool released)
        {
            if(_mapData == null || _coordsSelected.Count == 0)
            {
                PaintToggleButton.Checked = true;
                return;
            }
            Coordinate3 coordsStart = _coordsSelected[0];
            Coordinate3 coordsEnd = _coordsSelected[_coordsSelected.Count - 1];
            coordsStart.ClampCoords();
            coordsEnd.ClampCoords();
            SetBlockHandlerKeep();
            for (int z = coordsStart.z; z <= coordsEnd.z; z++)
            {
                for (int y = coordsStart.y; y <= coordsEnd.y; y++)
                {
                    for (int x = coordsStart.x; x <= coordsEnd.x; x++)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        HGCell cell = _mapData.GetCell(coords);
                        if (cell.blockHere && cell.blockType == HGCellBlockType.Teleport)
                        {
                            SetTeleportTargetPosX(coords, _coordsCursor.x, setControl: true, setCell: true);
                            SetTeleportTargetPosY(coords, _coordsCursor.y, setControl: true, setCell: true);
                            SetTeleportTargetPosZ(coords, _coordsCursor.z, setControl: true, setCell: true);
                        }
                        else if (pushed || released)
                        {
                            if ((cell.floorHere && cell.floorType == HGCellFloorType.Button) || (cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)))
                            {
                                if (pushed)
                                {
                                    SetButtonTargetInPositionX(coords, _coordsCursor.x, setControl: true, setCell: true);
                                    SetButtonTargetInPositionY(coords, _coordsCursor.y, setControl: true, setCell: true);
                                    SetButtonTargetInPositionZ(coords, _coordsCursor.z, setControl: true, setCell: true);
                                }
                                if (released)
                                {
                                    SetButtonTargetOutPositionX(coords, _coordsCursor.x, setControl: true, setCell: true);
                                    SetButtonTargetOutPositionY(coords, _coordsCursor.y, setControl: true, setCell: true);
                                    SetButtonTargetOutPositionZ(coords, _coordsCursor.z, setControl: true, setCell: true);
                                }
                            }
                        }
                    }
                }
            }
            SetBlockHandlerKeep(false);
            //UpdateMapView();
            MapViewPictureBox.Invalidate();
            UpdateDebugTextBox();
            AddStep();
        }
        private bool CheckSetCellType(SetCellPropertyType t, bool includeAll = true)
        {
            SetCellPropertyType tList = (SetCellPropertyType)SetCellPropertyDropdownList.SelectedIndex;
            if (t == SetCellPropertyType.Auxiliary)
            {
                return (includeAll && tList == SetCellPropertyType.All) || tList == t || tList == SetCellPropertyType.Item;
            }
            if (t == SetCellPropertyType.Egg)
            {
                return (includeAll && tList == SetCellPropertyType.All) || tList == t || tList == SetCellPropertyType.Auxiliary;
            }
            if (t == SetCellPropertyType.Item)
            {
                return (includeAll && tList == SetCellPropertyType.All) || tList == t || tList == SetCellPropertyType.Auxiliary;
            }
            return (includeAll && tList == SetCellPropertyType.All) || tList == t;
        }
        private void PaintCell(Coordinate3 coords)
        {
            PaintCell(coords, coords);
        }
        private void PaintCell(Coordinate3 coordsStart, Coordinate3 coordsEnd)
        {
            if(_mapData == null)
            {
                return;
            }
            coordsStart.ClampCoords();
            coordsEnd.ClampCoords();
            SetBlockHandlerKeep(true);
            for(int z = coordsStart.z; z <= coordsEnd.z; z++)
            {
                for (int y = coordsStart.y; y <= coordsEnd.y; y++)
                {
                    for (int x = coordsStart.x; x <= coordsEnd.x; x++)
                    {
                        Coordinate3 coords = new Coordinate3(x,y,z);
                        if (CheckSetCellType(SetCellPropertyType.Block))
                        {
                            SetBlockType(coords, GetBlockTypeFromIndex(BlockTypeDropDownList.SelectedIndex), BlockTypeDropDownList.SelectedIndex != 0 && BlockTypeDropDownList.SelectedIndex != BlockTypeDropDownList.Items.Count - 1, setControl: false, setCell: true);
                            SetPushable(coords, BlockPushableCheckBox.Checked, setControl: false, setCell: true);
                            SetOpaque(coords, BlockOpaqueCheckBox.Checked, setControl: false, setCell: true);
                            SetInvisible(coords, BlockInvisibleCheckBox.Checked, setControl: false, setCell: true);
                            int ttpX = int.TryParse(TeleportTargetPosXTextBox.Text, out ttpX) ? ttpX : 0;
                            int ttpY = int.TryParse(TeleportTargetPosXTextBox.Text, out ttpY) ? ttpY : 0;
                            int ttpZ = int.TryParse(TeleportTargetPosXTextBox.Text, out ttpZ) ? ttpZ : 0;
                            SetTeleportTargetPosX(coords, ttpX, setControl: false, setCell: true);
                            SetTeleportTargetPosY(coords, ttpY, setControl: false, setCell: true);
                            SetTeleportTargetPosZ(coords, ttpZ, setControl: false, setCell: true);
                            int fbv = int.TryParse(FitnessBoostValueTextBox.Text, out fbv) ? fbv : 0;
                            SetFitnessBoostValue(coords, fbv, setControl: false, setCell: true);
                            SetForceQuestionMark(coords, ForceQuestionMarkCheckBox.Checked, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Floor)) SetFloorType(coords, GetFloorTypeFromIndex(FloorTypeDropDownList.SelectedIndex), FloorTypeDropDownList.SelectedIndex != 0 && FloorTypeDropDownList.SelectedIndex != FloorTypeDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Panel)) SetPanelType(coords, GetPanelTypeFromIndex(PanelTypeDropDownList.SelectedIndex), PanelTypeDropDownList.SelectedIndex != 0 && PanelTypeDropDownList.SelectedIndex != PanelTypeDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Water))
                        {
                            SetWaterLevel(coords, GetWaterLevelFromIndex(WaterDropDownList.SelectedIndex), WaterDropDownList.SelectedIndex != 0 && WaterDropDownList.SelectedIndex != WaterDropDownList.Items.Count - 1, setControl: false, setCell: true);
                            SetTidalWaterFlowing(coords, TidalWaterFlowingCheckBox.Checked, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Auxiliary)) SetAuxiliaryType(coords, GetAuxiliaryTypeFromIndex(AuxiliaryDropDownList.SelectedIndex), AuxiliaryDropDownList.SelectedIndex != 0 && AuxiliaryDropDownList.SelectedIndex != AuxiliaryDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Egg))
                        {
                            SetEggDelay(coords, int.Parse(EggDelayTextBox.Text), setTextBoxControl: false, setDropdownListControl : false, setCell: true);
                            SetEggBlockMonster(coords, EggBlockMonsterCheckBox.Checked, setControl: false, setCell: true);
                            SetEggErase(coords, EggEraseCheckBox.Checked, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Sky))
                        {
                            SetSky(coords, 0, SkyNCheckBox.Checked, setControl: false, setCell: true);
                            SetSky(coords, 1, SkyECheckBox.Checked, setControl: false, setCell: true);
                            SetSky(coords, 2, SkySCheckBox.Checked, setControl: false, setCell: true);
                            SetSky(coords, 3, SkyWCheckBox.Checked, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Item))
                        {
                            SetItemID(coords, int.Parse(ItemIDTextBox.Text), setListControl: false, setTextBoxControl: false, setCell: true);
                            SetItemDamage(coords, int.Parse(ItemDamagedTextBox.Text), setControl: false, setCell: true);
                            SetItemAmmo(coords, int.Parse(ItemAmmunitionTextBox.Text), setControl: false, setCell: true);
                        }

                        HGCell cell = _mapData.GetCell(coords);
                        if (AutoStairsToggleButton.Checked && CheckSetCellType(SetCellPropertyType.Block))
                        {
                            if (cell.blockType == HGCellBlockType.StairsN && coords.y < HGMapData.MAP_HEIGHT - 1)
                            {
                                Coordinate3 coordsS = new Coordinate3(x, y + 1, z);
                                HGCell cellS = _mapData.GetCell(coordsS);
                                int waterLevel = cellS.waterLevel;
                                bool tidalWaterFlowing = cellS.tidalWaterFlowing;
                                bool waterHere = cellS.waterHere;
                                ClearCell(coordsS);
                                SetAuxiliaryType(coordsS, HGCellAuxiliaryType.StairsN, true, setControl: false, setCell: true);
                                SetWaterLevel(coordsS, waterLevel, waterHere, setControl: false, setCell: true);
                                SetTidalWaterFlowing(coordsS, tidalWaterFlowing, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsE && coords.x > 0)
                            {
                                Coordinate3 coordsW = new Coordinate3(x - 1, y, z);
                                HGCell cellW = _mapData.GetCell(coordsW);
                                int waterLevel = cellW.waterLevel;
                                bool tidalWaterFlowing = cellW.tidalWaterFlowing;
                                bool waterHere = cellW.waterHere;
                                ClearCell(coordsW);
                                SetAuxiliaryType(coordsW, HGCellAuxiliaryType.StairsE, true, setControl: false, setCell: true);
                                SetWaterLevel(coordsW, waterLevel, waterHere, setControl: false, setCell: true);
                                SetTidalWaterFlowing(coordsW, tidalWaterFlowing, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsS && coords.y > 0)
                            {
                                Coordinate3 coordsN = new Coordinate3(x, y - 1, z);
                                HGCell cellN = _mapData.GetCell(coordsN);
                                int waterLevel = cellN.waterLevel;
                                bool tidalWaterFlowing = cellN.tidalWaterFlowing;
                                bool waterHere = cellN.waterHere;
                                ClearCell(coordsN);
                                SetAuxiliaryType(coordsN, HGCellAuxiliaryType.StairsS, true, setControl: false, setCell: true);
                                SetWaterLevel(coordsN, waterLevel, waterHere, setControl: false, setCell: true);
                                SetTidalWaterFlowing(coordsN, tidalWaterFlowing, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsW && coords.x < HGMapData.MAP_WIDTH - 1)
                            {
                                Coordinate3 coordsE = new Coordinate3(x + 1, y, z);
                                HGCell cellE = _mapData.GetCell(coordsE);
                                int waterLevel = cellE.waterLevel;
                                bool tidalWaterFlowing = cellE.tidalWaterFlowing;
                                bool waterHere = cellE.waterHere;
                                ClearCell(coordsE);
                                SetAuxiliaryType(coordsE, HGCellAuxiliaryType.StairsW, true, setControl: false, setCell: true);
                                SetWaterLevel(coordsE, waterLevel, waterHere, setControl: false, setCell: true);
                                SetTidalWaterFlowing(coordsE, tidalWaterFlowing, setControl: false, setCell: true);
                            }
                        }
                        if (CheckSetCellType(SetCellPropertyType.Block))
                        {
                            if (cell.blockType == HGCellBlockType.DoorNS)
                            {
                                SetAuxiliaryType(coords, HGCellAuxiliaryType.DoorNS, true, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.DoorEW)
                            {
                                SetAuxiliaryType(coords, HGCellAuxiliaryType.DoorEW, true, setControl: false, setCell: true);
                            }
                        }
                        /*
                        SetLiftInitial(coords, String.IsNullOrEmpty(LiftIndexTextBox.Text) ? 0 : int.Parse(LiftIndexTextBox.Text), setControl: false, setCell: true);
                        SetLiftMaxZ(coords, String.IsNullOrEmpty(LiftMaxZTextBox.Text) ? 0 : int.Parse(LiftMaxZTextBox.Text), setControl: false, setCell: true);
                        SetLiftMinZ(coords, String.IsNullOrEmpty(LiftMinZTextBox.Text) ? 0 : int.Parse(LiftMinZTextBox.Text), setControl: false, setCell: true);
                        SetLiftCarry(coords, LiftCarryCheckBox.Checked, setControl: false, setCell: true);
                        SetLiftContinueAtTop(coords, LiftContinueAtTopCheckBox.Checked, setControl: false, setCell: true);
                        SetLiftContinueAtBottom(coords, LiftContinueAtBottomCheckBox.Checked, setControl: false, setCell: true);
                        SetLiftAutomoveTop(coords, LiftAutomoveTopCheckBox.Checked, setControl: false, setCell: true);
                        SetLiftAutomoveBottom(coords, LiftAutomoveBottomCheckBox.Checked, setControl: false, setCell: true);

                        SetDoorCloseDelay(coords, String.IsNullOrEmpty(DoorCloseDelayTextBox.Text) ? 0 : int.Parse(DoorCloseDelayTextBox.Text), setControl: false, setCell: true);
                        SetDoorKeyID(coords, String.IsNullOrEmpty(DoorKeyIDTextBox.Text) ? 0 : int.Parse(DoorKeyIDTextBox.Text), setControl: false, setCell: true);
                        SetDoorLocked(coords, DoorLockedCheckBox.Checked, setControl: false, setCell: true);
                        SetDoorButtonOnly(coords, DoorButtonOnlyCheckBox.Checked, setControl: false, setCell: true);

                        int buttonTargetInPosX = int.TryParse(ButtonTargetInPositionXTextBox.Text, out buttonTargetInPosX) ? buttonTargetInPosX : 0;
                        int buttonTargetInPosY = int.TryParse(ButtonTargetInPositionYTextBox.Text, out buttonTargetInPosY) ? buttonTargetInPosY : 0;
                        int buttonTargetInPosZ = int.TryParse(ButtonTargetInPositionZTextBox.Text, out buttonTargetInPosZ) ? buttonTargetInPosZ : 0;
                        int buttonTargetOutPosX = int.TryParse(ButtonTargetOutPositionXTextBox.Text, out buttonTargetOutPosX) ? buttonTargetOutPosX : 0;
                        int buttonTargetOutPosY = int.TryParse(ButtonTargetOutPositionYTextBox.Text, out buttonTargetOutPosY) ? buttonTargetOutPosY : 0;
                        int buttonTargetOutPosZ = int.TryParse(ButtonTargetOutPositionZTextBox.Text, out buttonTargetOutPosZ) ? buttonTargetOutPosZ : 0;
                        SetButtonTargetInPositionX(coords, String.IsNullOrEmpty(ButtonTargetInPositionXTextBox.Text) ? 0 : buttonTargetInPosX, setControl: false, setCell: true);
                        SetButtonTargetInPositionY(coords, String.IsNullOrEmpty(ButtonTargetInPositionYTextBox.Text) ? 0 : buttonTargetInPosY, setControl: false, setCell: true);
                        SetButtonTargetInPositionZ(coords, String.IsNullOrEmpty(ButtonTargetInPositionZTextBox.Text) ? 0 : buttonTargetInPosZ, setControl: false, setCell: true);
                        SetButtonTargetOutPositionX(coords, String.IsNullOrEmpty(ButtonTargetOutPositionXTextBox.Text) ? 0 : buttonTargetOutPosX, setControl: false, setCell: true);
                        SetButtonTargetOutPositionY(coords, String.IsNullOrEmpty(ButtonTargetOutPositionYTextBox.Text) ? 0 : buttonTargetOutPosY, setControl: false, setCell: true);
                        SetButtonTargetOutPositionZ(coords, String.IsNullOrEmpty(ButtonTargetOutPositionZTextBox.Text) ? 0 : buttonTargetOutPosZ, setControl: false, setCell: true);
                        SetButtonActionPushed(coords, (HGButtonAction)HelperUtil.Clamp<int>(ButtonActionPushedDropDownList.SelectedIndex,0, ButtonActionPushedDropDownList.Items.Count - 1), setControl: false, setCell: true);
                        SetButtonActionReleased(coords, (HGButtonAction)HelperUtil.Clamp<int>(ButtonActionReleasedDropDownList.SelectedIndex, 0, ButtonActionReleasedDropDownList.Items.Count - 1), setControl: false, setCell: true);
                        */
                    }
                }
            }
            SetBlockHandlerKeep(false);
            _mapData.UpdateMapTiles(coordsStart, coordsEnd);
            UpdateMapView(coordsStart, coordsEnd);
            UpdateDebugTextBox();
            AddStep();
        }
        private void ClearCell(Coordinate3 coords)
        {
            ClearCell(coords, coords);
        }
        private void ClearCell(Coordinate3 coordsStart, Coordinate3 coordsEnd)
        {
            if (_mapData == null)
            {
                return;
            }
            SetBlockHandlerKeep(true);
            coordsStart.ClampCoords();
            coordsEnd.ClampCoords();
            for (int z = coordsStart.z; z <= coordsEnd.z; z++)
            {
                for (int y = coordsStart.y; y <= coordsEnd.y; y++)
                {
                    for (int x = coordsStart.x; x <= coordsEnd.x; x++)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        HGCell cell = _mapData.GetCell(coords);
                        /*
                        if (CheckSetCellType(SetCellPropertyType.Floor) && cell.floorType == HGCellFloorType.Lift)
                        {
                            cell.liftIndex = 0;
                        }
                        if ((CheckSetCellType(SetCellPropertyType.Floor) && cell.floorType == HGCellFloorType.Button) || (CheckSetCellType(SetCellPropertyType.Panel) && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)))
                        {
                            cell.panelButtonIndex = 0;
                        }
                        if (CheckSetCellType(SetCellPropertyType.Block) && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW)
                        {
                            cell.doorIndex = 0;
                        }
                        */

                        if (CheckSetCellType(SetCellPropertyType.Block))
                        {
                            SetBlockType(coords, HGCellBlockType.Stone, false, setControl: false, setCell: true);
                            SetPushable(coords, false, setControl: false, setCell: true);
                            SetOpaque(coords, false, setControl: false, setCell: true);
                            SetInvisible(coords, false, setControl: false, setCell: true);
                            SetTeleportTargetPosX(coords, 0, setControl: false, setCell: true);
                            SetTeleportTargetPosY(coords, 0, setControl: false, setCell: true);
                            SetTeleportTargetPosZ(coords, 0, setControl: false, setCell: true);
                            SetFitnessBoostValue(coords, 0, setControl: false, setCell: true);
                            SetForceQuestionMark(coords, false, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Floor)) SetFloorType(coords, HGCellFloorType.Grass, false, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Panel)) SetPanelType(coords, HGCellPanelType.Text, false, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Water))
                        {
                            SetWaterLevel(coords, 0, false, setControl: false, setCell: true);
                            SetTidalWaterFlowing(coords, false, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Auxiliary)) SetAuxiliaryType(coords, HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Egg))
                        {
                            SetEggDelay(coords, 0, setTextBoxControl: false, setDropdownListControl : false, setCell: true);
                            SetEggBlockMonster(coords, false, setControl: false, setCell: true);
                            SetEggErase(coords, false, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Sky))
                        {
                            SetSky(coords, 0, false, setControl: false, setCell: true);
                            SetSky(coords, 1, false, setControl: false, setCell: true);
                            SetSky(coords, 2, false, setControl: false, setCell: true);
                            SetSky(coords, 3, false, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Item))
                        {

                            SetItemID(coords, 0, setListControl: false, setTextBoxControl: false, setCell: true);
                            SetItemDamage(coords, 0, setControl: false, setCell: true);
                            SetItemAmmo(coords, 0, setControl: false, setCell: true);
                        }
                        
                        if (AutoStairsToggleButton.Checked && CheckSetCellType(SetCellPropertyType.Block))
                        {
                            if (cell.blockType == HGCellBlockType.StairsN && coords.y < HGMapData.MAP_HEIGHT - 1)
                            {
                                SetAuxiliaryType(new Coordinate3(x, y + 1, z), HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsE && coords.x > 0)
                            {
                                SetAuxiliaryType(new Coordinate3(x - 1, y, z), HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsS && coords.y > 0)
                            {
                                SetAuxiliaryType(new Coordinate3(x, y - 1, z), HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                            }
                            else if (cell.blockType == HGCellBlockType.StairsW && coords.x < HGMapData.MAP_WIDTH - 1)
                            {
                                SetAuxiliaryType(new Coordinate3(x + 1, y, z), HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                            }
                        }
                        if (CheckSetCellType(SetCellPropertyType.Block))
                        {
                            if ((cell.blockType == HGCellBlockType.DoorNS && cell.auxiliaryType == HGCellAuxiliaryType.DoorNS) || (cell.blockType == HGCellBlockType.DoorEW && cell.auxiliaryType == HGCellAuxiliaryType.DoorEW))
                            {
                                SetAuxiliaryType(coords, HGCellAuxiliaryType.Monster, false, setControl: false, setCell: true);
                            }
                           }
                        /*
                        SetLiftInitial(coords, 0, setControl: false, setCell: true);
                        SetLiftMaxZ(coords, 0, setControl: false, setCell: true);
                        SetLiftMinZ(coords, 0, setControl: false, setCell: true);
                        SetLiftCarry(coords, false, setControl: false, setCell: true);
                        SetLiftContinueAtTop(coords, false, setControl: false, setCell: true);
                        SetLiftContinueAtBottom(coords, false, setControl: false, setCell: true);
                        SetLiftAutomoveTop(coords, false, setControl: false, setCell: true);
                        SetLiftAutomoveBottom(coords, false, setControl: false, setCell: true);

                        SetDoorCloseDelay(coords, 0, setControl: false, setCell: true);
                        SetDoorKeyID(coords, 0, setControl: false, setCell: true);
                        SetDoorLocked(coords, false, setCell: true);
                        SetDoorButtonOnly(coords, false, setCell: true);

                        SetButtonTargetInPositionX(coords, 0, setControl: false, setCell: true);
                        SetButtonTargetInPositionY(coords, 0, setControl: false, setCell: true);
                        SetButtonTargetInPositionZ(coords, 0, setControl: false, setCell: true);
                        SetButtonTargetOutPositionX(coords, 0, setControl: false, setCell: true);
                        SetButtonTargetOutPositionY(coords, 0, setControl: false, setCell: true);
                        SetButtonTargetOutPositionZ(coords, 0, setControl: false, setCell: true);
                        SetButtonActionPushed(coords, HGButtonAction.None, setControl: false, setCell: true);
                        SetButtonActionReleased(coords, HGButtonAction.None, setControl: false, setCell: true);
                        if(cell.floorType == HGCellFloorType.Button && z > 0)
                        {
                            _mapData.GetCell(new Coordinate3(x,y,z - 1)).panelButtonIndex = 0;
                        }
                        */
                    }
                }
            }
            SetBlockHandlerKeep(false);
            _mapData.UpdateMapTiles(coordsStart, coordsEnd);
            UpdateMapView(coordsStart, coordsEnd);
            UpdateDebugTextBox();
            AddStep();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: MENU STRIP
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CompressFile(string path)
        {
            //RNC ProPack release by Vladimir Kononovich (lab313ru) via https://github.com/lab313ru/rnc_propack_source
            //original RNC Packer by Rob Northen
            /*
            if (File.Exists("rnc.exe") == false)
            {
                AddConsoleMessage($"[CompressFile] {Directory.GetCurrentDirectory()}\\rnc.exe does not exist!", warning: true);
                return;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "rnc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Path.GetFileName(path);
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                AddConsoleMessage($"[CompressFile] Error while executing rnc.exe", warning: true);
            }
            */
            if (File.Exists("rnc_propack_x64.exe") == false)
            {
                AddConsoleMessage($"[CompressFile] {Directory.GetCurrentDirectory()}\\rnc_propack_x64.exe does not exist!", warning: true);
                return;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Environment.Is64BitOperatingSystem ? "rnc_propack_x64.exe" : "rnc_propack_x86.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = $"p {Path.GetFileName(path)} {Path.GetFileName(path)}";
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                AddConsoleMessage($"[CompressFile] Error while executing rnc_propack_x64.exe", warning: true);
            }
        }
        public void DecompressFile(string path)
        {
            //RNC ProPack release by Vladimir Kononovich (lab313ru) via https://github.com/lab313ru/rnc_propack_source
            //original RNC Packer by Rob Northen
            /*
            if (File.Exists("rnc.exe") == false)
            {
                AddConsoleMessage($"[DecompressFile] {Directory.GetCurrentDirectory()}\\rnc.exe does not exist!", warning: true);
                return;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "dernc.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = Path.GetFileName(path);
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                AddConsoleMessage($"[DecompressFile] Error while executing dernc.exe", warning: true);
            }
            */
            if (File.Exists("rnc_propack_x64.exe") == false)
            {
                AddConsoleMessage($"[DecompressFile] {Directory.GetCurrentDirectory()}\\rnc_propack_x64.exe does not exist!", warning: true);
                return;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = Environment.Is64BitOperatingSystem ? "rnc_propack_x64.exe" : "rnc_propack_x86.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = $"u {Path.GetFileName(path)} {Path.GetFileName(path)}";
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                AddConsoleMessage($"[DecompressFile] Error while executing rnc_propack_x64.exe", warning: true);
            }
        }
        public string GetMapDir()
        {
            return $"{gameDir}\\Maps";
        }
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save file check
            LoadDefaultMap();
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = MapFileName == _untitledMapName ? GetMapDir() : Path.GetDirectoryName(MapFileName);
            openFileDialog.Filter = "map files (*.map)|*.map|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream inStream;
                if ((inStream = openFileDialog.OpenFile()) != null)
                {
                    mruMenu.AddFile(openFileDialog.FileName);
                    LoadMap(openFileDialog.FileName);
                }
                inStream.Close();
            }
            mruMenu.SaveToRegistry();
        }
        private void LoadDefaultMap()
        {
            LoadMap(_defaultMapPath);
            MapFileName = _untitledMapName;
        }
        private void LoadMap(string path)
        {
            ClearConsoleMessages();
            if (string.IsNullOrEmpty(path) || File.Exists(path) == false)
            {
                MessageBox.Show($"{path} does not exist!");
                return;
            }
            if (MapChanged)
            {
                DialogResult res = MessageBox.Show("Save changes to the map before closing?", _programName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    SaveMap(asNew: false, loadAfterSaving: true, mapToLoadAfterSaving: path);
                    return;
                }
            }
            byte[] bytes = new byte[0];
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch
            {
                MessageBox.Show($"Couldn't read {path}!");
                return;
            }
            if (bytes.Length < HGMapData.MAP_BYTES)//must be RNC compressed
            {
                string initialDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory($"{editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                if (File.Exists("rnc_propack_x64.exe") == false)
                {
                    AddConsoleMessage($"[LoadMap] {Directory.GetCurrentDirectory()}\\rnc_propack_x64.exe does not exist!", warning: true);
                    Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                    return;
                }
                //Create a temporary copy of the compressed map file that gets decompressed in the rnc directory
                string tempFile = $"{Directory.GetCurrentDirectory()}\\{Path.GetFileName(path)}";
                File.Copy(path, tempFile);
                DecompressFile(Path.GetFileName(tempFile));
                try
                {
                    bytes = File.ReadAllBytes(tempFile);
                }
                catch
                {
                    MessageBox.Show($"[LoadMap] Couldn't read compressed map file!");
                    Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                    return;
                }
                File.Delete(Path.GetFileName(tempFile));//delete the temporary file in the rnc directory
                Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
            }
            MapFileName = path;
            LoadMap(HGMapData.LoadMap(bytes));
            MapChanged = false;
        }
        private void LoadMap(HGMapData mapData, bool fromState = false)
        {
            if (_mapData == null)
            {
                return;
            }
            _mapData = mapData;
            SetBlockHandlerKeep();
            WorldmapDescriptionTextBox.Text = _mapData.worldmapDescription;
            DetailedDescriptionTextBox.Text = _mapData.detailedDescription;
            IntroMessageTextBox.Text = _mapData.introMessage;

            Player1StartXTextBox.Text = _mapData.playerStart1.x.ToString();
            Player1StartYTextBox.Text = _mapData.playerStart1.y.ToString();
            Player1StartZTextBox.Text = _mapData.playerStart1.z.ToString();

            Player2StartXTextBox.Text = _mapData.playerStart2.x.ToString();
            Player2StartYTextBox.Text = _mapData.playerStart2.y.ToString();
            Player2StartZTextBox.Text = _mapData.playerStart2.z.ToString();

            Player3StartXTextBox.Text = _mapData.playerStart3.x.ToString();
            Player3StartYTextBox.Text = _mapData.playerStart3.y.ToString();
            Player3StartZTextBox.Text = _mapData.playerStart3.z.ToString();

            Player4StartXTextBox.Text = _mapData.playerStart4.x.ToString();
            Player4StartYTextBox.Text = _mapData.playerStart4.y.ToString();
            Player4StartZTextBox.Text = _mapData.playerStart4.z.ToString();

            Destination1TextBox.Text = _mapData.destination1.ToString();
            Destination2TextBox.Text = _mapData.destination2.ToString();
            Destination3TextBox.Text = _mapData.destination3.ToString();
            Destination4TextBox.Text = _mapData.destination4.ToString();

            TidalWaterSpeedTextBox.Text = _mapData.waterSpeed.ToString();
            TidalWaterMinHeightTextBox.Text = _mapData.waterLevelLow.ToString();
            TidalWaterMaxHeightTextBox.Text = _mapData.waterLevelHigh.ToString();

            StyleDropDownList.SelectedIndex = _mapData.style;
            GameTypeDropDownList.SelectedIndex = _mapData.gameType;
            MusicDropDownList.SelectedIndex = _mapData.music;
            AtmosDropDownList.SelectedIndex = _mapData.atmos;
            SkyDropDownList.SelectedIndex = _mapData.sky;

            TimeLimitTextBox.Text = _mapData.timeLimit.ToString();
            DiskTextBox.Text = _mapData.disk.ToString();
            FileTextBox.Text = _mapData.file.ToString();
            PictureTextBox.Text = _mapData.picture.ToString();
            PlayersTextBox.Text = _mapData.players.ToString();
            WorldPosXTextBox.Text = _mapData.worldMapPosX.ToString();
            WorldPosYTextBox.Text = _mapData.worldMapPosY.ToString();

            UnloadMonsterAll(false);
            if (_mapData.monsterType1 > 0)
            {
                LoadMonster(_mapData.monsterType1 - 1, false);
                _monsterButtons[_mapData.monsterType1 - 1].loadedType = 1;
            }
            if (_mapData.monsterType2 > 0)
            {
                LoadMonster(_mapData.monsterType2 - 1, false);
                _monsterButtons[_mapData.monsterType2 - 1].loadedType = 2;
            }
            UpdatePanels();
            SetBlockHandlerKeep(false);
            if (_pasting)
            {
                _pasting = false;
                PaintToggleButton.Checked = true;
            }
            UpdateLiftsListBox();
            UpdateDoorsListBox();
            UpdateButtonsListBox();
            UpdateTidalWaterHeightLabel();
            if (fromState == false)
            {
                ZLevel = _mapData.playerStart1.z;
            }
            SetCoordsSelected(_coordsCursor);
            UpdateMapView();
            if(fromState == false && _stateManager != null)
            {
                _stateManager.ResetStates();
                AddStep();
            }
            if (fromState == false)
            {
                if (string.IsNullOrEmpty(MapFileName) || Path.GetFileNameWithoutExtension(MapFileName) == _untitledMapName)
                {
                    AddConsoleMessage($"Started new map.");
                }
                else
                {
                    AddConsoleMessage($"[LoadMap] Loaded map: {MapFileName}");
                }
            }
        }
        private int CheckOriginalMapfile(string path)//0 = no, 1 = yes, 2 = cancel
        {
            if (path == _untitledMapName)
            {
                return 1;
            }
            if(File.Exists(path) == false)
            {
                return 1;
            }
            DateTime fileDate = new FileInfo(path).LastWriteTimeUtc;
            if (fileDate < new DateTime(1993, 12, 31))
            {
                DialogResult res = MessageBox.Show($"The selected map file seems to be from the original game.\r\nDate of last file writing: {fileDate}\r\rAre you sure you want to overwrite it?", "Original game map detected", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                return res == DialogResult.No ? 0 : res == DialogResult.Yes ? 1 : 2;
            }
            return 1;
        }
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap();
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMap(true);
        }
        private void SaveMap(bool asNew = false, bool loadAfterSaving = false, string mapToLoadAfterSaving = "")
        {
            if (MapFileName == _untitledMapName || (asNew == false && File.Exists(MapFileName) == false))
            {
                asNew = true;
            }
            int originalMapFileCheck = 1;
            if(asNew == false)
            {
                originalMapFileCheck = CheckOriginalMapfile(MapFileName);
            }
            if(originalMapFileCheck == 2)
            {
                return;
            }
            if(originalMapFileCheck == 0)
            {
                asNew = true;
            }
            bool savingSuccessful = false;
            if (asNew)
            {
                // The default behavior of Windows Application is that the menu
                // position will not change on a simple save, but will on a SaveAs
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = (Path.GetFileNameWithoutExtension(MapFileName) == _untitledMapName) ? GetMapDir() : Path.GetDirectoryName(MapFileName);
                saveFileDialog.Filter = "Uncompressed map files (*.map)|*.map|Compressed map files (*.map)|*.map|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    int originalMapFileCheck2 = CheckOriginalMapfile(saveFileDialog.FileName);
                    if (originalMapFileCheck2 == 2)
                    {
                        return;
                    }
                    if (originalMapFileCheck2 == 1)
                    {
                        byte[] bytes = HGMapData.MapDataToByteArray(defaultBytes, _mapData);
                        if (saveFileDialog.FilterIndex == 1)
                        {
                            File.WriteAllBytes(saveFileDialog.FileName, bytes);
                        }
                        else if (saveFileDialog.FilterIndex == 2)
                        {
                            string initialDir = Directory.GetCurrentDirectory();
                            Directory.SetCurrentDirectory($"{editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                            string tempFileName = Path.GetFileName(saveFileDialog.FileName);
                            File.WriteAllBytes(tempFileName, bytes);//create temp map file in rnc directory
                            CompressFile(tempFileName);
                            File.Copy(tempFileName, saveFileDialog.FileName, true);
                            File.Delete(tempFileName);//delete temp map file from rnc directory
                            Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                        }
                        MapFileName = saveFileDialog.FileName;
                        savingSuccessful = true;
                        int mapNum = 0;
                        if (int.TryParse(Path.GetFileNameWithoutExtension(MapFileName), out mapNum) == false)
                        {
                            AddConsoleMessage($"[SaveMap] Map filename {Path.GetFileNameWithoutExtension(MapFileName)} does not correspond to the Hired Guns map file naming scheme (two-digit number)!", warning: true);
                        }
                        if (mapNum != _mapData.file)
                        {
                            AddConsoleMessage($"[SaveMap] Map filename {Path.GetFileNameWithoutExtension(MapFileName)} differs from the HGMapData.file property (\"Map\" register -> File #)!", warning: true);
                        }
                    }
                }
            }
            else
            {
                if (originalMapFileCheck == 1)
                {
                    bool oldFileCompressed = new FileInfo(MapFileName).Length < HGMapData.MAP_BYTES;
                    //bool oldFileCompressed = false;
                    _mapData.originalCellLightingMethod = OriginalCellLightingMethod;
                    byte[] bytes = HGMapData.MapDataToByteArray(defaultBytes, _mapData);
                    if(oldFileCompressed)
                    {
                        string initialDir = Directory.GetCurrentDirectory();
                        Directory.SetCurrentDirectory($"{editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
                        string tempFileName = Path.GetFileName(MapFileName);
                        File.WriteAllBytes(tempFileName, bytes);//create temp map file in rnc directory
                        CompressFile(tempFileName);
                        File.Copy(tempFileName, MapFileName, true);
                        File.Delete(tempFileName);//delete temp map file from rnc directory
                        Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
                    }
                    else
                    {
                        File.WriteAllBytes(MapFileName, bytes);
                    }
                    //File.WriteAllBytes(MapFileName, bytes);
                    savingSuccessful = true;
                }
            }
            if (savingSuccessful)
            {
                MapChanged = false;
                string s = HGMapData.CheckMap(_mapData);
                if (string.IsNullOrEmpty(s) == false)
                {
                    AddConsoleMessage(s);
                }
                AddConsoleMessage($"[SaveMap] Saved map.");
            }
            if(loadAfterSaving)
            {
                if(string.IsNullOrEmpty(mapToLoadAfterSaving))
                {
                    OpenToolStripMenuItem_Click(OpenToolStripMenuItem, new EventArgs());
                }
                else
                {
                    LoadMap(mapToLoadAfterSaving);
                }
                
            }
        }
        private void OnMruFile(int number, String filename)
        {
            mruMenu.SetFirstFile(number);
            MapFileName = filename;
            LoadMap(MapFileName);
            mruMenu.SaveToRegistry();
            /*
            DialogResult result = MessageBox.Show(
                            "MRU open (" + number.ToString() + ") " + filename
                            + "\n\nClick \"Yes\" to mimic the behavior of the file opening successfully\n"
                            + "Click \"No\" to mimic the behavior of the file open failing"
                            , "MruToolStripMenu Demo"
                            , MessageBoxButtons.YesNo);

            // You may want to use exception handlers in your code

            if (result == DialogResult.Yes)
            {
                mruMenu.SetFirstFile(number);
                MapFileName = filename;
                LoadMap(MapFileName);
                mruMenu.SaveToRegistry();
            }
            else
            {
                MessageBox.Show("The file '" + filename + "'cannot be opened and will be removed from the Recent list(s)"
                    , "MruToolStripMenu Demo"
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Error);
                mruMenu.RemoveFile(number);
            }
            */
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Save file check
            Close();
        }
        private void OptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_optionsForm != null)
            {
                _optionsForm.Dispose();
            }
            _optionsForm = new OptionsForm(this);
            _optionsForm.Show();
        }
        private void HorizonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_horizonsForm != null)
            {
                _horizonsForm.Dispose();
            }
            _horizonsForm = new HorizonsForm(this);
            _horizonsForm.Show();
            _horizonsForm.UpdateHorizons(_mapData);
            /*
            if(_horizonsForm == null)
            {
                _horizonsForm = new HorizonsForm(this);
                
            }
            else
            {
                //_horizonsForm.Close();
            }
            _horizonsForm.Show();
            _horizonsForm.UpdateHorizons(_mapData);
            */
        }
        private void WorldmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_worldmapWindow != null)
            {
                _worldmapWindow.Dispose();
            }
            _worldmapWindow = new WorldmapWindow(this);
            _worldmapWindow.Show();
            _worldmapWindow.UpdateWorldmap(_mapData);
        }
        private void ConversationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_conversationsForm != null)
            {
                _conversationsForm.Dispose();
            }
            _conversationsForm = new ConversationsForm(this);
            _conversationsForm.Show();
            _conversationsForm.UpdateConversations(_mapData);
            /*
            if (_conversationsForm == null)
            {
                _conversationsForm = new ConversationsForm(this);
                
            }
            else
            {
                //_conversationsForm.Close();
            }
            _conversationsForm.Show();
            _conversationsForm.UpdateConversations(_mapData);
            */
        }
        public void SelectHiredGunsDirectory()
        {
            FolderBrowserDialog fbdGame = new FolderBrowserDialog();
            fbdGame.SelectedPath = gameDir;
            fbdGame.Description = "Select Hired Guns game directory";
            if (fbdGame.ShowDialog() == DialogResult.OK)
            {
                gameDir = fbdGame.SelectedPath;
                if(_locationsFileProgressWindow != null)
                {
                    if(_locationsFileProgressWindow.GameDirTextBox.Text != gameDir)
                    {
                        _locationsFileProgressWindow.GameDirTextBox.Text = gameDir;
                    }
                }
            }
        }
        public void SelectDevDirectory()
        {
            FolderBrowserDialog fbdWorking = new FolderBrowserDialog();
            fbdWorking.SelectedPath = devDir;
            fbdWorking.Description = "Select working directory (Hired Guns (CD32) or Dev files)";
            if (fbdWorking.ShowDialog() == DialogResult.OK)
            {
                devDir = fbdWorking.SelectedPath;
                if (_locationsFileProgressWindow != null)
                {
                    if (_locationsFileProgressWindow.DevDirTextBox.Text != devDir)
                    {
                        _locationsFileProgressWindow.DevDirTextBox.Text = devDir;
                    }
                }
            }
        }
        private void CreateLocationsdatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateLocationsDat();
        }
        private void CreateLocationsDat()
        {
            /*
            CheckDevRootDir();
            CheckGameDir();
            List<byte> bytes = new List<byte>();
            string[] fileNames = Directory.GetFiles($"{_devRootDir}\\Maps\\Game", "*.map", searchOption: SearchOption.AllDirectories);
            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i];
                int mapNum = 0;
                if (int.TryParse(Path.GetFileNameWithoutExtension(fileName), out mapNum) == false)
                {
                    continue;
                }
                //AddConsoleMessage($"[CreateLocationsDat] ({i}) fileName: {fileName}");

                byte[] fileBytes = File.ReadAllBytes(fileName);
                bytes.AddRange(fileBytes.Skip(0).Take(542));
            }
            string initialDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory($"{_editorDir}\\rnc");//set the current dir because Process.Start doesn't accept paths
            File.WriteAllBytes("Locations.dat", bytes.ToArray());
            CompressFile("Locations.dat");
            string locationsPath = $"{_gameDir}\\World\\Locations.dat";
            File.Copy("Locations.dat", locationsPath, true);
            File.Delete("Locations.dat");
            AddConsoleMessage($"[CreateLocationsDat] Created {locationsPath}");
            Directory.SetCurrentDirectory(initialDir);//reset the initialDir just in case
            */
            //CheckWorkingDir();
            //AddConsoleMessage($"[CreateLocationsDat] Selected game directory: {gameDir}");
            //AddConsoleMessage($"[CreateLocationsDat] Selected working directory: {workingDir}");
            if (_locationsFileProgressWindow != null)
            {
                _locationsFileProgressWindow.Dispose();
            }
            _locationsFileProgressWindow = new LocationsFileProgressWindow(this);
            _locationsFileProgressWindow.ShowDialog();
        }
        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Function disabled.");
            return;//DISABLED
            if(_stateManager == null)
            {
                return;
            }
            HGMapData mapData = _stateManager.StepBack();
            if (mapData != null)
            {
                LoadMap(mapData, true);
            }
        }
        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Function disabled.");
            return;//DISABLED
            if (_stateManager == null)
            {
                return;
            }
            HGMapData mapData = _stateManager.StepNext();
            if(mapData != null)
            {
                LoadMap(mapData,true);
            }
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_coordsSelected.Count == 0)
            {
                return;
            }
            CopyCellsToClipBoard(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1], true);
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_coordsSelected.Count == 0)
            {
                return;
            }
            CopyCellsToClipBoard(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1], false);
        }
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_cellClipboard.cells.Length == 0)
            {
                _pasting = false;
                return;
            }
            PaintToggleButton.Checked = true;
            _pasting = true;
        }
        private void CopyCellsToClipBoard(Coordinate3 coordsStart, Coordinate3 coordsEnd, bool cut = false)
        {
            if (_mapData == null || _mapData.cells == null)
            {
                return;
            }
            int startX = HelperUtil.Clamp<int>(Math.Min(coordsStart.x, coordsEnd.x), 0, HGMapData.MAP_WIDTH - 1);
            int startY = HelperUtil.Clamp<int>(Math.Min(coordsStart.y, coordsEnd.y), 0, HGMapData.MAP_HEIGHT - 1);
            int startZ = HelperUtil.Clamp<int>(Math.Min(coordsStart.z, coordsEnd.z), 0, HGMapData.MAP_LEVELS - 1);
            int endX = HelperUtil.Clamp<int>((coordsEnd.x < coordsStart.x ? coordsStart.x : coordsEnd.x), 0, HGMapData.MAP_WIDTH - 1);
            int endY = HelperUtil.Clamp<int>((coordsEnd.y < coordsStart.y ? coordsStart.y : coordsEnd.y), 0, HGMapData.MAP_HEIGHT - 1);
            int endZ = HelperUtil.Clamp<int>((coordsEnd.z < coordsStart.z ? coordsStart.z : coordsEnd.z), 0, HGMapData.MAP_LEVELS - 1);
            coordsStart = new Coordinate3(startX, startY, startZ);
            coordsEnd = new Coordinate3(endX, endY, endZ);
            _cellClipboard.CellArraySize = new Coordinate3(endX - startX + 1, endY - startY + 1, endZ - startZ + 1);

            for (int z = startZ; z <= endZ; z++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    for (int x = startX; x <= endX; x++)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        Coordinate3 coordsClipboard = new Coordinate3(x - startX, y - startY, z - startZ);
                        HGCell cell = _mapData.GetCell(coords);
                        _cellClipboard.cells[coordsClipboard.x, coordsClipboard.y, coordsClipboard.z] = (HGCell)cell.Clone();
                        if(cell.floorHere && cell.floorType == HGCellFloorType.Lift)
                        {
                            _cellClipboard.lifts.Add(coordsClipboard,(HGLift)_mapData.GetLift(coords,false).Clone());
                        }
                        if (cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW))
                        {
                            _cellClipboard.doors.Add(coordsClipboard, (HGDoor)_mapData.GetDoor(coords, false).Clone());
                        }
                        if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cell.floorHere && cell.floorType == HGCellFloorType.Button))
                        {
                            _cellClipboard.buttons.Add(coordsClipboard, (HGButton)_mapData.GetButton(coords, false).Clone());
                        }
                    }
                }
            }
            if(cut)
            {
                ClearCell(coordsStart,coordsEnd);
            }
        }
        private Coordinate3 GetClipboardOffsetCoords(Coordinate3 coords, Coordinate3 coordsOffset)
        {
            return new Coordinate3
            (
                coords.x + coordsOffset.x - (int)Math.Floor((double)_cellClipboard.cells.GetLength(0) / 2) + (_cellClipboard.cells.GetLength(0) % 2 == 0 ? 1 : 0), 
                coords.y + coordsOffset.y - (int)Math.Floor((double)_cellClipboard.cells.GetLength(1) / 2) + (_cellClipboard.cells.GetLength(1) % 2 == 0 ? 1 : 0), 
                coords.z + coordsOffset.z
            );
            /*
            return new Coordinate3
            (
                coords.x + coordsOffset.x - (_cellClipboard.cells.GetLength(0) / 2) + ((double)_cellClipboard.cells.GetLength(0) % 2 == 0 ? 1 : 0),
                coords.y + coordsOffset.y - (_cellClipboard.cells.GetLength(1) / 2) + ((double)_cellClipboard.cells.GetLength(1) % 2 == 0 ? 1 : 0),
                coords.z + coordsOffset.z
            );
            */
            /*
            return new Coordinate3
            (
                coords.x + coordsOffset.x,
                coords.y + coordsOffset.y,
                coords.z + coordsOffset.z
            );
            */
        }
        private Coordinate3 GetClipboardUpdateCoordsStart(Coordinate3 coords)
        {
            return new Coordinate3
            (
                _coordsCursor.x - (int)Math.Floor((double)_cellClipboard.cells.GetLength(0) / 2) + (_cellClipboard.cells.GetLength(0) % 2 == 0 ? 1 : 0),
                _coordsCursor.y - (int)Math.Floor((double)_cellClipboard.cells.GetLength(1) / 2) + (_cellClipboard.cells.GetLength(1) % 2 == 0 ? 1 : 0),
                _coordsCursor.z
            );
        }
        private Coordinate3 GetClipboardUpdateCoordsEnd(Coordinate3 coords)
        {
            return new Coordinate3
            (
                _coordsCursor.x - (int)Math.Floor((double)_cellClipboard.cells.GetLength(0) / 2) + (_cellClipboard.cells.GetLength(0) % 2 == 0 ? 1 : 0) + _cellClipboard.cells.GetLength(0) - 1,
                _coordsCursor.y - (int)Math.Floor((double)_cellClipboard.cells.GetLength(1) / 2) + (_cellClipboard.cells.GetLength(1) % 2 == 0 ? 1 : 0) + _cellClipboard.cells.GetLength(1) - 1,
                _coordsCursor.z + _cellClipboard.cells.GetLength(2)
            );
        }
        private void PasteCellsFromClipboard()
        {
            if (_mapData == null || _mapData.cells == null)
            {
                _pasting = false;
                return;
            }
            if(_cellClipboard.cells.Length == 0)
            {
                _pasting = false;
                return;
            }
            SetBlockHandlerKeep();
            for (int z = 0; z < _cellClipboard.cells.GetLength(2); z++)
            {
                for (int y = 0; y < _cellClipboard.cells.GetLength(1); y++)
                {
                    for (int x = 0; x < _cellClipboard.cells.GetLength(0); x++)
                    {
                        Coordinate3 coordsClipboard = new Coordinate3(x, y, z);
                        Coordinate3 coords = GetClipboardOffsetCoords(_coordsCursor, coordsClipboard);
                        
                        if (coords.x < 0 || coords.x >= HGMapData.MAP_WIDTH || coords.y < 0 || coords.y >= HGMapData.MAP_HEIGHT || coords.z < 0 || coords.z >= HGMapData.MAP_LEVELS)
                        {
                            continue;
                        }
                        HGCell cell = _cellClipboard.cells[coordsClipboard.x, coordsClipboard.y, coordsClipboard.z];
                        if (CheckSetCellType(SetCellPropertyType.Block))
                        {
                            SetBlockType(coords, cell.blockType, cell.blockHere, setControl: false, setCell: true);
                            SetPushable(coords, cell.pushable, setControl: false, setCell: true);
                            SetOpaque(coords, cell.opaque, setControl: false, setCell: true);
                            SetInvisible(coords, cell.invisible, setControl: false, setCell: true);
                            SetTeleportTargetPosX(coords, cell.teleportTargetPos.x, setControl: false, setCell: true);
                            SetTeleportTargetPosY(coords, cell.teleportTargetPos.y, setControl: false, setCell: true);
                            SetTeleportTargetPosZ(coords, cell.teleportTargetPos.z, setControl: false, setCell: true);
                            SetFitnessBoostValue(coords, cell.healingValue, setControl: false, setCell: true);
                            SetForceQuestionMark(coords, cell.forceQuestionMark, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Floor)) SetFloorType(coords, cell.floorType, cell.floorHere, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Panel)) SetPanelType(coords, cell.panelType, cell.panelHere, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Water))
                        {
                            SetWaterLevel(coords, cell.waterLevel, cell.waterHere, setControl: false, setCell: true);
                            SetTidalWaterFlowing(coords, cell.tidalWaterFlowing, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Auxiliary)) SetAuxiliaryType(coords, cell.auxiliaryType, cell.auxiliaryHere, setControl: false, setCell: true);
                        if (CheckSetCellType(SetCellPropertyType.Egg))
                        {
                            SetEggDelay(coords, cell.eggDelay, setTextBoxControl: false, setDropdownListControl : false, setCell: true);
                            SetEggBlockMonster(coords, cell.eggBlocked, setControl: false, setCell: true);
                            SetEggErase(coords, cell.eggErase, setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Sky))
                        {
                            SetSky(coords, 0, cell.skyRendered[0], setControl: false, setCell: true);
                            SetSky(coords, 1, cell.skyRendered[1], setControl: false, setCell: true);
                            SetSky(coords, 2, cell.skyRendered[2], setControl: false, setCell: true);
                            SetSky(coords, 3, cell.skyRendered[3], setControl: false, setCell: true);
                        }
                        if (CheckSetCellType(SetCellPropertyType.Item))
                        {
                            SetItemID(coords, cell.itemID, setListControl: false, setTextBoxControl: false, setCell: true);
                            SetItemDamage(coords, cell.itemDamaged, setControl: false, setCell: true);
                            SetItemAmmo(coords, cell.itemAmmo, setControl: false, setCell: true);
                        }

                        if (cell.floorHere && cell.floorType == HGCellFloorType.Lift)
                        {
                            HGLift liftClipboard = null;
                            if(_cellClipboard.lifts.TryGetValue(coordsClipboard, out liftClipboard))
                            {
                                SetLiftInitial(coords, liftClipboard.initial, setControl: false, setCell: true);
                                SetLiftMaxZ(coords, liftClipboard.maxZ, setControl: false, setCell: true);
                                SetLiftMinZ(coords, liftClipboard.minZ, setControl: false, setCell: true);
                                SetLiftCarry(coords, liftClipboard.carry, setControl: false, setCell: true);
                                SetLiftContinueAtTop(coords, liftClipboard.continueAtTop, setControl: false, setCell: true);
                                SetLiftContinueAtBottom(coords, liftClipboard.continueAtBottom, setControl: false, setCell: true);
                                SetLiftAutomoveTop(coords, liftClipboard.automove == 2 || liftClipboard.automove == 3, setControl: false, setCell: true);
                                SetLiftAutomoveBottom(coords, liftClipboard.automove == 1 || liftClipboard.automove == 3, setControl: false, setCell: true);
                            }
                        }
                        if (cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW))
                        {
                            HGDoor doorClipboard = null;
                            if (_cellClipboard.doors.TryGetValue(coordsClipboard, out doorClipboard))
                            {
                                SetDoorCloseDelay(coords, doorClipboard.closeDelay, setControl: false, setCell: true);
                                SetDoorKeyID(coords, doorClipboard.keyID, setControl: false, setCell: true);
                                SetDoorLocked(coords, doorClipboard.locked, setControl: false, setCell: true);
                                SetDoorButtonOnly(coords, doorClipboard.buttonOnly, setControl: false, setCell: true);
                            }
                        }
                        if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cell.floorHere && cell.floorType == HGCellFloorType.Button))
                        {
                            HGButton buttonClipboard = null;
                            if (_cellClipboard.buttons.TryGetValue(coordsClipboard, out buttonClipboard))
                            {
                                SetButtonTargetInPositionX(coords, buttonClipboard.targetPosPushed.x, setControl: false, setCell: true);
                                SetButtonTargetInPositionY(coords, buttonClipboard.targetPosPushed.y, setControl: false, setCell: true);
                                SetButtonTargetInPositionZ(coords, buttonClipboard.targetPosPushed.z, setControl: false, setCell: true);
                                SetButtonTargetOutPositionX(coords, buttonClipboard.targetPosReleased.x, setControl: false, setCell: true);
                                SetButtonTargetOutPositionY(coords, buttonClipboard.targetPosReleased.y, setControl: false, setCell: true);
                                SetButtonTargetOutPositionZ(coords, buttonClipboard.targetPosReleased.z, setControl: false, setCell: true);
                                SetButtonActionPushed(coords, buttonClipboard.actionPushed, setControl: false, setCell: true);
                                SetButtonActionReleased(coords, buttonClipboard.actionReleased, setControl: false, setCell: true);
                            }
                        }
                    }
                }
            }
            SetBlockHandlerKeep(false);
            Coordinate3 coordsStart = GetClipboardUpdateCoordsStart(_coordsCursor);
            Coordinate3 coordsEnd = GetClipboardUpdateCoordsEnd(_coordsCursor);
            _mapData.UpdateMapTiles(coordsStart, coordsEnd);
            UpdateMapView(coordsStart, coordsEnd);
            UpdateDebugTextBox();
            AddStep();
        }
        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_helpForm != null)
            {
                _helpForm.Dispose();
            }
            _helpForm = new HelpForm(this);
            _helpForm.Show();
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HGMEWAbout aboutWindow = new HGMEWAbout();
            aboutWindow.ShowDialog();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: TOP BAR
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ActiveModeToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            ActiveModeToggleButton.BackColor = ActiveModeToggleButton.Checked ? highlightColor : SystemColors.Control;
            ToggleButton_CheckedChanged(sender, e);
        }
        public void ShowWaterToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if(ShowWater != ShowWaterToggleButton.Checked)
            {
                ShowWater = ShowWaterToggleButton.Checked;
            }
            ShowWaterToggleButton.BackColor = ShowWaterToggleButton.Checked ? highlightColor : SystemColors.Control;
            ToggleButton_CheckedChanged(sender, e);
        }
        private void ShowSkyToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMapView();
            ShowSkyToggleButton.BackColor = ShowSkyToggleButton.Checked ? highlightColor : SystemColors.Control;
            ToggleButton_CheckedChanged(sender, e);
        }
        private void AutoStairsToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            AutoStairsToggleButton.BackColor = AutoStairsToggleButton.Checked ? highlightColor : SystemColors.Control;
            ToggleButton_CheckedChanged(sender, e);
        }

        private void ZLevelUpButton_Click(object sender, EventArgs e)
        {
            ZLevel++;
        }

        private void ZLevelDownButton_Click(object sender, EventArgs e)
        {
            ZLevel--;
        }

        private void ZLevelDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZLevel = HGMapData.MAP_LEVELS - 1 - ZLevelDropdownList.SelectedIndex;
        }
        private int ZLevelConvertToIndex()
        {
            return HGMapData.MAP_LEVELS - 1 - ZLevel;
        }
        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            MapTileScale++;
        }
        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            MapTileScale--;
        }
        private void SetCellPropertyDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetCellGroupBoxColors();
            if(CheckSetCellType(SetCellPropertyType.Block, false))
            {
                BlockGroupBox.BackColor = highlightColor;
            }
            else if (CheckSetCellType(SetCellPropertyType.Floor, false))
            {
                FloorGroupBox.BackColor = highlightColor;
            }
            else if (CheckSetCellType(SetCellPropertyType.Panel, false))
            {
                PanelGroupBox.BackColor = highlightColor;
            }
            else if (CheckSetCellType(SetCellPropertyType.Water, false))
            {
                WaterGroupBox.BackColor = highlightColor;
            }
            else if (CheckSetCellType(SetCellPropertyType.Auxiliary, false))
            {
                AuxiliaryGroupBox.BackColor = highlightColor;
            }
            if (CheckSetCellType(SetCellPropertyType.Egg, false))
            {
                EggGroupBox.BackColor = highlightColor;
            }
            if (CheckSetCellType(SetCellPropertyType.Sky, false))
            {
                SkyCellGroupBox.BackColor = highlightColor;
            }
            if (CheckSetCellType(SetCellPropertyType.Item, false))
            {
                ItemGroupBox.BackColor = highlightColor;
            }
        }
        private void SetCellPropertyDropdownList_DrawItem(object sender, DrawItemEventArgs e)
        {
            //You need to change ComboBox.DrawMode to OwnerDrawFixed/OwnerDrawVariable to fire the comboBox_DrawItem
            int index = e.Index >= 0 ? e.Index : -1;
            SetCellPropertyDropdownList.BackColor = SetCellPropertyDropdownList.SelectedIndex > 0 ? highlightColor : SystemColors.Window;
            e.DrawBackground();
            if (index != -1)
            {
                e.Graphics.DrawString(SetCellPropertyDropdownList.Items[index].ToString(), e.Font, new SolidBrush(SetCellPropertyDropdownList.ForeColor), e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }
        private void ResetCellGroupBoxColors()
        {
            Color col = Color.Transparent;
            BlockGroupBox.BackColor = col;
            FloorGroupBox.BackColor = col;
            PanelGroupBox.BackColor = col;
            WaterGroupBox.BackColor = col;
            AuxiliaryGroupBox.BackColor = col;
            EggGroupBox.BackColor = col;
            SkyCellGroupBox.BackColor = col;
            ItemGroupBox.BackColor = col;
        }
       
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: SIDE BAR
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SideBarToggleButton_paint(object sender, PaintEventArgs e)
        {
            CheckBox myCheckbox = (CheckBox)sender;
            Rectangle borderRectangle = myCheckbox.ClientRectangle;
            if (myCheckbox.Checked)
            {
                ControlPaint.DrawBorder3D(e.Graphics, borderRectangle,
                    Border3DStyle.Sunken);
            }
            else
            {
                ControlPaint.DrawBorder3D(e.Graphics, borderRectangle,
                    Border3DStyle.Raised);
            }
        }
        private void ToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox myCheckbox = (CheckBox)sender;
            myCheckbox.BackColor = myCheckbox.Checked ? highlightColor : SystemColors.Control;
        }
        private void DeselectSideBarButtons(ToggleButton exception)
        {
            if(exception != SelectToggleButton)
            {
                SelectToggleButton.Checked = false;
            }
            if (exception != PaintToggleButton)
            {
                PaintToggleButton.Checked = false;
            }
            if (exception != FillToggleButton)
            {
                FillToggleButton.Checked = false;
            }
            if (exception != EraseToggleButton)
            {
                EraseToggleButton.Checked = false;
            }
            if (exception != SelectTargetInToggleButton)
            {
                SelectTargetInToggleButton.Checked = false;
            }
            if (exception != SelectTargetOutToggleButton)
            {
                SelectTargetOutToggleButton.Checked = false;
            }
        }
        private ToggleButton GetToggledSideBarButton()
        {
            if (SelectToggleButton.Checked)
            {
                return SelectToggleButton;
            }
            if (PaintToggleButton.Checked)
            {
                return PaintToggleButton;
            }
            if (FillToggleButton.Checked)
            {
                return FillToggleButton;
            }
            if (EraseToggleButton.Checked)
            {
                return EraseToggleButton;
            }
            if (SelectTargetInToggleButton.Checked)
            {
                return SelectTargetInToggleButton;
            }
            if (SelectTargetOutToggleButton.Checked)
            {
                return SelectTargetOutToggleButton;
            }
            return null;
        }
        private void SelectToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if(SelectToggleButton.Checked)
            {
                DeselectSideBarButtons(SelectToggleButton);
                MapViewPictureBox.Cursor = _cursorSelect;
            }
            else if(GetToggledSideBarButton() == null)
            {
                SelectToggleButton.Checked = true;
            }
            ToggleButton_CheckedChanged(sender, e);
        }
        private void PaintToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PaintToggleButton.Checked)
            {
                DeselectSideBarButtons(PaintToggleButton);
                MapViewPictureBox.Cursor = _cursorPaint;
            }
            else if (GetToggledSideBarButton() == null)
            {
                PaintToggleButton.Checked = true;
            }
            if(PaintToggleButton.Checked == false)
            {
                _pasting = false;
            }
            ToggleButton_CheckedChanged(sender, e);
        }
        private void FillToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (FillToggleButton.Checked)
            {
                DeselectSideBarButtons(FillToggleButton);
                MapViewPictureBox.Cursor = _cursorFill;
            }
            else if (GetToggledSideBarButton() == null)
            {
                FillToggleButton.Checked = true;
            }
            ToggleButton_CheckedChanged(sender, e);
        }
        private void EraseToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (EraseToggleButton.Checked)
            {
                DeselectSideBarButtons(EraseToggleButton);
                MapViewPictureBox.Cursor = _cursorErase;
            }
            else if (GetToggledSideBarButton() == null)
            {
                EraseToggleButton.Checked = true;
            }
            ToggleButton_CheckedChanged(sender, e);
        }
        private void SelectTargetInToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTargetInToggleButton.Checked)
            {
                DeselectSideBarButtons(SelectTargetInToggleButton);
                MapViewPictureBox.Cursor = _cursorSelectTargetIn;
            }
            else if (GetToggledSideBarButton() == null)
            {
                SelectTargetInToggleButton.Checked = true;
            }
            ToggleButton_CheckedChanged(sender, e);
        }
        private void SelectTargetOutToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SelectTargetOutToggleButton.Checked)
            {
                DeselectSideBarButtons(SelectTargetOutToggleButton);
                MapViewPictureBox.Cursor = _cursorSelectTargetOut;
            }
            else if (GetToggledSideBarButton() == null)
            {
                SelectTargetOutToggleButton.Checked = true;
            }
            ToggleButton_CheckedChanged( sender, e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: MAIN
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Added manually in MainForm()
        private void MapViewPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if
            (
                TeleportTargetPosXTextBox.Focused == false && TeleportTargetPosYTextBox.Focused == false && TeleportTargetPosZTextBox.Focused == false
                && FitnessBoostValueTextBox.Focused == false && EggDelayTextBox.Focused == false 
                && ItemIDTextBox.Focused == false && ItemDamagedTextBox.Focused == false && ItemAmmunitionTextBox.Focused == false
                && ButtonTargetInPositionXTextBox.Focused == false && ButtonTargetInPositionYTextBox.Focused == false && ButtonTargetInPositionZTextBox.Focused == false
                 && ButtonTargetOutPositionXTextBox.Focused == false && ButtonTargetOutPositionYTextBox.Focused == false && ButtonTargetOutPositionZTextBox.Focused == false
                 && DoorCloseDelayTextBox.Focused == false && DoorKeyIDTextBox.Focused == false
                 && LiftMinZTextBox.Focused == false && LiftMaxZTextBox.Focused == false
                 && WorldmapDescriptionTextBox.Focused == false && DetailedDescriptionTextBox.Focused == false && IntroMessageTextBox.Focused == false
                 && Player1StartXTextBox.Focused == false && Player1StartYTextBox.Focused == false && Player1StartZTextBox.Focused == false
                 && Player2StartXTextBox.Focused == false && Player2StartYTextBox.Focused == false && Player2StartZTextBox.Focused == false
                 && Player3StartXTextBox.Focused == false && Player3StartYTextBox.Focused == false && Player3StartZTextBox.Focused == false
                 && Player4StartXTextBox.Focused == false && Player4StartYTextBox.Focused == false && Player4StartZTextBox.Focused == false
                 && Destination1TextBox.Focused == false && Destination2TextBox.Focused == false && Destination3TextBox.Focused == false && Destination4TextBox.Focused == false
                 && TidalWaterSpeedTextBox.Focused == false && TidalWaterMaxHeightTextBox.Focused == false && TidalWaterMaxHeightTextBox.Focused == false
                 && TimeLimitTextBox.Focused == false && DiskTextBox.Focused == false && FileTextBox.Focused == false && PictureTextBox.Focused == false && PlayersTextBox.Focused == false
                 && WorldPosXTextBox.Focused == false && WorldPosYTextBox.Focused == false
            )
            {
                if (keyData == Keys.Return)
                {
                    Focus();
                    return true;
                }
                if (keyData == Keys.Delete)
                {
                    ClearCell(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    ResetCellControls();
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    if (_mapData == null)
                    {
                        return true;
                    }
                    ZLevel++;
                    return true;
                }
                if (keyData == Keys.PageDown)
                {
                    if (_mapData == null)
                    {
                        return true;
                    }
                    ZLevel--;
                    return true;
                }
                if (keyData == Keys.Add)
                {
                    MapTileScale++;
                    return true;
                }
                if (keyData == Keys.Subtract)
                {
                    MapTileScale--;
                    return true;
                }
                if (keyData == Keys.A)
                {
                    ActiveModeToggleButton.Checked = !ActiveModeToggleButton.Checked;
                    return true;
                }
                if (keyData == Keys.W)
                {
                    //PanelExtension.ScrollUp(MapViewPanel, MapViewPanel.VerticalScroll.Value - MapTileSizeScaled);
                    MapViewScrollV -= MapTileSizeScaled;
                    return true;
                }
                if (keyData == Keys.S)
                {
                    //PanelExtension.ScrollDown(MapViewPanel, MapViewPanel.VerticalScroll.Value + MapTileSizeScaled);
                    MapViewScrollV += MapTileSizeScaled;
                    return true;
                }
                if (keyData == Keys.M)
                {
                    SelectToggleButton.Checked = true;
                    return true;
                }
                if (keyData == Keys.E)
                {
                    EraseToggleButton.Checked = true;
                    return true;
                }
                if (keyData == Keys.B)
                {
                    PaintToggleButton.Checked = true;
                    return true;
                }
                if (keyData == Keys.G)
                {
                    FillToggleButton.Checked = true;
                    return true;
                }
                if (keyData == Keys.T)
                {
                    //SelectTargetInToggleButton.Checked = true;
                    return true;
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void MapViewPictureBox_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void MapViewPictureBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

        }
        private void MapViewPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouse0Down = true;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _mouse1Down = true;
            }
            SetCoordsCursor(Coordinate3.PointToCoordinate3(_mapViewMousePos, ZLevel, MapTileSizeScaled));
            _coordsCursorDragStart = _coordsCursor;
            if (SelectToggleButton.Checked || Control.ModifierKeys == Keys.Shift)
            {
                if (e.Button == MouseButtons.Left)
                {
                    _coordsCursorDragStart = _coordsCursor;
                    SetCoordsSelected(_coordsCursor);
                    MapViewPictureBox.Invalidate();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PaintToggleButton.Checked = true;
                }
            }
            else if(PaintToggleButton.Checked)
            {
                if(e.Button == MouseButtons.Left)
                {
                    if (Control.ModifierKeys == Keys.Alt)
                    {
                        PickCell(_coordsCursor);
                        MapViewPictureBox.Invalidate();
                    }
                    else
                    {
                        if(_pasting)
                        {
                            PasteCellsFromClipboard();
                        }
                        else
                        {
                            PaintCell(_coordsCursor);
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (_pasting)
                    {
                        _pasting = false;
                    }
                    else if (SelectTargetInToggleButton.Checked || SelectTargetOutToggleButton.Checked)
                    {
                        PaintToggleButton.Checked = true;
                    }
                    else
                    {
                        ClearCell(_coordsCursor);
                    }
                    
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    PickCell(_coordsCursor);
                    MapViewPictureBox.Invalidate();
                }
            }
            else if (FillToggleButton.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (Control.ModifierKeys == Keys.Alt)
                    {
                        PickCell(_coordsCursor);
                        MapViewPictureBox.Invalidate();
                    }
                    else if(_coordsSelected.Contains(_coordsCursor))
                    {
                        PaintCell(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (_coordsSelected.Contains(_coordsCursor))
                    {
                        ClearCell(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    PickCell(_coordsCursor);
                    MapViewPictureBox.Invalidate();
                }
            }
            else if (EraseToggleButton.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ClearCell(_coordsCursor);
                }
            }
            else if (SelectTargetInToggleButton.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectTarget(true, false);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PaintToggleButton.Checked = true;
                }
            }
            else if (SelectTargetOutToggleButton.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    SelectTarget(false, true);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    PaintToggleButton.Checked = true;
                }
            }
        }
        private void MapViewPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mouse0Down = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                _mouse1Down = false;
            }
            SetCoordsCursor(Coordinate3.PointToCoordinate3(_mapViewMousePos, ZLevel, MapTileSizeScaled));
            if (SelectToggleButton.Checked || Control.ModifierKeys == Keys.Shift)
            {
                SetCoordsSelected(_coordsCursorDragStart, _coordsCursor);
                MapViewPictureBox.Invalidate();
            }
        }
        private void MapViewPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            _mapViewMousePos = e.Location;
            UpdateMapViewCursor();
        }
        private void MapViewPictureBox_MouseEnter(object sender, EventArgs e)
        {
            _cursorInsideMapView = true;
        }
        private void MapViewPictureBox_MouseLeave(object sender, EventArgs e)
        {
            _cursorInsideMapView = false;
            _mouse0Down = false;
            MapViewPictureBox.Invalidate();
        }
        //Added manually in MainForm()
        private void MapViewPictureBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {


            // Update the drawing based upon the mouse wheel scrolling.

            int numberOfPixelsToMove = e.Delta / 120;

            if (numberOfPixelsToMove != 0)
            {
                System.Drawing.Drawing2D.Matrix translateMatrix = new System.Drawing.Drawing2D.Matrix();
                translateMatrix.Translate(0, numberOfPixelsToMove);
                //mousePath.Transform(translateMatrix);

            }
            //MapViewPanel.Invalidate();

            //MapTileScale += e.Delta < 0 ? -1 : e.Delta > 0 ? 1 : 0;

            ZLevel = e.Delta > 0 ? ZLevel + 1 : ZLevel - 1;

            //block mousewheel scrolling
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;
        }
        private void MapViewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_mapData == null || _coordsSelected.Count == 0)
            {
                return;
            }
            /*
            int x = _coordsSelected[0].x;
            int y = _coordsSelected[0].y;
            int z = _coordsSelected[0].z;
            */
            Coordinate3 coords = new Coordinate3(_coordsSelected[0].x, _coordsSelected[0].y, _coordsSelected[0].z);
            HGCell cellSelected = _mapData.GetCell(_coordsSelected[0].x, _coordsSelected[0].y, _coordsSelected[0].z);
            HGCell cellCursor = _mapData.GetCell(_coordsCursor.x, _coordsCursor.y, _coordsCursor.z);
            if (cellSelected == null || cellCursor == null)
            {
                return;
            }
            bool drawTargetCoordsIn = false;
            bool drawTargetCoordsOut = false;
            Coordinate3 coordsIn = new Coordinate3();
            Coordinate3 coordsOut = new Coordinate3();
            if ((cellCursor.floorHere && cellCursor.floorType == HGCellFloorType.Button) || (cellCursor.panelHere && (cellCursor.panelType == HGCellPanelType.ButtonIn || cellCursor.panelType == HGCellPanelType.ButtonOut)))
            {
                HGButton btn = _mapData.GetButton(_coordsCursor, false);
                if(btn != null)
                {
                    drawTargetCoordsIn = btn.actionPushed != HGButtonAction.None && btn.actionPushed != HGButtonAction.Unused1 && btn.actionPushed != HGButtonAction.Unused2;
                    coordsIn = btn.targetPosPushed;
                    drawTargetCoordsOut = btn.actionReleased != HGButtonAction.None && btn.actionReleased != HGButtonAction.Unused1 && btn.actionReleased != HGButtonAction.Unused2;
                    coordsOut = btn.targetPosReleased;
                }
            }
            else if (cellCursor.blockHere && cellCursor.blockType == HGCellBlockType.Teleport)
            {
                drawTargetCoordsOut = true;
                coordsOut = cellCursor.teleportTargetPos;
            }
            else if ((cellSelected.floorHere && cellSelected.floorType == HGCellFloorType.Button) || (cellSelected.panelHere && (cellSelected.panelType == HGCellPanelType.ButtonIn || cellSelected.panelType == HGCellPanelType.ButtonOut)))
            {
                HGButton btn = _mapData.GetButton(coords, false);
                if (btn != null)
                {
                    drawTargetCoordsIn = btn.actionPushed != HGButtonAction.None && btn.actionPushed != HGButtonAction.Unused1 && btn.actionPushed != HGButtonAction.Unused2;
                    coordsIn = btn.targetPosPushed;
                    drawTargetCoordsOut = btn.actionReleased != HGButtonAction.None && btn.actionReleased != HGButtonAction.Unused1 && btn.actionReleased != HGButtonAction.Unused2;
                    coordsOut = btn.targetPosReleased;
                }
            }
            else if (cellSelected.blockHere && cellSelected.blockType == HGCellBlockType.Teleport)
            {
                drawTargetCoordsOut = true;
                coordsOut = cellSelected.teleportTargetPos;
            }
            Graphics g = e.Graphics;
            int lineWidth = 2;
            
            if(drawTargetCoordsIn)
            {
                g.DrawImage(_buttonTargetPushedImg, coordsIn.x * MapTileSizeScaled, coordsIn.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                if (coordsIn.z > ZLevel)
                {
                    g.DrawImage(_buttonTargetHigherImg, coordsIn.x * MapTileSizeScaled, coordsIn.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                }
                else if (coordsIn.z < ZLevel)
                {
                    g.DrawImage(_buttonTargetLowerImg, coordsIn.x * MapTileSizeScaled, coordsIn.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                }
            }
            if (drawTargetCoordsOut)
            {
                g.DrawImage(_buttonTargetReleasedImg, coordsOut.x * MapTileSizeScaled, coordsOut.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                if (coordsOut.z > ZLevel)
                {
                    g.DrawImage(_buttonTargetHigherImg, coordsOut.x * MapTileSizeScaled, coordsOut.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                }
                else if (coordsOut.z < ZLevel)
                {
                    g.DrawImage(_buttonTargetLowerImg, coordsOut.x * MapTileSizeScaled, coordsOut.y * MapTileSizeScaled, MapTileSizeScaled, MapTileSizeScaled);
                }
            }
            //selected coords
            Color colSel = Color.Transparent;
            if (_coordsSelected[0].z.Equals(_coordsSelected[_coordsSelected.Count - 1].z) && _coordsSelected[0].z.Equals(ZLevel))
            {
                colSel = Color.LimeGreen;
            }
            else if (_coordsSelected[0].z <= ZLevel && _coordsSelected[_coordsSelected.Count - 1].z >= ZLevel)
            {
                colSel = Color.MediumSeaGreen;
            }
            using (Pen selPen = new Pen(colSel, lineWidth))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.DrawRectangle(
                    selPen,
                    _coordsSelected[0].x * MapTileSizeScaled,
                    _coordsSelected[0].y * MapTileSizeScaled,
                    ((_coordsSelected[_coordsSelected.Count - 1].x - _coordsSelected[0].x) + 1) * MapTileSizeScaled,
                    ((_coordsSelected[_coordsSelected.Count - 1].y - _coordsSelected[0].y) + 1) * MapTileSizeScaled);
            }
            using (Pen selPen = new Pen(_cursorInsideMapView ? Color.White : Color.Transparent, lineWidth))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.DrawRectangle(
                    selPen,
                    _coordsCursor.x * MapTileSizeScaled,
                    _coordsCursor.y * MapTileSizeScaled,
                    MapTileSizeScaled, MapTileSizeScaled);
            }
            //clipboard cells
            if (_pasting)
            {
                using (Pen selPen = new Pen(Color.Magenta, lineWidth))
                {
                    Coordinate3 coordsStart = GetClipboardUpdateCoordsStart(_coordsCursor);
                    Coordinate3 coordsEnd = GetClipboardUpdateCoordsEnd(_coordsCursor);

                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawRectangle(
                        selPen,
                        coordsStart.x * MapTileSizeScaled,
                        coordsStart.y * MapTileSizeScaled,
                        (coordsEnd.x - coordsStart.x + 1) * MapTileSizeScaled, (coordsEnd.y - coordsStart.y + 1) * MapTileSizeScaled);
                }
            }
            //cursor coords
            float fontSize = g.DpiY * 12 / 72;
            Font cursorFont = new Font(FontFamily.GenericMonospace, fontSize);
            string cursorCoordsString = _coordsCursor.ToString();
            //string extCursorInfoString = "";
            //extended info
            if (ShowExtendedCursorInfo)
            {
                if (cellCursor.auxiliaryHere)
                {
                    if (cellCursor.auxiliaryType == HGCellAuxiliaryType.Monster && cellCursor.monsterType >= 0 && cellCursor.monsterType < _monsters.Length && cellCursor.monsterLevel < _monsters[cellCursor.monsterType].levels.Length)
                    {
                        HGCharacter monster = _monsters[cellCursor.monsterType];
                        cursorCoordsString += $"\r\n{monster.name} level {(cellCursor.monsterLevel + 1)}" + (monster.levels[cellCursor.monsterLevel].shielded ? "*" : "");
                        cursorCoordsString += $"\r\nEgg delay: {cellCursor.eggDelay}" + (cellCursor.eggErase ? ", erase egg" : "");
                        string eggDelayString = EggDelayDropDownList.Items[GetEggDelayIndexFromEggdelay(cellCursor.eggDelay)].ToString();
                        cursorCoordsString += $"\r\n{eggDelayString.Substring(HelperUtil.Clamp<int>(5, 0, eggDelayString.Length))}";
                    }
                    else if (cellCursor.auxiliaryType == HGCellAuxiliaryType.Dead1 || cellCursor.auxiliaryType == HGCellAuxiliaryType.Dead2)
                    {
                        string monsterType = "";
                        if(cellCursor.auxiliaryType == HGCellAuxiliaryType.Dead1 && _mapData.monsterType1 == 0)
                        {
                            monsterType = "[not loaded]";
                        }
                        else
                        {
                            monsterType = $"{_monsters[cellCursor.auxiliaryType == HGCellAuxiliaryType.Dead1 ? _mapData.monsterType1 - 1 : _mapData.monsterType2 - 1].name}";
                        }
                        cursorCoordsString += $"\r\n{AuxiliaryDropDownList.Items[GetIndexFromAuxiliaryType(cellCursor.auxiliaryType)]} ({monsterType})";
                    }
                    else if (cellCursor.auxiliaryType == HGCellAuxiliaryType.EggOpen)
                    {
                        cursorCoordsString += $"\r\nEgg open";
                    }
                }
                if (cellCursor.itemID != 0 && cellCursor.itemID < _items.Length)
                {
                    cursorCoordsString += $"\r\n{_items[cellCursor.itemID].name.Trim()}";
                }
                if ((cellCursor.floorHere && cellCursor.floorType == HGCellFloorType.Button) || (cellCursor.panelHere && (cellCursor.panelType == HGCellPanelType.ButtonIn || cellCursor.panelType == HGCellPanelType.ButtonOut)))
                {
                    HGButton btn = _mapData.GetButton(_coordsCursor, false);
                    if (btn != null)
                    {
                        cursorCoordsString += $"\r\nButton {btn.index}: {ButtonActionPushedDropDownList.Items[(int)btn.actionPushed]} | {ButtonActionReleasedDropDownList.Items[(int)btn.actionReleased]}";
                    }
                }
                if (cellCursor.blockHere && (cellCursor.blockType == HGCellBlockType.DoorNS || cellCursor.blockType == HGCellBlockType.DoorEW))
                {
                    HGDoor door = cellCursor.doorIndex >= 0 && cellCursor.doorIndex < _mapData.doors.Length ? _mapData.doors[cellCursor.doorIndex] : null;
                    if (door != null)
                    {
                        cursorCoordsString += $"\r\nDoor {door.index}, close delay: {door.closeDelay}";
                        if (door.locked && door.keyID != 0)
                        {
                            cursorCoordsString += $"\r\nlocked, key: {_items[HelperUtil.Clamp<int>(door.keyID, 0, _items.Length - 1)].name.TrimStart()}";
                        }
                        else if (door.locked && door.buttonOnly)
                        {
                            cursorCoordsString += $"\r\nlocked (button only)";
                        }
                        else if (door.locked)
                        {
                            cursorCoordsString += $"\r\nlocked";
                        }
                    }
                }
                if (cellCursor.floorHere && cellCursor.floorType == HGCellFloorType.Lift)
                {
                    HGLift lift = cellCursor.liftIndex >= 0 && cellCursor.liftIndex < _mapData.lifts.Length ? _mapData.lifts[cellCursor.liftIndex] : null;
                    if (lift != null)
                    {
                        cursorCoordsString += $"\r\nLift {lift.index}, initial: " + (lift.initial == 0 ? "Stop" : lift.initial == 1 ? "Up" : "Down");
                        cursorCoordsString += (ZLevel == lift.minZ ? "\r\n[" : "\r\n") + $"{lift.minZ}" + (ZLevel == lift.minZ ? "]" : "") + " <-> " + (ZLevel == lift.maxZ ? "[" : "") + $"{lift.maxZ}" + (ZLevel == lift.maxZ ? "]" : "") + $", carry: {lift.carry.ToString()}";
                    }
                }
                if (cellCursor.blockHere && cellCursor.blockType == HGCellBlockType.FitnessBoost)
                {
                    cursorCoordsString += $"\r\nFitness Boost: {_mapData.ConvertFitnessBoostValue(cellCursor.healingValue, false)}({cellCursor.healingValue})";
                }
                for(int i = 0; i < _mapData.conversations.Length; i++)
                {
                    HGConversation conv = _mapData.conversations[i];
                    if(conv == null || string.IsNullOrEmpty(conv.text) || conv.text.Length == 0 || conv.coords.Equals(_coordsCursor) == false)
                    {
                        continue;
                    }
                    if(conv.coords.Equals(_coordsCursor))
                    {
                        cursorCoordsString += $"\r\n{i}: {HelperUtil.WordWrap(conv.text,32,new char[] { ' ', ',' })}";
                        break;
                    }
                }
            }
            cursorCoordsString = cursorCoordsString.TrimStart(new char[] {'\r', '\n' });
            double bufferMultiplier = 0.75;
            //SizeF stringSizeBuffer = g.MeasureString(cursorCoordsString.Length >= extCursorInfoString.Length ? cursorCoordsString : extCursorInfoString.Trim(), EditorTabControl.Font);
            Size stringSizeBuffer = TextRenderer.MeasureText(cursorCoordsString, cursorFont);
            stringSizeBuffer = new Size((int)Math.Round((double)stringSizeBuffer.Width * bufferMultiplier),(int)Math.Round((double)stringSizeBuffer.Height * bufferMultiplier));
            int cursorLines = cursorCoordsString.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Length;
            int w = (MapViewPictureBox.Width < MapViewPanel.Width - (MapViewPanel.HorizontalScroll.Visible ? 16 : 0) ? MapViewPictureBox.Width : MapViewPanel.Width + MapViewPanel.HorizontalScroll.Value - (MapViewPanel.HorizontalScroll.Visible ? 16 : 0)) - 2;
            int h = (MapViewPictureBox.Height < MapViewPanel.Height - (MapViewPanel.VerticalScroll.Visible ? 16 : 0) ? MapViewPictureBox.Height : MapViewPanel.Height + MapViewPanel.VerticalScroll.Value - (MapViewPanel.VerticalScroll.Visible ? 16 : 0)) - 2;
            Point pos = new Point(HelperUtil.Clamp<int>(_mapViewMousePos.X + 16,0, w - (int)stringSizeBuffer.Width), HelperUtil.Clamp<int>(_mapViewMousePos.Y + 24, 0, h - (int)stringSizeBuffer.Height));
            if (ShowExtendedCursorInfo && cellCursor.panelHere && cellCursor.panelType == HGCellPanelType.Text)
            {
                pos = new Point(HelperUtil.Clamp<int>(pos.X, 0, w - Math.Min((int)stringSizeBuffer.Width, 39)), HelperUtil.Clamp<int>(pos.Y, 0, h - (int)stringSizeBuffer.Height - 40));
                g.DrawImage(_panelButtons[cellCursor.panelButtonIndex].Image, pos.X, pos.Y + (int)stringSizeBuffer.Height * cursorLines, 39, 40);
            }
            GraphicsPath p = new GraphicsPath();
            InterpolationMode interpolModeTemp = g.InterpolationMode;
            SmoothingMode smoothingModeTemp = g.SmoothingMode;
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            p.AddString(
                cursorCoordsString,             // text to draw
                cursorFont.FontFamily,  // or any other font family
                (int)FontStyle.Bold,      // font style (bold, italic, etc.)
                cursorFont.Size,       // em size
                pos,              // location where to draw text
                new StringFormat());          // set options here (e.g. center alignment)
            using (Pen outlinePen = new Pen(_cursorInsideMapView ? Color.Black : Color.Transparent, 4))
            {
                g.DrawPath(outlinePen, p);
                g.FillPath(new SolidBrush(_cursorInsideMapView ? Color.White : Color.Transparent), p);
            }

            g.InterpolationMode = interpolModeTemp;
            g.SmoothingMode = smoothingModeTemp;

            //g.Flush(FlushIntention.Sync);
            //g.Dispose();
        }
       
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: CELL
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BlockTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_mapData == null)
            {
                return;
            }
            if(BlockTypeDropDownList.SelectedIndex == BlockTypeDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetBlockType(_coordsSelected[i], GetBlockTypeFromIndex(BlockTypeDropDownList.SelectedIndex), BlockTypeDropDownList.SelectedIndex != 0 && BlockTypeDropDownList.SelectedIndex != BlockTypeDropDownList.Items.Count - 1,setControl : false, setCell : ActiveModeToggleButton.Checked);
                }
                if (BlockTypeDropDownList.SelectedIndex == 2)
                {
                    BlockPushableCheckBox.CheckState = CheckState.Checked;
                }
                else
                {
                    BlockPushableCheckBox.CheckState = CheckState.Unchecked;
                }
                if (BlockTypeDropDownList.SelectedIndex == 1 || BlockTypeDropDownList.SelectedIndex == 2)
                {
                    BlockOpaqueCheckBox.CheckState = CheckState.Checked;
                }
                else
                {
                    BlockOpaqueCheckBox.CheckState = CheckState.Unchecked;
                }
                if (BlockTypeDropDownList.SelectedIndex == 13)
                {
                    AuxiliaryDropDownList.SelectedIndex = 12;
                }
                if (BlockTypeDropDownList.SelectedIndex == 14)
                {
                    AuxiliaryDropDownList.SelectedIndex = 13;
                }
                TeleportTargetPosLabel.Enabled = BlockTypeDropDownList.SelectedIndex == GetIndexFromBlockType(HGCellBlockType.Teleport);
                TeleportTargetPosXTextBox.Enabled = TeleportTargetPosLabel.Enabled;
                TeleportTargetPosYTextBox.Enabled = TeleportTargetPosLabel.Enabled;
                TeleportTargetPosZTextBox.Enabled = TeleportTargetPosLabel.Enabled;
                if (BlockTypeDropDownList.SelectedIndex != GetIndexFromBlockType(HGCellBlockType.Teleport))
                {
                    TeleportTargetPosXTextBox.Text = "0";
                    TeleportTargetPosYTextBox.Text = "0";
                    TeleportTargetPosZTextBox.Text = "0";
                }
                FitnessBoostValueLabel.Enabled = BlockTypeDropDownList.SelectedIndex == GetIndexFromBlockType(HGCellBlockType.FitnessBoost);
                FitnessBoostValueTextBox.Enabled = FitnessBoostValueLabel.Enabled;
                if (BlockTypeDropDownList.SelectedIndex != GetIndexFromBlockType(HGCellBlockType.FitnessBoost))
                {
                    FitnessBoostValueTextBox.Text = "0";
                }
                if (ActiveModeToggleButton.Checked)
                {
                    _mapData.UpdateMapTiles(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if(ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetBlockType(Coordinate3 coords, HGCellBlockType blockType, bool add = true, bool setControl = true, bool setCell = true)
        {
            if(_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.blockType = blockType;
                cell.blockHere = add;
                if (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW)
                {
                    AddDoor(coords);
                }
                else
                {
                    RemoveDoor(coords);
                }
                if (cell.blockType != HGCellBlockType.Teleport)
                {
                    SetTeleportTargetPosX(coords,0,setControl : false, setCell : ActiveModeToggleButton.Checked);
                    SetTeleportTargetPosY(coords, 0, setControl: false, setCell: ActiveModeToggleButton.Checked);
                    SetTeleportTargetPosZ(coords, 0, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                if (cell.blockType != HGCellBlockType.FitnessBoost)
                {
                    SetFitnessBoostValue(coords, 0, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
            }
            if(setControl)
            {
                BlockTypeDropDownList.SelectedIndex = add ? HelperUtil.Clamp<int>(GetIndexFromBlockType(_mapData.GetCell(coords).blockType), 0, BlockTypeDropDownList.Items.Count - 1) : 0;
            }
        }
        private HGCellBlockType GetBlockTypeFromIndex(int i)
        {
            i = HelperUtil.Clamp<int>(i,1, BlockTypeDropDownList.Items.Count - 1);
            return (HGCellBlockType)((i - 1 + (i - 1 >= 8 ? 8 : 0)) * 8);
        }
        private int GetIndexFromBlockType(HGCellBlockType blockType)
        {
            int i = (int)blockType / 8;
            return i - (i >= 8 ? 8 : 0) + 1;
        }
        private void BlockPushableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetPushable(_coordsSelected[i], BlockPushableCheckBox.Checked, setControl : false, setCell : ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetPushable(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.pushable = add;
            }
            if (setControl)
            {
                BlockPushableCheckBox.Checked = add;
            }
        }
        private void BlockInvisibleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetInvisible(_coordsSelected[i], BlockInvisibleCheckBox.Checked, setControl : false, setCell : ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetInvisible(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.invisible = add;
            }
            if (setControl)
            {
                BlockInvisibleCheckBox.Checked = add;
            }
        }
        private void BlockOpaqueCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetOpaque(_coordsSelected[i], BlockOpaqueCheckBox.Checked, setControl: false, setCell : ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetOpaque(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.opaque = add;
            }
            if (setControl)
            {
                BlockOpaqueCheckBox.Checked = add;
            }
        }
        private void TeleportTargetPosXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posX = 0;
                    int.TryParse(TeleportTargetPosXTextBox.Text, out posX);
                    SetTeleportTargetPosX(_coordsSelected[i], posX, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                MapViewPictureBox.Invalidate();
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetTeleportTargetPosX(Coordinate3 coords, int posX, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.teleportTargetPos.x = posX;
            }
            if (setControl)
            {
                ButtonTargetInPositionXTextBox.Text = posX.ToString();
            }
        }
        private void TeleportTargetPosYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posY = 0;
                    int.TryParse(TeleportTargetPosYTextBox.Text, out posY);
                    SetTeleportTargetPosY(_coordsSelected[i], posY, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                MapViewPictureBox.Invalidate();
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetTeleportTargetPosY(Coordinate3 coords, int posY, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.teleportTargetPos.y = posY;
            }
            if (setControl)
            {
                TeleportTargetPosYTextBox.Text = posY.ToString();
            }
        }
        private void TeleportTargetPosZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posZ = 0;
                    int.TryParse(TeleportTargetPosZTextBox.Text, out posZ);
                    SetTeleportTargetPosZ(_coordsSelected[i], posZ, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                MapViewPictureBox.Invalidate();
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetTeleportTargetPosZ(Coordinate3 coords, int posZ, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.teleportTargetPos.z = posZ;
            }
            if (setControl)
            {
                TeleportTargetPosZTextBox.Text = posZ.ToString();
            }
        }
        private void FitnessBoostValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int fitnessBoostValue = 0;
                    int.TryParse(FitnessBoostValueTextBox.Text, out fitnessBoostValue);
                    SetFitnessBoostValue(_coordsSelected[i], fitnessBoostValue, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetFitnessBoostValue(Coordinate3 coords, int fitnessBoostValue, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.healingValue = _mapData.ConvertFitnessBoostValue(fitnessBoostValue, true);
            }
            if (setControl)
            {
                FitnessBoostValueTextBox.Text = fitnessBoostValue.ToString();
            }
        }
        private void ForceQuestionMarkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetForceQuestionMark(_coordsSelected[i], ForceQuestionMarkCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetForceQuestionMark(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.forceQuestionMark = add;
            }
            if (setControl)
            {
                ForceQuestionMarkCheckBox.Checked = add;
            }
        }
        private void FloorTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (FloorTypeDropDownList.SelectedIndex == FloorTypeDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetFloorType(_coordsSelected[i], GetFloorTypeFromIndex(FloorTypeDropDownList.SelectedIndex), FloorTypeDropDownList.SelectedIndex != 0 && FloorTypeDropDownList.SelectedIndex != FloorTypeDropDownList.Items.Count - 1, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                if(ActiveModeToggleButton.Checked)
                {
                    _mapData.UpdateMapTiles(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetFloorType(Coordinate3 coords, HGCellFloorType floorType, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.floorType = floorType;
                cell.floorHere = add;
                if (cell.floorType == HGCellFloorType.Lift)
                {
                    AddLift(coords);
                }
                else
                {
                    RemoveLift(coords);
                }
                if (cell.floorType == HGCellFloorType.Button)
                {
                    if (cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut))
                    {
                        AddConsoleMessage($"[SetPanelType] Removed panel button at same position as new floor button ({coords})");
                        SetPanelType(coords, HGCellPanelType.Text, false, setControl: false, setCell: true);
                    }
                    if (coords.z > 0)
                    {
                        Coordinate3 coordsBelow = new Coordinate3(coords.x, coords.y, coords.z - 1);
                        HGCell cellBelow = _mapData.GetCell(coordsBelow);
                        if (cellBelow.panelHere && (cellBelow.panelType == HGCellPanelType.ButtonIn || cellBelow.panelType == HGCellPanelType.ButtonOut))
                        {
                            AddConsoleMessage($"Removed panel button below new floor button ({coords})");
                            SetPanelType(coordsBelow, HGCellPanelType.Text, false, setControl: false, setCell: true);
                        }
                    }
                    AddButton(coords);
                }
                else if(cell.panelHere == false || (cell.panelType != HGCellPanelType.ButtonIn && cell.panelType != HGCellPanelType.ButtonOut))
                {
                    RemoveButton(coords);
                }
            }
            if (setControl)
            {
                FloorTypeDropDownList.SelectedIndex = add ? HelperUtil.Clamp<int>(GetIndexFromFloorType(_mapData.GetCell(coords).floorType), 0, FloorTypeDropDownList.Items.Count - 1) : 0;
            }
        }
        private HGCellFloorType GetFloorTypeFromIndex(int i)
        {
            i = HelperUtil.Clamp<int>(i, 1, FloorTypeDropDownList.Items.Count - 1);
            return (HGCellFloorType)((i - 1) * 2);
        }
        private int GetIndexFromFloorType(HGCellFloorType floorType)
        {
            return (int)floorType / 2 + 1;
        }
        private void PanelTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (PanelTypeDropDownList.SelectedIndex == PanelTypeDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetPanelType(_coordsSelected[i], GetPanelTypeFromIndex(PanelTypeDropDownList.SelectedIndex), PanelTypeDropDownList.SelectedIndex != 0 && PanelTypeDropDownList.SelectedIndex != PanelTypeDropDownList.Items.Count - 1, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetPanelType(Coordinate3 coords, HGCellPanelType panelType, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                bool removeButton = (add == false && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (add && cell.panelType != HGCellPanelType.ButtonIn && cell.panelType != HGCellPanelType.ButtonOut);
                cell.panelType = panelType;
                cell.panelHere = add;
                if(cell.panelHere)
                {
                    if (cell.panelType == HGCellPanelType.Text)
                    {
                        cell.panelButtonIndex = PanelSelected;
                    }
                    if (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)
                    {
                        if (cell.floorHere && cell.floorType == HGCellFloorType.Button)
                        {
                            AddConsoleMessage($"[SetPanelType] Removed floor button at same position as new panel button ({coords})");
                            SetFloorType(coords, HGCellFloorType.Grass, false, setControl: false, setCell: true);
                        }
                        if (coords.z < HGMapData.MAP_LEVELS - 1)
                        {
                            Coordinate3 coordsAbove = new Coordinate3(coords.x, coords.y, coords.z + 1);
                            HGCell cellAbove = _mapData.GetCell(coordsAbove);
                            if (cellAbove.floorHere && cellAbove.floorType == HGCellFloorType.Button)
                            {
                                AddConsoleMessage($"Removed floor button above new panel button ({coords})");
                                SetFloorType(coordsAbove, HGCellFloorType.Grass, false, setControl: false, setCell: true);
                            }
                        }
                        AddButton(coords);
                    }
                    else if (cell.floorHere == false || cell.floorType != HGCellFloorType.Button)
                    {
                        RemoveButton(coords);
                    }
                }
                else if (removeButton && (cell.floorHere == false || cell.floorType != HGCellFloorType.Button))
                {
                    RemoveButton(coords);
                }
            }
            if (setControl)
            {
                PanelTypeDropDownList.SelectedIndex = add ? HelperUtil.Clamp<int>(GetIndexFromPanelType(_mapData.GetCell(coords).panelType), 0, PanelTypeDropDownList.Items.Count - 1) : 0;
            }
        }
        private HGCellPanelType GetPanelTypeFromIndex(int i)
        {
            i = HelperUtil.Clamp<int>(i, 1, PanelTypeDropDownList.Items.Count - 1);
            return (HGCellPanelType)((i - 1) * 8);
        }
        private int GetIndexFromPanelType(HGCellPanelType panelType)
        {
            return (int)panelType / 8 + 1;
        }
        private void WaterDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (WaterDropDownList.SelectedIndex == WaterDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetWaterLevel(_coordsSelected[i], GetWaterLevelFromIndex(WaterDropDownList.SelectedIndex), WaterDropDownList.SelectedIndex != 0 && WaterDropDownList.SelectedIndex != WaterDropDownList.Items.Count - 1, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetWaterLevel(Coordinate3 coords, int waterLevel, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.waterLevel = HelperUtil.Clamp<int>(waterLevel,0,3);
                cell.waterHere = add;
            }
            if (setControl)
            {
                WaterDropDownList.SelectedIndex = add ? HelperUtil.Clamp<int>(waterLevel + 1, 0, WaterDropDownList.Items.Count - 1) : 0;
            }
        }
        private int GetWaterLevelFromIndex(int i)
        {
            i = HelperUtil.Clamp<int>(i, 1, BlockTypeDropDownList.Items.Count - 1);
            return (i - 1);
        }
        private void TidalWaterFlowingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetTidalWaterFlowing(_coordsSelected[i], TidalWaterFlowingCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetTidalWaterFlowing(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.tidalWaterFlowing = add;
            }
            if (setControl)
            {
                TidalWaterFlowingCheckBox.Checked = add;
            }
        }
        private void FillWaterButton_Click(object sender, EventArgs e)
        {
            FillWater();
        }
        private void FillWater()
        {
            if (_mapData == null)
            {
                return;
            }
            for(int y = 0; y < HGMapData.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < HGMapData.MAP_WIDTH; x++)
                {
                    HGCell cell = _mapData.GetCell(x,y,ZLevel);
                    if(cell.blockHere == false || cell.blockType != HGCellBlockType.Stone)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, ZLevel);
                        SetWaterLevel(coords, GetWaterLevelFromIndex(WaterDropDownList.SelectedIndex), WaterDropDownList.SelectedIndex != 0 && WaterDropDownList.SelectedIndex != WaterDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        SetTidalWaterFlowing(coords, TidalWaterFlowingCheckBox.Checked, setControl: false, setCell: true);
                    }
                }
            }
            for(int i = 0; i < _mapData.buttons.Length; i++)
            {
                HGButton btn = _mapData.buttons[i];
                if(btn != null)
                {
                    if((btn.actionPushed == HGButtonAction.BlockOff || btn.actionPushed == HGButtonAction.ToggleBlock) && btn.targetPosReleased.z == ZLevel)
                    {
                        SetWaterLevel(btn.targetPosPushed, GetWaterLevelFromIndex(WaterDropDownList.SelectedIndex), WaterDropDownList.SelectedIndex != 0 && WaterDropDownList.SelectedIndex != WaterDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        SetTidalWaterFlowing(btn.targetPosPushed, TidalWaterFlowingCheckBox.Checked, setControl: false, setCell: true);
                    }
                    if ((btn.actionReleased == HGButtonAction.BlockOff || btn.actionReleased == HGButtonAction.ToggleBlock) && btn.targetPosReleased.z == ZLevel)
                    {
                        SetWaterLevel(btn.targetPosReleased, GetWaterLevelFromIndex(WaterDropDownList.SelectedIndex), WaterDropDownList.SelectedIndex != 0 && WaterDropDownList.SelectedIndex != WaterDropDownList.Items.Count - 1, setControl: false, setCell: true);
                        SetTidalWaterFlowing(btn.targetPosReleased, TidalWaterFlowingCheckBox.Checked, setControl: false, setCell: true);
                    }
                }
            }
            AddStep();
        }
        private void ClearWaterButton_Click(object sender, EventArgs e)
        {
            ClearWater();
        }
        private void ClearWater()
        {
            if (_mapData == null)
            {
                return;
            }
            for (int y = 0; y < HGMapData.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < HGMapData.MAP_WIDTH; x++)
                {
                    Coordinate3 coords = new Coordinate3(x, y, ZLevel);
                    SetWaterLevel(coords, 0, false, setControl: false, setCell: true);
                    SetTidalWaterFlowing(coords, false, setControl: false, setCell: true);
                }
            }
            AddStep();
        }
        private void AuxiliaryDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (AuxiliaryDropDownList.SelectedIndex == AuxiliaryDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                HGCellAuxiliaryType auxType = GetAuxiliaryTypeFromIndex(AuxiliaryDropDownList.SelectedIndex);
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetAuxiliaryType(_coordsSelected[i], auxType, AuxiliaryDropDownList.SelectedIndex != 0 && AuxiliaryDropDownList.SelectedIndex != AuxiliaryDropDownList.Items.Count - 1, setControl: false, setCell : ActiveModeToggleButton.Checked);
                }
                if(ItemDropDownList.SelectedIndex != 0 && (auxType != HGCellAuxiliaryType.Container1 && auxType != HGCellAuxiliaryType.Container2 && auxType != HGCellAuxiliaryType.Container3 && auxType != HGCellAuxiliaryType.Container4 && auxType != HGCellAuxiliaryType.Container5))
                {
                    ItemDropDownList.SelectedIndex = 0;
                }
                if (ActiveModeToggleButton.Checked)
                {
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetAuxiliaryType(Coordinate3 coords, HGCellAuxiliaryType auxiliaryType, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.auxiliaryType = auxiliaryType;
                cell.auxiliaryHere = add;
                if(add && auxiliaryType == HGCellAuxiliaryType.Monster)
                {
                    cell.monsterType = MonsterSelected;
                    cell.monsterLevel = MonsterLevelDropDownList.SelectedIndex;
                }
            }
            if (setControl)
            {
                AuxiliaryDropDownList.SelectedIndex = add ? HelperUtil.Clamp<int>(GetIndexFromAuxiliaryType(_mapData.GetCell(coords).auxiliaryType), 0, BlockTypeDropDownList.Items.Count - 1) : 0;
            }
        }
        private HGCellAuxiliaryType GetAuxiliaryTypeFromIndex(int i)
        {
            i = HelperUtil.Clamp<int>(i, 1, AuxiliaryDropDownList.Items.Count - 1);
            return (HGCellAuxiliaryType)((i - 1 + (i - 1 >= 7 ? 1 : 0)) * 16);
        }
        private int GetIndexFromAuxiliaryType(HGCellAuxiliaryType auxType)
        {
            int i = (int)auxType / 16;
            return i - (i >= 7 ? 1 : 0) + 1;
        }
        private void EggDelayTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            int eggDelay = 0;
            if (blockHandlerKeep == false)
            {
                bool b = blockHandlerKeep;
                blockHandlerKeep = true;
                int.TryParse(EggDelayTextBox.Text, out eggDelay);
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetEggDelay(_coordsSelected[i], eggDelay,setTextBoxControl: false, setDropdownListControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                blockHandlerKeep = b;
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void EggDelayDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blockHandlerKeep == false)
            {
                int eggDelay = 0;
                switch (EggDelayDropDownList.SelectedIndex)
                {
                    case 1:
                        eggDelay = 0;
                        break;
                    case 2:
                        eggDelay = 4093;
                        break;
                    case 3:
                        eggDelay = 4094;
                        break;
                    case 4:
                        eggDelay = 4095;
                        break;
                    default:
                        eggDelay = 0;
                        break;
                }
                bool b = blockHandlerKeep;
                blockHandlerKeep = true;
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetEggDelay(_coordsSelected[i], eggDelay, setTextBoxControl: true, setDropdownListControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                blockHandlerKeep = b;
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetEggDelay(Coordinate3 coords, int eggDelay, bool setTextBoxControl = true, bool setDropdownListControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.eggDelay = eggDelay;
            }
            if (setTextBoxControl)
            {
                EggDelayTextBox.Text = eggDelay.ToString();
            }
            if (setDropdownListControl)
            {
                EggDelayDropDownList.SelectedIndex = GetEggDelayIndexFromEggdelay(eggDelay);
            }
        }
        private int GetEggDelayIndexFromEggdelay(int eggDelay)
        {
            switch (eggDelay)
            {
                case 0:
                    return 1;
                case 4093:
                    return 2;
                case 4094:
                    return 3;
                case 4095:
                    return 4;
                default:
                    return 0;
            }
        }
        private void EggBlockMonsterCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetEggBlockMonster(_coordsSelected[i], EggBlockMonsterCheckBox.Checked, setControl: false, setCell:ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetEggBlockMonster(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.eggBlocked = add;
            }
            if (setControl)
            {
                EggBlockMonsterCheckBox.Checked = add;
            }
        }
        private void EggEraseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetEggErase(_coordsSelected[i], EggEraseCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetEggErase(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.eggErase = add;
            }
            if (setControl)
            {
                EggEraseCheckBox.Checked = add;
            }
        }
        private void SkyNCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetSky(_coordsSelected[i], 0, SkyNCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SkyECheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetSky(_coordsSelected[i], 1, SkyECheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SkySCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetSky(_coordsSelected[i], 2, SkySCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SkyWCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetSky(_coordsSelected[i], 3, SkyWCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetSky(Coordinate3 coords, int skyIndex, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.skyRendered[HelperUtil.Clamp<int>(skyIndex,0,cell.skyRendered.Length - 1)] = add;
            }
            if (setControl)
            {
                switch(skyIndex)
                {
                    case 0:
                        SkyNCheckBox.Checked = add;
                        break;
                    case 1:
                        SkyECheckBox.Checked = add;
                        break;
                    case 2:
                        SkySCheckBox.Checked = add;
                        break;
                    case 3:
                        SkyWCheckBox.Checked = add;
                        break;
                    default:
                        break;
                }
            }
        }
        private void FillSkyButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Function disabled.");
        }
        private void ItemIDTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            int itemID = 0;
            int.TryParse(ItemIDTextBox.Text, out itemID);
            itemID = HelperUtil.Clamp<int>(itemID,0, ItemDropDownList.Items.Count - 1);
            if (itemID == ItemDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetItemID(_coordsSelected[i], itemID, setListControl: true, setTextBoxControl : false, setCell: ActiveModeToggleButton.Checked);
                }
                //"None", "Monster", "Egg open", "Container 1", "Container 2", "Container 3", "Container 4", "Container 5"
                HGItem item = _items[itemID];
                AuxiliaryDropDownList.SelectedIndex = GetIndexFromAuxiliaryType(item.container);
                ItemDamagedTextBox.Text = "0";
                ItemAmmunitionTextBox.Text = itemID > 0 ? item.ammo.ToString() : "0";
                if (ActiveModeToggleButton.Checked)
                {
                    _mapData.UpdateMapTiles(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void ItemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (ItemDropDownList.SelectedIndex == ItemDropDownList.Items.Count - 1)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetItemID(_coordsSelected[i], ItemDropDownList.SelectedIndex, setListControl: false, setTextBoxControl : true, setCell: ActiveModeToggleButton.Checked);
                }
                HGItem item = _items[ItemDropDownList.SelectedIndex];
                AuxiliaryDropDownList.SelectedIndex = GetIndexFromAuxiliaryType(item.container);
                AuxiliaryDropDownList.SelectedIndex = GetIndexFromAuxiliaryType(item.container);
                ItemDamagedTextBox.Text = "0";
                ItemAmmunitionTextBox.Text = ItemDropDownList.SelectedIndex > 0 ? item.ammo.ToString() : "0";
                if (ActiveModeToggleButton.Checked)
                {
                    _mapData.UpdateMapTiles(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                    UpdateMapView(_coordsSelected[0], _coordsSelected[_coordsSelected.Count - 1]);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
       
        private void SetItemID(Coordinate3 coords, int itemID, bool setListControl = true, bool setTextBoxControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            itemID = HelperUtil.Clamp<int>(itemID, 0, ItemDropDownList.Items.Count - 1);
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.itemID = itemID;
            }
            if (setListControl)
            {
                ItemDropDownList.SelectedIndex = itemID;
            }
            if (setTextBoxControl)
            {
                ItemIDTextBox.Text = itemID.ToString();
            }
            //HGItem item = _items[ItemDropDownList.SelectedIndex];
            //SetItemDamage(coords,item.damage, setControl : true);
            //SetItemAmmo(coords, itemID != 0 ? item.ammo : 0, setControl : true);
            //SetAuxiliaryType(coords, itemID != 0 ? item.container : HGCellAuxiliaryType.Monster,itemID != 0, setControl : true);
        }
        private void ItemCategoryFilterDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if(i >= ItemDropDownList.Items.Count)
                {
                    break;
                }
                HGItem item = _items[i];
                ItemDropDownList.Items[i] = ((ItemCategoryFilterDropdownList.SelectedIndex == 0) || (ItemCategoryFilterDropdownList.SelectedIndex > 0 && ItemCategoryFilterDropdownList.SelectedIndex - 1 == (int)item.category)) ? item.name : i.ToString().PadLeft(3);
            }
        }
        private void ItemCategoryFilterDropdownList_DrawItem(object sender, DrawItemEventArgs e)
        {
            //You need to change ComboBox.DrawMode to OwnerDrawFixed/OwnerDrawVariable to fire the comboBox_DrawItem
            int index = e.Index >= 0 ? e.Index : -1;
            ItemCategoryFilterDropdownList.BackColor = ItemCategoryFilterDropdownList.SelectedIndex > 0 ? highlightColor : SystemColors.Window;
            e.DrawBackground();
            if (index != -1)
            {
                e.Graphics.DrawString(ItemCategoryFilterDropdownList.Items[index].ToString(), e.Font, new SolidBrush(ItemCategoryFilterDropdownList.ForeColor), e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }
        private void ItemDamagedTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int damage = 0;
                    int.TryParse(ItemDamagedTextBox.Text, out damage);
                    SetItemDamage(_coordsSelected[i], damage, setControl: true, setCell : ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        
        private void SetItemDamage(Coordinate3 coords, int damage, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.itemDamaged = damage;
            }
            if (setControl)
            {
                ItemDamagedTextBox.Text = damage.ToString();
            }
        }
        private void ItemAmmunitionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int ammo = 0;
                    int.TryParse(ItemAmmunitionTextBox.Text, out ammo);
                    SetItemAmmo(_coordsSelected[i], ammo, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetItemAmmo(Coordinate3 coords, int ammo, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if(setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.itemAmmo = ammo;
            }
            if (setControl)
            {
                ItemAmmunitionTextBox.Text = ammo.ToString();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: MONSTER
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void MonsterButton_Click(object sender, EventArgs e)
        {
            MonsterButton btn = sender as MonsterButton;
            SelectMonster(btn.monsterIndex);
        }
        public void MonsterLoadButton_Click(object sender, EventArgs e)
        {
            if (_monsterButtons[MonsterSelected].MonsterLoaded)
            {
                UnloadMonster(MonsterSelected);
            }
            else
            {
                LoadMonster(MonsterSelected);
            }
        }
        private void MonsterLevelDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            HGCharacter monster = _monsters[_monsterSelected];
            HGCharacterLevel lvl = monster.levels[MonsterLevelDropDownList.SelectedIndex];
            MonsterLevelDescriptionLabel.Text = lvl.description + (lvl.shielded ? "\r\n\r\n*Shielded." : "");
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetMonsterLevel(_coordsSelected[i], MonsterLevelDropDownList.SelectedIndex, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetMonsterLevel(Coordinate3 coords, int level, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGCell cell = _mapData.GetCell(coords);
                cell.monsterLevel = level;
            }
            if (setControl)
            {
                MonsterLevelDropDownList.SelectedIndex = level;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: PANEL
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PanelButton_Click(object sender, EventArgs e)
        {
            PanelButton btn = sender as PanelButton;
            SelectPanel(btn.panelIndex);
        }
        private void PanelsImportButton_Click(object sender, EventArgs e)
        {
            ImportPanelsOpenFileDialogue();
        }
        private void ImportPanelsOpenFileDialogue()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = editorDir;
            openFileDialog.Filter = "PNG files (*.png)|*.png";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream inStream;
                if ((inStream = openFileDialog.OpenFile()) != null)
                {
                    ImportPanels(openFileDialog.FileName);
                }
                inStream.Close();
                AddStep();
            }
        }
        private void ImportPanels(string path)
        {
            if (_mapData == null)
            {
                return;
            }
            Bitmap original = new Bitmap(path);
            if (original.Width < HGMapData.MAP_PANELWIDTH * HGMapData.MAP_PANELSPERROW || original.Height < HGMapData.MAP_PANELHEIGHT * HGMapData.MAP_PANELSPERCOLUMN)
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
            _mapData.panels = new Bitmap[HGMapData.MAP_PANELS];
            for (int i = 0; i < _mapData.panels.Length; i++)
            {
                using (BmpPixelSnoop snoopImg = new BmpPixelSnoop(img))
                {
                    Bitmap bm = new Bitmap(HGMapData.MAP_PANELWIDTH, HGMapData.MAP_PANELHEIGHT, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                    {
                        for (int y = 0; y < bm.Height; y++)
                        {
                            for (int x = 0; x < bm.Width; x++)
                            {
                                //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                                //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                                snoop.SetPixel(x,y, snoopImg.GetPixel((i % HGMapData.MAP_PANELSPERROW) * HGMapData.MAP_PANELWIDTH + x, (i / HGMapData.MAP_PANELSPERCOLUMN) * HGMapData.MAP_PANELHEIGHT + y));
                            }
                        }
                    }
                    _mapData.panels[i] = bm;
                }
            }
            UpdatePanels();
        }
        private void PanelsExportToBitmapButton_Click(object sender, EventArgs e)
        {
            if(_mapData == null)
            {
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Select output filename and directory...";
            saveFileDialog.InitialDirectory = editorDir;
            saveFileDialog.Filter = "PNG files (*.png)|*.png";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bm = new Bitmap(PanelsBlankPictureBox.Image);
                for(int i = 0; i < _mapData.panels.Length; i++)
                {
                    if(_mapData.panels[i] == null)
                    {
                        continue;
                    }
                    HelperUtil.CopyRegionIntoImage(_mapData.panels[i], new Rectangle(6, 0, 39, 40), ref bm, new Rectangle((i % 6) * HGMapData.MAP_PANELWIDTH + 6, (i / 6) * HGMapData.MAP_PANELHEIGHT, 39, 40));
                    bm.Save(saveFileDialog.FileName);
                    
                }
                bm.Dispose();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: BUTTON
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ButtonTargetInPositionXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posX = 0;
                    int.TryParse(ButtonTargetInPositionXTextBox.Text, out posX);
                    SetButtonTargetInPositionX(_coordsSelected[i], posX, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetInPositionX(Coordinate3 coords, int posX, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosPushed.x = posX;
            }
            if (setControl)
            {
                ButtonTargetInPositionXTextBox.Text = posX.ToString();
            }
        }
        private void ButtonTargetInPositionYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posY = 0;
                    int.TryParse(ButtonTargetInPositionYTextBox.Text, out posY);
                    SetButtonTargetInPositionY(_coordsSelected[i], posY, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetInPositionY(Coordinate3 coords, int posY, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosPushed.y = posY;
            }
            if (setControl)
            {
                ButtonTargetInPositionYTextBox.Text = posY.ToString();
            }
        }
        private void ButtonTargetInPositionZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posZ = 0;
                    int.TryParse(ButtonTargetInPositionZTextBox.Text, out posZ);
                    SetButtonTargetInPositionZ(_coordsSelected[i], posZ, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetInPositionZ(Coordinate3 coords, int posZ, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosPushed.z = posZ;
            }
            if (setControl)
            {
                ButtonTargetInPositionZTextBox.Text = posZ.ToString();
            }
        }
        private void ButtonTargetOutPositionXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posX = 0;
                    int.TryParse(ButtonTargetOutPositionXTextBox.Text, out posX);
                    SetButtonTargetOutPositionX(_coordsSelected[i], posX, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetOutPositionX(Coordinate3 coords, int posX, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosReleased.x = posX;
            }
            if (setControl)
            {
                ButtonTargetOutPositionXTextBox.Text = posX.ToString();
            }
        }
        private void ButtonTargetOutPositionYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posY = 0;
                    int.TryParse(ButtonTargetOutPositionYTextBox.Text, out posY);
                    SetButtonTargetOutPositionY(_coordsSelected[i], posY, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetOutPositionY(Coordinate3 coords, int posY, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosReleased.y = posY;
            }
            if (setControl)
            {
                ButtonTargetOutPositionYTextBox.Text = posY.ToString();
            }
        }
        private void ButtonTargetOutPositionZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int posZ = 0;
                    int.TryParse(ButtonTargetOutPositionZTextBox.Text, out posZ);
                    SetButtonTargetOutPositionZ(_coordsSelected[i], posZ, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonTargetOutPositionZ(Coordinate3 coords, int posZ, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.targetPosReleased.z = posZ;
            }
            if (setControl)
            {
                ButtonTargetOutPositionZTextBox.Text = posZ.ToString();
            }
        }
        private void ButtonActionPushedDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetButtonActionPushed(_coordsSelected[i], (HGButtonAction)ButtonActionPushedDropDownList.SelectedIndex, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonActionPushed(Coordinate3 coords, HGButtonAction action, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.actionPushed = action;
            }
            if (setControl)
            {
                ButtonActionPushedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)action, 0, ButtonActionPushedDropDownList.Items.Count - 1);
            }
        }
        private void ButtonActionReleasedDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetButtonActionReleased(_coordsSelected[i], (HGButtonAction)ButtonActionReleasedDropDownList.SelectedIndex, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetButtonActionReleased(Coordinate3 coords, HGButtonAction action, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGButton button = _mapData.GetButton(coords, false);
                button.actionReleased = action;
            }
            if (setControl)
            {
                ButtonActionReleasedDropDownList.SelectedIndex = HelperUtil.Clamp<int>((int)action, 0, ButtonActionReleasedDropDownList.Items.Count - 1);
            }
        }
        private void ButtonsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                HGButton button = _mapData.buttons[HelperUtil.Clamp<int>(ButtonsListBox.SelectedIndex, 0, ButtonsListBox.Items.Count - 1)];
                HGCell cell = _mapData.GetCell(button.coords);
                if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cell.floorHere && cell.floorType == HGCellFloorType.Button))
                {
                    SetCoordsSelected(button.coords);
                    ZLevel = button.coords.z;
                }
            }
            SetBlockHandler(false);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: DOOR
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void DoorCloseDelayTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int closeDelay = 0;
                    int.TryParse(DoorCloseDelayTextBox.Text, out closeDelay);
                    SetDoorCloseDelay(_coordsSelected[i], closeDelay, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetDoorCloseDelay(Coordinate3 coords, int closeDelay, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGDoor door = _mapData.GetDoor(coords, false);
                if (door != null)
                {
                    door.closeDelay = closeDelay;
                }
            }
            if (setControl)
            {
                DoorCloseDelayTextBox.Text = closeDelay.ToString();
            }
        }
        private void DoorKeyIDTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int keyID = 0;
                    int.TryParse(DoorKeyIDTextBox.Text, out keyID);
                    SetDoorKeyID(_coordsSelected[i], keyID, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetDoorKeyID(Coordinate3 coords, int keyID, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGDoor door = _mapData.GetDoor(coords, false);
                if (door != null)
                {
                    door.keyID = keyID;
                }
            }
            if (setControl)
            {
                DoorKeyIDTextBox.Text = keyID.ToString();
            }
        }
        private void DoorLockedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetDoorLocked(_coordsSelected[i], DoorLockedCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetDoorLocked(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGDoor door = _mapData.GetDoor(coords, false);
                if (door != null)
                {
                    door.locked = add;
                }
            }
            if (setControl)
            {
                DoorLockedCheckBox.Checked = add;
            }
        }
        private void DoorButtonOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetDoorButtonOnly(_coordsSelected[i], DoorButtonOnlyCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetDoorButtonOnly(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGDoor door = _mapData.GetDoor(coords, false);
                if (door != null)
                {
                    door.buttonOnly = add;
                }
            }
            if (setControl)
            {
                DoorButtonOnlyCheckBox.Checked = add;
            }
        }
        private void DoorsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                HGDoor door = _mapData.doors[HelperUtil.Clamp<int>(DoorsListBox.SelectedIndex, 0, DoorsListBox.Items.Count - 1)];
                HGCell cell = _mapData.GetCell(door.coords);
                if (cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW))
                {
                    SetCoordsSelected(door.coords);
                    ZLevel = door.coords.z;
                }
            }
            SetBlockHandler(false);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: LIFT
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LiftInitialDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            /*
            if (LiftInitialDropDownList.SelectedIndex == LiftInitialDropDownList.Items.Count - 1)
            {
                return;
            }
            */
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftInitial(_coordsSelected[i], LiftInitialDropDownList.SelectedIndex, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftInitial(Coordinate3 coords, int dir, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if(lift != null)
                {
                    lift.initial = dir;
                }
            }
            if (setControl)
            {
                LiftInitialDropDownList.SelectedIndex = HelperUtil.Clamp<int>(dir, 0, LiftInitialDropDownList.Items.Count - 1);
            }
        }
        private void LiftMaxZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int maxZ = 0;
                    int.TryParse(LiftMaxZTextBox.Text, out maxZ);
                    SetLiftMaxZ(_coordsSelected[i], maxZ, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftMaxZ(Coordinate3 coords, int maxZ, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.maxZ = maxZ;
                }
            }
            if (setControl)
            {
                LiftMaxZTextBox.Text = maxZ.ToString();
            }
        }
        private void LiftMinZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    int minZ = 0;
                    int.TryParse(LiftMinZTextBox.Text, out minZ);
                    SetLiftMinZ(_coordsSelected[i], minZ, setControl: true, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftMinZ(Coordinate3 coords, int minZ, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.minZ = minZ;
                }
            }
            if (setControl)
            {
                LiftMinZTextBox.Text = minZ.ToString();
            }
        }
        private void LiftCarryCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftCarry(_coordsSelected[i], LiftCarryCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftCarry(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.carry = add;
                }
            }
            if (setControl)
            {
                LiftCarryCheckBox.Checked = add;
            }
        }
        private void LiftContinueAtTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftContinueAtTop(_coordsSelected[i], LiftContinueAtTopCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftContinueAtTop(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.continueAtTop = add;
                }
            }
            if (setControl)
            {
                LiftContinueAtTopCheckBox.Checked = add;
            }
        }
        private void LiftContinueAtBottomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftContinueAtBottom(_coordsSelected[i], LiftContinueAtBottomCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftContinueAtBottom(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.continueAtBottom = add;
                }
            }
            if (setControl)
            {
                LiftContinueAtBottomCheckBox.Checked = add;
            }
        }
        private void LiftAutomoveTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftAutomoveTop(_coordsSelected[i], LiftAutomoveTopCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftAutomoveTop(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.automove = GetLiftAutoMoveFromControls(LiftAutomoveTopCheckBox.Checked, add);
                }
            }
            if (setControl)
            {
                LiftAutomoveTopCheckBox.Checked = add;
            }
        }
        private void LiftAutomoveBottomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                for (int i = 0; i < _coordsSelected.Count; i++)
                {
                    SetLiftAutomoveBottom(_coordsSelected[i], LiftAutomoveBottomCheckBox.Checked, setControl: false, setCell: ActiveModeToggleButton.Checked);
                }
                UpdateDebugTextBox();
                if (ActiveModeToggleButton.Checked)
                {
                    AddStep();
                }
            }
            SetBlockHandler(false);
        }
        private void SetLiftAutomoveBottom(Coordinate3 coords, bool add = true, bool setControl = true, bool setCell = true)
        {
            if (_mapData == null)
            {
                return;
            }
            coords.ClampCoords();
            if (setCell)
            {
                HGLift lift = _mapData.GetLift(coords, false);
                if (lift != null)
                {
                    lift.automove = GetLiftAutoMoveFromControls(add, LiftAutomoveBottomCheckBox.Checked);
                }
            }
            if (setControl)
            {
                LiftAutomoveBottomCheckBox.Checked = add;
            }
        }
        private int GetLiftAutoMoveFromControls()
        {
            return GetLiftAutoMoveFromControls(LiftAutomoveTopCheckBox.Checked, LiftAutomoveBottomCheckBox.Checked);
        }
        private int GetLiftAutoMoveFromControls(bool boolTop, bool boolBottom)
        {
            return boolTop && boolBottom ? 3 : boolTop && !boolBottom ? 2 : !boolTop && boolBottom ? 1 : 0;
        }
        private void LiftsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                HGLift lift = _mapData.lifts[HelperUtil.Clamp<int>(LiftsListBox.SelectedIndex,0, LiftsListBox.Items.Count - 1)];
                HGCell cell = _mapData.GetCell(lift.coords);
                if (cell.floorHere && cell.floorType == HGCellFloorType.Lift)
                {
                    SetCoordsSelected(lift.coords);
                    ZLevel = lift.coords.z;
                }
            }
            SetBlockHandler(false);
        }
        private void ResetCellControls()
        {
            SetBlockHandlerKeep();
            BlockTypeDropDownList.SelectedIndex = 0;
            BlockPushableCheckBox.CheckState = CheckState.Unchecked;
            BlockInvisibleCheckBox.CheckState = CheckState.Unchecked;
            BlockOpaqueCheckBox.CheckState = CheckState.Unchecked;

            TeleportTargetPosLabel.Enabled = false;
            TeleportTargetPosXTextBox.Enabled = false;
            TeleportTargetPosYTextBox.Enabled = false;
            TeleportTargetPosZTextBox.Enabled = false;
            TeleportTargetPosXTextBox.Text = "0";
            TeleportTargetPosYTextBox.Text = "0";
            TeleportTargetPosZTextBox.Text = "0";

            FitnessBoostValueLabel.Enabled = false;
            FitnessBoostValueTextBox.Enabled = false;
            FitnessBoostValueTextBox.Text = "0";

            FloorTypeDropDownList.Items[4] = "Puddle";
            FloorTypeDropDownList.SelectedIndex = 0;


            PanelTypeDropDownList.SelectedIndex = 0;

            WaterDropDownList.SelectedIndex = 0;
            TidalWaterFlowingCheckBox.CheckState = CheckState.Unchecked;

            AuxiliaryDropDownList.SelectedIndex = 0;

            EggDelayTextBox.Text = "0";
            EggBlockMonsterCheckBox.CheckState = CheckState.Unchecked;
            EggEraseCheckBox.CheckState = CheckState.Unchecked;

            SkyNCheckBox.CheckState = CheckState.Unchecked;
            SkyECheckBox.CheckState = CheckState.Unchecked;
            SkySCheckBox.CheckState = CheckState.Unchecked;
            SkyWCheckBox.CheckState = CheckState.Unchecked;

            ItemIDTextBox.Text = "0";
            ItemDropDownList.SelectedIndex = 0;
            ItemDamagedTextBox.Text = "0";
            ItemAmmunitionTextBox.Text = "0";

            DeselectMonsterAll();
            DeselectPanelAll();
            LiftsListBox.SelectedIndex = 0;
            LiftIndexTextBox.Text = "0";
            LiftMaxZTextBox.Text = "0";
            LiftMinZTextBox.Text = "0";
            LiftInitialDropDownList.SelectedIndex = 0;
            LiftCarryCheckBox.CheckState = CheckState.Unchecked;
            LiftContinueAtTopCheckBox.CheckState = CheckState.Unchecked;
            LiftContinueAtBottomCheckBox.CheckState = CheckState.Unchecked;
            LiftAutomoveTopCheckBox.CheckState = CheckState.Unchecked;
            LiftAutomoveBottomCheckBox.CheckState = CheckState.Unchecked;

            ButtonsListBox.SelectedIndex = 0;
            ButtonIndexTextBox.Text = "0";
            ButtonTargetInPositionXTextBox.Text = "0";
            ButtonTargetInPositionYTextBox.Text = "0";
            ButtonTargetInPositionZTextBox.Text = "0";
            ButtonTargetOutPositionXTextBox.Text = "0";
            ButtonTargetOutPositionYTextBox.Text = "0";
            ButtonTargetOutPositionZTextBox.Text = "0";
            ButtonActionPushedDropDownList.SelectedIndex = 0;
            ButtonActionReleasedDropDownList.SelectedIndex = 0;

            ButtonTargetPosBytesTextBox.Text = "0";
            ButtonTargetPosFlatTextBox.Text = "0";

            DoorsListBox.SelectedIndex = 0;
            DoorIndexTextBox.Text = "0";
            DoorCloseDelayTextBox.Text = "0";
            DoorKeyIDTextBox.Text = "0";
            DoorLockedCheckBox.CheckState = CheckState.Unchecked;
            DoorButtonOnlyCheckBox.CheckState = CheckState.Unchecked;

            SetBlockHandlerKeep(false);
            MapViewPictureBox.Invalidate();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: MAP
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void WorldmapDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int[] wmdLengths = new int[] { 0,0,0,0 };
                bool tooLong = false;
                string[] worldMapDescriptionLines = WorldmapDescriptionTextBox.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < worldMapDescriptionLines.Length; i++)
                {
                    if(i < wmdLengths.Length)
                    {
                        wmdLengths[i] = worldMapDescriptionLines[i].Length;
                    }
                    if (worldMapDescriptionLines[i].Length > (i == 0 ? 45 : 60))
                    {
                        tooLong = true;
                    }
                }
                if (tooLong == false)
                {
                    _mapData.worldmapDescription = WorldmapDescriptionTextBox.Text.TrimEnd();
                }
                WorldmapDescriptionGroupBox.Text = $"Worldmap description [{ wmdLengths[0]}][{wmdLengths[1]}][{wmdLengths[2]}][{wmdLengths[3]}]";
                int sel = WorldmapDescriptionTextBox.SelectionStart;
                WorldmapDescriptionTextBox.Select(0, WorldmapDescriptionTextBox.Text.Length);
                WorldmapDescriptionTextBox.SelectionColor = WorldmapDescriptionTextBox.ForeColor;
                for(int i = 0; i < worldMapDescriptionLines.Length; i++)
                {
                    if (wmdLengths[i] > (i == 0 ? 45 : 60))
                    {
                        WorldmapDescriptionTextBox.Select((i > 0 ? wmdLengths[0] : 0 + i > 1 ? wmdLengths[1] : 0 + i > 2 ? wmdLengths[2] : 0), wmdLengths[i]);
                        WorldmapDescriptionTextBox.SelectionColor = Color.Red;
                    }
                }
                WorldmapDescriptionTextBox.Select(sel, 0);
                //AddStep();
            }
            SetBlockHandler(false);
        }
        private void DetailedDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int[] ddLengths = new int[] { 0, 0, 0, 0 };
                bool tooLong = false;
                string[] detailedDescriptionLines = DetailedDescriptionTextBox.Text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                for (int i = 0; i < detailedDescriptionLines.Length; i++)
                {
                    if (i < ddLengths.Length)
                    {
                        ddLengths[i] = detailedDescriptionLines[i].Length;
                    }
                    if (detailedDescriptionLines[i].Length > 60)
                    {
                        tooLong = true;
                    }
                }
                if(tooLong == false)
                {
                    _mapData.detailedDescription = DetailedDescriptionTextBox.Text.TrimEnd();
                }
                DetailedDescriptionGroupBox.Text = $"Detailed description [{ddLengths[0]}][{ddLengths[1]}][{ddLengths[2]}][{ddLengths[3]}]";
                int sel = DetailedDescriptionTextBox.SelectionStart;
                DetailedDescriptionTextBox.Select(0, DetailedDescriptionTextBox.Text.Length);
                DetailedDescriptionTextBox.SelectionColor = DetailedDescriptionTextBox.ForeColor;
                for (int i = 0; i < detailedDescriptionLines.Length; i++)
                {
                    if (ddLengths[i] > 60)
                    {
                        DetailedDescriptionTextBox.Select((i > 0 ? ddLengths[0] : 0 + i > 1 ? ddLengths[1] : 0 + i > 2 ? ddLengths[2] : 0), ddLengths[i]);
                        DetailedDescriptionTextBox.SelectionColor = Color.Red;
                    }
                }
                DetailedDescriptionTextBox.Select(sel, 0);
                //AddStep();
            }
            SetBlockHandler(false);
        }
        private void IntroMessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                if (IntroMessageTextBox.Text.Length <= 120)
                {
                    _mapData.introMessage = IntroMessageTextBox.Text.TrimEnd();
                }
                IntroMessageGroupBox.Text = $"Intro message [{IntroMessageTextBox.Text.Length}]";
                int sel = IntroMessageTextBox.SelectionStart;
                IntroMessageTextBox.Select(0, IntroMessageTextBox.Text.Length);
                IntroMessageTextBox.SelectionColor = IntroMessageTextBox.Text.Length <= 120 ? IntroMessageTextBox.ForeColor : Color.Red;
                IntroMessageTextBox.Select(sel, 0);
                //AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player1StartXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart1.Clone();
            int i = int.TryParse(Player1StartXTextBox.Text, out i) ? i : 0;
            _mapData.playerStart1.x = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player1StartYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart1.Clone();
            int i = int.TryParse(Player1StartYTextBox.Text, out i) ? i : 0;
            _mapData.playerStart1.y = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player1StartZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart1.Clone();
            int i = int.TryParse(Player1StartZTextBox.Text, out i) ? i : 0;
            _mapData.playerStart1.z = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player2StartXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart2.Clone();
            int i = int.TryParse(Player2StartXTextBox.Text, out i) ? i : 0;
            _mapData.playerStart2.x = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player2StartYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart2.Clone();
            int i = int.TryParse(Player2StartYTextBox.Text, out i) ? i : 0;
            _mapData.playerStart2.y = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player2StartZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart2.Clone();
            int i = int.TryParse(Player2StartZTextBox.Text, out i) ? i : 0;
            _mapData.playerStart2.z = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player3StartXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart3.Clone();
            int i = int.TryParse(Player3StartXTextBox.Text, out i) ? i : 0;
            _mapData.playerStart3.x = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player3StartYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart3.Clone();
            int i = int.TryParse(Player3StartYTextBox.Text, out i) ? i : 0;
            _mapData.playerStart3.y = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player3StartZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart3.Clone();
            int i = int.TryParse(Player3StartZTextBox.Text, out i) ? i : 0;
            _mapData.playerStart3.z = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player4StartXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart4.Clone();
            int i = int.TryParse(Player4StartXTextBox.Text, out i) ? i : 0;
            _mapData.playerStart4.x = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player4StartYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart4.Clone();
            int i = int.TryParse(Player4StartYTextBox.Text, out i) ? i : 0;
            _mapData.playerStart4.y = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player4StartZTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart4.Clone();
            int i = int.TryParse(Player4StartZTextBox.Text, out i) ? i : 0;
            _mapData.playerStart4.z = i;
            if (blockHandlerKeep == false)
            {
                UpdateMapView(GetCoordsSelected());
                UpdateMapView(coordsPlayer);
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Player1StartButton_Click(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coords = GetCoordsSelected();
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart1.Clone();
            SetBlockHandlerKeep();
            Player1StartXTextBox.Text = coords.x.ToString();
            Player1StartYTextBox.Text = coords.y.ToString();
            Player1StartZTextBox.Text = coords.z.ToString();
            SetBlockHandlerKeep(false);
            UpdateMapView(GetCoordsSelected());
            UpdateMapView(coordsPlayer);
            AddStep();
        }
        private void Player2StartButton_Click(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coords = GetCoordsSelected();
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart2.Clone();
            SetBlockHandlerKeep();
            Player2StartXTextBox.Text = coords.x.ToString();
            Player2StartYTextBox.Text = coords.y.ToString();
            Player2StartZTextBox.Text = coords.z.ToString();
            SetBlockHandlerKeep(false);
            UpdateMapView(GetCoordsSelected());
            UpdateMapView(coordsPlayer);
            AddStep();
        }
        private void Player3StartButton_Click(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coords = GetCoordsSelected();
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart3.Clone();
            SetBlockHandlerKeep();
            Player3StartXTextBox.Text = coords.x.ToString();
            Player3StartYTextBox.Text = coords.y.ToString();
            Player3StartZTextBox.Text = coords.z.ToString();
            SetBlockHandlerKeep(false);
            UpdateMapView(GetCoordsSelected());
            UpdateMapView(coordsPlayer);
            AddStep();
        }
        private void Player4Start_Click(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            Coordinate3 coords = GetCoordsSelected();
            Coordinate3 coordsPlayer = (Coordinate3)_mapData.playerStart4.Clone();
            SetBlockHandlerKeep();
            Player4StartXTextBox.Text = coords.x.ToString();
            Player4StartYTextBox.Text = coords.y.ToString();
            Player4StartZTextBox.Text = coords.z.ToString();
            SetBlockHandlerKeep(false);
            UpdateMapView(GetCoordsSelected());
            UpdateMapView(coordsPlayer);
            AddStep();
        }
        private void Destination1TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(Destination1TextBox.Text, out i) ? i : 0;
                _mapData.destination1 = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Destination2TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(Destination2TextBox.Text, out i) ? i : 0;
                _mapData.destination2 = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Destination3TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(Destination3TextBox.Text, out i) ? i : 0;
                _mapData.destination3 = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void Destination4TextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(Destination4TextBox.Text, out i) ? i : 0;
                _mapData.destination4 = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void TidalWaterSpeedTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(TidalWaterSpeedTextBox.Text, out i) ? i : 0;
                _mapData.waterSpeed = i;
                AddStep();
            }
            SetBlockHandler(false);
            UpdateTidalWaterHeightLabel();
        }
        private void TidalWaterMaxHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(TidalWaterMaxHeightTextBox.Text, out i) ? i : 0;
                _mapData.waterLevelHigh = i;
                AddStep();
            }
            SetBlockHandler(false);
            UpdateTidalWaterHeightLabel();
        }
        private void TidalWaterMinHeightTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(TidalWaterMinHeightTextBox.Text, out i) ? i : 0;
                _mapData.waterLevelLow = i;
                AddStep();
            }
            SetBlockHandler(false);
            UpdateTidalWaterHeightLabel();
        }
        private void UpdateTidalWaterHeightLabel()
        {
            if (_mapData == null)
            {
                return;
            }
            if (_mapData.waterSpeed == 0)
            {
                TidalWaterHeightLabel.Text = "Water does not flow.";
            }
            else
            {
                TidalWaterHeightLabel.Text = $"Flow: {Math.Floor((double)_mapData.waterLevelLow / 4)}-{(double)_mapData.waterLevelLow % 4} to {Math.Floor((double)_mapData.waterLevelHigh / 4)}-{(double)_mapData.waterLevelHigh % 4}";
            }
        }
        private void StyleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandler == false)
            {
                _mapData.style = StyleDropDownList.SelectedIndex;
                UpdateStyle();
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void GameTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            _mapData.gameType = GameTypeDropDownList.SelectedIndex;
            SetBlockHandler(false);
            AddStep();
        }
        private void MusicDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(_mapData == null)
            {
                return;
            }
            if (blockHandler == false)
            {
                _mapData.style = StyleDropDownList.SelectedIndex;
                UpdateStyle();
                AddStep();
            }
            SetBlockHandler(false);
        }

        private void AtmosDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandler == false)
            {
                _mapData.atmos = AtmosDropDownList.SelectedIndex;
                UpdateStyle();
                AddStep();
            }
            SetBlockHandler(false);
        }

        private void SkyDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandler == false)
            {
                _mapData.sky = SkyDropDownList.SelectedIndex;
                if(_horizonsForm != null)
                {
                    _horizonsForm.UpdateSkies();
                }
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void TimeLimitTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(TimeLimitTextBox.Text, out i) ? i : 0;
                _mapData.timeLimit = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void DiskTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(DiskTextBox.Text, out i) ? i : 0;
                _mapData.disk = i;
                AddStep();
            }
            SetBlockHandler(false);
        }

        private void FileTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(FileTextBox.Text, out i) ? i : 0;
                _mapData.file = i;
                blockHandlerKeep = true;
                DiskTextBox.Text = HGGameData.GetDiskFromFile(i).ToString();
                _mapData.disk = i;
                blockHandlerKeep = false;
                AddStep();
            }
            SetBlockHandler(false);
        }

        private void PictureTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(PictureTextBox.Text, out i) ? i : 0;
                _mapData.picture = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void PlayersTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(PlayersTextBox.Text, out i) ? i : 1;
                _mapData.players = HelperUtil.Clamp<int>(i, 1, 4);
                if (i < 1 || i > 4)
                {
                    AddConsoleMessage($"[PlayersTextBox_TextChanged] Players must be between 1 and 4!", warning: true);
                }
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void WorldPosXTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(WorldPosXTextBox.Text, out i) ? i : 0;
                _mapData.worldMapPosX = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        private void WorldPosYTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_mapData == null)
            {
                return;
            }
            if (blockHandlerKeep == false)
            {
                int i = int.TryParse(WorldPosYTextBox.Text, out i) ? i : 0;
                _mapData.worldMapPosY = i;
                AddStep();
            }
            SetBlockHandler(false);
        }
        public void SetWorldPos(int x, int y)
        {
            WorldPosXTextBox.Text = x.ToString();
            WorldPosYTextBox.Text = y.ToString();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //CONTROLS: GENERIC
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                ActiveControl = null;
            }
            if (e.KeyChar == (char)Keys.Escape)
            {
                ActiveControl = null;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private void TextBox_KeyPressText(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                ActiveControl = null;
            }
        }

        private void EditorToolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        private void DebugToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Function disabled.");
            Width = DebugToggleButton.Checked ? windowWidthDebug : windowWidthNoDebug;
            //DebugPanel.Enabled = DebugToggleButton.Checked;
            //DebugPanel.Visible = DebugToggleButton.Checked;
            DebugToggleButton.BackColor = DebugToggleButton.Checked ? highlightColor : SystemColors.Control;
        }

        private void ClearConsoleButton_Click(object sender, EventArgs e)
        {
            ClearConsoleMessages();
        }

        

    }

    [ToolboxItem(true)]
    public class PixelButton : Button
    {
        public InterpolationMode InterpolationMode { get; set; }
        public PixelButton() : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            paintEventArgs.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }


    }

    /// <summary>
    /// Inherits from PictureBox; adds Interpolation Mode Setting
    /// by JYelton via https://stackoverflow.com/questions/29157/how-do-i-make-a-picturebox-use-nearest-neighbor-resampling
    /// </summary>
    [ToolboxItem(true)]
    public class PictureBoxWithInterpolationMode : PictureBox
    {
        public InterpolationMode InterpolationMode { get; set; }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            paintEventArgs.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }
    }
    [ToolboxItem(true)]
    public class MonsterButton : Button
    {
        public int monsterIndex;
        public int loadedType;
        private bool _monsterLoaded;
        public bool MonsterLoaded
        {
            get
            {
                return _monsterLoaded;
            }
            set
            {
                _monsterLoaded = value;
                loadedType = 0;
                Invalidate();
            }
        }
        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                Invalidate();
            }
        }
        public InterpolationMode InterpolationMode { get; set; }
        public MonsterButton() : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            paintEventArgs.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);

            Graphics g = paintEventArgs.Graphics;
            if (Selected)
            {
                int lineWidth = 4;
                using (Pen selPen = new Pen(Color.Blue, lineWidth))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawRectangle(
                        selPen,
                        2,
                        2,
                        Width - 4, Height - 4);
                }
            }
            if (MonsterLoaded)
            {
                float fontSize = g.DpiY * 12 / 72;
                Point pos = new Point(Width - 4, 4);
                GraphicsPath pIndex = new GraphicsPath();
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                pIndex.AddString
                (
                    loadedType.ToString(),             // text to draw
                    FontFamily.GenericMonospace,  // or any other font family
                    (int)FontStyle.Bold,      // font style (bold, italic, etc.)
                    fontSize,       // em size
                    pos,              // location where to draw text
                    new StringFormat(StringFormatFlags.DirectionRightToLeft)   // set options here (e.g. center alignment)
                );
                using (Pen outlinePen = new Pen(Color.Black, 4))
                {
                    g.DrawPath(outlinePen, pIndex);
                    g.FillPath(new SolidBrush(Color.White), pIndex);
                }
            }
            //g.Dispose();
        }
    }
    [ToolboxItem(true)]
    public class ToggleButton : CheckBox
    {
        public InterpolationMode InterpolationMode { get; set; }
        public ToggleButton() : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            paintEventArgs.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
        }
    }
    
    [ToolboxItem(true)]
    public class MonsterLoadButton : Button
    {

    }

    [ToolboxItem(true)]
    public class MonsterLevelButton : Button
    {

    }

    [ToolboxItem(true)]
    public class PanelButton : Button
    {
        public int panelIndex;
        private bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                Invalidate();
            }
        }
        public InterpolationMode InterpolationMode { get; set; }
        public PanelButton() : base()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            paintEventArgs.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            paintEventArgs.Graphics.SmoothingMode = SmoothingMode.None;
            paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            paintEventArgs.Graphics.CompositingQuality = CompositingQuality.AssumeLinear;
            paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(paintEventArgs);
            if (Selected)
            {
                int lineWidth = 4;
                Graphics g = paintEventArgs.Graphics;

                using (Pen selPen = new Pen(Color.Blue, lineWidth))
                {
                    g.SmoothingMode = SmoothingMode.None;
                    g.DrawRectangle(
                        selPen,
                        7,
                        7,
                        43, 44);
                }
            }
        }
    }
    [ToolboxItem(true)]
    public class TextBoxNoFocus : TextBox
    {
        const int WM_LBUTTONDOWN = 0x0201;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
                return;
            base.WndProc(ref m);
        }
    }

    //This can help you control the scrollbar with scrolling up and down.
    //The position is a little special.
    //Position for scrolling up should be negative.
    //Position for scrolling down should be positive
    //by DMTintner via https://coderedirect.com/questions/143006/how-to-programmatically-scroll-a-panel
    public static class PanelExtension
    {
        public static void ScrollDown(this Panel p, int pos)
        {
            //pos passed in should be positive
            using (Control c = new Control() { Parent = p, Height = 1, Top = p.ClientSize.Height + pos })
            {
                p.ScrollControlIntoView(c);
            }
            //p.VerticalScroll.Value = HelperUtil.Clamp<int>(pos, p.VerticalScroll.Minimum, p.VerticalScroll.Maximum);
        }
        public static void ScrollUp(this Panel p, int pos)
        {
            //pos passed in should be negative
            using (Control c = new Control() { Parent = p, Height = 1, Top = pos })
            {
                p.ScrollControlIntoView(c);
            }
            //p.VerticalScroll.Value = HelperUtil.Clamp<int>(pos, p.VerticalScroll.Minimum, p.VerticalScroll.Maximum);
        }
    }
    //Nathan Baulch
    //https://stackoverflow.com/questions/1926264/color-different-parts-of-a-richtextbox-string
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
    public class CellClipboard
    {
        private Coordinate3 _cellArraySize = new Coordinate3();
        public Coordinate3 CellArraySize
        {
            get
            {
                return _cellArraySize;
            }
            set
            {
                _cellArraySize = value;
                cells = new HGCell[_cellArraySize.x, _cellArraySize.y, _cellArraySize.z];
            }
        }
        public HGCell[,,] cells = new HGCell[0,0,0];
        public Dictionary<Coordinate3, HGLift> lifts = new Dictionary<Coordinate3, HGLift>();
        public Dictionary<Coordinate3, HGDoor> doors = new Dictionary<Coordinate3, HGDoor>();
        public Dictionary<Coordinate3, HGButton> buttons = new Dictionary<Coordinate3, HGButton>();
    }
}
