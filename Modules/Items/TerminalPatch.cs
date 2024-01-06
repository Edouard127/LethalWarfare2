using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LethalWarfare2.Modules.Items
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start(ref Terminal __instance)
        {
            TerminalKeyword buyKeyword = __instance.terminalNodes.allKeywords.First(keyword => keyword.word == "buy");
            TerminalKeyword infoKeyword = __instance.terminalNodes.allKeywords.First(keyword => keyword.word == "info");
            TerminalNode cancelPurchaseNode = buyKeyword.compatibleNouns[0].result.terminalOptions[1].result;

            List<CustomItem> shopItems = StartOfRoundPatch.shopItems.FindAll(item => item.isShopItem);

            for (int i = 0; i < shopItems.Count; i++)
            {
                CustomItem item = shopItems[i];

                if (!item.isShopItem)
                {
                    continue;
                }
                
                TerminalKeyword keyword = StartOfRoundPatch.CreateTerminalKeyword(item, defaultVerb: buyKeyword);

                TerminalNode buyResult = ScriptableObject.CreateInstance<TerminalNode>();
                buyResult.name = $"{item.name.Replace(" ", "-")}BuyNode2";
                buyResult.displayText = buyResult.displayText = $"Ordered [variableAmount] {item.name}. Your new balance is [playerCredits].\n\nOur contractors enjoy fast, free shipping while on the job! Any purchased items will arrive hourly at your approximate location.\r\n\r\n";
                buyResult.buyItemIndex = item.index;
                buyResult.maxCharactersToType = 15;
                buyResult.shipUnlockableID = item.index;
                buyResult.creatureName = item.name;
                buyResult.isConfirmationNode = false;
                buyResult.itemCost = item.shopPrice;
                buyResult.playSyncedClip = 0;

                TerminalNode buyNode = ScriptableObject.CreateInstance<TerminalNode>();
                buyNode.name = $"{item.name.Replace(" ", "-")}BuyNode1";
                buyNode.displayText = $"You have requested to order {item.name}. Amount: [variableAmount].\nTotal cost of items: [totalCost].\n\nPlease CONFIRM or DENY.\r\n\r\n";
                buyNode.buyItemIndex = item.index;
                buyNode.clearPreviousText = true;
                buyNode.maxCharactersToType = 35;
                buyNode.shipUnlockableID = item.index;
                buyNode.creatureName = item.name;
                buyNode.isConfirmationNode = true;
                buyNode.overrideOptions = true;
                buyNode.itemCost = item.shopPrice;
                buyNode.terminalOptions = new CompatibleNoun[2]
                {
                    new CompatibleNoun()
                    {
                        noun = __instance.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "confirm"),
                        result = buyResult,
                    },


                    new CompatibleNoun()
                    {
                        noun = __instance.terminalNodes.allKeywords.First(keyword2 => keyword2.word == "deny"),
                        result = cancelPurchaseNode,
                    },
                };

                __instance.terminalNodes.allKeywords.AddItem(keyword);

                buyKeyword.compatibleNouns.AddItem(new CompatibleNoun()
                {
                    noun = keyword,
                    result = buyNode,
                });
            }   
        }
    }
}
