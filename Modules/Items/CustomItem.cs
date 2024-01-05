using System;
using System.Collections.Generic;
using System.Text;

namespace LethalWarfare2.Modules.Items
{
    internal class CustomItem : GrabbableObject
    {
        public Item itemProperties;
        public TerminalNode terminalNode;
        public UnlockableItem unlockableItem;

        protected CustomItem()
        {
        }
    }
}
