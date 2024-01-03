using HarmonyLib;
using LethalWarfare2.PlayerPatches;
using UnityEngine;

namespace LethalWarfare2.CameraVignette
{
    [HarmonyPatch(typeof(Camera))]
    public class CameraVignette
    {
        public static float fallOff { get; set; } = 0.5f;
        public static float intensity { get; set; } = 0.5f;
        private static Material vignetteMaterial;

        private static Material GetOrCreateVignetteMaterial()
        {
            if (vignetteMaterial == null)
            {
                vignetteMaterial = new Material(Assets.GetShaderFromName("VignetteShader"));
            }

            return vignetteMaterial;
        }

        [HarmonyPatch("OnRenderImage")]
        [HarmonyPostfix]
        private static void OnRenderImage(ref Camera __instance, ref RenderTexture __source, ref RenderTexture __destination)
        {
            if (PlayerControllerBPatch.switchCamera)
            {
                if (vignetteMaterial == null)
                {
                    GetOrCreateVignetteMaterial();
                }

                if (__instance == null)
                {
                    return;
                }

                vignetteMaterial.SetFloat("_FallOff", fallOff);
                vignetteMaterial.SetVector("_Center", new Vector2(__instance.aspect, 1f));

                Graphics.Blit(__source, __destination, vignetteMaterial);
            }
        }
    }
}
