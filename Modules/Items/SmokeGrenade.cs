using System;
using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    public class SmokeGrenade : GrabbableObject
    {
        private ParticleSystem smoke;
        private float particleTime = 15f;
        private float particleTimer = 0f;

        private float explodeTimer = 0f;
        private bool hasExploded = false;
        private bool isExploded = false;

        private AudioSource itemAudio;
        private AudioClip explodeSFX;
        private AudioClip pullPinSFX;
        private Animator itemAnimator;

        private bool pinPulled = false;
        private bool inPullingPinAnimation = false;
        private Coroutine pullPinCoroutine = null;

        public RaycastHit grenadeHit;
        public Ray grenadeThrowRay;

        private PlayerControllerB playerThrownBy;

        public void Awake()
        {
            base.itemProperties.name = "Smoke Grenade";
            base.itemProperties.weight = 0f;
            base.itemProperties.itemIcon = Assets.GetSpriteFromName("Smoke Grenade Image");
            explodeSFX = Assets.GetAudioClipFromName("Smoke Grenade Throw");
            itemAudio = GetComponent<AudioSource>();

            base.grabbable = true;
            base.grabbableToEnemies = true;
            base.mainObjectRenderer = GetComponent<MeshRenderer>();
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (inPullingPinAnimation)
            {
                return;
            }

            if (!pinPulled)
            {
                if (pullPinCoroutine == null)
                {
                    playerHeldBy.activatingItem = true;
                    pullPinCoroutine = StartCoroutine(pullPinAnimation());
                }
            }
            else if (base.IsOwner)
            {
                playerHeldBy.DiscardHeldObject(placeObject: true, null, GetGrenadeThrowDestination());
            }
        }

        public IEnumerator pullPinAnimation()
        {
            inPullingPinAnimation = true;
            playerHeldBy.activatingItem = true;
            playerHeldBy.doingUpperBodyEmote = 1.16f;
            playerHeldBy.playerBodyAnimator.SetTrigger("PullGrenadePin");
            itemAnimator.SetTrigger("pullPin");
            itemAudio.PlayOneShot(pullPinSFX);
            WalkieTalkie.TransmitOneShotAudio(itemAudio, pullPinSFX, 0.8f);
            yield return new WaitForSeconds(1f);
            if (playerHeldBy != null)
            {
                playerHeldBy.activatingItem = false;
                playerThrownBy = playerHeldBy;
            }

            inPullingPinAnimation = false;
            pinPulled = true;
            itemUsedUp = true;
        }

        public override void Update()
        {
            base.Update();  

            if (pinPulled && !hasExploded)
            {
                if (base.IsOwner)
                {
                    explodeTimer += Time.deltaTime;
                    if (explodeTimer > 2f)
                    {
                        ExplodeSmokeGrenade();
                    }
                }
            }
        }

        public Vector3 GetGrenadeThrowDestination()
        {
            Vector3 position = base.transform.position;
            Debug.DrawRay(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward, Color.yellow, 15f);
            grenadeThrowRay = new Ray(playerHeldBy.gameplayCamera.transform.position, playerHeldBy.gameplayCamera.transform.forward);
            position = ((!Physics.Raycast(grenadeThrowRay, out grenadeHit, 12f, StartOfRound.Instance.collidersAndRoomMaskAndDefault)) ? grenadeThrowRay.GetPoint(10f) : grenadeThrowRay.GetPoint(grenadeHit.distance - 0.05f));
            Debug.DrawRay(position, Vector3.down, Color.blue, 15f);
            grenadeThrowRay = new Ray(position, Vector3.down);
            if (Physics.Raycast(grenadeThrowRay, out grenadeHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
            {
                return grenadeHit.point + Vector3.up * 0.05f;
            }

            return grenadeThrowRay.GetPoint(30f);
        }

        private void ExplodeSmokeGrenade()
        {
            if (!hasExploded)
            {
                hasExploded = true;
                itemAudio.PlayOneShot(explodeSFX);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, explodeSFX);
                smoke.Play();
                DestroyObjectInHand(playerThrownBy);
            }
        }

        public override void __initializeVariables()
        {
            base.__initializeVariables();
            // TODO: Fix this
            smoke = ParticleSystem.Instantiate(new SteamValveHazard().valveSteamParticle, base.transform.position, Quaternion.identity);
            itemAnimator = new StunGrenadeItem().itemAnimator;
        }

        public override string __getTypeName()
        {
            return "SmokeGrenade";
        }
    }
}
