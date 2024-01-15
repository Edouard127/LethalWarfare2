using UnityEngine;
using ModelReplacement;

namespace LethalWarfare2.Modules.Model
{
    public class GhostReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("GhostNightwar");
        }
    }

    public class AlexReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("AlexKeller");
        }
    }

    public class ValeriaReplacement : BodyReplacementBase
    {
        protected override GameObject LoadAssetsAndReturnModel()
        {
            return Assets.GetAssetFromName<GameObject>("ValeriaGarza");
        }
    }
}
