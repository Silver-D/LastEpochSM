using HarmonyLib;
using LastEpochSM.Managers;
using UnityEngine;

namespace LastEpochSM.Mods
{
    class UI : Skins
    {
        public UI(System.IntPtr ptr) : base(ptr) { }

        public static void UpdateGUI()
        {
            if (CosmeticPanel.isOpen)
            {
                int margin_w = 3; //3%
                int margin_h = 8; //3%
                float position_x = CosmeticPanel.position.x - (Screen.width * margin_w / 100);
                float position_y = CosmeticPanel.position.y + (Screen.height * margin_h / 100);

                float position_center_x = position_x + (CosmeticPanel.size.x / 2);
                float slot_size = CosmeticPanel.size.x * 30 / 100;
                float slot_margin_w = (CosmeticPanel.size.x * 3 / 100);
                float slot_margin_h = (CosmeticPanel.size.y * 4 / 100);

                foreach (CosmeticPanel.Slots.SkinSlot slot in CosmeticPanel.Slots.list_slots)
                {
                    float pos_x = 0;
                    float pos_y = 0;
                    float size_w = slot_size + ((slot_size / 2) * (slot.w - 1));
                    float size_h = 0; //slot_size + ((slot_size / 2) * (slot.h - 1));

                    float double_size_h = slot_size + (slot_size / 2);
                    Config.Load.saved_skin user_skin = new Skins.Config.Load.saved_skin()
                    {
                        type = -1,
                        subtype = -1,
                        unique = false,
                        set = false,
                        unique_id = -1
                    };
                    EquipmentType base_equip_type = EquipmentType.IDOL_4x1;

                    if (slot.name == CosmeticPanel.Slots.helmet)
                    {
                        size_h = size_w;
                        pos_x = position_center_x - (size_w / 2);
                        pos_y = position_y;

                        user_skin = Config.Data.UserData.helmet;
                        base_equip_type = EquipmentType.HELMET;
                    }
                    else if (slot.name == CosmeticPanel.Slots.body_armor)
                    {
                        size_h = size_w * 1.4f;
                        pos_x = position_center_x - (size_w / 2);
                        pos_y = position_y + slot_size + slot_margin_h + 10;

                        user_skin = Config.Data.UserData.body;
                        base_equip_type = EquipmentType.BODY_ARMOR;
                    }
                    else if (slot.name == CosmeticPanel.Slots.weapon)
                    {
                        float old_size = size_w;
                        size_w = CosmeticPanel.size.x * 20 / 100;
                        size_h = size_w * 1.4f;
                        pos_x = position_center_x - (old_size / 2) - slot_margin_w - size_w;
                        pos_y = position_y + slot_size + slot_margin_h + 10;

                        user_skin = Config.Data.UserData.weapon;
                        base_equip_type = EquipmentType.ONE_HANDED_AXE;
                    }
                    else if (slot.name == CosmeticPanel.Slots.offhand)
                    {
                        float old_size = size_w;
                        size_w = CosmeticPanel.size.x * 20 / 100;
                        size_h = size_w * 1.4f;
                        pos_x = position_center_x + (old_size / 2) + slot_margin_w;
                        pos_y = position_y + slot_size + slot_margin_h + 10;

                        user_skin = Config.Data.UserData.offhand;
                        base_equip_type = EquipmentType.SHIELD;
                    }
                    else if (slot.name == CosmeticPanel.Slots.gloves)
                    {
                        float old_size = size_w;
                        size_w = CosmeticPanel.size.x * 20 / 100;
                        size_h = size_w;
                        pos_x = position_center_x - (old_size / 2) - slot_margin_w - size_w;
                        pos_y = position_y + slot_size + double_size_h + (2 * slot_margin_h);

                        user_skin = Config.Data.UserData.gloves;
                        base_equip_type = EquipmentType.GLOVES;
                    }
                    else if (slot.name == CosmeticPanel.Slots.boots)
                    {
                        size_h = size_w;
                        pos_x = position_center_x - (size_w / 2);
                        pos_y = position_y + slot_size + double_size_h + (2 * slot_margin_h);

                        user_skin = Config.Data.UserData.boots;
                        base_equip_type = EquipmentType.BOOTS;
                    }

                    try
                    {
                        GUI.DrawTexture(new Rect(pos_x, pos_y, size_w, size_h), slot.background.texture);
                    }
                    catch { Log.Error("Error Background"); }
                    //Selected_icon
                    pos_x += 5;
                    pos_y += 5;
                    size_w -= 10;
                    size_h -= 10;

                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    style.normal.background = null;
                    style.normal.textColor = Color.grey;
                    style.hover.background = style.normal.background;
                    style.hover.textColor = style.normal.textColor;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 16;

                    if ((user_skin.type > -1) && (user_skin.subtype > -1) && (user_skin.unique_id > -1))
                    {
                        EquipmentType skin_type = EquipmentType.IDOL_4x1;
                        try { skin_type = Helper.GetEquipmentTypeFromId((byte)user_skin.type); }
                        catch { Log.Error("Error Equipment Type"); }

                        try
                        {
                            int count = Lists.list_skins.Count;
                            if (count == 0) { Log.Error("list_skins count = 0"); }
                        }
                        catch { Log.Error("Error list_skins"); }

                        bool found = false;
                        int found_index = -1;
                        try
                        {
                            for (int index = 0; index < Lists.list_skins.Count; index++)
                            {
                                if ((Lists.list_skins[index].equip_type == skin_type) &&
                                    (Lists.list_skins[index].subtype == user_skin.subtype) &&
                                    (Lists.list_skins[index].unique == user_skin.unique) &&
                                    (Lists.list_skins[index].unique_id == user_skin.unique_id))
                                {
                                    //Log.Msg("Found : index = " + index);
                                    found = true;
                                    found_index = index;
                                    break;
                                }
                            }
                        }
                        catch { Log.Error("Error selected Skin"); }

                        if (found)
                        {
                            if (found_index < Lists.list_skins.Count)
                            {
                                try
                                {
                                    if (Lists.list_skins[found_index].icon != null)
                                    {
                                        style.normal.background = Lists.list_skins[found_index].icon.texture;
                                    }
                                    else { Lists.Initialized = false; }
                                }
                                catch { Log.Error("Error icon"); }

                                try
                                {
                                    if (GUI.Button(new Rect(pos_x, pos_y, size_w, size_h), "", style))
                                    {
                                        if (slot.name == CosmeticPanel.Slots.weapon) { CosmeticPanel.Slots.ClickWeapon(); }
                                        else if (slot.name == CosmeticPanel.Slots.offhand)  { CosmeticPanel.Slots.ClickOffhand(); }
                                        else { CosmeticPanel.Slots.Click(Lists.list_skins[found_index]); }
                                    }
                                }
                                catch { Log.Error("Error btn"); }
                            }
                            //else { Log.Error("Error index out of range"); }
                        }
                        //else { Log.Error("Error Skin not found"); }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(pos_x, pos_y, size_w, size_h), "", style))
                        {
                            if (slot.name == CosmeticPanel.Slots.weapon)
                            {
                                CosmeticPanel.Slots.ClickWeapon();
                            }
                            else if (slot.name == CosmeticPanel.Slots.offhand)
                            {
                                CosmeticPanel.Slots.ClickOffhand();
                            }
                            else
                            {
                                CosmeticPanel.Slots.Click(new skin()
                                {
                                    equip_type = base_equip_type,
                                    subtype = 0,
                                    unique = false,
                                    unique_id = 0,
                                });
                            }
                        }
                    }
                }
            }

            if (Content.isOpen)
            {
                int margin_lef = 1;
                int margin_right = 1;
                int margin_top = 5;
                int margin_bottom = 3;

                int content_margin = 20;
                float position_x = Content.Position.x + (Screen.width * margin_lef / 100);
                float position_y = Content.Position.y + (Screen.height * margin_top / 100); // + content_margin;
                float size_w = Content.Size.x - (Screen.width * margin_lef / 100) - (Screen.width * margin_right / 100);
                float size_h = Content.Size.y - (Screen.height * margin_top / 100) - (Screen.height * margin_bottom / 100);
                //GUI.DrawTexture(new Rect(Content.Position.x, position_y, Content.Size.x, size_h), GUI_Manager.Textures.grey);
                float scrollview_size_w = size_w;
                int row = 0;
                int column = 0;
                int column_max = 6;
                float btn_size = ((scrollview_size_w - ((column_max + 1) * content_margin)) / column_max);
                int nb_row = Content.list_skins_by_type.Count / column_max;
                float scrollview_size_h = nb_row * (btn_size + content_margin);//(nb_row * btn_size) + ((nb_row + 1) * content_margin);

                if (scrollview_size_h > size_h) { scrollview_size_w -= 20; }
                else { size_h = scrollview_size_h; }
                Content.dropdown_scrollview = GUI.BeginScrollView(new Rect(position_x, position_y, size_w, size_h), Content.dropdown_scrollview, new Rect(0, 0, scrollview_size_w, scrollview_size_h));

                //button reset
                GUIStyle none_style = new GUIStyle(GUI.skin.button);
                none_style.normal.background = GUI_Manager.Textures.grey;
                none_style.normal.textColor = Color.black;
                none_style.hover.background = none_style.normal.background;
                none_style.hover.textColor = none_style.normal.textColor;
                none_style.alignment = TextAnchor.MiddleCenter;
                none_style.fontSize = 20;
                if (GUI.Button(new Rect(column * (btn_size + content_margin), (row * btn_size) + (row * content_margin), btn_size, btn_size), "Reset", none_style))
                {
                    CosmeticPanelUI c_p_ui = InventoryPanelUI.instance.gameObject.GetComponent<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    if (c_p_ui.flyoutTitle.text == Content.Weapons_Tile) { Content.SkinWeaponReset(); }
                    else if (c_p_ui.flyoutTitle.text == Content.Offhand_Tile) { Content.SkinOffhandReset(); }
                    else { Content.SkinReset(); }
                }
                //GUI.TextField(new Rect(column * (btn_size + content_margin), (row * btn_size) + (row * content_margin), btn_size, btn_size), "Cheats", Styles.Content_Title());
                column++;

                for (int index = 0; index < Content.list_skins_by_type.Count; index++)
                {
                    if (column > (column_max - 1)) { row++; column = 0; }

                    float pos_x = column * (btn_size + content_margin);//(column * btn_size) + ((column + 1) * content_margin);
                    float pos_y = (row * btn_size) + (row * content_margin);

                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    try
                    {
                        style.normal.background = Content.list_skins_by_type[index].icon.texture;
                    }
                    catch
                    {
                        Lists.Initialized = false;
                        Content.NeedToReopen = true;
                        break;
                    }
                    style.normal.textColor = Color.grey;
                    style.hover.background = style.normal.background;
                    style.hover.textColor = style.normal.textColor;
                    style.onFocused.background = style.normal.background;
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 16;

                    Texture2D background = GUI_Manager.Textures.black;
                    if (Content.list_skins_by_type[index].set) { background = GUI_Manager.Textures.texture_set; }
                    else if (Content.list_skins_by_type[index].unique) { background = GUI_Manager.Textures.texture_unique; }

                    GUI.DrawTexture(new Rect(pos_x, pos_y, btn_size, btn_size), background);
                    if (GUI.Button(new Rect(pos_x, pos_y, btn_size, btn_size), "", style))
                    {
                        CosmeticPanelUI c_p_ui = InventoryPanelUI.instance.gameObject.GetComponent<InventoryPanelUI>().cosmeticPanel.GetComponent<CosmeticPanelUI>();
                        if (c_p_ui.flyoutTitle.text == Content.Weapons_Tile)
                        {
                            Content.SkinClickWeapon(Content.list_skins_by_type[index]);
                        }
                        else if (c_p_ui.flyoutTitle.text == Content.Offhand_Tile)
                        {
                            Content.SkinClickOffHand(Content.list_skins_by_type[index]);
                        }
                        else { Content.SkinClick(Content.list_skins_by_type[index]); }
                    }
                    column++;
                }
                GUI.EndScrollView();

            }
        }

