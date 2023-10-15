using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiredGunsData
{
    public class HGCharacter
    {
        public string name;
        public string description;
        public string portraitName;
        public int level;
        public HGCharacterLevel[] levels = new HGCharacterLevel[4];
        private int _health;
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = (int)Math.Round(value * ((levels != null && levels.Length > 0) ? levels[level].healthMultiplier : 1));
            }
        }
        public HGCharacter()
        {
            name = "Monster";
            description = "Monster description.";
            portraitName = $"{name.ToLower()}_portrait";
            Health = 1;
            level = 1;
            levels = new HGCharacterLevel[]
            {
                new HGCharacterLevel() { description = "Monster level 0 description" },
                new HGCharacterLevel() { description = "Monster level 1 description" },
                new HGCharacterLevel() { description = "Monster level 2 description" },
                new HGCharacterLevel() { description = "Monster level 3 description", shielded = true }
            };
        }

        public static HGCharacter[] CreateMonsters()
        {
            return new HGCharacter[]
            {
                new HGCharacter()
                {
                    name = "Leadhile",
                    description = "Fast and agile. Brave. Not strong but high endurance.",
                    portraitName = "leadhile_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Weak but fast."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Normal but fast."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong, fires strength 1 fireballs."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong, fires strength 3 fireballs.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Sentry gun",
                    description = "Won't move but turns and fires fireballs. Very tough.",
                    portraitName = "sentrygun_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 1 fireballs."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 2 fireballs."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 3 fireballs."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 4 fireballs."
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Rahl",
                    description = "Very ferocious. Slow but very strong. Posionous.",
                    portraitName = "rahl_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Quite strong but poor hit."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 1 fireballs."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Extremely strong. Fires strength 2 fireballs.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Skeleton",
                    description = "Very weak. Average speed. Very persistent.",
                    portraitName = "skeleton_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Very poor. (0.5x man strength)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Normal."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Above average.", shielded = true
                        },
                        new HGCharacterLevel()
                        {
                            description = "Quite strong. (2x man strength).", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "AHL-CIM",
                    description = "Very tough but not strong hit. Extremely slow.",
                    portraitName = "ahl-cim_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Poor. (3x man strengh)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average. (4x man strengh)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong. (5x man strengh)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong. (6x man strengh).", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Morg",
                    description = "Tough with a deadly hit. Powerful fireball and poison.",
                    portraitName = "morg_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Poor. Only strength 2 fireball."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average. 4 fireball and 2 poison."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong. 4 fireball and 2 poison."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong. Non-decay fireball!", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Druid",
                    description = "Quite strong. Liable to go beserk at any time. Cowardly.",
                    portraitName = "druid_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Undefined, only strength 2 poison."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average, 4 poison and 2 fireball."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong, 4 poison and 2 fireball."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong, 4 poison and 2 fireball.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Lemming",
                    description = "Slow but deadly. Avoid at all costs. Beserks easily.",
                    portraitName = "lemming_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Poor."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong, fires strength 1 fireball.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Hadrahl",
                    description = "Robotic equivalent of a leadhile but will beserk easily.",
                    portraitName = "hadrahl_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Weak but fast."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Normal but fast."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Bat",
                    description = "Very weak, but goes beserk easily. Very fast.",
                    portraitName = "bat_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Exceptionally weak."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Poor."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Ok."
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Hoverbot",
                    description = "Weak with a very poor hit. Long range fireball, though.",
                    portraitName = "hoverbot_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Poor. Strength 1 fireball. Range 8."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average. Strength 2 fireball."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Ok. Strength 2 fireball. Range 8."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong. Strength 2 fireball. Range 8.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Lizard",
                    description = "Very cowardly weak creature. Poisonous.",
                    portraitName = "lizard_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Exceptionally weak."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Poor."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Ok."
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Squirrel",
                    description = "Suprisingly strong and beserks when first hit.",
                    portraitName = "squirrel_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Poor. (4x man strengh)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average. (5x man strengh)."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong. (6x man strengh).", shielded = true
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong. (7x man strengh).", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Elyste",
                    description = "Powerful posion and fireball. Moves fairly fast.",
                    portraitName = "elyste_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Fireball 2, poison 2."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fireball 4, poison 4."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Strong, fireball 4, poison 4.", shielded = true
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong, fireball 4, poison 4.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "ED-209",
                    description = "Slighty less power than a Rahl but moves faster.",
                    portraitName = "ed-209_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Quite strong but poor hit."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Fires strength 1 fireballs.", shielded = true
                        },
                        new HGCharacterLevel()
                        {
                            description = "Extremely strong. Str 2 balls.", shielded = true
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Puppy",
                    description = "The weakest enemy. Harmless.",
                    portraitName = "puppy_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel()
                        {
                            description = "Exceptionally weak."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Average."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Poor."
                        },
                        new HGCharacterLevel()
                        {
                            description = "Very strong. Weak hit."
                        }
                    }
                },
                new HGCharacter()
                {
                    name = "Shark",
                    description = "Extremely dangerous predator, especially when beserk.",
                    portraitName = "shark_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel("Poor."),
                        new HGCharacterLevel("Average."),
                        new HGCharacterLevel("Strong."),
                        new HGCharacterLevel("Very strong.")
                    }
                },
                new HGCharacter()
                {
                    name = "Rats",
                    description = "Poisonous and persistent.",
                    portraitName = "rats_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel("Poor."),
                        new HGCharacterLevel("Average."),
                        new HGCharacterLevel("Strong."),
                        new HGCharacterLevel("Very strong, fires strength 1 fireball.")
                    }
                },
                new HGCharacter()
                {
                    name = "Baby Gargoyle",
                    description = "Fairly strong but slow. Will beserk.",
                    portraitName = "babygargoyle_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel("Poor."),
                        new HGCharacterLevel("Average."),
                        new HGCharacterLevel("Strong."),
                        new HGCharacterLevel("Very strong. Non-decay fireball!", shielded : true)
                    }
                },
                new HGCharacter()
                {
                    name = "Gargoyle",
                    description = "Ultimate bad dude. Very, very, very strong. Most heinous.",
                    portraitName = "gargoyle_portrait",
                    levels = new HGCharacterLevel[]
                    {
                        new HGCharacterLevel("Poor. (40x man strengh)."),
                        new HGCharacterLevel("Average. (50x man strengh)."),
                        new HGCharacterLevel("Strong. (60x man strengh)."),
                        new HGCharacterLevel("Very strong. (70x man strengh).", shielded : true)
                    }
                }
            };
        }
    }
    public struct HGCharacterLevel
    {
        public string description;
        public double healthMultiplier;
        public bool shielded;

        public HGCharacterLevel(string description = "Monster level description.", double healthMultiplier = 1.0, bool shielded = false)
        {
            this.description = description;
            this.healthMultiplier = healthMultiplier;
            this.shielded = shielded;
        }
    }
    public enum HGItemCategory
    {
        Weapon = 0,
        Ammo = 1,
        Consumable = 2,
        Key = 3,
        PsiAmp = 4,
        Other = 5,
        Unusable = 6
    }
    public struct HGItem
    {
        public string name;
        public HGCellAuxiliaryType container;
        public int ammo;
        public int damage;
        public HGItemCategory category;
        public HGItem(string name, HGCellAuxiliaryType container = HGCellAuxiliaryType.Container1, int ammo = 0, HGItemCategory category = HGItemCategory.Weapon)
        {
            this.name = name;
            this.container = container;
            this.ammo = ammo;
            this.damage = 0;
            this.category = category;
        }
        public static HGItem[] CreateItems()
        {
            HGItem[] items = new HGItem[]
            {
                new HGItem("Empty",HGCellAuxiliaryType.Container1,1,category : HGItemCategory.Unusable),
                new HGItem("Disruptor Cannon",HGCellAuxiliaryType.Container5,1,category : HGItemCategory.Weapon),
                new HGItem("Rocket Launcher",HGCellAuxiliaryType.Container5,1,category : HGItemCategory.Weapon),
                new HGItem("Laser Cannon",HGCellAuxiliaryType.Container5,200,category : HGItemCategory.Weapon),
                new HGItem("Mounted Machinegun",HGCellAuxiliaryType.Container5,250,category : HGItemCategory.Weapon),
                new HGItem("Hand Laser",HGCellAuxiliaryType.Container5,100,category : HGItemCategory.Weapon),
                new HGItem("Beam Rifle",HGCellAuxiliaryType.Container5,100,category : HGItemCategory.Weapon),
                new HGItem("Imperial Auto",HGCellAuxiliaryType.Container5,54,category : HGItemCategory.Weapon),
                new HGItem("Flamer",HGCellAuxiliaryType.Container5,40,category : HGItemCategory.Weapon),
                new HGItem("Flamer Junior",HGCellAuxiliaryType.Container4,40,category : HGItemCategory.Weapon),

                new HGItem("Super Covert",HGCellAuxiliaryType.Container4,54,category : HGItemCategory.Weapon),
                new HGItem("Silent Auto",HGCellAuxiliaryType.Container4,54,category : HGItemCategory.Weapon),
                new HGItem("Mini Gun",HGCellAuxiliaryType.Container5,200,category : HGItemCategory.Weapon),
                new HGItem("Auto Gun",HGCellAuxiliaryType.Container5,128,category : HGItemCategory.Weapon),
                new HGItem("Sniper Rifle",HGCellAuxiliaryType.Container5,54,category : HGItemCategory.Weapon),
                new HGItem("Tungsten Bore Rifle",HGCellAuxiliaryType.Container5,16,category : HGItemCategory.Weapon),
                new HGItem("S&W 29",HGCellAuxiliaryType.Container4,16,category : HGItemCategory.Weapon),
                new HGItem("Shotgun",HGCellAuxiliaryType.Container5,2,category : HGItemCategory.Weapon),
                new HGItem("UPBI 38",HGCellAuxiliaryType.Container4,64,category : HGItemCategory.Weapon),
                new HGItem("Blaster 52",HGCellAuxiliaryType.Container4,50,category : HGItemCategory.Weapon),

                new HGItem("Tungsten Bore Hand Gun",HGCellAuxiliaryType.Container4,32,category : HGItemCategory.Weapon),
                new HGItem("Sonic Stunner",HGCellAuxiliaryType.Container4,50,category : HGItemCategory.Weapon),
                new HGItem("Small Remains",HGCellAuxiliaryType.Container4,category : HGItemCategory.Unusable),
                new HGItem("Banana",HGCellAuxiliaryType.Container4,255,category : HGItemCategory.Weapon),
                new HGItem("EMF Stunner",HGCellAuxiliaryType.Container4,20,category : HGItemCategory.Weapon),
                new HGItem("Naomi IV Assault Rifle",HGCellAuxiliaryType.Container5,65,category : HGItemCategory.Weapon),
                new HGItem("Naomi Grenade Launcher",HGCellAuxiliaryType.Container5,3,category : HGItemCategory.Weapon),
                new HGItem("40mm Grenades",HGCellAuxiliaryType.Container4,6,category : HGItemCategory.Ammo),
                new HGItem("Sub-Machine Gun",HGCellAuxiliaryType.Container5,36,category : HGItemCategory.Weapon),
                new HGItem("Fusion Bomb 1",HGCellAuxiliaryType.Container5,1,category : HGItemCategory.Other),

                new HGItem("Sentry Gun Uni-D",HGCellAuxiliaryType.Container5,3,category : HGItemCategory.Other),
                new HGItem("Brick",HGCellAuxiliaryType.Container4,category : HGItemCategory.Unusable),
                new HGItem("Broken Bottle",HGCellAuxiliaryType.Container4,category : HGItemCategory.Unusable),
                new HGItem("Sentry Gun Multi-D",HGCellAuxiliaryType.Container5,3,category : HGItemCategory.Other),
                new HGItem("Proximity Mine",HGCellAuxiliaryType.Container4,category : HGItemCategory.Other),
                new HGItem("Proximity Mine Mk 2",HGCellAuxiliaryType.Container4,category : HGItemCategory.Other),
                new HGItem("Disruptor Charge",HGCellAuxiliaryType.Container4,1,category : HGItemCategory.Ammo),
                new HGItem("Rocket",HGCellAuxiliaryType.Container4,1,category : HGItemCategory.Ammo),
                new HGItem(".44 AET",HGCellAuxiliaryType.Container5,250,category : HGItemCategory.Ammo),
                new HGItem(".44 THV",HGCellAuxiliaryType.Container4,150,category : HGItemCategory.Ammo),

                new HGItem("9mm AET",HGCellAuxiliaryType.Container4,250,category : HGItemCategory.Ammo),
                new HGItem("Flamer Refill",HGCellAuxiliaryType.Container4,40,category : HGItemCategory.Ammo),
                new HGItem("Flamer UX Refill",HGCellAuxiliaryType.Container4,20,category : HGItemCategory.Ammo),
                new HGItem("Flamer Junior Refill",HGCellAuxiliaryType.Container4,43,category : HGItemCategory.Ammo),
                new HGItem("Flamer Junior UX Refill",HGCellAuxiliaryType.Container4,20,category : HGItemCategory.Ammo),
                new HGItem("Explosive Slug",HGCellAuxiliaryType.Container4,90,category : HGItemCategory.Ammo),
                new HGItem("Rifled Slug",HGCellAuxiliaryType.Container4,50,category : HGItemCategory.Ammo),
                new HGItem("9mm THV",HGCellAuxiliaryType.Container4,150,category : HGItemCategory.Ammo),
                new HGItem(".38 Glaser",HGCellAuxiliaryType.Container4,64,category : HGItemCategory.Ammo),
                new HGItem("1kA Cell",HGCellAuxiliaryType.Container4,50,category : HGItemCategory.Ammo),

                new HGItem("3kA Cell",HGCellAuxiliaryType.Container4,100,category : HGItemCategory.Ammo),
                new HGItem("6kA Cell",HGCellAuxiliaryType.Container4,200,category : HGItemCategory.Ammo),
                new HGItem("Large Remains",HGCellAuxiliaryType.Container5,category : HGItemCategory.Unusable),
                new HGItem("Banana Pieces",HGCellAuxiliaryType.Container4,255,category : HGItemCategory.Unusable),
                new HGItem("Baguette",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Cheese",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Bottle",HGCellAuxiliaryType.Container2,category : HGItemCategory.Unusable),
                new HGItem("Apple",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Chicken",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Loaf",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),

                new HGItem("Flask",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Standard Rations 1",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Standard Rations 2",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Standard Rations 3",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Medical Pack",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Immunisation",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Repair Kit",HGCellAuxiliaryType.Container2,category : HGItemCategory.Consumable),
                new HGItem("Fusion Bomb 2",HGCellAuxiliaryType.Container5,1,category : HGItemCategory.Other),
                new HGItem("Fusion Bomb 3",HGCellAuxiliaryType.Container5,1,category : HGItemCategory.Other),
                new HGItem("Fusion Bomb 4",HGCellAuxiliaryType.Container2,1,category : HGItemCategory.Other),

                new HGItem("Unused",HGCellAuxiliaryType.Container2,category : HGItemCategory.Unusable),
                new HGItem("Common Key",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Ornamental Key",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Cvilian Wafer",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Unrestricted Wafer",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Employee Wafer",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Officer's Wafer",HGCellAuxiliaryType.Container1,category : HGItemCategory.Key),
                new HGItem("Unused",HGCellAuxiliaryType.Container1,category : HGItemCategory.Unusable),
                new HGItem("Unused",HGCellAuxiliaryType.Container1,category : HGItemCategory.Unusable),
                new HGItem("DTS",HGCellAuxiliaryType.Container1,category : HGItemCategory.Other),

                new HGItem("Unused",HGCellAuxiliaryType.Container1,category : HGItemCategory.Unusable),
                new HGItem("Unused",HGCellAuxiliaryType.Container1,category : HGItemCategory.Unusable),
                new HGItem("PSI fireball",HGCellAuxiliaryType.Container3,20,category : HGItemCategory.PsiAmp),
                new HGItem("PSI inferno",HGCellAuxiliaryType.Container3,15,category : HGItemCategory.PsiAmp),
                new HGItem("PSI blast",HGCellAuxiliaryType.Container3,10,category : HGItemCategory.PsiAmp),
                new HGItem("PSI devastate",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI firesheld",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI shield",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI granite",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI titanium",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),

                new HGItem("PSI wall",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI banish wall",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI bridge",HGCellAuxiliaryType.Container3,4,category : HGItemCategory.PsiAmp),
                new HGItem("Unused",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.Unusable),
                new HGItem("PSI feather",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI sonic",HGCellAuxiliaryType.Container3,10,category : HGItemCategory.PsiAmp),
                new HGItem("PSI shock",HGCellAuxiliaryType.Container3,6,category : HGItemCategory.PsiAmp),
                new HGItem("PSI gills",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI part waves",HGCellAuxiliaryType.Container3,2,category : HGItemCategory.PsiAmp),
                new HGItem("PSI quench",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),

                new HGItem("PSI water tight",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("PSI electrify",HGCellAuxiliaryType.Container3,20,category : HGItemCategory.PsiAmp),
                new HGItem("PSI lightning",HGCellAuxiliaryType.Container3,15,category : HGItemCategory.PsiAmp),
                new HGItem("PSI fry",HGCellAuxiliaryType.Container3,10,category : HGItemCategory.PsiAmp),
                new HGItem("PSI farsight",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI teleport",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.PsiAmp),
                new HGItem("Unused",HGCellAuxiliaryType.Container3,5,category : HGItemCategory.Unusable),
                new HGItem("PSI cure poison",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI heal",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("PSI transmute",HGCellAuxiliaryType.Container3,10,category : HGItemCategory.PsiAmp),

                new HGItem("PSI miracle",HGCellAuxiliaryType.Container3,3,category : HGItemCategory.PsiAmp),
                new HGItem("Medium Remains",HGCellAuxiliaryType.Container4,category : HGItemCategory.Other),
                new HGItem("Mine Armed",HGCellAuxiliaryType.Container4,category : HGItemCategory.Other),
                new HGItem("Mine Mk.2 Armed",HGCellAuxiliaryType.Container4,category : HGItemCategory.Other),
                new HGItem("Nuts",HGCellAuxiliaryType.Container2,category : HGItemCategory.Other)
            };
            for(int i = 0; i < items.Length; i++)
            {
                items[i].name = items[i].name.Insert(0, $"{i,3}: ");
            }
            return items;
        }
    }
}
