using UnityEngine;
using ModelReplacement;
using System;

namespace LethalWarfare2.Modules.Model
{
    public class GhostReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("Ghost Nightwar");
        }
        /*protected override void OnEmoteStart(int emoteId)
        {
            if (emoteId == 1)
            {
                var clip = Assets.engineerE1.GetRandomAudio(StartOfRound.Instance.randomMapSeed);
                controller.movementAudio.PlayOneShot(clip, 0.8f);
            }
            if (emoteId == 2)
            {
                var clip = Assets.engineerE2.GetRandomAudio(StartOfRound.Instance.randomMapSeed);
                controller.movementAudio.PlayOneShot(clip, 0.8f);
            }
        }*/
    }
}
