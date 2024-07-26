using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LastEpochSM.Managers
{
    public class Asset_Manager
    {
        public static string modPath   { get { return Directory.GetCurrentDirectory() + "/Mods/LastEpochSM"; } private set { } }
        public static string assetPath { get { return modPath + "/Assets"; } private set { } }

        private static Dictionary<string, AssetBundle> bundleList = new Dictionary<string, AssetBundle>();

        public static Logger Log = new Logger("Assets");

        public static AssetBundle LoadBundle(string asset_name)
        {
            asset_name = asset_name.TrimStart('/', '\\');

            if (bundleList.ContainsKey(asset_name))
                return bundleList[asset_name];

            Log.Msg("Loading " + asset_name);

            IEnumerable loaded_bundles = (IEnumerable)AssetBundle.GetAllLoadedAssetBundles();

            foreach(AssetBundle bundle in loaded_bundles)
            {
                if (bundle.name == Path.GetFileNameWithoutExtension(asset_name))
                {
                    Log.Msg("Found an already loaded AssetBundle: " + bundle.name);

                    bundleList.Add(asset_name, bundle);

                    return bundle;
                }
            }

            AssetBundle asset_bundle = null;

            string path = Path.Combine(assetPath, asset_name);

            if (File.Exists(path))
            {
                asset_bundle = AssetBundle.LoadFromFile(path);

                if (asset_bundle == null)
                    Log.Error("AssetBundle " + asset_name + " was not loaded");

                else
                    Object.DontDestroyOnLoad(asset_bundle);
            }

            else
                Log.Error("File not found: " + path);

            bundleList.Add(asset_name, asset_bundle);
            
            return asset_bundle;
        }

        public static byte[] LoadFromResource(System.Type mod, string rsrc)
        {
            using (Stream stream = Asset_Manager.GetResourceStream(mod, rsrc))
               return LoadFromStream(stream).ToArray();
        }

        public static MemoryStream LoadFromStream(Stream stream)
        {
            if (stream.IsNullOrDestroyed())
                return null;

            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                     stream.CopyTo(memStream);
                     return memStream;
                }
            }
        }

        public static Stream GetResourceStream(System.Type mod, string rsrc)
        {
            Stream stream = mod.Assembly.GetManifestResourceStream(rsrc);

            if (stream.IsNullOrDestroyed())
                    Log.Error("Embedded resource " + rsrc + " not found");

            return stream;
        }

        public class Extensions
        {
            public static readonly string jpg = ".jpg";
            public static readonly string png = ".png";
            public static readonly string prefab = ".prefab";
        }

        public class Functions
        {
            public static bool Check_Texture(string name)
            {
                if ((name.Substring(name.Length - Extensions.jpg.Length, Extensions.jpg.Length).ToLower() == Extensions.jpg) ||
                            (name.Substring(name.Length - Extensions.png.Length, Extensions.png.Length).ToLower() == Extensions.png))
                {
                    return true;
                }
                else { return false; }
            }
            public static bool Check_Prefab(string name)
            {
                if (name.Substring(name.Length - Extensions.prefab.Length, Extensions.prefab.Length).ToLower() == Extensions.prefab)
                {
                    return true;
                }
                else { return false; }
            }
        }
    }
}
