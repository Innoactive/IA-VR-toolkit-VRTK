using UnityEngine;
using UnityEngine.Events;

namespace VRTK
{
    public interface IMenuButton
    {
        Sprite ButtonIcon { get; set; }
        UnityEvent OnClick { get; set; }
        UnityEvent OnHold { get; set; }
        UnityEvent OnHoverEnter { get; set; }
        UnityEvent OnHoverExit { get; set; }
    }
}