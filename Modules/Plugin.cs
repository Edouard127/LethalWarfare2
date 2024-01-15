using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using System;
using LethalWarfare2.Modules.Model;
using System.Collections.Generic;

namespace LethalWarfare2.Modules
{
    public static class PluginInfo
    {
        public const string GUID = "Edouard127.LethalWarfare2";
        public const string NAME = "Lethal Warfare 2";
        public const string VERSION = "1.2.6";
        public const string WEBSITE = "https://github.com/Edouard127/LethalWarfare2";
    }

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("x753.More_Suits", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Edouard127.LethalWarfare2";
        public const string NAME = "Lethal Warfare 2";
        public const string VERSION = "1.2.6";

        public static ConfigFile config;

        public static List<BepInEx.PluginInfo> problematicMods = new List<BepInEx.PluginInfo>();

        private Harmony harmony = new Harmony(GUID);


        public static ConfigEntry<bool> disablePhysics;
        public static ConfigEntry<double> disablePhysicsRange;

        public static ConfigEntry<bool> tacticalCamera;
        public static ConfigEntry<float> tacticalCameraOffsetX;
        public static ConfigEntry<float> tacticalCameraOffsetY;
        public static ConfigEntry<float> tacticalCameraOffsetZ;

        public static ConfigEntry<KeyCode> toggleKey;

        public void Awake()
        {
            config = Config;
            Assets.PopulateAssets();

            // Not implemented yet
            disablePhysics = config.Bind("Lethal Warfare 2", "Disable Physics", false, "Disable physics for all players");
            disablePhysicsRange = config.Bind("Lethal Warfare 2", "Disable Physics Range", 100.0, "Disable physics for all players within this range");

            tacticalCamera = config.Bind("Lethal Warfare 2", "Tactical Camera View", true, "Set the camera position above the spectated player's head");
            tacticalCameraOffsetX = config.Bind("Lethal Warfare 2", "Tactical Camera Offset X", 0.25f, "Set the camera offset on the X axis");
            tacticalCameraOffsetY = config.Bind("Lethal Warfare 2", "Tactical Camera Offset Y", 0.25f, "Set the camera offset on the Y axis");
            tacticalCameraOffsetZ = config.Bind("Lethal Warfare 2", "Tactical Camera Offset Z", 1.2f, "Set the camera offset on the Z axis");

            toggleKey = config.Bind("Lethal Warfare 2", "Keyboard Shortcut", KeyCode.F1, "Toggle the tactical camera view");

            ModelReplacementAPI.RegisterSuitModelReplacement("Ghost Nightwar", typeof(GhostReplacement));
            ModelReplacementAPI.RegisterSuitModelReplacement("Alex Keller", typeof(AlexReplacement));
            ModelReplacementAPI.RegisterSuitModelReplacement("Valeria Garza", typeof(ValeriaReplacement));

            /*SmokeGrenade smokeGrenade = SmokeGrenade.LoadAssetAndReturnInstance();
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(smokeGrenade.itemProperties.spawnPrefab);
            LethalLib.Modules.Utilities.FixMixerGroups(smokeGrenade.itemProperties.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(smokeGrenade.itemProperties, 1000, LethalLib.Modules.Levels.LevelTypes.All);
            LethalLib.Modules.Items.RegisterShopItem(smokeGrenade.itemProperties, null, null, SmokeGrenade.CreateTerminalNode(), 1);*/

            harmony.PatchAll();
            Logger.LogInfo($"Plugin {NAME} {VERSION} loaded!");

            PluginCheck();
        }

        private void PluginCheck()
        {
            foreach (string pluginName in new string[] { "me.swipez.melonloader.morecompany" })
            {
                if (IsPluginPresent(pluginName, out BepInEx.PluginInfo plugin))
                {
                    problematicMods.Add(plugin);
                }
            }

            if (problematicMods.Count > 0)
            {
                Logger.LogWarning($"The following mods are known to cause issues with {NAME}:");
                foreach (var mod in problematicMods)
                {
                    Logger.LogWarning($"- {mod.Metadata.Name} {mod.Metadata.Version}");
                }
            }
        }

        private static bool IsPluginPresent(string pluginName, out BepInEx.PluginInfo plugin)
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.TryGetValue(pluginName, out plugin);
        }
    }
    public static class Assets
    {
        public static string modelBundleName = "mw2bundle";
        public static string assetBundleName = "itembundle";
        public static string postProcessingBundleName = "postprocessing";
        public static AssetBundle MainModelBundle = null; // Used for character models
        public static AssetBundle MainAssetBundle = null; // Used for item models, sounds, etc.
        public static AssetBundle PostProcessingBundle = null; // Used for post processing effects

        public static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + modelBundleName))
            {
                MainModelBundle = AssetBundle.LoadFromStream(assetStream);
            }

            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + assetBundleName))
            {
                MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
            }

            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + postProcessingBundleName))
            {
                PostProcessingBundle = AssetBundle.LoadFromStream(assetStream);
            }
        }

        public static T GetAssetFromName<T>(string name) where T : UnityEngine.Object 
        {
            return MainModelBundle.LoadAsset<T>(name) ?? MainAssetBundle.LoadAsset<T>(name) ?? PostProcessingBundle.LoadAsset<T>(name) ?? throw new Exception($"Could not find asset {name}!");
        }

        /*public class RandomAudioClip
        {
            List<AudioClip> audioClipList = new List<AudioClip>();

            private System.Random randomSource = new System.Random();

            public AudioClip GetRandomAudio(int seed)
            {
                return audioClipList[randomSource.Next(audioClipList.Count)];
            }

            public void AddAudio(string name)
            {
                var clip = GetAudioClipFromName(name);
                if (clip != null)
                {
                    audioClipList.Add(clip);
                }
            }
        }*/
    }
}