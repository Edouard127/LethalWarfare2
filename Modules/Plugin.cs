using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using System;
using UnityEngine.InputSystem;
using LethalWarfare2.Modules.Model;

namespace LethalWarfare2.Modules
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("x753.More_Suits", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Edouard127.LethalWarfare2";
        public const string NAME = "Lethal Warfare 2";
        public const string VERSION = "1.2.1";

        public static ConfigFile config;

        private Harmony harmony = new Harmony(GUID);


        // TODO: Implement this
        public static ConfigEntry<bool> disablePhysics;
        public static ConfigEntry<double> disablePhysicsRange;

        public static ConfigEntry<bool> tacticalCamera;
        public static ConfigEntry<float> tacticalCameraOffsetX;
        public static ConfigEntry<float> tacticalCameraOffsetY;
        public static ConfigEntry<float> tacticalCameraOffsetZ;

        public static ConfigEntry<KeyCode> toggleKey;

        private void Awake()
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

            harmony.PatchAll();
            Logger.LogInfo($"Plugin {NAME} {VERSION} loaded!");
        }
    }
    public static class Assets
    {
        // The reason why I can't have one bundle for everything is because I lack the knowledge of Unity bundles and the SDK I use to bundle the models has a monopole on the bundles
        public static string modelBundleName = "mw2bundle";
        public static string assetBundleName = "itembundle";
        public static string postProcessingBundleName = "postprocessing";
        public static AssetBundle MainModelBundle = null; // Used for character models
        public static AssetBundle MainAssetBundle = null; // Used for item models, sounds, etc.
        public static AssetBundle PostProcessingBundle = null; // Used for post processing effects

        public static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            if (MainModelBundle == null)
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

            if (MainModelBundle == null)
            {
                throw new Exception("Could not load model bundle!");
            }

            if (MainAssetBundle == null)
            {
                throw new Exception("Could not load asset bundle!");
            }
        }

        public static UnityEngine.Object GetAssetFromName(string name) => MainModelBundle.LoadAsset(name) ?? MainAssetBundle.LoadAsset(name);
        public static Shader GetShaderFromName(string name) => GetAssetFromName(name) as Shader;
        public static AudioClip GetAudioClipFromName(string name) => GetAssetFromName(name) as AudioClip;
        public static Sprite GetSpriteFromName(string name) => GetAssetFromName(name) as Sprite;

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