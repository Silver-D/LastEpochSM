using MelonLoader;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace LastEpochSM.Managers
{
    [RegisterTypeInIl2Cpp]
    public class Mod_Manager : MonoBehaviour
    {
        public Mod_Manager(System.IntPtr ptr) : base(ptr) { }
        public static Mod_Manager instance { get; private set; }

        private static Dictionary<string, GameObject> modList;

        public static Logger Log = new Logger("Manager");

        void Awake()
        {
            instance = this;

            modList = new Dictionary<string, GameObject>();

            Log.Msg("Ready");
        }

        public static void Register<T>() where T : MonoBehaviour
        {
            string modId = typeof(T).ToString();
            string modName = typeof(T).Name;
            bool isActive = (instance.gameObject.active);

            if (modList.ContainsKey(modId))
            {
                Log.Msg("[" + modName + "] is already registered");

                return;
            }

            GameObject modObject = Object.Instantiate(new GameObject(name: modId), Vector3.zero, Quaternion.identity);
            Object.DontDestroyOnLoad(modObject);

            modList.Add(modId, modObject);

            Log.Msg("Registered [" + modName + "]");

            modObject.active = isActive;
            modObject.AddComponent<T>();
        }

        public static bool CanPatch(bool cond = true)
        {
            if (!cond)
                return false;

            return (!instance.IsNullOrDestroyed() && instance.gameObject.active);
        }

        void Enable()
        {
            modList.Values.ToList().ForEach(m => { if (!m.IsNullOrDestroyed()) { m.SetActive(true); } });
        }

        void Disable()
        {
            modList.Values.ToList().ForEach(m => { if (!m.IsNullOrDestroyed()) { m.SetActive(false); } });
        }
    }
}
