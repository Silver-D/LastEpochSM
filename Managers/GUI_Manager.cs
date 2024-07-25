using MelonLoader;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpochSM.Managers
{
    [RegisterTypeInIl2Cpp]
    public class GUI_Manager : MonoBehaviour
    {
        public GUI_Manager(System.IntPtr ptr) : base(ptr) { }
        public static GUI_Manager instance { get; private set; }

        public static UIBase Game_UIBase { get; private set; }
        public static Logger Log = new Logger("GUI");

        void Awake()
        {
            instance = this;

            MelonEvents.OnSceneWasInitialized.Subscribe(this.OnSceneWasInitialized);
        }

        void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            Base.Init();
        }

        static class Base
        {
            public static bool Initialized = false;

            public static void Init()
            {
                if (Game_UIBase.IsNullOrDestroyed())
                {
                    if (!UIBase.instance.IsNullOrDestroyed())
                        Game_UIBase = UIBase.instance;
                }

                Initialized = (!Game_UIBase.IsNullOrDestroyed());
            }
        }

        public class Functions
        {
            public static GameObject GetChild(GameObject obj, string name)
            {
                GameObject result = null;
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    string obj_name = obj.transform.GetChild(i).gameObject.name;
                    if (obj_name == name)
                    {
                        result = obj.transform.GetChild(i).gameObject;
                        break;
                    }
                }

                return result;
            }
            public static Texture2D MakeTextureFromColor(Color color)
            {
                Texture2D texture = new Texture2D(2, 2);
                Color[] pixels = new Color[2 * 2];
                for (int i = 0; i < pixels.Length; i++) { pixels[i] = color; }
                texture.SetPixels(pixels);
                texture.Apply();

                return texture;
            }
            public static Texture2D MakeTextureFromRGBAColor(float r, float g, float b, float a)
            {
                float new_r = r / 255;
                float new_g = g / 255;
                float new_b = b / 255;
                UnityEngine.Color color = new Color(new_r, new_g, new_b, a);

                Texture2D texture = new Texture2D(2, 2);
                Color[] pixels = new Color[2 * 2];
                for (int i = 0; i < pixels.Length; i++) { pixels[i] = color; }
                texture.SetPixels(pixels);
                texture.Apply();

                return texture;
            }
            public static string AffixRollPercent(byte affix_roll)
            {
                string affix_roll_percent = "Error";
                try
                {
                    double roll_value = (double)affix_roll;
                    double roll_double = roll_value / 255.00 * 100.00;
                    int roll_int = (int)roll_double;
                    affix_roll_percent = roll_int + " %";
                }
                catch { }

                return affix_roll_percent;
            }
            public static GameObject GetViewportContent(GameObject obj, string panel_name, string panel_content_name)
            {
                GameObject result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed())
                {
                    GameObject content = GetChild(panel, panel_content_name);
                    if (!content.IsNullOrDestroyed())
                    {
                        GameObject viewport = GetChild(content, "Viewport");
                        if (!viewport.IsNullOrDestroyed()) { result = GetChild(viewport, "Content"); }
                    }
                }

                return result;
            }
            public static Toggle Get_ToggleInPanel(GameObject obj, string panel_name, string obj_name)
            {
                Toggle result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed()) { result = GetChild(panel, obj_name).GetComponent<Toggle>(); }

                return result;
            }
            public static Slider Get_SliderInPanel(GameObject obj, string panel_name, string obj_name)
            {
                Slider result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed()) { result = GetChild(panel, obj_name).GetComponent<Slider>(); }

                return result;
            }
            public static Button Get_ButtonInPanel(GameObject obj, string obj_name)
            {
                Button result = null;
                GameObject panel = GetChild(obj, obj_name);
                if (!panel.IsNullOrDestroyed()) { result = panel.GetComponent<Button>(); }

                return result;
            }
            public static Text Get_TextInToggle(GameObject obj, string panel_name, string toggle_name, string obj_name)
            {
                Text result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed())
                {
                    GameObject toogle = GetChild(panel, toggle_name);
                    if (!toogle.IsNullOrDestroyed())
                    {
                        result = GetChild(toogle, obj_name).GetComponent<Text>();
                    }
                }

                return result;
            }
            public static Text Get_TextInButton(GameObject obj, string button_name, string text_name)
            {
                Text result = null;
                GameObject button = GetChild(obj, button_name);
                if (!button.IsNullOrDestroyed())
                {
                    result = GetChild(button, text_name).GetComponent<Text>();
                }

                return result;
            }
            public static Dropdown Get_DopboxInPanel(GameObject obj, string panel_name, string dropdown_name)
            {
                Dropdown result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed())
                {
                    GameObject dropdown = GetChild(panel, dropdown_name);
                    if (!dropdown.IsNullOrDestroyed())
                    {
                        result = dropdown.GetComponent<Dropdown>();
                    }
                }

                return result;
            }
            public static Toggle Get_ToggleInLabel(GameObject obj, string panel_name, string obj_name)
            {
                Toggle result = null;
                GameObject panel = GetChild(obj, panel_name);
                if (!panel.IsNullOrDestroyed())
                {
                    GameObject label = GetChild(panel, "Title");
                    if (!label.IsNullOrDestroyed()) { result = GetChild(label, obj_name).GetComponent<Toggle>(); }
                }

                return result;
            }
        }
        public class Textures
        {
            public static Texture2D Btns_Enable = null;
            public static Texture2D Btns_Disable = null;

            public static Texture2D windowBackground = null;
            public static Texture2D texture_grey = null;
            public static Texture2D texture_green = null;
            public static Texture2D texture_unique = null;
            public static Texture2D texture_set = null;
            public static Texture2D texture_affix_idol = null;
            public static Texture2D texture_affix_prefix = null;
            public static Texture2D texture_affix_suffix = null;

            public static Texture2D black = null;
            public static Texture2D gray = null;
            public static Texture2D grey = null;
            public static Texture2D green = null;
            public static Texture2D red = null;

            public static bool Initialized = false;

            public static void Load()
            {
                Btns_Enable = Functions.MakeTextureFromRGBAColor(5, 68, 15, 1);
                Btns_Disable = Functions.MakeTextureFromRGBAColor(121, 28, 3, 1);

                windowBackground = Functions.MakeTextureFromColor(Color.black);
                texture_grey = Functions.MakeTextureFromColor(Color.grey);
                texture_green = Functions.MakeTextureFromColor(Color.green);
                texture_unique = Functions.MakeTextureFromColor(Color.grey);
                texture_set = Functions.MakeTextureFromColor(Color.green);
                texture_affix_idol = Functions.MakeTextureFromColor(Color.blue);
                texture_affix_prefix = Functions.MakeTextureFromColor(Color.grey);
                texture_affix_suffix = Functions.MakeTextureFromColor(Color.yellow);
                black = Functions.MakeTextureFromColor(Color.black);
                grey = Functions.MakeTextureFromColor(Color.grey);
                gray = Functions.MakeTextureFromColor(Color.gray);
                green = Functions.MakeTextureFromColor(Color.green);
                red = Functions.MakeTextureFromColor(Color.red);
                
                Initialized = true;
            }
        }
        public class Styles
        {
            public static GUIStyle Content_Title()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Functions.MakeTextureFromRGBAColor(83, 91, 92, 1);
                style.normal.textColor = Color.white;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.focused.background = style.normal.background;
                style.focused.textColor = style.normal.textColor;
                style.active.background = style.normal.background;
                style.active.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 24;

                return style;
            }
            public static GUIStyle Content_Text()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 16;

                return style;
            }
            public static GUIStyle ContentR_Text()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleRight;
                style.fontSize = 16;

                return style;
            }
            public static GUIStyle Content_TextArea()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleRight;
                style.fontSize = 16;

                return style;
            }
            public static GUIStyle Content_Infos()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.red;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.focused.background = style.normal.background;
                style.focused.textColor = style.normal.textColor;
                style.active.background = style.normal.background;
                style.active.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 14;

                return style;
            }
            public static GUIStyle Content_Button()
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.normal.background = Textures.black;
                style.normal.textColor = Color.grey;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 16;

                return style;
            }
            public static GUIStyle Content_Enable_Button(bool select)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (select) { style.normal.background = Textures.Btns_Enable; }
                else { style.normal.background = Textures.Btns_Disable; }
                style.normal.textColor = Color.white;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 16;

                return style;
            }

            public static GUIStyle Label_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.gray;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.gray;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.gray;
                style.focused.textColor = Color.black;
                style.active.background = Textures.gray;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            public static GUIStyle Text_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;

                return style;
            }
            public static GUIStyle Button_Style(bool select)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (select) { style.normal.background = Textures.green; }
                else { style.normal.background = Textures.grey; }
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            public static GUIStyle TextArea_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.normal.background = Textures.grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.grey;
                style.hover.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            public static GUIStyle Window_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.window);
                float alpha = 0f;
                Color new_color = Color.black;
                Color transparent_color = new Color(new_color.r, new_color.g, new_color.b, alpha);
                style.normal.background = Functions.MakeTextureFromColor(transparent_color);
                style.normal.textColor = Color.white;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;

                return style;
            }
            public static GUIStyle Title_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.normal.background = Textures.windowBackground;
                style.normal.textColor = Color.white;
                style.hover.background = Textures.windowBackground;
                style.hover.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            public static GUIStyle Infos_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.normal.background = Textures.texture_grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.texture_grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.texture_grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.texture_grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;
                style.fontSize = 14;

                return style;
            }
            public static GUIStyle TextField_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = Textures.texture_grey;
                style.normal.textColor = Color.black;
                style.hover.background = Textures.texture_grey;
                style.hover.textColor = Color.black;
                style.focused.background = Textures.texture_grey;
                style.focused.textColor = Color.black;
                style.active.background = Textures.texture_grey;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;

                return style;
            }
            public static GUIStyle DropdownLabelMidle_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.background = null;
                style.normal.textColor = Color.black;
                style.hover.background = null;
                style.hover.textColor = Color.black;
                style.focused.background = null;
                style.focused.textColor = Color.black;
                style.active.background = null;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
            public static GUIStyle DropdownLabelLeft_Style()
            {
                GUIStyle style = new GUIStyle(GUI.skin.textField);
                style.normal.background = null;
                style.normal.textColor = Color.black;
                style.hover.background = null;
                style.hover.textColor = Color.black;
                style.focused.background = null;
                style.focused.textColor = Color.black;
                style.active.background = null;
                style.active.textColor = Color.black;
                style.alignment = TextAnchor.MiddleLeft;

                return style;
            }
            public static GUIStyle Unique_Style(bool IsSet)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (IsSet) { style.normal.background = Textures.texture_set; }
                else { style.normal.background = Textures.texture_unique; }
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleLeft;

                return style;
            }
            public static GUIStyle Affixs_Style(bool Idol, bool prefix)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (Idol) { style.normal.background = Textures.texture_affix_idol; }
                else
                {
                    if (prefix) { style.normal.background = Textures.texture_affix_prefix; }
                    else { style.normal.background = Textures.texture_affix_suffix; }
                }
                style.normal.textColor = Color.black;
                style.hover.background = style.normal.background;
                style.hover.textColor = style.normal.textColor;
                style.alignment = TextAnchor.MiddleLeft;

                return style;
            }
        }
        public class CustomControls
        {
            public delegate void DelegateFunction();
            public static void Title(string text, float pos_x, float pos_y)
            {
                GUI.TextField(new Rect(pos_x, pos_y, 200, 40), text, Styles.TextField_Style());
            }
            public static void EnableButton(string text, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                GUI.Label(new Rect(pos_x, pos_y, 5, 40), "", Styles.TextField_Style());
                GUI.Label(new Rect(pos_x + 5, pos_y, 135, 40), text, Styles.TextField_Style());
                string btn_enable = "Enable";
                if (enable) { btn_enable = "Disable"; }
                if (GUI.Button(new Rect((pos_x + 140), pos_y, 60, 40), btn_enable, Styles.Button_Style(enable))) { function(); }
            }
           
            public static void RarityInfos(float pos_x, float pos_y)
            {
                float w = 100f;
                float h = 20f;
                GUI.TextField(new Rect(pos_x, pos_y, w, h), "Basic : 0", Styles.Infos_Style());
                GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Unique : 7", Styles.Infos_Style());
                pos_y += h;
                GUI.TextField(new Rect(pos_x, pos_y, w, h), "Magic : 1-2", Styles.Infos_Style());
                GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Set : 8", Styles.Infos_Style());
                pos_y += h;
                GUI.TextField(new Rect(pos_x, pos_y, w, h), "Rare : 3-4", Styles.Infos_Style());
                GUI.TextField(new Rect(pos_x + 100, pos_y, w, h), "Legendary : 9", Styles.Infos_Style());
            }
            
            public static int IntValue(string text, int minvalue, int maxvalue, int value, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                float multiplier = maxvalue / 255;
                EnableButton(text, pos_x, pos_y, enable, function);
                GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Textures.texture_grey);

                int result; // = value;
                float min = minvalue / multiplier;
                float max = maxvalue / multiplier;
                if (max > 255) { max = 255; }
                float temp_value = value / multiplier;
                temp_value = (GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max) * multiplier);
                string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), (temp_value * multiplier).ToString(), Styles.TextArea_Style());
                try
                {
                    int str = int.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                    temp_value = str / multiplier;
                }
                catch { }
                result = (int)(temp_value * multiplier);

                return result;
            }
            public static float FloatValue(string text, float minvalue, float maxvalue, float value, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                EnableButton(text, pos_x, pos_y, enable, function);
                float temp_value = value;
                GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Textures.texture_grey);
                temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, minvalue, maxvalue);
                string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
                try { temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
                catch { }

                return temp_value;
            }
            public static long LongValue(string text, long minvalue, long maxvalue, long value, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                long result = value;
                EnableButton(text, pos_x, pos_y, enable, function);
                float temp_value = System.Convert.ToSingle(value);
                GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Textures.texture_grey);
                float min = System.Convert.ToSingle(minvalue);
                float max = System.Convert.ToSingle(maxvalue);
                temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
                result = System.Convert.ToInt64(temp_value);
                string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
                try
                {
                    temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                    result = System.Convert.ToInt64(temp_value);
                }
                catch { }

                return result;
            }
            public static byte ByteValue(string text, byte minvalue, byte maxvalue, byte value, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                byte result = value;
                EnableButton(text, pos_x, pos_y, enable, function);
                float temp_value = System.Convert.ToSingle(value);
                GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Textures.texture_grey);
                float min = System.Convert.ToSingle(minvalue);
                float max = System.Convert.ToSingle(maxvalue);
                temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
                result = (byte)temp_value;
                string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), temp_value.ToString(), Styles.TextArea_Style());
                try
                {
                    temp_value = float.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat);
                    result = (byte)temp_value;
                }
                catch { }

                return result;
            }
            public static ushort UshortValue(string text, int minvalue, int maxvalue, ushort value, float pos_x, float pos_y, bool enable, DelegateFunction function)
            {
                ushort result = value;
                float multiplier = maxvalue / 255;
                EnableButton(text, pos_x, pos_y, enable, function);
                float temp_value = value / multiplier;//System.Convert.ToSingle(value / multiplier);
                GUI.DrawTexture(new Rect(pos_x, (pos_y + 40), 140, 40), Textures.texture_grey);
                float min = 0;
                float max = 255;
                temp_value = GUI.HorizontalSlider(new Rect(pos_x + 5, (pos_y + 40 + 20), 135, 20), temp_value, min, max);
                result = (ushort)(temp_value * multiplier);
                string value_str = GUI.TextArea(new Rect((pos_x + 140), (pos_y + 40), 60, 40), (temp_value * multiplier).ToString(), Styles.TextArea_Style());
                try { result = ushort.Parse(value_str, CultureInfo.InvariantCulture.NumberFormat); }
                catch { }

                return result;
            }
        }
    }
}
