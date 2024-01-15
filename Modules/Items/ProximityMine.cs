using System;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    internal class ProximityMine : PhysicsProp
    {
        public static ProximityMine LoadAssetAndReturnInstance()
        {
            Item proximityMine = Assets.GetAssetFromName<Item>("ProximityMine");
            ProximityMine proximityMineInstance = proximityMine.spawnPrefab.AddComponent<ProximityMine>();
            proximityMineInstance.itemProperties = proximityMine;
            return proximityMineInstance;
        }

        public static TerminalNode CreateTerminalNode()
        {
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "Ping an enemy for 5 seconds when triggered";
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