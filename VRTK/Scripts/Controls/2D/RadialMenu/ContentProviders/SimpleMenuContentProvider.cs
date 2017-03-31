using System;
using System.Collections.Generic;

namespace VRTK
{
    [Serializable]
    public class SimpleMenuContentProvider : IMenuContentProvider
    {
        public List<MenuButton> buttons;

        public List<MenuButton> Buttons { get { return buttons; } set { buttons = value; } }
    }
}