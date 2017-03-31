using UnityEngine;

namespace VRTK.Builders
{
    public interface IMenuBuilder
    {
        IMenuBuilder AddButton(MenuButton button);

        IMenuBuilder RemoveAllButtons();

        GameObject GetResult();
    }
}