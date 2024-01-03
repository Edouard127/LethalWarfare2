using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using ModelReplacement;
using BepInEx.Configuration;
using LethalWarfare2.Replacements;
using System.Collections.Generic;

namespace LethalWarfare2
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("meow.ModelReplacementAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("x753.More_Suits", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Edouard127.LethalWarfare2";
        public const string NAME = "Lethal Warfare 2";
        public const string VERSION = "1.1.0";

        public static ConfigFile config;

        private Harmony harmony = new Harmony(GUID);


        // TODO: Implement this
        public static ConfigEntry<bool> disablePhysics { get; private set; }
        public static ConfigEntry<double> disablePhysicsRange { get; private set; }
        public static ConfigEntry<bool> tacticalCameraView { get; private set; }
        public static ConfigEntry<KeyboardShortcut> keyboardShortcut { get; private set; }

        private void Awake()
        {
            config = base.Config;
            Assets.PopulateAssets();

            disablePhysics = config.Bind("Lethal Warfare 2", "Disable Physics", false, "Disable physics for all players");
            disablePhysicsRange = config.Bind("Lethal Warfare 2", "Disable Physics Range", 100.0, "Disable physics for all players within this range");
            tacticalCameraView = config.Bind("Lethal Warfare 2", "Tactical Camera View", true, "Set the camera position above the spectated player's head");
            keyboardShortcut = config.Bind("Lethal Warfare 2", "Keyboard Shortcut", new KeyboardShortcut(KeyCode.F1), "Toggle the tactical camera view");

            ModelReplacementAPI.RegisterSuitModelReplacement("Ghost Nightwar", typeof(GhostReplacement));

            harmony.PatchAll();
            Logger.LogInfo($"Plugin {NAME} {VERSION} loaded!");
        }
    }
    public static class Assets
    {
        public static string mainAssetBundleName = "mw2bundle";
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundle MainShaderBundle = null;

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
        public static void PopulateAssets()
        {
            if (MainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + "." + mainAssetBundleName))
                {
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }

            }

            if (MainAssetBundle == null)
            {
                throw new System.Exception("Could not load asset bundle!");
            }

            // If you want to add audio clips, use this
            // new RandomAudioClip().AddAudio("mw2_sound");
        }

        public static Shader GetShaderFromName(string name) => Assets.MainAssetBundle.LoadAsset(name) as Shader;

        public static AudioClip GetAudioClipFromName(string name) =>  Assets.MainAssetBundle.LoadAsset(name) as AudioClip;

        public class RandomAudioClip
        {
            List<AudioClip> audioClipList = new List<AudioClip>();

            private System.Random randomSource = new System.Random();

            public AudioClip GetRandomAudio(int seed)
            {
                return audioClipList[randomSource.Next(audioClipList.Count)];
            }

            public void AddAudio(string name)
            {
                var clip = Assets.GetAudioClipFromName(name);
                if (clip != null) { 
                    audioClipList.Add(clip); 
                }
            }
        }
    }
}