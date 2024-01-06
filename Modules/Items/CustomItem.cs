using System;
using System.Collections.Generic;
using System.Text;

namespace LethalWarfare2.Modules.Items
{
    public class CustomItem : GrabbableObject
    {
        public int index = -1; // The index of the item in the list of items
        public Item properties = new Item(); // The item itself

        public bool isShopItem = false; // Is the item in the shop
        public int shopPrice = 0; // The price of the item in the shop
        public UnlockableItem unlockableItem = new UnlockableItem(); // The item in the shop

        public CustomItem() { }

        public CustomItem SetName(string name)
        {
            this.properties.itemName = name;
            this.unlockableItem.unlockableName = name;
            return this;
        }

        public CustomItem SetCost(int cost)
        {
            this.isShopItem = true;
            this.shopPrice = cost;
            return this;
        }

        public CustomItem SetAlwaysInStock(bool alwaysInStock)
        {
            this.unlockableItem.alwaysInStock = alwaysInStock;
            return this;
        }

        public CustomItem SetWeight(float weight)
        {
            this.properties.weight = weight;
            return this;
        }

        public CustomItem SetIcon(string icon)
        {
            this.properties.itemIcon = Assets.GetSpriteFromName(icon);
            return this;
        }

        public CustomItem SetThrowSFX(string sfx)
        {
            this.properties.throwSFX = Assets.GetAudioClipFromName(sfx);
            return this;
        }

        public CustomItem SetIsDefensiveWeapon(bool isDefensiveWeapon)
        {
            this.properties.isDefensiveWeapon = isDefensiveWeapon;
            return this;
        }

        public CustomItem SetCanBeGrabbedBeforeGameStart(bool canBeGrabbedBeforeGameStart)
        {
            this.properties.canBeGrabbedBeforeGameStart = canBeGrabbedBeforeGameStart;
            return this;
        }

        public CustomItem SetItemIsTrigger(bool itemIsTrigger)
        {
            this.properties.itemIsTrigger = itemIsTrigger;
            return this;
        }
    }
}
