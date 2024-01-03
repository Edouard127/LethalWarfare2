using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalWarfare2.PlayerPatches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch : PlayerControllerB
    {
        public static bool switchCamera = false;
        private static bool lastKeyState = false;

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

            switchCamera = __instance.isPlayerDead && __instance.spectatedPlayerScript != null;

            if (switchCamera)
            {
                __instance.disableLookInput = !__instance.quickMenuManager.isMenuOpen;
                __instance.spectateCameraPivot.Rotate(0f, __instance.spectatedPlayerScript.gameplayCamera.transform.localEulerAngles.y, 0f);
                __instance.spectateCameraPivot.transform.localEulerAngles = new Vector3(__instance.spectateCameraPivot.transform.localEulerAngles.x, 0f, 0f);
                __instance.spectateCameraPivot.transform.rotation = __instance.spectatedPlayerScript.gameplayCamera.transform.rotation;
                return;
            }

            __instance.disableLookInput = false;
        }

        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        private static void LateUpdate(ref PlayerControllerB __instance)
        {
            if (switchCamera)
            {
                __instance.spectateCameraPivot.position = __instance.spectatedPlayerScript.playerGlobalHead.position + __instance.spectatedPlayerScript.playerGlobalHead.right * 0.25f + __instance.spectatedPlayerScript.playerGlobalHead.up * 0.3f - __instance.spectatedPlayerScript.playerGlobalHead.forward * 0.9f;
            }
        }

        [HarmonyPatch("RaycastSpectateCameraAroundPivot")]
        [HarmonyPrefix]
        private static bool RaycastSpectateCameraAroundPivot(ref PlayerControllerB __instance)
        {
            if (switchCamera)
            {
                // TODO: Vignette shader
                __instance.playersManager.spectateCamera.fieldOfView = 70f;
                __instance.playersManager.spectateCamera.transform.LookAt(__instance.spectateCameraPivot.position);
            }

            return !switchCamera;
        }
    }
}