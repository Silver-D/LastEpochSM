using MelonLoader;
using System.Linq;
using UnityEngine;
using LastEpochSM.Managers;

namespace LastEpochSM
{
    public class Main : MelonMod
    {
        public static Main instance { get; private set; }

        private static GameObject coreComponents;

        public static Logger Log = new Logger("");

        public override void OnInitializeMelon()
        {
            instance = this;

            if (Base.Initialized && !coreComponents.IsNullOrDestroyed())
                coreComponents.SetActive(true);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Scenes.SceneName = sceneName;
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!Base.Initializing && !Base.Initialized)
                Base.Init();
        }

        public override void OnDeinitializeMelon()
        {
            if (!coreComponents.IsNullOrDestroyed())
                coreComponents.SetActive(false);
        }

        public override void OnApplicationQuit()
        {
            Caching.ClearCache();
        }

        static class Base
        {
            public static bool Initialized { get; private set; }
            public static bool Initializing { get; private set; }

            public static void Init()
            {
                Initializing = true;

                coreComponents = Object.Instantiate(new GameObject(name: typeof(Main).ToString()), Vector3.zero, Quaternion.identity);
                Object.DontDestroyOnLoad(coreComponents);

                coreComponents.AddComponent<Mod_Manager>();
                coreComponents.AddComponent<GUI_Manager>();

                Initialized = true;
                Initializing = false;
            }
        }
    }

    public class Scenes
    {
        public static string SceneName = "";
        private static string[] SceneMenuNames = { "ClientSplash", "PersistentUI", "Login", "CharacterSelectScene" };

        public static bool IsGameScene()
        {
            if ((SceneName != "") && (!SceneMenuNames.Contains(SceneName))) { return true; }
            else { return false; }
        }
        public static bool IsCharacterSelection()
        {
            if ((SceneName != "") && (SceneName == SceneMenuNames[3])) { return true; }
            else { return false; }
        }
        public static bool IsGameReady()
        {
            return (IsCharacterSelection() || IsGameScene());
        }
    }

    public class Timer
    {
        private bool isRunning = false;
        private System.DateTime startTime;
        private float duration;

        public void StartTimer(float dur)
        {
            isRunning = true;
            duration  = dur;
            startTime = System.DateTime.Now;
        }

        public bool HasElapsed()
        {
            if (!isRunning)
                return true;

            System.TimeSpan elaspedTime = System.DateTime.Now - startTime;

            if ((System.Double)elaspedTime.TotalSeconds > (duration))
            {
                isRunning = false;

                return true;
            }

            return false;
        }
    }

    public class Logger
    {
        private string name;
        private bool debug;

        public Logger(string str, bool debug = false)
        {
            name = str;

            if (name != "")
                name = name + " : ";

            this.debug = debug;
        }

        public void Msg(string str)
        {
            Main.instance.LoggerInstance.Msg(name + str);
        }

        public void Error(string str)
        {
            Main.instance.LoggerInstance.Error(name + str);
        }

        public void Debug(string str)
        {
            if (!debug)
                return;

            Main.instance.LoggerInstance.Msg(name + str);
        }
    }

    public static class Methods
    {
        public static bool IsNullOrDestroyed(this object obj)
        {
            try
            {
                if (obj == null) { return true; }
                else if (obj is Object unityObj && !unityObj) { return true; }
                return false;
            }
            catch { return true; }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : System.IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