        public class Assets
        {
            public struct Sprites
            {
                public static Sprite Armor;
                public static Sprite Boots;
                public static Sprite Gloves;
                public static Sprite Helmet;
                public static Sprite Shield;
                public static Sprite Weapon;
            }

            public static void Load()
            {
                /*AssetBundle asset_bundle = Asset_Manager.LoadBundle("lastepochmods.asset");

                if (asset_bundle != null)
                {
                    foreach (string name in asset_bundle.GetAllAssetNames())
                    {
                        if ((Asset_Manager.Functions.Check_Texture(name)) && (name.Contains("/skins/")))
                        {
                            Texture2D texture = asset_bundle.LoadAsset(name).TryCast<Texture2D>();
                            Sprite picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                                 if (name.Contains("armor"))  { Sprites.Armor = picture; }
                            else if (name.Contains("boots"))  { Sprites.Boots = picture; }
                            else if (name.Contains("gloves")) { Sprites.Gloves = picture; }
                            else if (name.Contains("helmet")) { Sprites.Helmet = picture; }
                            //else if (name.Contains("shield")) { Sprites.Shield = picture; }
                            else if (name.Contains("weapon")) { Sprites.Weapons = picture; }
                        }
                    }
                }*/

                Texture2D texture;
                string[] equipment = { "Helmet", "Armor", "Boots", "Gloves", "Weapon", "Shield"};

                foreach(string e in equipment)
                {
                    texture = new Texture2D(2, 2);
                    ImageConversion.LoadImage(texture, Asset_Manager.LoadFromResource(instance.GetType(), "Skins.Resources." + e + ".png"));
                    Sprite picture = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                    if      (e == "Helmet") { Sprites.Helmet = picture; }
                    else if (e == "Armor")  { Sprites.Armor  = picture; }
                    else if (e == "Boots")  { Sprites.Boots  = picture; }
                    else if (e == "Gloves") { Sprites.Gloves = picture; }
                    else if (e == "Weapon") { Sprites.Weapon = picture; }
                    else if (e == "Shield") { Sprites.Shield = picture; }
                }
            }
        }

