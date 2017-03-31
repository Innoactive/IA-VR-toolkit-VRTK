using System;
using System.Collections.Generic;

namespace VRTK
{
    [Serializable]
    public class SimpleMenuContentProvider : IMenuContentProvider
    {
        public List<RadialMenuButton> buttons;

        public List<RadialMenuButton> Buttons { get { return buttons; } set { buttons = value; } }
    }
}