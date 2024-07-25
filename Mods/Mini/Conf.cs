using LastEpochSM.Managers;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastEpochSM.Mods
{
    class Conf : Mini
    {
        public Conf(System.IntPtr ptr) : base(ptr) { }

        private static JObject defConf = null;
        private static JObject usrConf = null;
        private static string usrFile  = @"\Mini\Mini.Conf.json";

        public static bool Initialized = false;

        public static void Load()
        {
            defConf = Conf_Manager.LoadFromResource(instance.GetType(), "Mini.DefaultConf.json");
            usrConf = Conf_Manager.LoadFromFile(usrFile);

            if (defConf != null && usrConf == null)
            {
                if (Save())
                    Log.Msg("Created config file");
            }

            else if (defConf == null)
                defConf = new JObject();

            Initialized = true;
        }

        private static bool Save()
        {
            return Conf_Manager.SaveToFile(usrFile, (usrConf == null) ? defConf : usrConf);
        }

        public static T Get<T>(string key)
        {
            JToken val = null;

            if (usrConf != null)
                val = usrConf.SelectToken(key);

            if (val == null)
            {
                val = defConf.SelectToken(key, true);

                if (val == null)
                    return default(T);
            }

            return (T)val.ToObject<T>();
        }
    }
}
