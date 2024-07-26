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
        private static string  usrDir  = "Mini/";

        public static bool Initialized = false;

        public static void Load()
        {
            string usrConfFile = usrDir + "Conf.json";
            string usrBTFile   = usrDir + "BlessingTransfers.json";

            defConf = Conf_Manager.LoadFromResource(instance.GetType(), "Mini.Conf.json");
            usrConf = Conf_Manager.LoadFromFile(usrConfFile);

            if (defConf == null)
                return;

            if (usrConf == null)
            {
                usrConf = new JObject(defConf);

                if (Save(usrConfFile, usrConf))
                    Log.Msg("Created default Config file");
            }

            string btProperty = "blessingTransfers";

            JObject defBT = Conf_Manager.LoadFromResource(instance.GetType(), "Mini.BlessingTransfers.json");

            ((JObject)defConf.SelectToken("Monolith")).Add(btProperty, defBT);

            JObject usrBT = Conf_Manager.LoadFromFile(usrBTFile);

            if (usrBT == null)
            {
                usrBT = new JObject(defBT);

                if (Save(usrBTFile, usrBT))
                    Log.Msg("Created default Blessing Transfers file");
            }

            ((JObject)usrConf.SelectToken("Monolith")).Add(btProperty, usrBT);

            Initialized = true;
        }

        private static bool Save(string file, JObject obj)
        {
            return Conf_Manager.SaveToFile(file, obj);
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
