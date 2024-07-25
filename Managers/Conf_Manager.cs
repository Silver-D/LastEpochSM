using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LastEpochSM.Managers
{
    public class Conf_Manager
    {
        public static string confPath { get { return Asset_Manager.modPath; } private set { } }

        public static Logger Log = new Logger("Conf");

        public static JObject LoadFromFile(string file)
        {
            file = file.TrimStart('/', '\\');

            if (!File.Exists(Path.Combine(confPath, file)))
            {
                Log.Msg("Config file " + file + " not found");

                return null;
            }

            using (Stream stream = new FileStream(Path.Combine(confPath, file), FileMode.Open))
            {
                if (stream.IsNullOrDestroyed())
                    Log.Msg("Config file " + file + " not found");

                return LoadFromStream(stream);
            }
        }

        public static JObject LoadFromResource(System.Type mod, string rsrc)
        {
            using (Stream stream = Asset_Manager.GetResourceStream(mod, rsrc) /*mod.Assembly.GetManifestResourceStream(rsrc)*/)
               return LoadFromStream(stream);
        }

        public static JObject LoadFromStream(Stream stream)
        {
            if (stream.IsNullOrDestroyed())
                return null;

            string json;

            using (StreamReader streamReader = new StreamReader(stream))
                json = streamReader.ReadToEnd();

            return JObject.Parse(json);
        }

        public static bool SaveToFile(string file, JObject conf)
        {
            if (conf == null)
            {
                Log.Error("Conf object is null");

                return false;
            }

            file = file.TrimStart('/', '\\');
            file = Path.Combine(confPath, file);

            string dir = Path.GetFullPath(Path.GetDirectoryName(file));

            if (!dir.Contains(Path.GetFullPath(confPath)))
            {
                Log.Error("Something happened with the config path, it's not located in the Mod directory:");
                Log.Error(dir);
                Log.Error("Aborting.");

                return false;
            }

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (File.Exists(file))
                File.Delete(file);

            File.WriteAllText(file, conf.ToString());

            return (File.Exists(file));
        }
    }
}
