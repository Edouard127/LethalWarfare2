using System;
using GameNetcodeStuff;
using HarmonyLib;
using ModelReplacement;
using UnityEngine;

namespace LethalWarfare2.Modules
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : PlayerControllerB
    {
        public static bool switchCamera = false;
        private static bool lastKeyState = false;

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

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update(ref PlayerControllerB __instance)
        {
            // Check if key is pressed
            /*if (Plugin.keyboardShortcut.Value.IsDown())
            {
                lastKeyState = true;
            }


            if (Plugin.keyboardShortcut.Value.IsUp() && lastKeyState)
            {
                lastKeyState = false;
                switchCamera = !switchCamera && __instance.isPlayerDead && __instance.spectatedPlayerScript != null;
            }*/

            switchCamera = __instance.isPlayerDead && __instance.spectatedPlayerScript != null && !__instance.spectatedPlayerScript.isPlayerDead;
            __instance.disableLookInput = !__instance.quickMenuManager.isMenuOpen && switchCamera;
            PostProcessing.SetActive(switchCamera);

            if (switchCamera)
            {
                __instance.spectateCameraPivot.Rotate(0f, __instance.spectatedPlayerScript.gameplayCamera.transform.localEulerAngles.y-30, 0f);
                __instance.spectateCameraPivot.transform.localEulerAngles = new Vector3(__instance.spectateCameraPivot.transform.localEulerAngles.x, 0f, 0f);
                __instance.spectateCameraPivot.transform.rotation = __instance.spectatedPlayerScript.gameplayCamera.transform.rotation;
            }
        }

        [HarmonyPatch("RaycastSpectateCameraAroundPivot")]
        [HarmonyPrefix]
        private static bool RaycastSpectateCameraAroundPivot(ref PlayerControllerB __instance)
        {
            if (switchCamera)
            {
                // This will assure that the tactical camera will always the same distance from the player regardless of the player's model
                BodyReplacementBase component = __instance.spectatedPlayerScript.thisPlayerBody.gameObject.GetComponent<BodyReplacementBase>();
                if (component == null)
                {
                    return true; // Player doesn't have a model replacement
                }

                // /Environment/HangarShip/Player/ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004
                Transform boneRig = component.avatar.GetAvatarTransformFromBoneName("spine.004");
                if (boneRig == null)
                {
                    return true; // Player doesn't have a bone rig
                }

                __instance.playersManager.spectateCamera.fieldOfView = 100f;
                __instance.spectateCameraPivot.position = boneRig.position
                    + __instance.spectatedPlayerScript.gameplayCamera.transform.right * 0.25f
                    + __instance.spectatedPlayerScript.gameplayCamera.transform.up * 0.3f
                    + __instance.spectatedPlayerScript.gameplayCamera.transform.forward * 1.3f;

                __instance.playersManager.spectateCamera.transform.LookAt(__instance.spectateCameraPivot.position);
            }

            return !switchCamera;
        }
    }
}