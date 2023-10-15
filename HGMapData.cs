/*Thanks to styx from the Official Commodore Amiga Discord for help with bitflag enumeration
 * Thanks to temisu for his rnc packing tools
 * 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using HGHelperUtilities;
using System.Collections;
using System.Text;

namespace HiredGunsData
{
    public class HGGameData
    {
        public HGStyle[] styles;
        public HGMapData[] maps;
        private static Dictionary<int, int> fileDiskDict = new Dictionary<int, int>()
        {
            {1,3},{6,3},{10,3},{14,3},{20,3},{21,3},{23,3},{26,3},{28,3},{37,3},{38,3},{40,3},{41,3},{43,3},{44,3},{45,3},{46,3},{47,3},
            {2,4},{3,4},{4,4},{5,4},{7,4},{8,4},{9,4},{11,4},{12,4},{15,4},{17,4},{18,4},{19,4},{25,4},{29,4},{30,4},{31,4},{33,4},{34,4},{35,4},{39,4},{42,4}
        };
        public static int GetDiskFromFile(int file)
        {
            int f = 0;
            fileDiskDict.TryGetValue(file, out f);
            return f;
        }
    }
    
    public class HGLift : ICloneable
    {
        public int index;
        public Coordinate3 coords = new Coordinate3();
        public int minZ;
        public int maxZ;
        public int initial;//Stop = 0, up = 1, down = 2
        public bool carry;
        public bool continueAtTop;
        public bool continueAtBottom;
        public int automove;//Off = 0, bottom = 1, top = 2, both = 3

        public object Clone()
        {
            HGLift lift = (HGLift)MemberwiseClone();
            lift.coords = (Coordinate3)coords.Clone();
            return lift;
        }
        public void Clear()
        {
            coords = new Coordinate3();
            minZ = 0;
            maxZ = 0;
            initial = 0;
            carry = false;
            continueAtTop = false;
            continueAtBottom = false;
            automove = 0;
        }
        public void Set(Coordinate3 coords = new Coordinate3(), int minZ = 0, int maxZ = 0, int initial = 0, bool carry = false, bool continueOnTop = false, bool continueAtBottom = false, int automove = 0)
        {
            this.coords = coords;
            this.minZ = minZ;
            this.maxZ = maxZ;
            this.initial = initial;
            this.carry = carry;
            this.continueAtTop = continueAtTop;
            this.continueAtBottom = continueAtBottom;
            this.automove = automove;
        }
    }
    public class HGDoor : ICloneable
    {
        public int index;
        public Coordinate3 coords = new Coordinate3();
        public bool locked;
        public bool dirNS;
        public int closeDelay = 18;
        public int keyID;
        public bool buttonOnly;

        public byte[] posBytes = new byte[4];
        public int posFlat;

        public object Clone()
        {
            HGDoor door = (HGDoor)MemberwiseClone();
            door.coords = (Coordinate3)this.coords.Clone();
            return door;
        }
        public void Clear()
        {
            coords = new Coordinate3();
            locked = false;
            dirNS = false;
            closeDelay = 0;
            keyID = 0;
            buttonOnly = false;
        }
        public void Set(Coordinate3 coords = new Coordinate3(), bool locked = false, bool dirNS = false, int closeDelay = 0, int keyID = 0, bool buttonOnly = false)
        {
            this.coords = coords;
            this.locked = locked;
            this.dirNS = dirNS;
            this.closeDelay = closeDelay;
            this.keyID = keyID;
            this.buttonOnly = buttonOnly;
        }
    }
    public class HGButton : ICloneable
    {
        public int index;
        public int valid = 0;
        public Coordinate3 coords = new Coordinate3();
        public HGButtonAction actionPushed;
        public HGButtonAction actionReleased;
        public Coordinate3 targetPosPushed = new Coordinate3();
        public Coordinate3 targetPosReleased = new Coordinate3();

        public byte[] targetPosPushedBytes = new byte[4];
        public int targetPosPushedFlat;

        public HGButton()
        {

        }
        public HGButton(int index, HGButtonAction actionPushed, HGButtonAction actionReleased, Coordinate3 targetPosPushed, Coordinate3 targetPosReleased)
        {
            this.valid = 255;
            this.index = index;
            this.actionPushed = actionPushed;
            this.actionReleased = actionReleased;
            this.targetPosPushed = targetPosPushed;
            this.targetPosReleased = targetPosReleased;
        }
        public HGButton(int index, Coordinate3 coords, HGButtonAction actionPushed, HGButtonAction actionReleased, Coordinate3 targetPosPushed, Coordinate3 targetPosReleased)
        {
            this.valid = 255;
            this.index = index;
            this.coords = coords;
            this.actionPushed = actionPushed;
            this.actionReleased = actionReleased;
            this.targetPosPushed = targetPosPushed;
            this.targetPosReleased = targetPosReleased;
        }
        public object Clone()
        {
            HGButton button = (HGButton)MemberwiseClone();
            button.coords = (Coordinate3)this.coords.Clone();
            button.targetPosPushed = (Coordinate3)this.targetPosPushed.Clone();
            button.targetPosReleased = (Coordinate3)this.targetPosReleased.Clone();
            return button;
        }
        public void Clear()
        {
            valid = 0;
            coords = new Coordinate3();
            actionPushed = HGButtonAction.None;
            actionReleased = HGButtonAction.None;
            targetPosPushed = new Coordinate3();
            targetPosReleased = new Coordinate3();
        }
        public void Set(Coordinate3 coords = new Coordinate3(), HGButtonAction actionPushed = HGButtonAction.None, HGButtonAction actionReleased = HGButtonAction.None, Coordinate3 targetPosPushed = new Coordinate3(), Coordinate3 targetPosReleased = new Coordinate3())
        {
            this.valid = 255;
            this.coords = coords;
            this.actionPushed = actionPushed;
            this.actionReleased = actionReleased;
            this.targetPosPushed = targetPosPushed;
            this.targetPosReleased = targetPosReleased;
        }
    }
    public enum HGButtonAction
    {
        None = 0,
        FloorOn = 1,
        FloorOff = 2,
        ToggleFloor = 3,
        BlockOn = 4,
        BlockOff = 5,
        ToggleBlock = 6,
        HatchEgg = 7,
        Unused1 = 8,
        LiftUp = 9,
        LiftDown = 10,
        StopLift = 11,
        ToggleLift = 12,
        Unused2 = 13,
        OpenDoor = 14,
        CloseDoor = 15,
        ToggleDoor = 16,
        LockDoor = 17,
        Unlockdoor = 18
    }
    public struct Coordinate3 : ICloneable
    {
        public int x;
        public int y;
        public int z;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Coordinate3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Coordinate3(int[] intArray)
        {
            if(intArray == null || intArray.Length < 3)
            {
                x = 0;
                y = 0;
                z = 0;
                return;
            }
            x = intArray[0];
            y = intArray[1];
            z = intArray[2];
        }
        public override string ToString()
        {
            return $"{x},{y},{z}";
        }
        public void ClampPositive()
        {
            x = HelperUtil.Clamp<int>(x, 0, x);
            y = HelperUtil.Clamp<int>(y, 0, y);
            z = HelperUtil.Clamp<int>(z, 0, z);
        }
        public void ClampCoords()
        {
            x = HelperUtil.Clamp<int>(x, 0, HGMapData.MAP_WIDTH - 1);
            y = HelperUtil.Clamp<int>(y, 0, HGMapData.MAP_HEIGHT - 1);
            z = HelperUtil.Clamp<int>(z, 0, HGMapData.MAP_LEVELS - 1);
        }
        public static Coordinate3 PointToCoordinate3(Point p, int coordsZ, int step)
        {
            return new Coordinate3(
                (int)Math.Floor((float)(p.X / step)),
                (int)Math.Floor((float)(p.Y / step)),
                coordsZ);
        }
        public int To1D(int xMax, int yMax)
        {
            return HelperUtil.to1D(x, y, z, xMax, yMax);
        }
        public override bool Equals(object ob)
        {
            if (ob is Coordinate3)
            {
                Coordinate3 other = (Coordinate3)ob;
                return x == other.x && y == other.y && z == other.z;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
    }
    public class HGStyle
    {
        public string name;
        public Bitmap mapTilesImage;
        public string mapTilesImageName;
        public Bitmap[] mapTiles;
    }
    [Flags]
    public enum HGCellData1_3
    {
        Default = 0,
        Pushable = 1,
        Button = 2,
        Lift = 4,
        PuddleLight = 6,
        Push = 8,
        FitnessBoost = 16,
        Exit = 24,
        Tree = 32,
        Barrier = 40,
        Teleport = 48,
        Hydraulic = 56,
        StairsN = 128,
        StairsE = 136,
        StairsS = 144,
        StairsW = 152,
        DoorNS = 160,
        DoorEW = 168
    }
    [Flags]
    public enum HGCellData1_4
    {
        Default = 0,
        Floor = 1,
        Wall = 2,
        Water = 4,
        Panel = 8,
        Auxiliary = 32,
        Opaque = 64,
        Invisible = 128
    }
    [Flags]
    public enum HGCellBlockType
    {
        Stone = 0,
        Push = 8,
        FitnessBoost = 16,
        Exit = 24,
        Tree = 32,
        Barrier = 40,
        Teleport =	48,
        Hydraulic = 56,
        StairsN = 128,
        StairsE = 136,
        StairsS	= 144,
        StairsW = 152,
        DoorNS = 160,
        DoorEW = 168
    }
    [Flags]
    public enum HGCellFloorType
    {
        Grass = 0,
        Button = 2,
        Lift = 4,
        PuddleLight = 6
    }
    [Flags]
    public enum HGCellPanelType
    {
        Text = 0,
        ButtonIn = 8,
        ButtonOut = 16
    }
    [Flags]
    public enum HGCellWaterLevel
    {
        Level0 = 0,
        Level1 = 2,
        Level2 = 4,
        Level3 = 6
    }
    [Flags]
    public enum HGCellAuxiliaryType
    {
        Monster = 0,
        EggOpen = 16,
        Container1 = 32,
        Container2 = 48,
        Container3 = 64,
        Container4 = 80,
        Container5 = 96,
        StairsN = 128,
        StairsE = 144,
        StairsS = 160,
        StairsW = 176,
        DoorNS = 192,
        DoorEW = 208,
        Dead1 = 224,
        Dead2 = 240
    }
    public class HGConversation : ICloneable
    {
        public string text = "";
        public Coordinate3 coords = new Coordinate3();
        public int offset;
        public int length;
        public byte[] textBytes = new byte[0];

        public HGConversation()
        {

        }

        public HGConversation(string text, Coordinate3 coords)
        {
            this.text = text;
            this.coords = coords;
        }
        public HGConversation(int offset, Coordinate3 coords)
        {
            this.offset = offset;
            this.coords = coords;
        }
        public HGConversation(int offset, Coordinate3 coords, string text, int length, byte[] textBytes)
        {
            this.offset = offset;
            this.coords = coords;
            this.text = text;
            this.length = length;
            this.textBytes = textBytes;
        }
        public object Clone()
        {
            HGConversation conv = (HGConversation)MemberwiseClone();
            conv.coords = (Coordinate3)coords.Clone();
            return conv;
        }
    }
    public class HGCell : ICloneable
    {
        public HGCellBlockType blockType = HGCellBlockType.Stone;
        public bool blockHere = false;
        public bool pushable = false;
        public bool invisible = false;
        public bool opaque = false;
        public HGCellFloorType floorType = HGCellFloorType.Grass;
        public bool floorHere = false;
        public int floorTile;
        public HGCellPanelType panelType = HGCellPanelType.Text;
        public bool panelHere = false;
        public int panelButtonIndex;
        public int liftIndex;//not stored by the actual map file
        public int doorIndex;//not stored by the actual map file
        public int waterLevel;
        public bool waterHere = false;
        public HGCellAuxiliaryType auxiliaryType = HGCellAuxiliaryType.Monster;
        public bool auxiliaryHere = false;
        public int cellData1_1Byte;
        public int cellData1_2Byte;
        public int cellData1_3Byte;
        public int cellData1_4Byte;

        public int cellData2_1Byte;
        public int cellData2_2Byte;
        public int cellData2_3Byte;
        public int cellData2_4Byte;

        public int cellData3_1Byte;
        public int cellData3_2Byte;
        public int cellData3_3Byte;
        public int cellData3_4Byte;

        public int eggDelay;
        public int monsterType;
        public int monsterLevel;
        public bool tidalWaterFlowing;

        public bool eggBlocked;
        public bool eggErase;

        public bool[] skyRendered = new bool[] { false,false,false,false };
        public int itemID;
        public int itemDamaged;
        public int itemAmmo;

        public bool lit;
        public bool forceQuestionMark;
        public int healingValue;

        public Coordinate3 teleportTargetPos;

        public object Clone()
        {
            HGCell cell = (HGCell)MemberwiseClone();
            cell.teleportTargetPos = (Coordinate3)teleportTargetPos.Clone();
            cell.skyRendered = new bool[skyRendered.Length];
            for(int i = 0; i < skyRendered.Length; i++)
            {
                cell.skyRendered[i] = skyRendered[i];
            }
            return cell;
        }
    }
    public class HGMapData :ICloneable
    {
        public static int MAP_BYTES = 157738;
        public static int MAPLOCATION_BYTES = 542;
        public static int MAP_WIDTH = 23;
        public static int MAP_HEIGHT = 23;
        public static int MAP_LEVELS = 20;
        public static int MAP_CONVERSATIONS = 50;
        public static int MAP_LIFTS = 32;
        public static int MAP_DOORS = 32;
        public static int MAP_BUTTONS = 32;
        public static int MAP_PANELS = 32;
        public static int MAP_PANELWIDTH = 48;
        public static int MAP_PANELHEIGHT = 40;
        public static int MAP_PANELSPERROW = 6;
        public static int MAP_PANELSPERCOLUMN = 6;
        public static int MAP_PUSHABLES = 32;
        public static int MAP_MONSTERS = 128;
        public static int MAP_HORIZONWIDTH = 144;
        public static int MAP_HORIZONHEIGHT = 32;
        public static int MAP_FITNESSBOOSTFACTOR = 610;
        public HGCell[,,] cells = new HGCell[MAP_WIDTH, MAP_HEIGHT, MAP_LEVELS];

        public int timeLimit = 0;
        public string worldmapDescription = "Worldmap description";
        public string detailedDescription = "Detailed description";
        public int worldMapPosX = 0;
        public int worldMapPosY = 0;
        public Coordinate3 playerStart1, playerStart2, playerStart3, playerStart4;
        public int waterLevelLow;
        public int waterLevelHigh;
        public int waterSpeed;
        public string introMessage;
        public int monsterType1;
        public int monsterType2;
        public int gameType;
        public int disk;
        public int file;
        public int picture;
        public int music;
        public int atmos;
        public int sky = 0;
        public int style = 1;
        public Coordinate3 exitPos = new Coordinate3();
        public int players;
        public int destination1, destination2, destination3, destination4;
        public int pushablesCount = 0;
        public int monsterCount = 0;
        public HGConversation[] conversations = new HGConversation[50];
        public Bitmap[] horizons = new Bitmap[0];
        public Bitmap[] panels = new Bitmap[0];
        public HGLift[] lifts = new HGLift[0];
        public HGDoor[] doors = new HGDoor[0];
        public HGButton[] buttons = new HGButton[0];
        public bool originalCellLightingMethod;//not saved, only set when saving the map.

        public object Clone()
        {
            HGMapData mapData = (HGMapData)MemberwiseClone();
            mapData.cells = new HGCell[cells.GetLength(0), cells.GetLength(1), cells.GetLength(2)];
            mapData.playerStart1 = (Coordinate3)playerStart1.Clone();
            mapData.playerStart2 = (Coordinate3)playerStart2.Clone();
            mapData.playerStart3 = (Coordinate3)playerStart3.Clone();
            mapData.playerStart4 = (Coordinate3)playerStart4.Clone();
            mapData.conversations = new HGConversation[conversations.Length];
            mapData.horizons = new Bitmap[horizons.Length];
            mapData.panels = new Bitmap[panels.Length];
            mapData.lifts = new HGLift[lifts.Length];
            mapData.doors = new HGDoor[doors.Length];
            mapData.buttons = new HGButton[buttons.Length];
            for(int z = 0; z < cells.GetLength(2); z++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    for (int x = 0; x < cells.GetLength(0); x++)
                    {
                        mapData.cells[x, y, z] = (HGCell)cells[x, y, z].Clone();
                    }
                }
            }
            for (int i = 0; i < conversations.Length; i++)
            {
                if(conversations[i] == null)
                {
                    continue;
                }
                mapData.conversations[i] = (HGConversation)conversations[i].Clone();
            }
            for (int i = 0; i < horizons.Length; i++)
            {
                if (horizons[i] == null)
                {
                    continue;
                }
                mapData.horizons[i] = (Bitmap)horizons[i].Clone();
            }
            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i] == null)
                {
                    continue;
                }
                mapData.panels[i] = (Bitmap)panels[i].Clone();
            }
            for (int i = 0; i < lifts.Length; i++)
            {
                if (lifts[i] == null)
                {
                    continue;
                }
                mapData.lifts[i] = (HGLift)lifts[i].Clone();
            }
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i] == null)
                {
                    continue;
                }
                mapData.doors[i] = (HGDoor)doors[i].Clone();
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == null)
                {
                    continue;
                }
                mapData.buttons[i] = (HGButton)buttons[i].Clone();
            }
            return mapData;
        }
        public HGCell GetCell(Coordinate3 coords)
        {
            coords.ClampCoords();
            return cells[coords.x,coords.y,coords.z];
        }
        public HGCell GetCell(int x, int y, int z)
        {
            x = HelperUtil.Clamp<int>(x,0,HGMapData.MAP_WIDTH);
            y = HelperUtil.Clamp<int>(y, 0, HGMapData.MAP_HEIGHT);
            z = HelperUtil.Clamp<int>(z, 0, HGMapData.MAP_LEVELS);
            return cells[x,y,z];
        }
        public HGLift GetLift(Coordinate3 coords, bool addIfNull)
        {
            for(int i = 0; i < lifts.Length; i++)
            {
                HGLift lift = lifts[i];
                if(coords.Equals(lift.coords))
                {
                    return lift;
                }
            }
            for (int i = 0; i < lifts.Length; i++)
            {
                HGLift lift = lifts[i];
                HGCell cell = GetCell(lift.coords);
                if (cell.floorHere == false || cell.floorType != HGCellFloorType.Lift)
                {
                    if (addIfNull)
                    {
                        lift.coords = coords;
                    }
                    return lift;
                }
            }
            return null;
        }
        public int GetLiftIndex(Coordinate3 coords)
        {
            for (int i = 0; i < lifts.Length; i++)
            {
                HGLift lift = lifts[i];
                if (coords.Equals(lift.coords))
                {
                    return i;
                }
            }
            return -1;
        }
        public int CountLifts()
        {
            int liftsAmount = 0;
            for (int i = 0; i < lifts.Length; i++)
            {
                HGLift lift = lifts[i];
                HGCell cell = GetCell(lift.coords);
                if(cell.floorHere == false || cell.floorType != HGCellFloorType.Lift)
                {
                    continue;
                }
                liftsAmount++;
            }
            return liftsAmount;
        }
        public HGDoor GetDoor(Coordinate3 coords, bool addIfNull)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                HGDoor door = doors[i];
                if (coords.Equals(door.coords))
                {
                    return door;
                }
            }
            for (int i = 0; i < doors.Length; i++)
            {
                HGDoor door = doors[i];
                HGCell cell = GetCell(door.coords);
                if (cell.blockHere == false || (cell.blockType != HGCellBlockType.DoorNS && cell.blockType != HGCellBlockType.DoorEW))
                {
                    if (addIfNull)
                    {
                        door.coords = coords;
                    }
                    return door;
                }
            }
            return null;
        }
        public int GetDoorIndex(Coordinate3 coords)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                HGDoor door = doors[i];
                if (coords.Equals(door.coords))
                {
                    return i;
                }
            }
            return -1;
        }
        public int CountDoors()
        {
            int doorsAmount = 0;
            for (int i = 0; i < doors.Length; i++)
            {
                HGDoor door = doors[i];
                HGCell cell = GetCell(door.coords);
                if (cell.blockHere == false || (cell.blockType != HGCellBlockType.DoorNS && cell.blockType != HGCellBlockType.DoorEW))
                {
                    continue;
                }
                doorsAmount++;
            }
            return doorsAmount;
        }
        public HGButton GetButton(int buttonIndex, bool addIfNull)
        {
            if(buttonIndex < 0 || buttonIndex >= buttons.Length)
            {
                return null;
            }
            return buttons[buttonIndex];
        }
        public HGButton GetButton(Coordinate3 coords, bool addIfNull)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                HGButton button = buttons[i];
                if (coords.Equals(button.coords))
                {
                    return button;
                }
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                HGButton button = buttons[i];
                HGCell cell = GetCell(button.coords);
                if ((cell.panelHere == false || (cell.panelType != HGCellPanelType.ButtonIn && cell.panelType != HGCellPanelType.ButtonOut)) && (cell.floorHere == false || (cell.floorType != HGCellFloorType.Button)))
                {
                    if (addIfNull)
                    {
                        button.coords = coords;
                    }
                    return button;
                }
            }
            return null;
        }
        public int GetButtonIndex(Coordinate3 coords)
        {
            HGCell cell = GetCell(coords);
            int buttonIndex = cell.panelButtonIndex;
            if (cell.floorType == HGCellFloorType.Button && coords.z > 0)
            {
                buttonIndex = cells[coords.x, coords.y, coords.z - 1].panelButtonIndex;
            }
            return buttonIndex;
        }
        public int CountButtons()
        {
            int buttonsAmount = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                HGButton button = buttons[i];
                HGCell cell = GetCell(button.coords);
                if ((cell.panelHere == false || (cell.panelType != HGCellPanelType.ButtonIn && cell.panelType != HGCellPanelType.ButtonOut)) && (cell.floorHere == false || cell.floorType != HGCellFloorType.Button))
                {
                    continue;
                }
                buttonsAmount++;
            }
            return buttonsAmount;
        }
        public int CountConversations()
        {
            if(conversations == null)
            {
                return 0;
            }
            for(int i = 0; i < conversations.Length; i++)
            {
                if (conversations[i] == null || string.IsNullOrEmpty(conversations[i].text))
                {
                    return i;
                }
            }
            return conversations.Length;
        }
        public int ConvertFitnessBoostValue(int i, bool toCellProperty)
        {
            if(toCellProperty)
            {
                return i * HGMapData.MAP_FITNESSBOOSTFACTOR;
            }
            return (int)Math.Round((double)i / HGMapData.MAP_FITNESSBOOSTFACTOR,0);
        }
        public void UpdateMapTiles(Coordinate3 coords)
        {
            UpdateMapTiles(coords, coords);
        }
        public void UpdateMapTiles(Coordinate3 coordsStart, Coordinate3 coordsEnd)
        {
            if (cells == null)
            {
                return;
            }
            coordsStart.ClampCoords();
            coordsEnd.ClampCoords();
            HGCell nullCell = new HGCell();
            HGCell fillCell = new HGCell()
            {
                blockHere = true,
                blockType = HGCellBlockType.Stone
            };
            Random rand = new Random();
            for (int z = coordsStart.z; z <= coordsEnd.z + 1; z++)
            {
                for (int y = coordsStart.y - 1; y <= coordsEnd.y + 1; y++)
                {
                    for (int x = coordsStart.x - 1; x <= coordsEnd.x + 1; x++)
                    {
                        HGCell cellNW = x > 0 && y > 0 && z < MAP_LEVELS ? cells[x - 1, y - 1, z] : fillCell;
                        HGCell cellN = x >= 0 && x < MAP_WIDTH && y > 0 && z < MAP_LEVELS? cells[x, y - 1, z] : fillCell;
                        if (x == 0 && y == 0 && z == 0)
                        {
                            //cellN = nullCell;//to conform with Hired Guns
                        }
                        else if (x > 0 && y == 0 && z == 0)
                        {
                            //cellN = nullCell;//to conform with Hired Guns
                        }
                        HGCell cellW = x > 0 && y >= 0 && y < MAP_HEIGHT && z < MAP_LEVELS ? cells[x - 1, y, z] : fillCell;
                        if (x == 0 && y == 0 && z == 0)
                        {
                            //cellW = nullCell;//to conform with Hired Guns
                        }
                        HGCell cellCur =  x >= 0 && y >= 0 && x < MAP_WIDTH && y < MAP_HEIGHT && z < MAP_LEVELS ? cells[x, y, z] : fillCell;
                        HGCell cellE = x >= 0 && x < MAP_WIDTH - 1 && y >= 0 && y < MAP_HEIGHT && z < MAP_LEVELS ? cells[x + 1, y, z] : fillCell;
                        HGCell cellS = x >= 0 && x < MAP_WIDTH && y < MAP_HEIGHT - 1 && z < MAP_LEVELS ? cells[x, y + 1, z] : fillCell;
                        HGCell cellB = x >= 0 && x < MAP_WIDTH && y >= 0 && y < MAP_HEIGHT && z > 0 ? cells[x, y, z - 1] : fillCell;
                        int tileNum = 0;
                        int r = rand.Next(0, 3);
                        if ((cellB != null && cellB.blockHere) && (cellB != null && cellB.blockType == HGCellBlockType.Stone))
                        {
                            switch (r)
                            {
                                case 0:
                                    tileNum = 9;
                                    break;
                                case 1:
                                    tileNum = 2;
                                    break;
                                case 2:
                                    tileNum = 16;
                                    break;
                            }
                            if(cellNW != null && cellNW.blockHere && cellNW.pushable == false && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 18;
                            }
                            if(cellN != null && cellN.blockHere && cellN.pushable == false && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 17;
                            }
                            if (cellW != null && cellW.blockHere && cellW.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 10;
                            }
                            if (cellW != null && cellNW != null && cellW.blockHere && cellNW.pushable == false && cellW.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport) && cellNW.blockHere && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 8;
                            }
                            if (cellN != null && cellNW != null && cellNW.blockHere && cellNW.pushable == false && cellN.pushable == false && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport) && cellN.blockHere && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 1;
                            }
                            if (cellW != null && cellN != null && cellW.blockHere && cellW.pushable == false && cellN.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport) && cellN.blockHere && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 22;
                            }
                            if(cellCur.blockHere && cellCur.blockType == HGCellBlockType.Tree)
                            {
                                tileNum = 35;
                            }
                        }
                        if(cellCur.floorHere && cellCur.floorType == HGCellFloorType.Grass)
                        {
                            switch (r)
                            {
                                case 0:
                                    tileNum = 6;
                                    break;
                                case 1:
                                    tileNum = 26;
                                    break;
                                case 2:
                                    tileNum = 33;
                                    break;
                            }
                            if(cellNW != null && cellNW.blockHere && cellNW.pushable == false && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 14;
                            }
                            if (cellN != null && cellN.blockHere && cellN.pushable == false && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 7;
                            }
                            if (cellW != null && cellW.blockHere && cellW.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 34;
                            }
                            if (cellW != null && cellNW != null && cellW.blockHere && cellNW.pushable == false && cellW.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport) && cellNW.blockHere && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 32;
                            }
                            if (cellN != null && cellNW != null && cellNW.blockHere && cellNW.pushable == false && cellN.pushable == false && (cellNW.blockType == HGCellBlockType.Stone || cellNW.blockType == HGCellBlockType.FitnessBoost || cellNW.blockType == HGCellBlockType.Exit || cellNW.blockType == HGCellBlockType.Barrier || cellNW.blockType == HGCellBlockType.Teleport) && cellN.blockHere && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 25;
                            }
                            if (cellW != null && cellN != null && cellW.blockHere && cellW.pushable == false && cellN.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport) && cellN.blockHere && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                            {
                                tileNum = 24;
                            }
                            if(cellCur.blockHere && cellCur.blockType == HGCellBlockType.Tree)
                            {
                                tileNum = 36;
                                if (cellW != null && cellW.blockHere && cellW.pushable == false && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport))
                                {
                                    tileNum = 60;
                                }
                                if (cellN != null && cellN.blockHere && cellN.pushable == false && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport))
                                {
                                    tileNum = 61;
                                }
                                if (cellW != null && cellN != null && cellN.blockHere && cellW.pushable == false && cellN.pushable == false && (cellN.blockType == HGCellBlockType.Stone || cellN.blockType == HGCellBlockType.FitnessBoost || cellN.blockType == HGCellBlockType.Exit || cellN.blockType == HGCellBlockType.Barrier || cellN.blockType == HGCellBlockType.Teleport) && cellW.blockHere && (cellW.blockType == HGCellBlockType.Stone || cellW.blockType == HGCellBlockType.FitnessBoost || cellW.blockType == HGCellBlockType.Exit || cellW.blockType == HGCellBlockType.Barrier || cellW.blockType == HGCellBlockType.Teleport))
                                {
                                    tileNum = 62;
                                }
                            }
                        }
                        if(cellCur.blockHere && cellCur.blockType == HGCellBlockType.Stone)
                        {
                            tileNum = 12;
                            if(cellN != null && (cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone))
                            {
                                //if ((!cell2->block_here_bit)||(cell2->block>0)) block_num=4;
                                tileNum = 4;
                            }
                            if (cellW != null && (cellW.blockHere == false || cellW.blockType != HGCellBlockType.Stone))
                            {
                                tileNum = 11;
                            }
                            if (cellE != null && (cellE.blockHere == false || cellE.blockType != HGCellBlockType.Stone))
                            {
                                tileNum = 13;
                            }
                            if (cellS != null && (cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone))
                            {
                                tileNum = 20;
                            }
                            if (cellN != null && cellW != null && ((cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone)) && (cellW.blockHere == false || (cellW.blockType != HGCellBlockType.Stone && cellW.blockType != HGCellBlockType.Push)))
                            {
                                //if (((!cell2->block_here_bit)||(cell2->block>0))&&((!cell3->block_here_bit)||(cell3->block>1))) block_num=3;
                                tileNum = 3;
                            }
                            if (cellS != null && cellW != null && ((cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone)) && (cellW.blockHere == false || (cellW.blockType != HGCellBlockType.Stone && cellW.blockType != HGCellBlockType.Push)))
                            {
                                tileNum = 19;
                            }
                            if (cellS != null && cellE != null && ((cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone)) && (cellE.blockHere == false || (cellE.blockType != HGCellBlockType.Stone && cellE.blockType != HGCellBlockType.Push)))
                            {
                                tileNum = 21;
                            }
                            if (cellN != null && cellE != null && ((cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone)) && (cellE.blockHere == false || (cellE.blockType != HGCellBlockType.Stone && cellE.blockType != HGCellBlockType.Push)))
                            {
                                tileNum = 5;
                            }
                            if (cellW != null && cellE != null && ((cellW.blockHere == false || cellW.blockType != HGCellBlockType.Stone)) && (cellE.blockHere == false || (cellE.blockType != HGCellBlockType.Stone && cellE.blockType != HGCellBlockType.Push)))
                            {
                                //if (((!cell3->block_here_bit)||(cell3->block>0))&&((!cell5->block_here_bit)||(cell5->block>1))) block_num=27;
                                tileNum = 27;
                            }
                            if (cellN != null && cellS != null && ((cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone)) && (cellS.blockHere == false || (cellS.blockType != HGCellBlockType.Stone && cellS.blockType != HGCellBlockType.Push)))
                            {
                                //if (((!cell2->block_here_bit) || (cell2->block > 0)) && ((!cell6->block_here_bit) || (cell6->block > 1))) block_num = 28;
                                tileNum = 28;
                            }
                            if (cellN != null && cellW != null && cellE != null && ((cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone)) && (cellW.blockHere == false || (cellW.blockType != HGCellBlockType.Stone && cellW.blockType != HGCellBlockType.Push)) && (cellE.blockHere == false || cellE.blockType != HGCellBlockType.Stone))
                            {
                                //if (((!cell2->block_here_bit)||(cell2->block>0))&&((!cell3->block_here_bit)||(cell3->block>1))&&((!cell5->block_here_bit)||(cell5->block>0))) block_num=37;
                                tileNum = 37;
                            }
                            if (cellN != null && cellW != null && cellS != null && ((cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone)) && (cellW.blockHere == false || (cellW.blockType != HGCellBlockType.Stone && cellW.blockType != HGCellBlockType.Push)) && (cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone))
                            {
                                //if (((!cell2->block_here_bit)||(cell2->block>0))&&((!cell3->block_here_bit)||(cell3->block>1))&&((!cell6->block_here_bit)||(cell6->block>0))) block_num=29;
                                tileNum = 29;
                            }
                            if (cellE != null && cellW != null && cellS != null && ((cellE.blockHere == false || cellE.blockType != HGCellBlockType.Stone)) && (cellW.blockHere == false || (cellW.blockType != HGCellBlockType.Stone && cellW.blockType != HGCellBlockType.Push)) && (cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone))
                            {
                                // if (((!cell5->block_here_bit)||(cell5->block>0))&&((!cell3->block_here_bit)||(cell3->block>1))&&((!cell6->block_here_bit)||(cell6->block>0))) block_num=38;
                                tileNum = 38;
                            }
                            if (cellE != null && cellN != null && cellS != null && ((cellE.blockHere == false || cellE.blockType != HGCellBlockType.Stone)) && (cellN.blockHere == false || (cellN.blockType != HGCellBlockType.Stone && cellN.blockType != HGCellBlockType.Push)) && (cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone))
                            {
                                //if (((!cell5->block_here_bit)||(cell5->block>0))&&((!cell2->block_here_bit)||(cell2->block>1))&&((!cell6->block_here_bit)||(cell6->block>0))) block_num=30;
                                tileNum = 30;
                            }
                            if (cellW != null && cellE != null && cellN != null && cellS != null && ((cellW.blockHere == false || cellW.blockType != HGCellBlockType.Stone)) && (cellE.blockHere == false || (cellE.blockType != HGCellBlockType.Stone && cellE.blockType != HGCellBlockType.Push)) && (cellN.blockHere == false || cellN.blockType != HGCellBlockType.Stone) && (cellS.blockHere == false || cellS.blockType != HGCellBlockType.Stone))
                            {
                                // if (((!cell3->block_here_bit)||(cell3->block>0))&&((!cell5->block_here_bit)||(cell5->block>1))&&((!cell2->block_here_bit)||(cell2->block>0))&&((!cell6->block_here_bit)||(cell6->block>0))) block_num=23;
                                tileNum = 23;
                            }
                           
                        }
                        if(cellCur.blockHere & cellCur.blockType == HGCellBlockType.StairsN)
                        {
                            // if ((cell4->block_here_bit)&&(cell4->block==16))
                            tileNum = 40;
                            if(cellS != null && cellS.auxiliaryHere && cellS.auxiliaryType > (HGCellAuxiliaryType)112 && cellS.auxiliaryType < (HGCellAuxiliaryType)192)
                            {
                                //if ((cell6->aux_here_bit)&&(cell6->auxiliary>7)&&(cell6->auxiliary<12)) block_num=41;
                                tileNum = 41;
                            }
                        }
                        if (cellCur.blockHere & cellCur.blockType == HGCellBlockType.StairsE)
                        {
                            // if ((cell4->block_here_bit)&&(cell4->block==17))
                            tileNum = 44;
                            if (cellW != null && cellW.auxiliaryHere && cellW.auxiliaryType > (HGCellAuxiliaryType)112 && cellW.auxiliaryType < (HGCellAuxiliaryType)192)
                            {
                                // if ((cell3->aux_here_bit)&&(cell3->auxiliary>7)&&(cell3->auxiliary<12)) block_num=45;
                                tileNum = 45;
                            }
                        }
                        if (cellCur.blockHere & cellCur.blockType == HGCellBlockType.StairsS)
                        {
                            //if ((cell4->block_here_bit)&&(cell4->block==18))
                            tileNum = 52;
                            if (cellN != null && cellN.auxiliaryHere && cellN.auxiliaryType > (HGCellAuxiliaryType)112 && cellN.auxiliaryType < (HGCellAuxiliaryType)192)
                            {
                                // if ((cell2->aux_here_bit)&&(cell2->auxiliary>7)&&(cell2->auxiliary<12)) block_num=53;
                                tileNum = 53;
                            }
                        }
                        if (cellCur.blockHere & cellCur.blockType == HGCellBlockType.StairsW)
                        {
                            // if ((cell4->block_here_bit)&&(cell4->block==19))
                            tileNum = 48;
                            if (cellE != null && cellE.auxiliaryHere && cellE.auxiliaryType > (HGCellAuxiliaryType)112 && cellE.auxiliaryType < (HGCellAuxiliaryType)192)
                            {
                                //if ((cell5->aux_here_bit)&&(cell5->auxiliary>7)&&(cell5->auxiliary<12)) block_num=49;
                                tileNum = 49;
                            }
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.StairsN)
                        {
                            // if ((cell4->aux_here_bit)&&(cell4->auxiliary==8))
                            tileNum = 42;
                            if (cellB != null && cellB.blockHere && cellB.blockType == HGCellBlockType.Stone)
                            {
                                //if ((cell7->block_here_bit)&&(cell7->block==0)) block_num=43;
                                tileNum = 43;
                            }
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.StairsE)
                        {
                            //if ((cell4->aux_here_bit)&&(cell4->auxiliary==9))
                            tileNum = 46;
                            if (cellB != null && cellB.blockHere && cellB.blockType == HGCellBlockType.Stone)
                            {
                                //if ((cell7->block_here_bit)&&(cell7->block==0)) block_num=47;
                                tileNum = 47;
                            }
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.StairsS)
                        {
                            // if ((cell4->aux_here_bit)&&(cell4->auxiliary==10))
                            tileNum = 54;
                            if (cellB != null && cellB.blockHere && cellB.blockType == HGCellBlockType.Stone)
                            {
                                //if ((cell7->block_here_bit)&&(cell7->block==0)) block_num=55;
                                tileNum = 55;
                            }
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.StairsW)
                        {
                            // if ((cell4->aux_here_bit)&&(cell4->auxiliary==11))
                            tileNum = 50;
                            if (cellB != null && cellB.blockHere && cellB.blockType == HGCellBlockType.Stone)
                            {
                                //if ((cell7->block_here_bit)&&(cell7->block==0)) block_num=51;
                                tileNum = 51;
                            }
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.DoorEW)
                        {
                            //  if ((cell4->aux_here_bit)&&(cell4->auxiliary==13)) block_num=57;
                            tileNum = 57;
                        }
                        if (cellCur.auxiliaryHere & cellCur.auxiliaryType == HGCellAuxiliaryType.DoorNS)
                        {
                            //if ((cell4->aux_here_bit)&&(cell4->auxiliary==12)) block_num=59;
                            tileNum = 59;
                        }
                        if (cellCur.blockHere & cellCur.blockType == HGCellBlockType.DoorEW)
                        {
                            //	if ((cell4->block_here_bit)&&((cell4->block==21)||(cell4->block==23))) block_num=56;
                            tileNum = 56;
                        }
                        if (cellCur.blockHere & cellCur.blockType == HGCellBlockType.DoorNS)
                        {
                            //if ((cell4->block_here_bit)&&((cell4->block==20)||(cell4->block==22))) block_num=58;
                            tileNum = 58;
                        }
                        if (cellB != null && cellB.blockHere & cellB.blockType == HGCellBlockType.StairsN)
                        {
                            // if ((cell7->block_here_bit)&&(cell7->block==16)) block_num=40;
                            tileNum = 40;
                        }
                        if (cellB != null && cellB.blockHere & cellB.blockType == HGCellBlockType.StairsE)
                        {
                            //if ((cell7->block_here_bit)&&(cell7->block==17)) block_num=44;
                            tileNum = 44;
                        }
                        if (cellB != null && cellB.blockHere & cellB.blockType == HGCellBlockType.StairsS)
                        {
                            //if ((cell7->block_here_bit)&&(cell7->block==18)) block_num=52;
                            tileNum = 52;
                        }
                        if (cellB != null && cellB.blockHere & cellB.blockType == HGCellBlockType.StairsW)
                        {
                            //if ((cell7->block_here_bit)&&(cell7->block==19)) block_num=48;
                            tileNum = 48;
                        }
                        if (tileNum == 0 && cellB != null && cellB.auxiliaryHere & cellB.auxiliaryType == HGCellAuxiliaryType.StairsN)
                        {
                            //if ((block_num==0)&&(cell7->aux_here_bit)&&(cell7->auxiliary==8)) block_num=42;
                            tileNum = 42;
                        }
                        if (tileNum == 0 && cellB != null && cellB.auxiliaryHere & cellB.auxiliaryType == HGCellAuxiliaryType.StairsE)
                        {
                            //if ((block_num==0)&&(cell7->aux_here_bit)&&(cell7->auxiliary==9)) block_num=46;
                            tileNum = 46;
                        }
                        if (tileNum == 0 && cellB != null && cellB.auxiliaryHere & cellB.auxiliaryType == HGCellAuxiliaryType.StairsS)
                        {
                            //if ((block_num==0)&&(cell7->aux_here_bit)&&(cell7->auxiliary==10)) block_num=54;
                            tileNum = 54;
                        }
                        if (tileNum == 0 && cellB != null && cellB.auxiliaryHere & cellB.auxiliaryType == HGCellAuxiliaryType.StairsW)
                        {
                            //if ((block_num==0)&&(cell7->aux_here_bit)&&(cell7->auxiliary==11)) block_num=50;
                            tileNum = 50;
                        }
                        if(cellCur.blockHere && (cellCur.blockType == HGCellBlockType.FitnessBoost || cellCur.blockType == HGCellBlockType.Exit || cellCur.blockType == HGCellBlockType.Barrier || cellCur.blockType == HGCellBlockType.Teleport))
                        {
                            //if ((cell4->block_here_bit)&&((cell4->block==2)||(cell4->block==3)||(cell4->block==5)||(cell4->block==6))) block_num=39;
                            tileNum = 39;
                        }
                        cellCur.floorTile = tileNum;
                    }
                }
            }
        }
        public static HGMapData LoadMap(byte[] bytes)
        {
           HGCellData1_3[] cellData1_3Enums = (HGCellData1_3[])Enum.GetValues(typeof(HGCellData1_3));
           HGCellWaterLevel[] waterLevelEnums = (HGCellWaterLevel[])Enum.GetValues(typeof(HGCellWaterLevel));
           HGCellPanelType[] panelEnums = (HGCellPanelType[])Enum.GetValues(typeof(HGCellPanelType));
           HGCellAuxiliaryType[] auxiliaryEnums = (HGCellAuxiliaryType[])Enum.GetValues(typeof(HGCellAuxiliaryType));
            /*
            var cellData1_3Enums = Enum.GetValues(typeof(HGCellData1_3));
           var waterLevelEnums = Enum.GetValues(typeof(HGCellWaterLevel));
           var panelEnums = Enum.GetValues(typeof(HGCellPanelType));
           var auxiliaryEnums = Enum.GetValues(typeof(HGCellAuxiliaryType));
            */
          

            HGMapData mapData = new HGMapData();

            mapData.worldMapPosX = BitConverter.ToUInt16(new byte[] { bytes[1], bytes[0] }, 0);
            mapData.worldMapPosY = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);

            mapData.players = bytes[8];
            mapData.gameType = bytes[9];
            mapData.monsterType1 = bytes[530];
            mapData.monsterType2 = bytes[531];
            mapData.sky = bytes[532];
            mapData.style = bytes[533];
            mapData.disk = bytes[525];
            mapData.file = bytes[526];
            mapData.picture = bytes[527];
            mapData.music = bytes[528];
            mapData.atmos = bytes[529];
            
            mapData.destination1 = bytes[535];
            mapData.destination2 = bytes[537];
            mapData.destination3 = bytes[545];
            mapData.destination4 = bytes[547];
            mapData.playerStart1 = new Coordinate3(bytes[670], bytes[671], bytes[672]);
            mapData.playerStart2 = new Coordinate3(bytes[673], bytes[674], bytes[675]);
            mapData.playerStart3 = new Coordinate3(bytes[676], bytes[677], bytes[678]);
            mapData.playerStart4 = new Coordinate3(bytes[679], bytes[680], bytes[681]);

            mapData.timeLimit = bytes[685];
            mapData.waterLevelLow = bytes[689];
            mapData.waterLevelHigh = bytes[691];
            mapData.waterSpeed = bytes[695];

            int i;

            mapData.worldmapDescription = "";
            int[] wmdLineOffsets = new int[] { 16,67,133, 199};
            for(i = 0; i < wmdLineOffsets.Length; i++)
            {
                int byteLength = i > 0 ? 60 : 45;
                byte[] worldMapDescriptionLine = new byte[byteLength];
                Array.Copy(bytes, wmdLineOffsets[i], worldMapDescriptionLine, 0, byteLength);
                mapData.worldmapDescription += System.Text.Encoding.UTF8.GetString(worldMapDescriptionLine);
                mapData.worldmapDescription += i < 3 ? "\r\n" : "";
            }

            mapData.detailedDescription = "";
            int[] ddLineOffsets = new int[] { 266, 332, 398, 464 };
            for (i = 0; i < 4; i++)
            {
                int byteLength = 60;
                byte[] detailedDescriptionLine = new byte[byteLength];
                Array.Copy(bytes, ddLineOffsets[i], detailedDescriptionLine, 0, byteLength);
                mapData.detailedDescription += System.Text.Encoding.UTF8.GetString(detailedDescriptionLine);
                mapData.detailedDescription += i < 3 ? "\r\n" : "";
            }

            byte[] introMessageBytes = new byte[120];
            Array.Copy(bytes, 550, introMessageBytes, 0, introMessageBytes.Length);
            mapData.introMessage = System.Text.Encoding.UTF8.GetString(introMessageBytes).TrimEnd();

            List<HGConversation> convList = new List<HGConversation>();
            /*
            int convIndex = 0;
            for(i = 3512; i < 3824; i += 6, convIndex++)
            {
                if(convIndex >= mapData.conversations.Length)
                {
                    break;
                }
                byte[] coordsBytes = new byte[] { bytes[i + 3], bytes[i + 2] };
                byte[] offsetBytes = new byte[] { bytes[i + 5], bytes[i + 4] };
                int offset = BitConverter.ToUInt16(offsetBytes, 0);
                if (convIndex > 0 && offset == 0)
                {
                    break;
                }
                int convLength = ((i < convList.Count - 1 && convList[i + 1] != null) ? mapData.conversations[i + 1].offset - offset : 480);
                byte[] convStringBytes = new byte[convLength];
                Array.Copy(bytes, 3814 + offset + 10, convStringBytes, 0, convStringBytes.Length - 10);
                string convText = System.Text.Encoding.UTF8.GetString(convStringBytes).TrimEnd('\x0').TrimEnd();
                if (convText.Length == 0)
                {
                    continue;
                }
                convList.Add(new HGConversation(offset, new Coordinate3(HelperUtil.to3D(BitConverter.ToUInt16(coordsBytes, 0) / 4, MAP_WIDTH, MAP_HEIGHT)), convText, convLength, convStringBytes));
            }
            */
            int convIndex = 0;
            for (i = 3512; i < 3824; i += 6, convIndex++)
            {
                byte[] coordsBytes = new byte[] { bytes[i + 3], bytes[i + 2] };
                byte[] offsetBytes = new byte[] { bytes[i + 5], bytes[i + 4] };
                int offset = BitConverter.ToUInt16(offsetBytes, 0);
                if (convIndex > 0 && offset == 0)
                {
                    break;
                }
                //System.Windows.Forms.MessageBox.Show($"{convIndex} offset: {offset}");
                convList.Add(new HGConversation(offset, new Coordinate3(HelperUtil.to3D(BitConverter.ToUInt16(coordsBytes, 0) / 4, MAP_WIDTH, MAP_HEIGHT))));
            }
            for (i = 0; i < convList.Count; i++)
            {
                HGConversation conv = convList[i];
                
                int convLength = ((i < convList.Count - 1 && convList[i + 1] != null) ? convList[i + 1].offset - conv.offset : 480);
                //System.Windows.Forms.MessageBox.Show($"{i} offset: {conv.offset}, convLength: {convLength}");
                byte[] convStringBytes = new byte[convLength];
                Array.Copy(bytes, 3814 + conv.offset + 10, convStringBytes, 0, convStringBytes.Length - 10);
                string convText = System.Text.Encoding.UTF8.GetString(convStringBytes).TrimEnd('\x0').TrimEnd();
                if (convText.Length == 0)
                {
                    continue;
                }
                conv.text = convText;
                conv.length = convLength;
                conv.textBytes = convStringBytes;
                convList[i] = conv;
            }
            mapData.conversations = convList.ToArray();
            
            mapData.horizons = new Bitmap[4];
            Color transColor = Color.FromArgb(0,255,255,255);
            for(i = 0; i < mapData.horizons.Length; i++)
            {
                Bitmap bm = new Bitmap(MAP_HORIZONWIDTH, MAP_HORIZONHEIGHT);
                using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                {
                    int pixelIndex = 0;
                    byte[] horizonBytes = new byte[576];
                    Array.Copy(bytes, 1208 + i * horizonBytes.Length, horizonBytes, 0, horizonBytes.Length);
                    for (int byteIndex = 0; byteIndex < horizonBytes.Length; byteIndex++)
                    {
                        var bits = new BitArray(new byte[] { horizonBytes[byteIndex] });
                        HelperUtil.BitArrayReverse(bits);
                        for (int bitIndex = 0; bitIndex < bits.Length; bitIndex++, pixelIndex++)
                        {
                            //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                            //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                            snoop.SetPixel(pixelIndex % MAP_HORIZONWIDTH, pixelIndex / MAP_HORIZONWIDTH, bits[bitIndex] ? Color.Black : transColor);
                        }
                    }
                }
                mapData.horizons[i] = bm;
            }
            //Color[] _panelColors = new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 224, 208, 192), Color.FromArgb(255, 160, 144, 128), Color.FromArgb(255, 96, 80, 64) };
            Color[] _panelColors = new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 240, 224, 208), Color.FromArgb(255, 176, 160, 144), Color.FromArgb(255, 112, 96, 80) };
            mapData.panels = new Bitmap[MAP_PANELS];
            for (i = 0; i < mapData.panels.Length; i++)
            {
                Bitmap bm = new Bitmap(48, 40);
                using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                {
                    int pixelIndex = 0;
                    byte[] panelBitplane1Bytes = new byte[240];
                    byte[] panelBitplane2Bytes = new byte[240];
                    Array.Copy(bytes, 10738 + i * panelBitplane1Bytes.Length * 2, panelBitplane1Bytes, 0, panelBitplane1Bytes.Length);
                    Array.Copy(bytes, 10738 + i * panelBitplane1Bytes.Length * 2 + panelBitplane1Bytes.Length, panelBitplane2Bytes, 0, panelBitplane2Bytes.Length);
                    for (int byteIndex = 0; byteIndex < panelBitplane1Bytes.Length; byteIndex++)
                    {
                        var bits1 = new BitArray(new byte[] { panelBitplane1Bytes[byteIndex] });
                        var bits2 = new BitArray(new byte[] { panelBitplane2Bytes[byteIndex] });
                        HelperUtil.BitArrayReverse(bits1);
                        HelperUtil.BitArrayReverse(bits2);
                        for (int bitIndex = 0; bitIndex < bits1.Length; bitIndex++, pixelIndex++)
                        {
                            var bits = new BitArray(2);
                            bits[0] = bits1[bitIndex];
                            bits[1] = bits2[bitIndex];
                            int panelColorIndex = HelperUtil.GetIntFromBitArray(bits);
                            if (panelColorIndex < _panelColors.Length && pixelIndex < 1920)
                            {
                                //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                                //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                                snoop.SetPixel(pixelIndex % 48, pixelIndex / 48, _panelColors[panelColorIndex]);
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show($"{panelBitplane1Bytes[byteIndex]} | {panelBitplane2Bytes[byteIndex]} = {panelColorIndex}");
                            }
                        }
                    }
                }
                mapData.panels[i] = bm;
            }

            mapData.lifts = new HGLift[MAP_LIFTS];
            for(i = 0; i < mapData.lifts.Length; i++)
            {
                byte[] liftBytes = new byte[10];
                Array.Copy(bytes, 9836 + i * liftBytes.Length, liftBytes, 0, liftBytes.Length);
                byte[] coordsBytes = new byte[] { liftBytes[1], liftBytes[0] };
                mapData.lifts[i] = new HGLift()
                {
                    index = i,
                    coords = new Coordinate3(HelperUtil.to3D(BitConverter.ToUInt16(coordsBytes, 0) / 4, MAP_WIDTH, MAP_HEIGHT)),
                    minZ = liftBytes[3],
                    maxZ = liftBytes[4],
                    initial = liftBytes[5],
                    carry = liftBytes[6] != 0,
                    continueAtTop = liftBytes[7] != 0,
                    continueAtBottom = liftBytes[8] != 0,
                    automove = liftBytes[9]
                };
            }

            mapData.doors = new HGDoor[MAP_DOORS];
            for (i = 0; i < mapData.doors.Length; i++)
            {
                byte[] doorBytes = new byte[12];
                Array.Copy(bytes, 10158 + i * doorBytes.Length, doorBytes, 0, doorBytes.Length);
                byte[] coordsBytes = new byte[] { doorBytes[1], doorBytes[0] };
                mapData.doors[i] = new HGDoor()
                {
                    index = i,
                    coords = new Coordinate3(HelperUtil.to3D(BitConverter.ToUInt16(coordsBytes, 0) / 4, MAP_WIDTH, MAP_HEIGHT)),
                    locked = doorBytes[3] == 3,
                    dirNS = doorBytes[6] == 160,
                    closeDelay = doorBytes[8],
                    keyID = doorBytes[10],
                    buttonOnly = doorBytes[11] == 255,
                    posBytes = coordsBytes,
                    posFlat = (int)BitConverter.ToUInt16(coordsBytes, 0)
                };
            }

            mapData.buttons = new HGButton[MAP_BUTTONS];
            for (i = 0; i < mapData.buttons.Length; i++)
            {
                byte[] buttonBytes = new byte[28];
                Array.Copy(bytes, 8940 + i * buttonBytes.Length, buttonBytes, 0, buttonBytes.Length);
                HGButtonAction actionPushed = (HGButtonAction)(int)buttonBytes[1];
                HGButtonAction actionReleased = (HGButtonAction)(int)buttonBytes[2];
                byte[] coordsPushedBytes = new byte[] { buttonBytes[7], buttonBytes[6], buttonBytes[5], buttonBytes[4] };
                byte[] coordsReleasedBytes = new byte[] { buttonBytes[11], buttonBytes[10], buttonBytes[9], buttonBytes[8] };
                //int targetPosPushedSubtrahend = actionPushed == HGButtonAction.ToggleLift ? 9284 : actionPushed == HGButtonAction.Unlockdoor ? 9612 : 27936;
                //int targetPosReleasedSubtrahend = actionReleased == HGButtonAction.ToggleLift ? 9284 : actionReleased == HGButtonAction.Unlockdoor ? 9612 : 27936;
                //int targetPosPushedSubtrahend = 0;
                //int targetPosReleasedSubtrahend = 0;
                //int targetPosPushedDivisor = actionPushed == HGButtonAction.ToggleLift ? 10 : actionPushed == HGButtonAction.Unlockdoor ? 4 : 4;
                //int targetPosReleasedDivisor = actionReleased == HGButtonAction.ToggleLift ? 10 : actionReleased == HGButtonAction.Unlockdoor ? 4 : 4;
                //int targetPosPushedFlat = (int)(BitConverter.ToUInt32(coordsPushedBytes, 0) - targetPosPushedSubtrahend) / targetPosPushedDivisor;
                //int targetPosReleasedFlat = (int)(BitConverter.ToUInt32(coordsReleasedBytes, 0) - targetPosReleasedSubtrahend) / targetPosReleasedDivisor;
                //Coordinate3 targetCoordsPushed = new Coordinate3(HelperUtil.to3D(targetPosPushedFlat, MAP_WIDTH, MAP_HEIGHT));
                //Coordinate3 targetCoordsReleased = new Coordinate3(HelperUtil.to3D(targetPosReleasedFlat, MAP_WIDTH, MAP_HEIGHT));
                Coordinate3 targetCoordsPushed = new Coordinate3(0,0,0);
                Coordinate3 targetCoordsReleased = new Coordinate3(0,0,0);
                switch ((int)actionPushed)
                {
                    case 0:
                    case 8:
                    case 13:
                        break;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        targetCoordsPushed = mapData.lifts[HelperUtil.Clamp<int>((int)(BitConverter.ToUInt32(coordsPushedBytes, 0) - 9294) / 10, 0, mapData.lifts.Length - 1)].coords;
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        targetCoordsPushed = mapData.doors[HelperUtil.Clamp<int>((int)(BitConverter.ToUInt32(coordsPushedBytes, 0) - 9616) / 12, 0, mapData.doors.Length - 1)].coords;
                        break;
                    default:
                        targetCoordsPushed = new Coordinate3(HelperUtil.to3D((int)(BitConverter.ToUInt32(coordsPushedBytes, 0) - 27936) / 4, MAP_WIDTH, MAP_HEIGHT));
                        break;
                }
                switch ((int)actionReleased)
                {
                    case 0:
                    case 8:
                    case 13:
                        break;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        targetCoordsReleased = mapData.lifts[HelperUtil.Clamp<int>((int)(BitConverter.ToUInt32(coordsReleasedBytes, 0) - 9294) / 10, 0, mapData.lifts.Length - 1)].coords;
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        targetCoordsReleased = mapData.doors[HelperUtil.Clamp<int>((int)(BitConverter.ToUInt32(coordsReleasedBytes, 0) - 9616) / 12, 0, mapData.doors.Length - 1)].coords;
                        break;
                    default:
                        targetCoordsReleased = new Coordinate3(HelperUtil.to3D((int)(BitConverter.ToUInt32(coordsReleasedBytes, 0) - 27936) / 4, MAP_WIDTH, MAP_HEIGHT));
                        break;
                }
                mapData.buttons[i] = new HGButton(i, actionPushed, actionReleased, targetCoordsPushed, targetCoordsReleased)
                {
                    valid = buttonBytes[0],
                    targetPosPushedBytes = coordsPushedBytes,
                    targetPosPushedFlat = (int)BitConverter.ToUInt32(coordsPushedBytes, 0)
                };
            }

            byte[] cellData1 = new byte[mapData.cells.Length * 4];
            Array.Copy(bytes,28478, cellData1,0, cellData1.Length);

            byte[] cellData2 = new byte[mapData.cells.Length * 4];
            Array.Copy(bytes, 71718, cellData2, 0, cellData2.Length);

            byte[] cellData3 = new byte[mapData.cells.Length * 4];
            Array.Copy(bytes, 114958, cellData3, 0, cellData2.Length);

            i = 0;
            for (int z = 0; z < HGMapData.MAP_LEVELS; z++)
            {
                for (int y = 0; y < HGMapData.MAP_HEIGHT; y++)
                {
                    for (int x = 0; x < HGMapData.MAP_WIDTH; x++, i += 4)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        HGCell cellCur = new HGCell();

                        ////////////////////////////////////////////////////////////////
                        //Cell Data 1
                        ////////////////////////////////////////////////////////////////

                        byte[] cellData1Cur = new byte[4];
                        Array.Copy(cellData1, i, cellData1Cur, 0, 4);
                        int cellData1Cur1 = cellData1Cur[0];
                        int cellData1Cur2 = cellData1Cur[1];
                        int cellData1Cur3 = cellData1Cur[2];
                        int cellData1Cur4 = cellData1Cur[3];

                        cellCur.cellData1_1Byte = cellData1Cur1;
                        cellCur.cellData1_2Byte = cellData1Cur2;
                        cellCur.cellData1_3Byte = cellData1Cur3;
                        cellCur.cellData1_4Byte = cellData1Cur4;

                        //foreach (int val in cellData1_3Enums)
                        // {
                        for (int j = 0; j < cellData1_3Enums.Length; j++)
                        {
                            int val = (int)cellData1_3Enums[j];
                            if ((cellData1Cur3 & val) == val)
                            {
                                if ((HGCellData1_3)val == HGCellData1_3.Pushable)
                                {
                                    cellCur.pushable = true;
                                }
                                if (cellData1Cur3 >= 2 && cellData1Cur3 < 8)
                                {
                                    cellCur.floorType = (HGCellFloorType)val;
                                }
                                else if (cellData1Cur3 >= 8)
                                {
                                    cellCur.blockType = (HGCellBlockType)val;
                                }
                            }
                        }

                        //HGCellData1_4
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Floor) == HGCellData1_4.Floor)
                        {
                            cellCur.floorHere = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Wall) == HGCellData1_4.Wall)
                        {
                            cellCur.blockHere = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Water) == HGCellData1_4.Water)
                        {
                            cellCur.waterHere = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Panel) == HGCellData1_4.Panel)
                        {
                            cellCur.panelHere = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Auxiliary) == HGCellData1_4.Auxiliary)
                        {
                            cellCur.auxiliaryHere = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Opaque) == HGCellData1_4.Opaque)
                        {
                            cellCur.opaque = true;
                        }
                        if (((HGCellData1_4)cellData1Cur4 & HGCellData1_4.Invisible) == HGCellData1_4.Invisible)
                        {
                            cellCur.invisible = true;
                        }

                        //HGCellData1_1 & HGCellData1_2 (Panels, buttons, water, auxiliaries)
                        if (cellCur.auxiliaryHere)
                        {
                            //foreach (int val in auxiliaryEnums)
                            //{
                            for (int j = 0; j < auxiliaryEnums.Length; j++)
                            {
                                int val = (int)auxiliaryEnums[j];
                                if ((cellData1Cur1 & val) == val)
                                {
                                    cellCur.auxiliaryType = (HGCellAuxiliaryType)val;
                                }
                            }
                        }
                        //foreach (int val in waterLevelEnums)
                        //{
                        for (int j = 0; j < waterLevelEnums.Length; j++)
                        {
                            int val = (int)waterLevelEnums[j];
                            if ((cellData1Cur2 & val) == val)
                            {
                                cellCur.waterLevel = val / 2;
                            }
                        }
                        if (cellCur.panelHere || cellCur.floorType == HGCellFloorType.Button)
                        {
                            //foreach (int val in panelEnums)
                            //{
                            for (int j = 0; j < panelEnums.Length; j++)
                            {
                                int val = (int)panelEnums[j];
                                if ((cellData1Cur2 & val) == val)
                                {
                                    cellCur.panelType = (HGCellPanelType)val;
                                }
                            }
                        }

                        //Always read panel/button index because non-wall button cells will store the button index of a floor button above.
                        //Assign byte array elements in the correct order (little endian). No Array.Copy is used, so no reversing is needed.
                        //Lower bits (cellData1Cur2): Clamp to multiples of 128
                        //Higher bits (cellData1Cur1): Subtract value of auxiliaryType (which start at 16) from it.
                        /*
                        byte[] panelButtonIndexBytes = new byte[] { (byte)(Math.Floor((double)cellData1Cur2 / 128) * 128), (byte)(cellData1Cur1 - cellCur.auxiliaryType) };
                        cellCur.panelButtonIndex = BitConverter.ToUInt16(panelButtonIndexBytes, 0) / 128;
                        */
                        cellCur.panelButtonIndex = BitConverter.ToUInt16(new byte[] { (byte)(cellData1Cur2 - (int)cellCur.panelType - cellCur.waterLevel * 2), (byte)(cellData1Cur1 - (int)cellCur.auxiliaryType) },0) / 128;
                        if (cellData1Cur1 < 16)
                        {
                            //System.Windows.Forms.MessageBox.Show($"{coords}: {cellData1Cur1} {cellData1Cur2} -> {cellCur.panelButtonIndex}\r\n[{cellData1Cur2} - {(int)cellCur.panelType} - {cellCur.waterLevel * 2} = {cellData1Cur2 - (int)cellCur.panelType - cellCur.waterLevel * 2}] [{cellData1Cur1} - {(int)cellCur.auxiliaryType} = {(cellData1Cur1 - (int)cellCur.auxiliaryType)}]");
                        }

                        ////////////////////////////////////////////////////////////////
                        //Cell Data 2
                        ////////////////////////////////////////////////////////////////

                        byte[] cellData2Cur = new byte[4];
                        Array.Copy(cellData2, i, cellData2Cur, 0, 4);
                        int cellData2Cur1 = cellData2Cur[0];
                        int cellData2Cur2 = cellData2Cur[1];
                        int cellData2Cur3 = cellData2Cur[2];
                        int cellData2Cur4 = cellData2Cur[3];

                        cellCur.cellData2_1Byte = cellData2Cur1;
                        cellCur.cellData2_2Byte = cellData2Cur2;
                        cellCur.cellData2_3Byte = cellData2Cur3;
                        cellCur.cellData2_4Byte = cellData2Cur4;

                        byte[] eggDelayBytes = new byte[] { (byte)(Math.Floor((double)cellData2Cur2 / 16) * 16), (byte)cellData2Cur1 };
                        cellCur.eggDelay = BitConverter.ToUInt16(eggDelayBytes, 0) / 16;

                        byte[] monsterTypeLevelBytes = new byte[] { (byte)(Math.Floor((double)cellData2Cur3 / 16) * 16), (byte)(cellData2Cur2 >= 16 ? cellData2Cur2 % 16 : cellData2Cur2) };
                        int monsterTypeLevel = BitConverter.ToUInt16(monsterTypeLevelBytes, 0) / 16;
                        cellCur.monsterType = (int)Math.Floor((decimal)monsterTypeLevel / 4);
                        cellCur.monsterLevel = monsterTypeLevel - cellCur.monsterType * 4;

                        if ((cellData2Cur3 & 4) == 4)
                        {
                            cellCur.forceQuestionMark = true;
                        }
                        if ((cellData2Cur3 & 8) == 8)
                        {
                            cellCur.tidalWaterFlowing = true;
                        }

                        cellCur.floorTile = cellData2Cur4;

                        ////////////////////////////////////////////////////////////////
                        //Cell Data 3
                        ////////////////////////////////////////////////////////////////

                        byte[] cellData3Cur = new byte[4];
                        Array.Copy(cellData3, i, cellData3Cur, 0, 4);
                        int cellData3Cur1 = cellData3Cur[0];
                        int cellData3Cur2 = cellData3Cur[1];
                        int cellData3Cur3 = cellData3Cur[2];
                        int cellData3Cur4 = cellData3Cur[3];

                        cellCur.cellData3_1Byte = cellData3Cur1;
                        cellCur.cellData3_2Byte = cellData3Cur2;
                        cellCur.cellData3_3Byte = cellData3Cur3;
                        cellCur.cellData3_4Byte = cellData3Cur4;


                        cellCur.skyRendered[0] = (cellData3Cur1 & 1) == 1;
                        cellCur.skyRendered[1] = (cellData3Cur1 & 2) == 2;
                        cellCur.skyRendered[2] = (cellData3Cur1 & 4) == 4;
                        cellCur.skyRendered[3] = (cellData3Cur1 & 8) == 8;
                        cellCur.eggErase = (cellData3Cur1 & 16) == 16;
                        cellCur.eggBlocked = (cellData3Cur1 & 32) == 32;
                        cellCur.lit = (cellData3Cur1 & 128) == 128;
                        cellCur.itemAmmo = cellData3Cur2;

                        if (cellCur.blockType == HGCellBlockType.Teleport)
                        {
                            cellCur.teleportTargetPos = new Coordinate3(HelperUtil.to3D((int)BitConverter.ToUInt16(new byte[] { (byte)cellData3Cur4, (byte)cellData3Cur3 }, 0) / 4, MAP_WIDTH, MAP_HEIGHT));
                        }
                        else if(cellCur.blockType == HGCellBlockType.FitnessBoost)
                        {
                            cellCur.healingValue = (int)BitConverter.ToUInt16(new byte[] { (byte)cellData3Cur4, (byte)cellData3Cur3 }, 0);
                        }
                        else
                        {
                            cellCur.itemDamaged = cellData3Cur3;
                            cellCur.itemID = cellData3Cur4;
                        }
                        ////////////////////////////////////////////////////////////////
                        //Lift
                        ////////////////////////////////////////////////////////////////
                        if (cellCur.floorHere && cellCur.floorType == HGCellFloorType.Lift)
                        {
                            HGLift lift = mapData.GetLift(coords,false);
                            if(lift != null)
                            {
                                cellCur.liftIndex = lift.index;
                            }
                        }
                        ////////////////////////////////////////////////////////////////
                        //Door
                        ////////////////////////////////////////////////////////////////
                        if (cellCur.blockHere && (cellCur.blockType == HGCellBlockType.DoorNS || cellCur.blockType == HGCellBlockType.DoorEW))
                        {
                            HGDoor door = mapData.GetDoor(coords,false);
                            if (door != null)
                            {
                                cellCur.doorIndex = door.index;
                            }
                        }
                        ////////////////////////////////////////////////////////////////
                        //Finish
                        ////////////////////////////////////////////////////////////////
                        mapData.cells[x, y, z] = cellCur;
                    }
                }
            }
            for (int z = 0; z < HGMapData.MAP_LEVELS; z++)
            {
                for (int y = 0; y < HGMapData.MAP_HEIGHT; y++)
                {
                    for (int x = 0; x < HGMapData.MAP_WIDTH; x++)
                    {
                        Coordinate3 coords = new Coordinate3(x, y, z);
                        Coordinate3 coordsAbove = new Coordinate3(x, y, z + 1);
                        HGCell cell = mapData.GetCell(coords);
                        HGCell cellAbove = z < HGMapData.MAP_LEVELS - 1 ? mapData.GetCell(coordsAbove) : null;
                        ////////////////////////////////////////////////////////////////
                        //Button
                        ////////////////////////////////////////////////////////////////
                        if ((cell.panelHere && (cell.panelType == HGCellPanelType.ButtonIn || cell.panelType == HGCellPanelType.ButtonOut)) || (cellAbove != null && (cellAbove.floorHere && cellAbove.floorType == HGCellFloorType.Button)))
                        {
                            //int buttonIndex = mapData.GetButtonIndex(coords);
                            HGButton button = mapData.GetButton(cell.panelButtonIndex, false);
                            if (button != null)
                            {
                                if(cellAbove != null && (cellAbove.floorHere && cellAbove.floorType == HGCellFloorType.Button))
                                {
                                    button.coords = coordsAbove;
                                }
                                else
                                {
                                    button.coords = coords;
                                }
                            }
                        }
                    }
                }
            }
            return mapData;
        }

        public static byte[] MapDataToByteArray(byte[] bytes, HGMapData mapData)
        //public static byte[] MapDataToByteArray(HGMapData mapData)
        {
            //Byte[] bytes = new byte[HGMapData.MAP_BYTES];
            if (bytes.Length < 157738)
            {
                System.Windows.Forms.MessageBox.Show($"[HGMapData/MapDataToByteArray] bytes.Length too short ({bytes.Length}), must be 157738!");
            }
            
            mapData.monsterCount = 0;
            //1-byte number: source start 3 ([0] [0] [0] [X]), destination start 0, length 1
            //2-byte number: source start 2 ([0] [0] [X] [X]), destination start 0, length 2
            //3-byte number: source start 1 ([0] [X] [X] [X]), destination start 0, length 3
            //4-byte number: source start 0 ([X] [X] [X] [X]), destination start 0, length 4
            byte[] worldMapPosXBytes = BitConverter.GetBytes(mapData.worldMapPosX).Reverse().ToArray();
            byte[] worldMapPosYBytes = BitConverter.GetBytes(mapData.worldMapPosY).Reverse().ToArray();
            bytes[0] = worldMapPosXBytes[2];
            bytes[1] = worldMapPosXBytes[3];
            bytes[2] = worldMapPosYBytes[2];
            bytes[3] = worldMapPosYBytes[3];
            bytes[8] = (byte)mapData.players;
            bytes[9] = (byte)mapData.gameType;
            bytes[530] = (byte)mapData.monsterType1;
            bytes[531] = (byte)mapData.monsterType2;
            bytes[532] = (byte)mapData.sky;
            bytes[533] = (byte)mapData.style;
            bytes[525] = (byte)mapData.disk;
            bytes[526] = (byte)mapData.file;
            bytes[527] = (byte)mapData.picture;
            bytes[528] = (byte)mapData.music;
            bytes[529] = (byte)mapData.atmos;
            bytes[535] = (byte)mapData.destination1;
            bytes[537] = (byte)mapData.destination2;
            bytes[545] = (byte)mapData.destination3;
            bytes[547] = (byte)mapData.destination4;
            bytes[670] = (byte)mapData.playerStart1.x;
            bytes[671] = (byte)mapData.playerStart1.y;
            bytes[672] = (byte)mapData.playerStart1.z;
            bytes[673] = (byte)mapData.playerStart2.x;
            bytes[674] = (byte)mapData.playerStart2.y;
            bytes[675] = (byte)mapData.playerStart2.z;
            bytes[676] = (byte)mapData.playerStart3.x;
            bytes[677] = (byte)mapData.playerStart3.y;
            bytes[678] = (byte)mapData.playerStart3.z;
            bytes[679] = (byte)mapData.playerStart4.x;
            bytes[680] = (byte)mapData.playerStart4.y;
            bytes[681] = (byte)mapData.playerStart4.z;
            bytes[685] = (byte)mapData.timeLimit;
            bytes[689] = (byte)mapData.waterLevelLow;
            bytes[691] = (byte)mapData.waterLevelHigh;
            bytes[695] = (byte)mapData.waterSpeed;

            int i = 0;
            for (int z = 1; z < MAP_LEVELS - 1; z++)
            {
                for (int y = 0; y < MAP_HEIGHT; y++)
                {
                    for (int x = 0; x < MAP_WIDTH; x++)
                    {
                        HGCell cellCur = mapData.cells[x,y,z];
                        if(cellCur == null)
                        {
                            continue;
                        }
                        if(cellCur.pushable)
                        {
                            if(i == 31)
                            {
                                break;
                            }
                            byte[] pushableBytes = new byte[6];
                            byte[] coordsBytes = BitConverter.GetBytes(new Coordinate3(x, y, z).To1D(MAP_WIDTH, MAP_HEIGHT) * 4).Reverse().ToArray();
                            byte[] panelButtonIndexBytes = BitConverter.GetBytes(cellCur.panelButtonIndex * 128).Reverse().ToArray();
                            pushableBytes[0] = coordsBytes[2];
                            pushableBytes[1] = coordsBytes[3];
                            pushableBytes[0 + 2] = panelButtonIndexBytes[2];
                            pushableBytes[1 + 2] = panelButtonIndexBytes[3];
                            pushableBytes[2 + 2] += (byte)cellCur.blockType;
                            pushableBytes[3 + 2] += (byte)(cellCur.blockHere ? 2 : 0);
                            pushableBytes[2 + 2] += (byte)(cellCur.pushable ? 1 : 0);
                            pushableBytes[3 + 2] += (byte)(cellCur.opaque ? 64 : 0);
                            pushableBytes[3 + 2] += (byte)(cellCur.invisible ? 128 : 0);
                            pushableBytes[1 + 2] += (byte)cellCur.panelType;
                            pushableBytes[3 + 2] += (byte)(cellCur.panelHere ? 8 : 0);
                            Array.Copy(pushableBytes,0,bytes,10544 + i * pushableBytes.Length, pushableBytes.Length);
                            i++;
                            /*
                            MapData->pushables[blocks].pushable_posn = (x + y * MAP_DEPTH + z * FLOOR_SIZE) * 4;
                            MapData->pushables[blocks].pushable_cell.auxiliary = 0;
                            MapData->pushables[blocks].pushable_cell.variant = cell1->variant;
                            MapData->pushables[blocks].pushable_cell.explosion = 0;
                            MapData->pushables[blocks].pushable_cell.panel = cell1->panel;
                            MapData->pushables[blocks].pushable_cell.water = 0;
                            MapData->pushables[blocks].pushable_cell.block = cell1->block;
                            MapData->pushables[blocks].pushable_cell.floor = 0;
                            MapData->pushables[blocks].pushable_cell.pushable_bit = cell1->pushable_bit;
                            MapData->pushables[blocks].pushable_cell.invisible_bit = cell1->invisible_bit;
                            MapData->pushables[blocks].pushable_cell.opaque_bit = cell1->opaque_bit;
                            MapData->pushables[blocks].pushable_cell.aux_here_bit = 0;
                            MapData->pushables[blocks].pushable_cell.explosion_here_bit = 0;
                            MapData->pushables[blocks].pushable_cell.panel_here_bit = cell1->panel_here_bit;
                            MapData->pushables[blocks].pushable_cell.water_here_bit = 0;
                            MapData->pushables[blocks].pushable_cell.block_here_bit = cell1->block_here_bit;
                            MapData->pushables[blocks++].pushable_cell.floor_here_bit = 0;
                            */
                        }
                    }
                }
            }
            mapData.pushablesCount = i;
            if (i > HGMapData.MAP_PUSHABLES)
            {
                System.Windows.Forms.MessageBox.Show($"[MapDataToByteArray] Too many pushables ({i})! Maximum is {HGMapData.MAP_PUSHABLES}");
                i = HGMapData.MAP_PUSHABLES;
            }
            if (i < 31)
            {
                Array.Copy(new byte[(31 - i) * 6], 0, bytes, 10544 + (i + 1) * 6, (31 - i) * 6);
            }
            
            string[] worldMapDescriptionLines = mapData.worldmapDescription.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int[] wmdLineOffsets = new int[] { 16, 67, 133, 199 };
            for (i = 0; i < worldMapDescriptionLines.Length; i++)
            {
                //worldMapDescriptionLines[i] = worldMapDescriptionLines[i].Substring(0, Math.Min(worldMapDescriptionLines[i].Length, i == 0 ? 45 : 60)).PadRight(i == 0 ? 45 : 60,' ');
                byte[] worldMapDescriptionBytes = new byte[i == 0 ? 45 : 60];
                //byte[] worldMapDescriptionBytes = Encoding.ASCII.GetBytes(worldMapDescriptionLines[i]);
                Array.Copy(Encoding.ASCII.GetBytes(worldMapDescriptionLines[i]), 0, worldMapDescriptionBytes, 0, HelperUtil.Clamp<int>(worldMapDescriptionLines[i].Length,0, i == 0 ? 45 : 60));
                Array.Copy(worldMapDescriptionBytes, 0, bytes, wmdLineOffsets[i], worldMapDescriptionBytes.Length);
                bytes[wmdLineOffsets[i] - 6] = 251;
                bytes[wmdLineOffsets[i] - 4] = (byte)(i == 0 ? 10 : 60);
                bytes[wmdLineOffsets[i] - 2] = (byte)(5 + i * 10);
                bytes[wmdLineOffsets[i] - 1] = 252;

            }
            //System.Windows.Forms.MessageBox.Show($"{worldMapDescriptionLines.Length}");
            string[] detailedDescriptionLines = mapData.detailedDescription.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int[] ddLineOffsets = new int[] { 266, 332, 398, 464 };
            for (i = 0; i < detailedDescriptionLines.Length; i++)
            {
                //detailedDescriptionLines[i] = detailedDescriptionLines[i].Substring(0, Math.Min(detailedDescriptionLines[i].Length, 60)).PadRight(60, ' ');
                byte[] detailedDescriptionBytes = new byte[60];
                //byte[] detailedDescriptionBytes = Encoding.ASCII.GetBytes(detailedDescriptionLines[i]);
                Array.Copy(Encoding.ASCII.GetBytes(detailedDescriptionLines[i]), 0, detailedDescriptionBytes, 0, HelperUtil.Clamp<int>(detailedDescriptionLines[i].Length,0,60));
                Array.Copy(detailedDescriptionBytes, 0, bytes, ddLineOffsets[i], detailedDescriptionBytes.Length);
                bytes[ddLineOffsets[i] - 6] = 251;
                bytes[ddLineOffsets[i] - 4] = 10;
                bytes[ddLineOffsets[i] - 2] = (byte)(3 + i * 9);
                bytes[ddLineOffsets[i] - 1] = 252;
            }

            //string introMessage = mapData.introMessage.Substring(0, Math.Min(mapData.introMessage.Length, 120)).PadRight(120, ' ');
            //byte[] introMessageBytes = Encoding.ASCII.GetBytes(introMessage);
            byte[] introMessageBytes = new byte[120];
            Array.Copy(Encoding.ASCII.GetBytes(mapData.introMessage), 0, introMessageBytes, 0, HelperUtil.Clamp<int>(mapData.introMessage.Length,0,120));
            Array.Copy(introMessageBytes, 0, bytes, 550, introMessageBytes.Length);

            int convOffset = 0;
            Array.Copy(new byte[300 + 3072], 0, bytes, 3512, 300 + 3072);
            for (i = 0; i < mapData.conversations.Length; i++)
            {
                if(i == MAP_CONVERSATIONS)
                {
                    break;
                }
                HGConversation conv = mapData.conversations[i];
                if(conv == null || (i > 0 && conv.offset == 0))
                {
                    //break;
                }
                conv.offset = convOffset;
                Array.Copy(BitConverter.GetBytes(conv.coords.To1D(MAP_WIDTH,MAP_HEIGHT) * 4).Reverse().ToArray(), 2, bytes, 3512 + i * 6 + 2, 2);
                Array.Copy(BitConverter.GetBytes(conv.offset).Reverse().ToArray(), 2, bytes, 3512 + i * 6 + 4, 2);
                byte[] convBytes = Encoding.ASCII.GetBytes(conv.text);
                Array.Copy(convBytes, 0, bytes, 3814 + conv.offset + 10, convBytes.Length);

                convOffset += conv.text.Length + 10;
            }
            for(i = 0; i < mapData.horizons.Length; i++)
            {
                Bitmap bm = mapData.horizons[i];
                using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                {
                    int pixelIndex = 0;
                    byte[] horizonBytes = new byte[576];
                    
                    for (int byteIndex = 0; byteIndex < horizonBytes.Length; byteIndex++)
                    {
                        var bits = new BitArray(8);
                        for (int bitIndex = 0; bitIndex < bits.Length; bitIndex++, pixelIndex++)
                        {
                            //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                            //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                            Color pixel = snoop.GetPixel(pixelIndex % 144, pixelIndex / 144);
                            bits[bitIndex] = pixel.R < 128 ? true : false;
                        }
                        HelperUtil.BitArrayReverse(bits);
                        bits.CopyTo(horizonBytes, byteIndex);
                    }
                    Array.Copy(horizonBytes, 0, bytes, 1208 + i * horizonBytes.Length, horizonBytes.Length);
                }
            }
            Color[] panelColors = new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 240, 224, 208), Color.FromArgb(255, 176, 160, 144), Color.FromArgb(255, 112, 96, 80) };
            int[] panelBr = new int[] 
            { 
                (panelColors[0].R + panelColors[0].G + panelColors[0].B) / 3, (panelColors[1].R + panelColors[1].G + panelColors[1].B) / 3,
                (panelColors[2].R + panelColors[2].G + panelColors[2].B) / 3, (panelColors[3].R + panelColors[3].G + panelColors[3].B) / 3
            };
            for (i = 0; i < mapData.panels.Length; i++)
            {
                Bitmap bm = mapData.panels[i];
                if(bm == null)
                {
                    continue;
                }
                using (BmpPixelSnoop snoop = new BmpPixelSnoop(bm))
                {
                    int pixelIndex = 0;
                    byte[] panelBitplane1Bytes = new byte[240];
                    byte[] panelBitplane2Bytes = new byte[240];
                    
                    for (int byteIndex = 0; byteIndex < panelBitplane1Bytes.Length; byteIndex++)
                    {
                        var bits1 = new BitArray(8);
                        var bits2 = new BitArray(8);
                        
                        for (int bitIndex = 0; bitIndex < bits1.Length; bitIndex++, pixelIndex++)
                        {
                            if (pixelIndex < 1920)
                            {
                                //BmpPixelSnoop fast pixel operations by MIŁOSZ ORZEŁ
                                //https://en.morzel.net/post/Fast-pixel-operations-in-NET-(with-and-without-unsafe)
                                Color pixel = snoop.GetPixel(pixelIndex % 48, pixelIndex / 48);
                                int br = (pixel.R + pixel.G + pixel.B) / 3;
                                if (br >= panelBr[2] + (panelBr[1] - panelBr[2]) / 2)
                                {
                                    bits1[bitIndex] = true;
                                    bits2[bitIndex] = false;
                                }
                                else if (br >= panelBr[3] + (panelBr[2] - panelBr[3]) / 2)
                                {
                                    bits1[bitIndex] = false;
                                    bits2[bitIndex] = true;
                                }
                                else if (br >= 0 + panelBr[3] / 2)
                                {
                                    bits1[bitIndex] = true;
                                    bits2[bitIndex] = true;
                                }
                                else
                                {
                                    bits1[bitIndex] = false;
                                    bits2[bitIndex] = false;
                                }
                            }
                        }
                        HelperUtil.BitArrayReverse(bits1);
                        HelperUtil.BitArrayReverse(bits2);
                        panelBitplane1Bytes[byteIndex] = (byte)HelperUtil.GetIntFromBitArray(bits1);
                        panelBitplane2Bytes[byteIndex] = (byte)HelperUtil.GetIntFromBitArray(bits2);
                    }
                    Array.Copy(panelBitplane1Bytes, 0, bytes, 10738 + i * panelBitplane1Bytes.Length * 2, panelBitplane1Bytes.Length);
                    Array.Copy(panelBitplane2Bytes, 0, bytes, 10738 + i * panelBitplane1Bytes.Length * 2 + panelBitplane1Bytes.Length, panelBitplane2Bytes.Length);
                }
            }
            for (i = 0; i < mapData.lifts.Length; i++)
            {
                HGLift lift = mapData.lifts[i];
                byte[] liftBytes = new byte[10];
                byte[] coordsBytes = BitConverter.GetBytes(lift.coords.To1D(MAP_WIDTH, MAP_HEIGHT) * 4).Reverse().ToArray();
                liftBytes[0] = coordsBytes[2];
                liftBytes[1] = coordsBytes[3];
                liftBytes[2] = (byte)lift.coords.z;
                liftBytes[3] = (byte)lift.minZ;
                liftBytes[4] = (byte)lift.maxZ;
                liftBytes[5] = (byte)lift.initial;
                liftBytes[6] = (byte)(lift.carry ? 1 : 0);
                liftBytes[7] = (byte)(lift.continueAtTop ? 2 : 0);
                liftBytes[8] = (byte)(lift.continueAtBottom ? 1 : 0);
                liftBytes[9] = (byte)lift.automove;
                Array.Copy(liftBytes, 0, bytes, 9836 + i * liftBytes.Length, liftBytes.Length);
            }
            for (i = 0; i < mapData.doors.Length; i++)
            {
                HGDoor door = mapData.doors[i];
                if (mapData.GetDoor(door.coords, true) == null)
                {
                    break;
                }
                HGCell cell = mapData.GetCell(door.coords);
                byte[] doorBytes = new byte[12];
                byte[] coordsBytes = BitConverter.GetBytes(door.coords.To1D(MAP_WIDTH, MAP_HEIGHT) * 4).Reverse().ToArray();
                doorBytes[0] = coordsBytes[2];
                doorBytes[1] = coordsBytes[3];
                doorBytes[2] = 0;
                doorBytes[3] = (byte)(door.locked ? 3 : 0);
                doorBytes[4] = 0;
                doorBytes[5] = 0;
                doorBytes[6] = (byte)(cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW) ? (door.dirNS ? 160 : 168) : 0);
                doorBytes[7] = (byte)(cell.blockHere && (cell.blockType == HGCellBlockType.DoorNS || cell.blockType == HGCellBlockType.DoorEW) ? 2 : 0);
                doorBytes[8] = (byte)door.closeDelay;
                doorBytes[9] = 0;
                doorBytes[10] = (byte)door.keyID;
                doorBytes[11] = (byte)(door.buttonOnly ? 255 : 0);
                Array.Copy(doorBytes, 0, bytes, 10158 + i * doorBytes.Length, doorBytes.Length);
            }
            for(i = 0; i < mapData.buttons.Length; i++)
            {
                HGButton btn = mapData.buttons[i];
                byte[] btnBytes = new byte[28];
                byte[] coordsPushedBytes = new byte[4];
                byte[] coordsReleasedBytes = new byte[4];
                switch ((int)btn.actionPushed)
                {
                    case 0:
                    case 8:
                    case 13:
                        break;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        int liftIndex = mapData.GetLiftIndex(btn.targetPosPushed);
                        if(liftIndex >= 0)
                        {
                            coordsPushedBytes = BitConverter.GetBytes(liftIndex * 10 + 9294).Reverse().ToArray();
                        }
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        int doorIndex = mapData.GetDoorIndex(btn.targetPosPushed);
                        if (doorIndex >= 0)
                        {
                            coordsPushedBytes = BitConverter.GetBytes(doorIndex * 12 + 9616).Reverse().ToArray();
                        }
                        break;
                    default:
                        coordsPushedBytes = BitConverter.GetBytes(btn.targetPosPushed.To1D(MAP_WIDTH, MAP_HEIGHT) * 4 + 27936).Reverse().ToArray();
                        break;
                }
                switch ((int)btn.actionReleased)
                {
                    case 0:
                    case 8:
                    case 13:
                        break;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        int liftIndex = mapData.GetLiftIndex(btn.targetPosReleased);
                        if (liftIndex >= 0)
                        {
                            coordsReleasedBytes = BitConverter.GetBytes(liftIndex * 10 + 9294).Reverse().ToArray();
                        }
                        break;
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        int doorIndex = mapData.GetDoorIndex(btn.targetPosReleased);
                        if (doorIndex >= 0)
                        {
                            coordsReleasedBytes = BitConverter.GetBytes(doorIndex * 12 + 9616).Reverse().ToArray();
                        }
                        break;
                    default:
                        coordsReleasedBytes = BitConverter.GetBytes(btn.targetPosReleased.To1D(MAP_WIDTH, MAP_HEIGHT) * 4 + 27936).Reverse().ToArray();
                        break;
                }
                btnBytes[0] = (byte)btn.valid;
                btnBytes[1] = (byte)btn.actionPushed;
                btnBytes[2] = (byte)btn.actionReleased;
                btnBytes[6] = coordsPushedBytes[2];
                btnBytes[7] = coordsPushedBytes[3];
                btnBytes[10] = coordsReleasedBytes[2];
                btnBytes[11] = coordsReleasedBytes[3];
                Array.Copy(btnBytes, 0, bytes, 8940 + i * btnBytes.Length, btnBytes.Length);
            }
            for (int y = 0; y < HGMapData.MAP_HEIGHT; y++)
            {
                for (int x = 0; x < HGMapData.MAP_WIDTH; x++)
                {
                    bool skyLightBlocked = false;
                    for (int z = (mapData.originalCellLightingMethod ? HGMapData.MAP_LEVELS - 2  : HGMapData.MAP_LEVELS - 1); z >= 0; z--)
                    {
                        HGCell cell = mapData.GetCell(x, y, z);
                        //if(skyLightBlocked && (z < HGMapData.MAP_LEVELS - 1 && mapData.GetCell(x,y,z + 1).floorType != HGCellFloorType.PuddleLight))
                        if (skyLightBlocked && (z < HGMapData.MAP_LEVELS - 1 && (mapData.GetCell(x, y, z + 1).floorType != HGCellFloorType.PuddleLight && mapData.GetCell(x, y, z + 1).lit == false)))
                        {
                            cell.lit = false;
                            continue;
                        }
                        if (cell.opaque)
                        {
                            skyLightBlocked = true;
                            cell.lit = false;
                            continue;
                        }
                        cell.lit = true;
                    }
                }
            }
            byte[] cellData1 = new byte[mapData.cells.Length * 4];
            byte[] cellData2 = new byte[mapData.cells.Length * 4];
            byte[] cellData3 = new byte[mapData.cells.Length * 4];
            int monsters = 0;
            bool tooManyMonsters = false;
            i = 0;
            for (int z = 0; z < HGMapData.MAP_LEVELS; z++)
            {
                for (int y = 0; y < HGMapData.MAP_HEIGHT; y++)
                {
                    for (int x = 0; x < HGMapData.MAP_WIDTH; x++, i += 4)
                    {
                        HGCell cellCur = mapData.cells[x,y,z];

                        ////////////////////////////////////////////////////////////////
                        //Cell Data 1
                        ////////////////////////////////////////////////////////////////
                        if(cellCur.auxiliaryHere && cellCur.auxiliaryType == HGCellAuxiliaryType.Monster)
                        {
                            monsters++;
                        }
                        if (monsters > HGMapData.MAP_MONSTERS && tooManyMonsters == false)
                        {
                            tooManyMonsters = true;
                            System.Windows.Forms.MessageBox.Show($"[MapDataToByteArray] Too many monsters ({monsters})! Maximum is {HGMapData.MAP_MONSTERS}");
                        }
                        byte[] cellData1Cur = new byte[4];
                        byte[] panelButtonIndexBytes = BitConverter.GetBytes(cellCur.panelButtonIndex * 128).Reverse().ToArray();
                        if (cellCur.auxiliaryHere && (cellCur.auxiliaryType != HGCellAuxiliaryType.Monster || (cellCur.auxiliaryType == HGCellAuxiliaryType.Monster && monsters <= HGMapData.MAP_MONSTERS)))
                        {
                            cellData1Cur[0] += (byte)cellCur.auxiliaryType;
                        }
                        cellData1Cur[0] += panelButtonIndexBytes[2];
                        cellData1Cur[1] += panelButtonIndexBytes[3];
                        cellData1Cur[1] += (byte)cellCur.panelType;
                        cellData1Cur[1] += (byte)(cellCur.waterLevel * 2);
                        cellData1Cur[2] += (byte)(cellCur.pushable ? 1 : 0);
                        cellData1Cur[2] += (byte)cellCur.blockType;
                        cellData1Cur[2] += (byte)cellCur.floorType;
                        cellData1Cur[3] += (byte)(cellCur.floorHere ? 1 : 0);
                        cellData1Cur[3] += (byte)(cellCur.blockHere ? 2 : 0);
                        cellData1Cur[3] += (byte)(cellCur.waterHere ? 4 : 0);
                        cellData1Cur[3] += (byte)(cellCur.panelHere ? 8 : 0);
                        if (cellCur.auxiliaryHere && (cellCur.auxiliaryType != HGCellAuxiliaryType.Monster || (cellCur.auxiliaryType == HGCellAuxiliaryType.Monster && monsters <= HGMapData.MAP_MONSTERS)))
                        {
                            cellData1Cur[3] += (byte)(cellCur.auxiliaryHere ? 32 : 0);
                        }
                        cellData1Cur[3] += (byte)(cellCur.opaque ? 64 : 0);
                        cellData1Cur[3] += (byte)(cellCur.invisible ? 128 : 0);

                        if(cellCur.blockHere && cellCur.blockType == HGCellBlockType.Exit)
                        {
                            mapData.exitPos = new Coordinate3(x,y,z);
                            bytes[682] = (byte)mapData.exitPos.x;
                            bytes[683] = (byte)mapData.exitPos.y;
                            bytes[684] = (byte)mapData.exitPos.z;
                        }
                        ////////////////////////////////////////////////////////////////
                        //Cell Data 2
                        ////////////////////////////////////////////////////////////////
                        if(cellCur.auxiliaryHere && cellCur.auxiliaryType == HGCellAuxiliaryType.Monster)
                        {
                            mapData.monsterCount++;
                        }
                        byte[] cellData2Cur = new byte[4];
                        byte[] eggDelayBytes = BitConverter.GetBytes(cellCur.eggDelay * 16).Reverse().ToArray();
                        byte[] monsterTypeLevelBytes = BitConverter.GetBytes(HelperUtil.to1D(cellCur.monsterLevel,cellCur.monsterType,4) * 16).Reverse().ToArray();
                        if (cellCur.auxiliaryHere && cellCur.auxiliaryType == HGCellAuxiliaryType.Monster && monsters <= HGMapData.MAP_MONSTERS)
                        {
                            cellData2Cur[0] += (byte)eggDelayBytes[2];
                            cellData2Cur[1] += (byte)eggDelayBytes[3];
                            cellData2Cur[1] += (byte)monsterTypeLevelBytes[2];
                            cellData2Cur[2] += (byte)monsterTypeLevelBytes[3];
                        }
                        cellData2Cur[2] += (byte)(cellCur.forceQuestionMark ? 4 : 0);
                        cellData2Cur[2] += (byte)(cellCur.tidalWaterFlowing ? 8 : 0);
                        cellData2Cur[3] += (byte)cellCur.floorTile;

                        ////////////////////////////////////////////////////////////////
                        //Cell Data 3
                        ////////////////////////////////////////////////////////////////
                        byte[] cellData3Cur = new byte[4];

                        cellData3Cur[0] += (byte)(cellCur.lit ? 128 : 0);
                        cellData3Cur[0] += (byte)(cellCur.skyRendered[0] ? 1 : 0);
                        cellData3Cur[0] += (byte)(cellCur.skyRendered[1] ? 2 : 0);
                        cellData3Cur[0] += (byte)(cellCur.skyRendered[2] ? 4 : 0);
                        cellData3Cur[0] += (byte)(cellCur.skyRendered[3] ? 8 : 0);
                        if (cellCur.auxiliaryHere && cellCur.auxiliaryType == HGCellAuxiliaryType.Monster && monsters <= HGMapData.MAP_MONSTERS)
                        {
                            cellData3Cur[0] += (byte)(cellCur.eggErase ? 16 : 0);
                            cellData3Cur[0] += (byte)(cellCur.eggBlocked ? 32 : 0);
                        }
                        cellData3Cur[1] += (byte)cellCur.itemAmmo;

                        if (cellCur.blockType == HGCellBlockType.Teleport)
                        {
                            byte[] teleportTargetPosBytes = BitConverter.GetBytes(cellCur.teleportTargetPos.To1D(MAP_WIDTH, MAP_HEIGHT) * 4).Reverse().ToArray();
                            cellData3Cur[2] = teleportTargetPosBytes[2];
                            cellData3Cur[3] = teleportTargetPosBytes[3];
                        }
                        else if(cellCur.blockType == HGCellBlockType.FitnessBoost)
                        {
                            byte[] healingValueBytes = BitConverter.GetBytes(cellCur.healingValue).Reverse().ToArray();
                            cellData3Cur[2] += healingValueBytes[2];
                            cellData3Cur[3] += healingValueBytes[3];
                        }
                        else
                        {
                            cellData3Cur[2] += (byte)cellCur.itemDamaged;
                            cellData3Cur[3] += (byte)cellCur.itemID;
                        }
                        Array.Copy(cellData1Cur, 0, cellData1, i, cellData1Cur.Length);
                        Array.Copy(cellData2Cur, 0, cellData2, i, cellData2Cur.Length);
                        Array.Copy(cellData3Cur, 0, cellData3, i, cellData3Cur.Length);
                    }
                }
            }

            Array.Copy(cellData1, 0, bytes, 28478, cellData1.Length);
            Array.Copy(cellData2, 0, bytes, 71718, cellData2.Length);
            Array.Copy(cellData3, 0, bytes, 114958, cellData2.Length);

            return bytes;
        }
        public static string CheckMap(HGMapData mapData)
        {
            if (mapData == null)
            {
                return "";
            }
            string s = "";
            if (mapData.conversations.Length > MAP_CONVERSATIONS)
            {
                s += $"[CheckMap] Too many conversations ({mapData.conversations.Length})! Maximum is {MAP_CONVERSATIONS}.\r\n";
            }
            if(mapData.pushablesCount > MAP_PUSHABLES)
            {
                s += $"[CheckMap] Too many pushables ({mapData.pushablesCount})! Maximum is {MAP_PUSHABLES}.\r\n";
            }
            int buttonCount = mapData.CountButtons();
            if (buttonCount > MAP_BUTTONS)
            {
                s += $"[CheckMap] Too many buttons ({buttonCount})! Maximum is {MAP_BUTTONS}.\r\n";
            }
            int liftCount = mapData.CountLifts();
            if (liftCount > MAP_LIFTS)
            {
                s += $"[CheckMap] Too many lifts ({liftCount})! Maximum is {MAP_LIFTS}.\r\n";
            }
            int doorCount = mapData.CountDoors();
            if (doorCount > MAP_DOORS)
            {
                s += $"[CheckMap] Too many doors ({doorCount})! Maximum is {MAP_LIFTS}.\r\n";
            }
            if (mapData.monsterCount > MAP_MONSTERS)
            {
                s += $"[CheckMap] Too many monsters ({mapData.monsterCount})! Maximum is {MAP_MONSTERS}.\r\n";
            }
            return s;
        }
    }
}