        public class CosmeticPanel
        {
            public static bool load = false;
            public static bool isOpen = false;
            public static Vector2 size;
            public static Vector2 position;

            [HarmonyPatch(typeof(InventoryPanelUI), "OnEnable")]
            public class InventoryPanelUI_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref InventoryPanelUI __instance)
                {
                    if (!Mod_Manager.CanPatch()) { return; }

                    load = true;
                    Tabs.Init();
                }
            }

            [HarmonyPatch(typeof(CosmeticPanelUI), "OnEnable")]
            public class CosmeticPanelUI_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticPanelUI __instance)
                {
                    if (!Mod_Manager.CanPatch()) { return; }

                    try
                    {
                        __instance.getCoinsButton.gameObject.active = false;
                        __instance.openShopButton.gameObject.active = false;
                        __instance.availableTitlesDropdown.gameObject.active = false;
                        foreach (CosmeticItemSlot equip_slot in __instance.equipSlots) { equip_slot.gameObject.active = false; }
                        foreach (CosmeticItemSlot pet_slot in __instance.petSlots) { pet_slot.gameObject.active = false; }
                    }
                    catch { Log.Error("CosmeticPanelUI:OnEnable Error Hidding defaults slots"); }

                    try
                    {
                        //GameObject SkillNavigation =
                            GUI_Manager.Functions.GetChild(__instance.gameObject, "SkillNavigation").active = false;
                        //SkillNavigation.active = false;
                        //GameObject SkillCosmetics =
                            GUI_Manager.Functions.GetChild(__instance.gameObject, "Skill Cosmetics").active = false;
                    }
                    catch { }

                    try
                    {
                        RectTransform rect_transform = __instance.gameObject.GetComponent<RectTransform>();
                        position = new Vector2(Screen.width + rect_transform.rect.x, (Screen.height / 2) + rect_transform.rect.y); //Anchor Middle Right
                        size = new Vector2(rect_transform.rect.width, rect_transform.rect.height);
                        isOpen = true;
                    }
                    catch { Log.Error("CosmeticPanelUI:OnEnable Error Initializing UI"); }
                }
            }

            [HarmonyPatch(typeof(CosmeticPanelUI), "OnDisable")]
            public class CosmeticPanelUI_OnDisable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticPanelUI __instance)
                {
                    if (!Mod_Manager.CanPatch()) { return; }

                    isOpen = false;
                }
            }

            public class Tabs
            {
                public static bool Initialized = false;
                public static void Init()
                {
                    try
                    {
                        if (Lists.Initialized)
                        {
                            foreach (TabUIElement tab in InventoryPanelUI.instance.tabController.tabElements)
                            {
                                if (tab.gameObject.name == "AppearanceTab")
                                {
                                    tab.isDisabled = false;
                                    tab.canvasGroup.TryCast<Behaviour>().enabled = false;
                                    break;
                                }
                            }
                            Initialized = true;
                        }
                    }
                    catch { Log.Error("Tabs:Init(); -> TabElements"); }
                }
            }

            public class Slots
            {
                public static System.Collections.Generic.List<SkinSlot> list_slots = new System.Collections.Generic.List<SkinSlot>();
                public struct SkinSlot
                {
                    public string name;
                    public Sprite background;
                    public int h;
                    public int w;
                }
                public static string helmet = "Helmet";
                public static string body_armor = "Body";
                public static string weapon = "Weapon";
                public static string offhand = "OffHand";
                public static string gloves = "Gloves";
                public static string boots = "Boots";

                public static void Init()
                {
                    //Log.Msg("Initialize Slots for Skins");
                    list_slots = new System.Collections.Generic.List<SkinSlot>();
                    try
                    {
                        list_slots.Add(new SkinSlot
                        {
                            name = helmet,
                            background = GetSlotSprite(helmet),
                            h = 1,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = body_armor,
                            background = GetSlotSprite(body_armor),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = weapon,
                            background = GetSlotSprite(weapon),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = offhand,
                            background = GetSlotSprite(offhand),
                            h = 2,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = gloves,
                            background = GetSlotSprite(gloves),
                            h = 1,
                            w = 1
                        });
                        list_slots.Add(new SkinSlot
                        {
                            name = boots,
                            background = GetSlotSprite(boots),
                            h = 1,
                            w = 1
                        });
                    }
                    catch { Log.Error("Init Slots"); }
                }
                public static void Click(skin s)
                {
                    Content.Open(Helper.GetIdFromEquipmentType(s.equip_type));
                }
                public static void ClickWeapon()
                {
                    Content.OpenWeapon();
                }
                public static void ClickOffhand()
                {
                    Content.OpenOffhand();
                }
                private static Sprite GetSlotSprite(string slot_name)
                {
                    Sprite sprite = null;
                    if (slot_name == helmet) { sprite = Assets.Sprites.Helmet; }
                    else if (slot_name == body_armor) { sprite = Assets.Sprites.Armor; }
                    else if (slot_name == weapon) { sprite = Assets.Sprites.Weapon; }
                    else if (slot_name == offhand) { sprite = Assets.Sprites.Shield; }
                    else if (slot_name == gloves) { sprite = Assets.Sprites.Gloves; }
                    else if (slot_name == boots) { sprite = Assets.Sprites.Boots; }

                    return sprite;
                }
            }
        }

        public class Content
        {
            public static string Weapons_Tile = "Weapons Appearance";
            public static string Offhand_Tile = "OffHand Appearance";

            public static bool NeedToReopen = false;
            public static bool isOpen = false;
            public static Vector3 Position = Vector3.zero;
            public static Vector2 Size = Vector2.zero;
            public static Rect dropdown_rect = new Rect(125, 50, 250, 400);
            public static Vector2 dropdown_scrollview = Vector2.zero;
            public static int item_type = 0;
            public static System.Collections.Generic.List<skin> list_skins_by_type = new System.Collections.Generic.List<skin>();

            [HarmonyPatch(typeof(CosmeticsFlyoutSelectionContentNavigable), "OnEnable")]
            public class CosmeticsFlyoutSelectionContentNavigable_OnEnable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticsFlyoutSelectionContentNavigable __instance)
                {
                    if (!Mod_Manager.CanPatch()) { return; }

                    //Get Position & Size
                    Helper.Ui_Info infos = Helper.Get_UI_Infos_Center(__instance.gameObject);
                    Size = infos.size;
                    Position = infos.position;

                    //Hide All Cosmetics
                    GameObject content = null;
                    try { content = GUI_Manager.Functions.GetChild(__instance.contentMaskRect.gameObject, "Content"); }
                    catch { content = null; }
                    if (!content.IsNullOrDestroyed())
                    {
                        for (int i = 0; i < content.transform.childCount; i++)
                        {
                            content.transform.GetChild(i).gameObject.active = false;
                        }
                    }
                    //Hide Buy Btn
                    __instance.shopButton.gameObject.active = false;
                    isOpen = true;
                }
            }

            [HarmonyPatch(typeof(CosmeticsFlyoutSelectionContentNavigable), "OnDisable")]
            public class CosmeticsFlyoutSelectionContentNavigable_OnDisable
            {
                [HarmonyPostfix]
                static void Postfix(ref CosmeticsFlyoutSelectionContentNavigable __instance)
                {
                    if (!Mod_Manager.CanPatch()) { return; }

                    isOpen = false;
                }
            }

            public static void Open(int type)
            {
                try
                {
                    item_type = type;
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if (s.equip_type == Helper.GetEquipmentTypeFromId((byte)item_type)) { list_skins_by_type.Add(s); }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Helper.GetEquipmentTypeFromId((byte)item_type) + " Appearance";
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Log.Error("Content Open"); }
            }

            public static void OpenWeapon()
            {
                try
                {
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if ((s.equip_type == EquipmentType.ONE_HANDED_AXE) || (s.equip_type == EquipmentType.TWO_HANDED_AXE) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_MACES) || (s.equip_type == EquipmentType.TWO_HANDED_MACE) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_SWORD) || (s.equip_type == EquipmentType.TWO_HANDED_SWORD) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_FIST) || (s.equip_type == EquipmentType.ONE_HANDED_DAGGER) ||
                                (s.equip_type == EquipmentType.TWO_HANDED_SPEAR) || (s.equip_type == EquipmentType.TWO_HANDED_STAFF) ||
                                (s.equip_type == EquipmentType.BOW) || (s.equip_type == EquipmentType.CROSSBOW) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_SCEPTRE) || (s.equip_type == EquipmentType.WAND))
                        {
                            list_skins_by_type.Add(s);
                        }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Weapons_Tile;
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Log.Error("Weapons Open"); }
            }

            public static void OpenOffhand()
            {
                try
                {
                    list_skins_by_type = new System.Collections.Generic.List<skin>();
                    foreach (skin s in Lists.list_skins)
                    {
                        if ((s.equip_type == EquipmentType.ONE_HANDED_AXE) || (s.equip_type == EquipmentType.ONE_HANDED_MACES) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_SWORD) || (s.equip_type == EquipmentType.ONE_HANDED_FIST) ||
                                (s.equip_type == EquipmentType.ONE_HANDED_DAGGER) || (s.equip_type == EquipmentType.ONE_HANDED_SCEPTRE) ||
                                (s.equip_type == EquipmentType.WAND) || (s.equip_type == EquipmentType.SHIELD) ||
                                (s.equip_type == EquipmentType.QUIVER) || (s.equip_type == EquipmentType.CATALYST))
                        {
                            list_skins_by_type.Add(s);
                        }
                    }
                    CosmeticPanelUI cosmetic_panel_ui = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>();
                    cosmetic_panel_ui.flyoutTitle.text = Offhand_Tile;
                    cosmetic_panel_ui.flyoutSelectionWindow.gameObject.active = true;
                }
                catch { Log.Error("Offhand Open"); }
            }

            public static void Close()
            {
                try
                {
                    CosmeticsFlyoutSelectionContentNavigable content = InventoryPanelUI.instance.cosmeticPanel.GetComponent<CosmeticPanelUI>().flyoutSelectionWindow;
                    content.gameObject.active = false;
                }
                catch { Log.Error("Content Close"); }
            }

            public static void SkinClick(skin s)
            {
                //Log.Msg("Skin Clicked : Type = " + s.equip_type.ToString() +
                //    ", SubType = " + s.subtype + ", Unique = " + s.unique + ", Set = " + s.set + ", UniqueId = " + s.unique_id);
                if (s.equip_type == EquipmentType.HELMET)
                {
                    Config.Data.UserData.helmet = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equip_type == EquipmentType.BODY_ARMOR)
                {
                    Config.Data.UserData.body = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equip_type == EquipmentType.GLOVES)
                {
                    Config.Data.UserData.gloves = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equip_type == EquipmentType.BOOTS)
                {
                    Config.Data.UserData.boots = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if ((s.equip_type == EquipmentType.ONE_HANDED_AXE) || (s.equip_type == EquipmentType.TWO_HANDED_AXE) ||
                            (s.equip_type == EquipmentType.ONE_HANDED_MACES) || (s.equip_type == EquipmentType.TWO_HANDED_MACE) ||
                            (s.equip_type == EquipmentType.ONE_HANDED_SWORD) || (s.equip_type == EquipmentType.ONE_HANDED_SWORD) ||
                            (s.equip_type == EquipmentType.ONE_HANDED_FIST) || (s.equip_type == EquipmentType.ONE_HANDED_DAGGER) ||
                            (s.equip_type == EquipmentType.TWO_HANDED_SPEAR) || (s.equip_type == EquipmentType.TWO_HANDED_STAFF) ||
                            (s.equip_type == EquipmentType.BOW) || (s.equip_type == EquipmentType.CROSSBOW) ||
                            (s.equip_type == EquipmentType.ONE_HANDED_SCEPTRE) || (s.equip_type == EquipmentType.WAND))
                {
                    Config.Data.UserData.weapon = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                else if (s.equip_type == EquipmentType.SHIELD)
                {
                    Config.Data.UserData.offhand = new Config.Load.saved_skin()
                    {
                        type = Helper.GetIdFromEquipmentType(s.equip_type),
                        subtype = s.subtype,
                        unique = s.unique,
                        set = s.set,
                        unique_id = s.unique_id
                    };
                    Config.Save.Skins();
                }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipGear(s.equip_type, 0, false, 0);
            }

            public static void SkinClickWeapon(skin s)
            {
                Config.Data.UserData.weapon = new Config.Load.saved_skin()
                {
                    type = Helper.GetIdFromEquipmentType(s.equip_type),
                    subtype = s.subtype,
                    unique = s.unique,
                    set = s.set,
                    unique_id = s.unique_id
                };
                Config.Save.Skins();
                int rarity = 0;
                if (s.unique) { rarity = 7; }
                if (s.set) { rarity = 8; }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipWeapon(Config.Data.UserData.weapon.type, s.subtype, rarity, s.unique_id, IMSlotType.MainHand, WeaponEffect.None);
            }

            public static void SkinClickOffHand(skin s)
            {
                Config.Data.UserData.offhand = new Config.Load.saved_skin()
                {
                    type = Helper.GetIdFromEquipmentType(s.equip_type),
                    subtype = s.subtype,
                    unique = s.unique,
                    set = s.set,
                    unique_id = s.unique_id
                };
                Config.Save.Skins();
                int rarity = 0;
                if (s.unique) { rarity = 7; }
                if (s.set) { rarity = 8; }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().EquipWeapon(Config.Data.UserData.offhand.type, s.subtype, rarity, s.unique_id, IMSlotType.OffHand, WeaponEffect.None);
            }

            public static void SkinReset()
            {
                EquipmentType type = Helper.GetEquipmentTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                if (type == EquipmentType.HELMET)
                {
                    Config.Data.UserData.helmet = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.BODY_ARMOR)
                {
                    Config.Data.UserData.body = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.GLOVES)
                {
                    Config.Data.UserData.gloves = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.BOOTS)
                {
                    Config.Data.UserData.boots = null_skin;
                    Config.Save.Skins();
                }
                else if ((type == EquipmentType.ONE_HANDED_AXE) || (type == EquipmentType.TWO_HANDED_AXE) ||
                            (type == EquipmentType.ONE_HANDED_MACES) || (type == EquipmentType.TWO_HANDED_MACE) ||
                            (type == EquipmentType.ONE_HANDED_SWORD) || (type == EquipmentType.ONE_HANDED_SWORD) ||
                            (type == EquipmentType.ONE_HANDED_FIST) || (type == EquipmentType.ONE_HANDED_DAGGER) ||
                            (type == EquipmentType.TWO_HANDED_SPEAR) || (type == EquipmentType.TWO_HANDED_STAFF) ||
                            (type == EquipmentType.BOW) || (type == EquipmentType.CROSSBOW) ||
                            (type == EquipmentType.ONE_HANDED_SCEPTRE) || (type == EquipmentType.WAND))
                {
                    Config.Data.UserData.weapon = null_skin;
                    Config.Save.Skins();
                }
                else if (type == EquipmentType.SHIELD)
                {
                    Config.Data.UserData.offhand = null_skin;
                    Config.Save.Skins();
                }
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveGear((byte)item_type);
            }

            public static void SkinWeaponReset()
            {
                EquipmentType type = Helper.GetEquipmentTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                Config.Data.UserData.weapon = null_skin;
                Config.Save.Skins();
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveWeapon(false, true);
            }

            public static void SkinOffhandReset()
            {
                EquipmentType type = Helper.GetEquipmentTypeFromId((byte)item_type);
                Config.Load.saved_skin null_skin = new Config.Load.saved_skin()
                {
                    type = -1,
                    subtype = -1,
                    unique = false,
                    set = false,
                    unique_id = -1
                };
                Config.Data.UserData.offhand = null_skin;
                Config.Save.Skins();
                Close();
                PlayerFinder.getPlayerVisuals().GetComponent<EquipmentVisualsManager>().RemoveWeapon(true, true);
            }
        }
    }
}
