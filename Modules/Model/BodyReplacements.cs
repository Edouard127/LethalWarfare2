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
    }

    public class AlexReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("Alex Keller");
        }
    }

    public class ValeriaReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("Valeria Shadow");
        }
    }
}
