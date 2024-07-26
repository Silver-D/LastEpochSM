using MelonLoader;
using HarmonyLib;
using LastEpochSM.Managers;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace LastEpochSM.Mods
{
    [RegisterTypeInIl2Cpp]
    class Skins : MonoBehaviour
    {
        public class Main : MelonMod
        {
            public override void OnLateUpdate()
            {
                if (LastEpochSM.Main.instance.IsNullOrDestroyed())
                    { Unregister("LastEpochSM.dll is not loaded"); return; }

                if (!Mod_Manager.instance.IsNullOrDestroyed())
                {
                    Mod_Manager.Register<Skins>();
                    MelonEvents.OnSceneWasInitialized.Subscribe(Skins.instance.OnSceneWasInitialized);

                    MelonEvents.OnLateUpdate.Unsubscribe(this.OnLateUpdate);
                }
            }
        }

        public Skins(System.IntPtr ptr) : base(ptr) { }
        public static Skins instance { get; private set; }

        public static Logger Log = new Logger("Skins");

        void Awake()
        {
            instance = this;
        }

        void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            Reset();

            if (Scenes.IsGameReady())
            {
                GUI_Manager.Textures.Load();
                UI.Assets.Load();
            }

            if (Scenes.IsGameScene())
            {
                Config.Init();
                Lists.Init();
                Visuals.Load();

                UI.CosmeticPanel.Tabs.Init();
                UI.CosmeticPanel.Slots.Init();
            }
        }

        void Update()
        {
            if (!Scenes.IsGameScene())
                return;

            if ((UI.Content.NeedToReopen) && (Lists.Initialized))
            {
                UI.Content.Open(UI.Content.item_type);
                UI.Content.NeedToReopen = false;
            }
        }

        void OnGUI()
        {
            if (Scenes.IsGameScene())
                UI.UpdateGUI();
        }

        public struct skin
        {
            public EquipmentType equip_type;
            public int subtype;
            public bool unique;
            public bool set;
            public ushort unique_id;
            public Sprite icon;
        }

        public struct equipment_type
        {
            public EquipmentType equip_type;
            public byte id;
        }

        public static void Reset()
        {
            UI.CosmeticPanel.load = false;
            Lists.Initialized = false;
            Config.Initialized = false;            
            UI.CosmeticPanel.Tabs.Initialized = false;
            Visuals.Character_Loaded = false;
        }

        public class Lists
        {
            public static string Character_Class = "";
            public static int Character_Class_Id = 0;

            public static System.Collections.Generic.List<skin> list_skins = new System.Collections.Generic.List<skin>();
            public static System.Collections.Generic.List<equipment_type> list_Equipment_types = new System.Collections.Generic.List<equipment_type>();

            public static bool Initialized = false;
            public static void Init()
            {
                Initialized = false;
                if (UniqueList.instance.IsNullOrDestroyed())
                {
                    Log.Msg("Try to Init UniqueList");
                    try { UniqueList.getUnique(0); }
                    catch { Log.Error("Init UniqueList"); }
                }

                list_Equipment_types = new System.Collections.Generic.List<equipment_type>();
                list_skins = new System.Collections.Generic.List<skin>();
                int basic_count = 0;
                try
                {
                    foreach (ItemList.BaseEquipmentItem item in ItemList.instance.EquippableItems)
                    {
                        list_Equipment_types.Add(new equipment_type
                        {
                            equip_type = item.type,
                            id = (byte)item.TryCast<ItemList.BaseItem>().baseTypeID
                        });

                        foreach (ItemList.EquipmentItem sub_item in item.subItems)
                        {
                            if (ItemList.instance.classCanEquip((byte)item.baseTypeID, sub_item.subTypeID, Lists.Character_Class_Id, 0, false))
                            {
                                ItemDataUnpacked new_item = new ItemDataUnpacked
                                {
                                    LvlReq = 0,
                                    classReq = ItemList.ClassRequirement.Any,
                                    itemType = (byte)item.baseTypeID,
                                    subType = (ushort)sub_item.subTypeID,
                                    rarity = 0,
                                    sockets = 0,
                                    uniqueID = 0
                                };
                                list_skins.Add(new skin
                                {
                                    equip_type = item.type,
                                    subtype = sub_item.subTypeID,
                                    unique = false,
                                    set = false,
                                    unique_id = 0,
                                    icon = Helper.GetItemIcon(new_item)
                                });
                            }
                        }
                    }
                    basic_count = list_skins.Count;
                }
                catch { Log.Error("Error ItemList"); }

                try
                {
                    foreach (UniqueList.Entry unique in UniqueList.instance.uniques)
                    {
                        if (unique.hasClassCompatibleSubType(Character_Class_Id, 0, false))
                        {
                            int item_rarity = 7;
                            if (unique.isSetItem) { item_rarity = 8; }
                            ItemDataUnpacked new_item = new ItemDataUnpacked
                            {
                                LvlReq = 0,
                                classReq = ItemList.ClassRequirement.Any,
                                itemType = unique.baseType,
                                subType = unique.subTypes[0],
                                rarity = (byte)item_rarity,
                                sockets = 0,
                                uniqueID = unique.uniqueID
                            };

                            list_skins.Add(new skin
                            {
                                equip_type = Helper.GetEquipmentTypeFromId(unique.baseType),
                                subtype = (int)unique.subTypes[0],
                                unique = true,
                                set = unique.isSetItem,
                                unique_id = unique.uniqueID,
                                icon = Helper.GetItemIcon(new_item)
                            });
                        }
                    }

                }
                catch { /*Log.Error("Error UniqueList");*/ }

                int unique_count = list_skins.Count - basic_count;
                if (unique_count > 0) { Initialized = true; }
            }
        }

        public class Config
        {
            public static string path;
            public static string filename = "";
            public static bool Initialized = false;

            private static bool loading = false;
            public static void Init()
            {
                if (loading)
                    return;

                try
                {
                    loading = true;
                    LE.Data.CharacterData char_data = PlayerFinder.getPlayerData();
                    string character_name = char_data.CharacterName;
                    if (character_name != "")
                    {
                        Lists.Character_Class_Id = char_data.CharacterClass;
                        Lists.Character_Class = char_data.GetCharacterClass().className;

                        path = Path.Combine(Conf_Manager.confPath, "Skins");

                        if (char_data.Cycle == LE.Data.Cycle.Legacy || char_data.Cycle == LE.Data.Cycle.Beta)
                            path = Path.Combine(path, "Legacy");
                        else if (char_data.Cycle == LE.Data.Cycle.Release)
                            path = Path.Combine(path, "Release");
                        else
                        {
                            string cycleName = ((LE.Data.Cycle)char_data.Cycle).ToString();
                            int cycleNum = (int)char_data.Cycle;

                            path = Path.Combine(path, cycleNum + "-" + cycleName);
                        }

                        filename = "/" + character_name + ".json";
                        Load.UserConfig();
                    }
                    else
                    {
                        filename = "";
                        loading = false;
                        Log.Error("Error Loading Skin Config, character name is null");
                    }
                }
                catch { }
            }            
            public class Data
            {
                public static UserSkin UserData = new UserSkin();

                public struct UserSkin
                {
                    public Load.saved_skin helmet;
                    public Load.saved_skin body;
                    public Load.saved_skin gloves;
                    public Load.saved_skin boots;
                    public Load.saved_skin weapon;
                    public Load.saved_skin offhand;
                }
            }            
            public class Load
            {
                public struct saved_skin
                {
                    public int type;
                    public int subtype;
                    public bool unique;
                    public bool set;
                    public int unique_id;
                }
                public static void UserConfig()
                {
                    Data.UserData = DefaultConfig();
                    if (File.Exists(path + filename))
                    {
                        try
                        {
                            Data.UserData = JsonConvert.DeserializeObject<Data.UserSkin>(File.ReadAllText(path + filename));
                            //Log.Msg("User Skins Loaded");
                        }
                        catch { Log.Error("Error loading file : " + filename); }
                    }
                    else { Save.Skins(); }
                    //Log.Msg("Config Init Done");
                    Initialized = true;
                    loading = false;
                }
                public static Data.UserSkin DefaultConfig()
                {
                    //Log.Msg("Skins Default Config Loaded");
                    saved_skin default_skin = new saved_skin()
                    {
                        type = -1,
                        subtype = -1,
                        unique = false,
                        set = false,
                        unique_id = -1
                    };
                    Data.UserSkin result = new Data.UserSkin
                    {
                        helmet = default_skin,
                        body = default_skin,
                        gloves = default_skin,
                        boots = default_skin,
                        weapon = default_skin,
                        offhand = default_skin
                    };

                    return result;
                }
            }
            public class Save
            {
                public static void Skins()
                {
                    string jsonString = JsonConvert.SerializeObject(Data.UserData, Formatting.Indented);
                    if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                    if (File.Exists(path + filename)) { File.Delete(path + filename); }
                    File.WriteAllText(path + filename, jsonString);
                }
            }
        }

        public class Helper
        {
            public static int GetIdFromEquipmentType(EquipmentType type)
            {
                int result = -1;
                foreach (equipment_type eq_t in Lists.list_Equipment_types)
                {
                    if (type == eq_t.equip_type) { result = eq_t.id; break; }
                }

                return result;
            }
            public static EquipmentType GetEquipmentTypeFromId(byte id)
            {
                EquipmentType result = EquipmentType.IDOL_4x1;
                foreach (equipment_type eq_t in Lists.list_Equipment_types)
                {
                    if (id == eq_t.id) { result = eq_t.equip_type; break; }
                }
                //Log.Msg("GetEquipmentTypeFromId : Id = " + id + ", Type = " + result.ToString());

                return result;
            }
            public static Sprite GetItemIcon(ItemDataUnpacked item)
            {                
                Sprite result = null;
                try { result = UITooltipItem.SetItemSprite(item); }
                catch { Log.Error("Error GetItemIcon"); }
                if (result == null) { Log.Error("Error GetItemIcon = null"); }

                return result;
            }

            public struct Ui_Info
            {
                public Vector2 position;
                public Vector2 size;
            }
            public static Ui_Info Get_UI_Infos_Center(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    //Log.Msg("Found : " + go.name);
                    //Log.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    //Log.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    //Log.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x - (Size.x / 2), position.y - (Size.y / 2) + 40);
                    //Log.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
            public static Ui_Info Get_UI_Infos_Right(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    Log.Msg("Found : " + go.name);
                    Log.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    Log.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    Log.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x - Size.x, position.y - (Size.y / 2));
                    Log.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
            public static Ui_Info Get_UI_Infos(GameObject go)
            {
                Ui_Info result = new Ui_Info();
                try
                {
                    Log.Msg("Found : " + go.name);
                    Log.Msg("Position : X = " + go.transform.position.x + ", Y = " + go.transform.position.y);
                    Log.Msg("Local Position : X = " + go.transform.localPosition.x + ", Y = " + go.transform.localPosition.y);
                    Vector3 position = go.transform.position;
                    Vector3 lossy = go.transform.lossyScale;
                    RectTransform rect_transform = go.GetComponent<RectTransform>();
                    Vector2 Size = new Vector2(rect_transform.rect.width * lossy.x, rect_transform.rect.height * lossy.y);
                    Log.Msg("size : X = " + Size.x + ", Y = " + Size.y);
                    Vector3 Position = new Vector3(position.x, position.y);
                    Log.Msg("Calculed Real position : X = " + Position.x + ", Y = " + Position.y);

                    result.position = Position;
                    result.size = Size;
                }
                catch { }

                return result;
            }
        }

        public class Visuals
        {
            public static bool Character_Loaded = false;
            private static bool loading = false;

            [HarmonyPatch(typeof(EquipmentVisualsManager), "EquipWeapon")]
            public class EquipmentVisualsManager_EquipWeapon
            {
                [HarmonyPrefix]
                static void Prefix(ref EquipmentVisualsManager __instance, ref int __0, ref int __1, ref int __2, ref ushort __3, ref IMSlotType __4, ref WeaponEffect __5)
                {
                    if (Mod_Manager.CanPatch(Config.Initialized && Lists.Initialized))
                    {
                        //Log.Msg("EquipmentVisualsManager:EquipWeapon");
                        //Log.Msg("Default -> __0 = " + __0 + ", __1 = " + __1 + ", __2 = " + __2 + ", uniqueId = " + __3 + ", Slot = " + __4.ToString());
                        try
                        {
                            bool found = false;
                            Config.Load.saved_skin skin = new Config.Load.saved_skin()
                            {
                                type = -1,
                                subtype = -1,
                                unique = false,
                                set = false,
                                unique_id = -1
                            };
                            if (__4 == IMSlotType.MainHand)
                            {
                                skin = Config.Data.UserData.weapon;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__4 == IMSlotType.OffHand)
                            {
                                skin = Config.Data.UserData.offhand;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }

                            if (found)
                            {
                                int rarity = 7;
                                if (skin.set) { rarity = 8; }
                                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                                {
                                    __0 = skin.type;
                                    __1 = skin.subtype;
                                    __2 = rarity;
                                    __3 = (ushort)skin.unique_id;
                                    //__5 = WeaponEffect.Fire; //not working
                                }
                                //Log.Msg("Override -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                            }
                        }
                        catch { Log.Error("EquipmentVisualsManager:EquipWeapon"); }
                    }
                }
            }

            [HarmonyPatch(typeof(EquipmentVisualsManager), "EquipGear")]
            public class EquipmentVisualsManager_EquipGear
            {
                [HarmonyPrefix]
                static void Prefix(ref EquipmentVisualsManager __instance, ref EquipmentType __0, ref int __1, ref bool __2, ref ushort __3)
                {
                    //Log.Msg("EquipmentVisualsManager:EquipGear");
                    //Log.Msg("Default -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                    if (Mod_Manager.CanPatch(Config.Initialized))
                    {
                        try
                        {
                            bool found = false;
                            Config.Load.saved_skin skin = new Config.Load.saved_skin()
                            {
                                type = -1,
                                subtype = -1,
                                unique = false,
                                set = false,
                                unique_id = -1
                            };
                            if (__0 == EquipmentType.HELMET)
                            {
                                skin = Config.Data.UserData.helmet;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.BODY_ARMOR)
                            {
                                skin = Config.Data.UserData.body;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.GLOVES)
                            {
                                skin = Config.Data.UserData.gloves;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }
                            else if (__0 == EquipmentType.BOOTS)
                            {
                                skin = Config.Data.UserData.boots;
                                if (skin.unique) { skin.subtype = UniqueList.GetVisualSubType((ushort)skin.unique_id, skin.subtype); }
                                found = true;
                            }

                            //Set
                            if (found)
                            {
                                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                                {
                                    __0 = Helper.GetEquipmentTypeFromId((byte)skin.type);
                                    __1 = skin.subtype;
                                    __2 = skin.unique;
                                    __3 = (ushort)skin.unique_id;
                                }
                                //Log.Msg("Override -> Type = " + __0.ToString() + ", SubType = " + __1 + ", isUnique = " + __2 + ", uniqueId = " + __3);
                            }
                        }
                        catch { Log.Error("EquipmentVisualsManager:EquipGear"); }
                    }
                }
            }

            public static bool CheckIsValid(Config.Load.saved_skin skin)
            {
                bool result = false;
                if ((skin.type > -1) && (skin.subtype > -1) && (skin.unique_id > -1))
                {
                    result = true;
                }

                return result;
            }

            public static void Load()
            {
                if ((Config.Initialized) && (Lists.Initialized) && (!loading))
                {
                    try
                    {
                        loading = true;
                        EquipmentVisualsManager visual_manager = PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>();
                        if (CheckIsValid(Config.Data.UserData.helmet)) { visual_manager.EquipGear(EquipmentType.HELMET, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.body)) { visual_manager.EquipGear(EquipmentType.BODY_ARMOR, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.gloves)) { visual_manager.EquipGear(EquipmentType.GLOVES, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.boots)) { visual_manager.EquipGear(EquipmentType.BOOTS, 0, false, 0); }
                        if (CheckIsValid(Config.Data.UserData.offhand)) { visual_manager.EquipWeapon(0, 0, 0, 0, IMSlotType.OffHand, WeaponEffect.None); }
                        if (CheckIsValid(Config.Data.UserData.weapon)) { visual_manager.EquipWeapon(0, 0, 0, 0, IMSlotType.MainHand, WeaponEffect.None); }

                        Character_Loaded = true;
                        //Log.Msg("Skin Loaded");
                        loading = false;
                    }
                    catch { loading = false; }
                }
            }
        }
    }
}
