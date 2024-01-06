using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        public static List<CustomItem> items = new List<CustomItem>();
        public static List<CustomItem> shopItems = new List<CustomItem>();

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(ref StartOfRound __instance)
        {
            AddShopItem(AddItem(new SmokeGrenade()));
        }

        public static CustomItem AddItem(CustomItem item)
        {
            item.index = items.Count - 1;
            items.Add(item);
            return item;
        }

        public static CustomItem AddShopItem(CustomItem item)
        {
            shopItems.Add(item);
            return item;
        }

        public static TerminalKeyword CreateTerminalKeyword(CustomItem item, bool isVerb = false, CompatibleNoun[] compatibleNouns = null, TerminalNode specialKeywordResult = null, TerminalKeyword defaultVerb = null, bool accessTerminalObjects = false)
        {
            string name = item.properties.itemName.ToLowerInvariant().Replace(" ", "-");
            TerminalKeyword keyword = new TerminalKeyword();
            keyword.name = name;
            keyword.word = name;
            keyword.isVerb = isVerb;
            keyword.compatibleNouns = compatibleNouns;
            keyword.specialKeywordResult = specialKeywordResult;
            keyword.defaultVerb = defaultVerb;
            keyword.accessTerminalObjects = accessTerminalObjects;
            return keyword;
        }
    }
}
