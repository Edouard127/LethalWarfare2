using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace LethalWarfare2.Modules.Items
{
    public class ItemLoader
    {
        public static void Register()
        {
            AddTerminalNode(AddItem(new SmokeGrenade()));
        }

        private static CustomItem AddItem(CustomItem item)
        {
            //StartOfRound.Instance.allItemsList.itemsList.Add(item.itemProperties);
            return item;
        }

        private static void AddTerminalNode(CustomItem item)
        {
            //StartOfRound.Instance.unlockablesList.unlockables.Add(item.unlockableItem);
        }
    }
}
