using System;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    internal class SmokeGrenade : PhysicsProp
    {
        private ParticleSystem smoke;

        public static SmokeGrenade LoadAssetAndReturnInstance()
        {
            Item smokeGrenade = Assets.GetAssetFromName<Item>("SmokeGrenadeItem");
            SmokeGrenade smokeGrenadeInstance = smokeGrenade.spawnPrefab.AddComponent<SmokeGrenade>();
            smokeGrenadeInstance.itemProperties = smokeGrenade;
            return smokeGrenadeInstance;
        }

        public static TerminalNode CreateTerminalNode()
        {
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Allows a player to cut line of sight with an enemy for 15 seconds";
            return node;
        }

        /*public void Awake()
        {
            smoke = GameObject.Find("SteamValve").GetComponent<ParticleSystem>();
            smoke.startColor = Color.gray;
        }*/

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                playerHeldBy.DamagePlayer(20);
            }
        }

        /*private void ExplodeSmokeGrenade()
        {
            if (!hasExploded)
            {
                hasExploded = true;
                //itemAudio.PlayOneShot(explodeSFX);//                //WalkieTalkie.TransmitOneShotAudio(//emAudio, explodeSFX);
                smoke.Play();
                DestroyObjectInHand(playerHeldBy);
            }
        }*/
    }
}
