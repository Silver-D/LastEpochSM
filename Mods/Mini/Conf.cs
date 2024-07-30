using LastEpochSM.Managers;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastEpochSM.Mods
{
    class Conf : Mini
    {
        public Conf(System.IntPtr ptr) : base(ptr) { }

        static JObject defConf = null;
        static JObject usrConf = null;
        static string  usrDir  = "Mini/";
        static string  usrFile = "Conf.json";

        public static bool Initialized { get; private set; }

        public static void Load()
        {
            string usrConfFile = usrDir + usrFile;
            string usrBTFile   = usrDir + "BlessingTransfers.json";

            defConf = Conf_Manager.LoadFromResource(instance.GetType(), "Mini.Conf.json");
            usrConf = Conf_Manager.LoadFromFile(usrConfFile, out bool usrFileExists);

            if (defConf == null)
                return;

            if (usrConf == null)
            {
                usrConf = new JObject(defConf);

                if (usrFileExists)
                    Log.Error("Error parsing " + usrConfFile);

                else if (Save(usrConfFile, usrConf))
                    Log.Msg("Created default config file");
            }

            else
                CheckConfig();

            string btProperty = "blessingTransfers";

            JObject defBT = Conf_Manager.LoadFromResource(instance.GetType(), "Mini.BlessingTransfers.json");

            ((JObject)defConf.SelectToken("Monolith")).Add(btProperty, defBT);

            JObject usrBT = Conf_Manager.LoadFromFile(usrBTFile, out bool usrBTExists);

            if (usrBT == null)
            {
                usrBT = new JObject(defBT);

                if (usrFileExists)
                    Log.Error("Error parsing " + usrBTFile);

                else if (Save(usrBTFile, usrBT))
                    Log.Msg("Created default Blessing Transfers file");
            }

            ((JObject)usrConf.SelectToken("Monolith")).Add(btProperty, usrBT);

            Initialized = true;
        }

        public static void ReLoad()
        {
            if (!Initialized)
                return;

            Initialized = false;

            string usrConfFile = usrDir + usrFile;

            usrConf = Conf_Manager.LoadFromFile(usrConfFile, out bool usrFileExists);

            if (usrConf == null)
            {
                usrConf = new JObject(defConf);

                if (usrFileExists)
                    Log.Error("Error parsing " + usrConfFile);
            }

            Initialized = true;
        }

        static void CheckConfig()
        {
            bool to_save = false;

            foreach(var sect in (JObject)defConf)
            {
                if (usrConf.SelectToken(sect.Key) == null)
                {
                    usrConf.Add(sect.Key, sect.Value);

                    to_save = true;
                }

                foreach(var opt in (JObject)sect.Value)
                {
                    if (usrConf.SelectToken(sect.Key + "." + opt.Key) == null)
                    {
                        ((JObject)usrConf.SelectToken(sect.Key)).Add(opt.Key, opt.Value);

                        to_save = true;
                    }
                }
            }

            if (to_save)
            {
                if (Save(usrDir + usrFile, usrConf))
                    Log.Msg("Updated user config");
            }
        }

        static bool Save(string file, JObject obj)
        {
            return Conf_Manager.SaveToFile(file, obj);
        }

        public static T Get<T>(string key)
        {
            JToken val = null;

            if (Initialized)
            {
                if (usrConf != null)
                    val = usrConf.SelectToken(key);

                if (val == null)
                    val = defConf.SelectToken(key, true);
            }

            if (val == null)
                return (T)default(T);

            return (T)val.ToObject<T>();
        }
    }
}
