using System.Collections.Generic;

namespace VRTK
{
    public interface IMenuContentProvider
    {
        List<MenuButton> Buttons { get; set; }
    }
}
