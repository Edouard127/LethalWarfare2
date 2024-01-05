﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    internal class SmokeGrenade : CustomItem
    {
        public ParticleSystem smoke = new ParticleSystem();

        // Used to get the methods
        private StunGrenadeItem stunGrenadeItem = new StunGrenadeItem();

        public float timeToExplode = 2f;

        public AudioClip throwSFX;

        public bool pinPulled;

        public bool inPullingPinAnimation;

        public Coroutine pullPinCoroutine;

        public Animator itemAnimator;

        public AudioSource itemAudio;

        public AudioClip pullPinSFX;

        public AudioClip explodeSFX;

        public float explodeTimer;

        public bool hasExploded;

        public PlayerControllerB playerThrownBy;
        public SmokeGrenade() : base()
        {
            // THIS IS TEMPORARY
            this.itemProperties = new Item();
            this.terminalNode = new TerminalNode();
            this.unlockableItem = new UnlockableItem();

            this.terminalNode.displayText = "Smoke Grenade";
            this.terminalNode.itemCost = 80;

            this.itemProperties.itemName = "Smoke Grenade";
            this.itemProperties.canBeGrabbedBeforeGameStart = true;
            this.itemProperties.weight = 0f;
            this.itemProperties.itemIsTrigger = true;
            this.itemProperties.itemIcon = Assets.GetSpriteFromName("Smoke Grenade Image");
            // this.itemProperties.throwSFX = Assets.GetAudioClipFromName("Smoke Grenade Throw");
            this.itemProperties.isDefensiveWeapon = true;

            this.unlockableItem.unlockableName = "Smoke Grenade";
            this.unlockableItem.alwaysInStock = true;

            itemAnimator = stunGrenadeItem.itemAnimator;
            itemAudio = stunGrenadeItem.itemAudio;
            pullPinSFX = stunGrenadeItem.pullPinSFX;
            explodeSFX = Assets.GetAudioClipFromName("Smoke Grenade Throw");
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (!this.pinPulled && !this.inPullingPinAnimation && pullPinCoroutine == null)
            {
                this.inPullingPinAnimation = true;
                this.pullPinCoroutine = base.StartCoroutine(PinAnimation());
            }
        }

        public override void DiscardItem()
        {
            if (playerHeldBy != null)
            {
                playerHeldBy.activatingItem = false;
            }
            base.DiscardItem();
        }

        public override void EquipItem()
        {
            EnableItemMeshes(enable: true);
            isPocketed = false;
        }

        // Realistic smoke grenade throw with realistic physics
        public override void FallWithCurve()
        {
            float magnitude = (startFallingPosition - targetFloorPosition).magnitude;
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.Euler(itemProperties.restingRotation.x, base.transform.eulerAngles.y, itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
            base.transform.localPosition = Vector3.Lerp(startFallingPosition, targetFloorPosition, Mathf.Sin(fallTime * 3.14159274f));
            fallTime += Time.deltaTime * 0.5f;
        }

        public override void Update()
        {
            base.Update();
            if (pinPulled && !hasExploded)
            {
                explodeTimer += Time.deltaTime;
                if (explodeTimer >= timeToExplode)
                {
                    ExplodeSmokeGrenade();
                }
            }
        }

        public virtual void ExplodeSmokeGrenade()
        {
            if (!hasExploded)
            {
                hasExploded = true;
                itemAudio.PlayOneShot(explodeSFX);
                WalkieTalkie.TransmitOneShotAudio(itemAudio, explodeSFX, 1f);
                smoke.Emit(transform.position, new Vector3(0.5f, 0.5f, 0.5f), 1f, 15f, Color.white);
            }
            DestroyObjectInHand(playerThrownBy);
        }

        public IEnumerator PinAnimation()
        {
            inPullingPinAnimation = true;
            playerHeldBy.activatingItem = true;
            playerHeldBy.doingUpperBodyEmote = 1.16f;
            playerHeldBy.playerBodyAnimator.SetTrigger("PullGrenadePin");
            itemAnimator.SetTrigger("pullPin");
            itemAudio.PlayOneShot(pullPinSFX);
            WalkieTalkie.TransmitOneShotAudio(itemAudio, pullPinSFX, 1f);
            yield return new WaitForSeconds(1f);
            inPullingPinAnimation = false;
            pinPulled = true;
            itemUsedUp = true;
            if (playerHeldBy != null)
            {
                playerThrownBy = playerHeldBy;
            }
        }
    }
}
