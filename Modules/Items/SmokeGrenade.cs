using HarmonyLib;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    [HarmonyPatch(typeof(StunGrenadeItem))]
    internal class SmokeGrenade
    {
        private static ParticleSystem smoke = new ParticleSystem();
        private static float particleTime = 15f;
        private static float particleTimer = 0f;

        [HarmonyPatch("__initializeVariables")]
        [HarmonyPostfix]
        private static void Start(ref StunGrenadeItem __instance)
        {
            __instance.itemProperties.name = "Smoke Grenade";
            __instance.itemProperties.weight = 0f;
            __instance.itemProperties.itemIcon = Assets.GetSpriteFromName("Smoke Grenade Image");
            // __instance.itemProperties.throwSFX = Assets.GetAudioClipFromName("Smoke Grenade Throw");
            __instance.itemProperties.isDefensiveWeapon = true;
            __instance.itemProperties.canBeGrabbedBeforeGameStart = true;
            __instance.explodeSFX = Assets.GetAudioClipFromName("Smoke Grenade Throw");
            __instance.TimeToExplode = 1f;
        }

        [HarmonyPatch("ExplodeStunGrenade")]
        [HarmonyPrefix]
        private static bool ExplodeStunGrenade(ref StunGrenadeItem __instance)
        {
            if (!__instance.hasExploded)
            {
                __instance.hasExploded = true;
                __instance.itemAudio.PlayOneShot(__instance.explodeSFX);
                WalkieTalkie.TransmitOneShotAudio(__instance.itemAudio, __instance.explodeSFX);
                Object.Instantiate(parent: (!__instance.isInElevator) ? RoundManager.Instance.mapPropsContainer.transform : StartOfRound.Instance.elevatorTransform, original: __instance.stunGrenadeExplosion, position: __instance.transform.position, rotation: Quaternion.identity);
                if (__instance.DestroyGrenade)
                {
                    __instance.DestroyObjectInHand(__instance.playerThrownBy);
                }
            }
            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool Update(ref StunGrenadeItem __instance)
        {
            if (__instance.hasExploded && particleTimer > -1f)
            {
                if (particleTimer > particleTime)
                {
                    particleTimer = -1f;
                    smoke.Stop();
                }
                else
                {
                    smoke.Emit(__instance.transform.position, new Vector3(2f, 2f, 2f), 200f, 15000f, Color.gray);
                    particleTimer += Time.deltaTime;
                }
            }
            return false;
        }
    }
}
