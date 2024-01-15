using BepInEx.Bootstrap;
using GameNetcodeStuff;
using HarmonyLib;
using ModelReplacement;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace LethalWarfare2.Modules
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : PlayerControllerB
    {
        public static InputAction toggleKey = new InputAction("Toggle Tactical Camera", InputActionType.Value, "<Keyboard>/" + Plugin.toggleKey.Value.ToString());
        public static bool switchCamera = false;
        public static bool forceDisable = false;

        public static bool hasCustomModel = false;
        public static bool spectatedHasCustomModel = false;

        public static bool displayedTip = false;

        private static GameObject globalPrefab;
        private static GameObject _postProcessing;
        public static GameObject PostProcessing
        {
            get
            {
                if (_postProcessing == null)
                {
                    globalPrefab = Assets.PostProcessingBundle.LoadAsset<GameObject>("Volume Global");
                    _postProcessing = Instantiate(globalPrefab);
                    _postProcessing.SetActive(false);
                }

                return _postProcessing;
            }
        }

        private static GameObject _playerHelmet;
        public static GameObject PlayerHelmet
        {
            get
            {
                if (_playerHelmet == null)
                {
                    _playerHelmet = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");
                }

                return _playerHelmet;
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(ref PlayerControllerB __instance)
        {
            toggleKey.started += ToggleTacticalCamera;
            toggleKey.Enable();
        }


        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update(ref PlayerControllerB __instance)
        {
            switchCamera = __instance.isPlayerDead && __instance.spectatedPlayerScript != null && spectatedHasCustomModel && !forceDisable;

            __instance.disableLookInput = switchCamera && !__instance.quickMenuManager.isMenuOpen;

            PostProcessing.SetActive(__instance.disableLookInput);
            PlayerHelmet.SetActive(false);

            if (switchCamera)
            {
                if (!displayedTip)
                {
                    HUDManager.Instance.DisplaySpectatorTip($"Press [{Plugin.toggleKey.Value}] to switch te camera view");
                    displayedTip = true;
                }

                __instance.spectateCameraPivot.Rotate(0f, __instance.spectatedPlayerScript.gameplayCamera.transform.localEulerAngles.y, 0f);
                __instance.spectateCameraPivot.transform.localEulerAngles = new Vector3(__instance.spectateCameraPivot.transform.localEulerAngles.x, 0f, 0f);
                __instance.spectateCameraPivot.transform.rotation = __instance.spectatedPlayerScript.gameplayCamera.transform.rotation;
            }
        }

        [HarmonyPatch("SpectateNextPlayer")]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)] // Patch every mods that messes with this
        private static void SpectateNextPlayer(ref PlayerControllerB __instance)
        {
            RaycastSpectateCameraAroundPivot(ref __instance);
        }

        [HarmonyPatch("RaycastSpectateCameraAroundPivot")]
        [HarmonyPrefix]
        private static bool RaycastSpectateCameraAroundPivot(ref PlayerControllerB __instance)
        {
            // /Environment/HangarShip/Player/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004
            Transform? boneRig = GetBoneTransform(__instance.spectatedPlayerScript, "spine.004");
            spectatedHasCustomModel = boneRig != null;
            hasCustomModel = false; // Later

            if (switchCamera && spectatedHasCustomModel)
            {
                __instance.spectateCameraPivot.position = boneRig.position
                 + __instance.spectatedPlayerScript.gameplayCamera.transform.right * Plugin.tacticalCameraOffsetX.Value
                 + __instance.spectatedPlayerScript.gameplayCamera.transform.up * Plugin.tacticalCameraOffsetY.Value
                 + __instance.spectatedPlayerScript.gameplayCamera.transform.forward * Plugin.tacticalCameraOffsetZ.Value;

                __instance.playersManager.spectateCamera.transform.LookAt(__instance.spectateCameraPivot.position);
            }

            return !switchCamera;
        }

        public static Transform? GetBoneTransform(PlayerControllerB __instance, string bone)
        {
            // This will assure that the tactical camera will always the same distance from the player regardless of the player's model
            // We could even add an object directly on the model to make it easier to the position
            if (__instance.isPlayerDead) return null;
            ModelReplacementAPI.GetPlayerModelReplacement(__instance, out BodyReplacementBase component);
            return component?.avatar?.GetAvatarTransformFromBoneName(bone);
        }

        public static void ToggleTacticalCamera(CallbackContext ctx)
        {
            forceDisable = !forceDisable;
        }
    }
}