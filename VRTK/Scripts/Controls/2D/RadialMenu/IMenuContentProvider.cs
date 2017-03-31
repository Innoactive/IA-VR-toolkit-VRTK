using System.Collections.Generic;

namespace VRTK
{
    public interface IMenuContentProvider
    {
        List<RadialMenuButton> Buttons { get; set; }
    }
}
